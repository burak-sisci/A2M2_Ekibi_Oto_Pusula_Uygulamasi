import 'dart:async';
import 'package:dio/dio.dart';
import '../constants/api_endpoints.dart';
import '../constants/app_constants.dart';
import '../storage/token_storage.dart';

/// İstek başına access token ekler; 401 alınırsa /auth/refresh ile yeni token alıp
/// orijinal isteği bir kez tekrarlar. Refresh başarısızsa storage temizlenir
/// (AuthSessionViewModel.init bunu görüp kullanıcıyı login'e yönlendirir).
class AuthInterceptor extends Interceptor {
  final TokenStorage _tokenStorage;

  /// Eşzamanlı 401'ler için tek refresh çağrısı garantisi.
  Completer<bool>? _refreshing;

  /// Refresh çağrısının kendisi 401 dönerse sonsuz döngüye girmemek için ayrı bir Dio.
  late final Dio _refreshDio;

  /// Refresh başarısız olduğunda haber verilmek istenirse atanır.
  /// AuthSessionViewModel bağlandığında bunu doldurur.
  void Function()? onSessionExpired;

  AuthInterceptor(this._tokenStorage) {
    _refreshDio = Dio(
      BaseOptions(
        baseUrl: AppConstants.baseUrl,
        connectTimeout: const Duration(seconds: 15),
        receiveTimeout: const Duration(seconds: 30),
        headers: {'Accept': 'application/json'},
      ),
    );
  }

  @override
  Future<void> onRequest(RequestOptions options, RequestInterceptorHandler handler) async {
    final token = await _tokenStorage.readToken();
    if (token != null) {
      options.headers['Authorization'] = 'Bearer $token';
    }
    handler.next(options);
  }

  @override
  Future<void> onError(DioException err, ErrorInterceptorHandler handler) async {
    final response  = err.response;
    final request   = err.requestOptions;
    final isUnauth  = response?.statusCode == 401;
    final isRefresh = request.path.contains(ApiEndpoints.authRefresh);

    // Refresh endpoint'inin kendisinden veya tekrar edilmiş istekten 401 → bırak
    if (!isUnauth || isRefresh || request.extra['retried'] == true) {
      return handler.next(err);
    }

    final refreshToken = await _tokenStorage.readRefreshToken();
    if (refreshToken == null) {
      onSessionExpired?.call();
      return handler.next(err);
    }

    final ok = await _runRefresh(refreshToken);
    if (!ok) {
      await _tokenStorage.clearAll();
      onSessionExpired?.call();
      return handler.next(err);
    }

    // Orijinal isteği yeni token ile tekrarla
    final newToken = await _tokenStorage.readToken();
    request.headers['Authorization'] = 'Bearer $newToken';
    request.extra['retried'] = true;

    try {
      final retryDio = Dio(BaseOptions(baseUrl: AppConstants.baseUrl));
      final retried = await retryDio.fetch(request);
      handler.resolve(retried);
    } catch (e) {
      handler.next(err);
    }
  }

  Future<bool> _runRefresh(String refreshToken) async {
    // Zaten süren bir refresh varsa bekle
    if (_refreshing != null) {
      return await _refreshing!.future;
    }

    final completer = Completer<bool>();
    _refreshing = completer;

    try {
      final res = await _refreshDio.post(
        ApiEndpoints.authRefresh,
        data: {'RefreshToken': refreshToken},
      );
      final data = res.data as Map<String, dynamic>;
      final newAccess  = data['accessToken'] as String?;
      final newRefresh = data['refreshToken'] as String?;
      if (newAccess != null && newRefresh != null) {
        await _tokenStorage.saveToken(newAccess);
        await _tokenStorage.saveRefreshToken(newRefresh);
        completer.complete(true);
        return true;
      }
      completer.complete(false);
      return false;
    } catch (_) {
      completer.complete(false);
      return false;
    } finally {
      _refreshing = null;
    }
  }
}
