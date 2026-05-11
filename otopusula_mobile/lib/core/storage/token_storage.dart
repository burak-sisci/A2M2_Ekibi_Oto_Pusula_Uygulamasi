import 'package:flutter/foundation.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class TokenStorage {
  static const _tokenKey = 'jwt_token';
  static const _userIdKey = 'user_id';

  // Debug modda Keystore tamamen bypass edilir; release'de şifreli depolama kullanılır.
  final FlutterSecureStorage? _storage;
  final Map<String, String> _mem = {};

  TokenStorage()
      : _storage = kDebugMode
            ? null
            : const FlutterSecureStorage(
                aOptions: AndroidOptions(
                  encryptedSharedPreferences: true,
                  resetOnError: true,
                ),
              );

  Future<void> saveToken(String token) async {
    if (kDebugMode) { _mem[_tokenKey] = token; return; }
    await _storage!.write(key: _tokenKey, value: token);
  }

  Future<String?> readToken() async {
    if (kDebugMode) return _mem[_tokenKey];
    return _storage!.read(key: _tokenKey);
  }

  Future<void> deleteToken() async {
    if (kDebugMode) { _mem.remove(_tokenKey); return; }
    await _storage!.delete(key: _tokenKey);
  }

  Future<void> saveUserId(String userId) async {
    if (kDebugMode) { _mem[_userIdKey] = userId; return; }
    await _storage!.write(key: _userIdKey, value: userId);
  }

  Future<String?> readUserId() async {
    if (kDebugMode) return _mem[_userIdKey];
    return _storage!.read(key: _userIdKey);
  }

  Future<void> deleteUserId() async {
    if (kDebugMode) { _mem.remove(_userIdKey); return; }
    await _storage!.delete(key: _userIdKey);
  }

  Future<void> clearAll() async {
    if (kDebugMode) { _mem.clear(); return; }
    await _storage!.deleteAll();
  }
}
