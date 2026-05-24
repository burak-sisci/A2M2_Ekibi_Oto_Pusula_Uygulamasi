import 'package:flutter/foundation.dart';
import '../../core/push/fcm_service.dart';
import '../../core/storage/token_storage.dart';
import '../../data/repositories/auth_repository.dart';

enum AuthStatus { initializing, authenticated, unauthenticated }

class AuthSessionViewModel extends ChangeNotifier {
  final AuthRepository _authRepository;
  final TokenStorage _tokenStorage;
  final FcmService? _fcmService;

  AuthStatus _status = AuthStatus.initializing;
  String? _userId;
  String? _token;

  AuthStatus get status => _status;
  bool get isLoggedIn => _status == AuthStatus.authenticated;
  String? get userId => _userId;

  AuthSessionViewModel({
    required AuthRepository authRepository,
    required TokenStorage tokenStorage,
    FcmService? fcmService,
  })  : _authRepository = authRepository,
        _tokenStorage = tokenStorage,
        _fcmService = fcmService;

  Future<void> init() async {
    try {
      final results = await Future.wait([
        _tokenStorage.readToken(),
        _tokenStorage.readUserId(),
      ]).timeout(const Duration(seconds: 5));
      final token = results[0];
      final userId = results[1];
      if (token != null && userId != null) {
        _token = token;
        _userId = userId;
        _status = AuthStatus.authenticated;
        // Uygulama yeniden açıldıysa FCM token'ı tekrar kayıt et
        _fcmService?.registerForUser();
      } else {
        _status = AuthStatus.unauthenticated;
      }
    } catch (_) {
      _status = AuthStatus.unauthenticated;
    }
    notifyListeners();
  }

  Future<void> setAuthenticated(
    String token,
    String userId, {
    String? refreshToken,
  }) async {
    await _tokenStorage.saveToken(token);
    await _tokenStorage.saveUserId(userId);
    if (refreshToken != null) {
      await _tokenStorage.saveRefreshToken(refreshToken);
    }
    _token = token;
    _userId = userId;
    _status = AuthStatus.authenticated;
    notifyListeners();

    // FCM token'ı backend'e kaydet (async, hata olsa bile login'i bozma)
    _fcmService?.registerForUser();
  }

  Future<void> logout() async {
    // 1) Backend'den cihazı sil
    try {
      await _fcmService?.unregisterCurrentToken();
    } catch (_) {/* yut */}

    // 2) Backend logout (refresh varsa onu da revoke et)
    try {
      final refresh = await _tokenStorage.readRefreshToken();
      await _authRepository.logout(refreshToken: refresh);
    } catch (_) {
      // Sunucu hatası olsa bile yerel oturumu temizle
    } finally {
      await _clearSession();
    }
  }

  /// AuthInterceptor refresh başarısız olunca çağırır.
  Future<void> handleSessionExpired() async {
    await _clearSession();
  }

  Future<void> _clearSession() async {
    await _tokenStorage.clearAll();
    _token = null;
    _userId = null;
    _status = AuthStatus.unauthenticated;
    notifyListeners();
  }
}
