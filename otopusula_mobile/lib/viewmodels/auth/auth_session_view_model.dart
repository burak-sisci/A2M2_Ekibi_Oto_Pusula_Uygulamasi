import 'package:flutter/foundation.dart';
import '../../core/storage/token_storage.dart';
import '../../data/repositories/auth_repository.dart';

enum AuthStatus { initializing, authenticated, unauthenticated }

class AuthSessionViewModel extends ChangeNotifier {
  final AuthRepository _authRepository;
  final TokenStorage _tokenStorage;

  AuthStatus _status = AuthStatus.initializing;
  String? _userId;
  String? _token;

  AuthStatus get status => _status;
  bool get isLoggedIn => _status == AuthStatus.authenticated;
  String? get userId => _userId;

  AuthSessionViewModel({
    required AuthRepository authRepository,
    required TokenStorage tokenStorage,
  })  : _authRepository = authRepository,
        _tokenStorage = tokenStorage;

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
      } else {
        _status = AuthStatus.unauthenticated;
      }
    } catch (_) {
      // Keystore zaman aşımı veya hata → login sayfasına yönlendir
      _status = AuthStatus.unauthenticated;
    }
    notifyListeners();
  }

  Future<void> setAuthenticated(String token, String userId) async {
    await _tokenStorage.saveToken(token);
    await _tokenStorage.saveUserId(userId);
    _token = token;
    _userId = userId;
    _status = AuthStatus.authenticated;
    notifyListeners();
  }

  Future<void> logout() async {
    try {
      await _authRepository.logout();
    } catch (_) {
      // Sunucu hatası olsa bile yerel oturumu temizle
    } finally {
      await _clearSession();
    }
  }

  Future<void> _clearSession() async {
    await _tokenStorage.clearAll();
    _token = null;
    _userId = null;
    _status = AuthStatus.unauthenticated;
    notifyListeners();
  }
}
