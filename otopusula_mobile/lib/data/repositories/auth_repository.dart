import '../../core/network/api_client.dart';
import '../../core/constants/api_endpoints.dart';
import '../dto/user_login_dto.dart';
import '../dto/user_register_dto.dart';
import '../dto/user_update_dto.dart';
import '../models/user.dart';

class AuthRepository {
  final ApiClient _apiClient;

  AuthRepository({required ApiClient apiClient}) : _apiClient = apiClient;

  Future<Map<String, dynamic>> register(UserRegisterDto dto) async {
    final response = await _apiClient.dio.post(
      ApiEndpoints.authRegister,
      data: dto.toJson(),
    );
    return response.data as Map<String, dynamic>;
  }

  Future<Map<String, dynamic>> login(UserLoginDto dto) async {
    final response = await _apiClient.dio.post(
      ApiEndpoints.authLogin,
      data: dto.toJson(),
    );
    return response.data as Map<String, dynamic>;
  }

  Future<void> logout({String? refreshToken}) async {
    await _apiClient.dio.post(
      ApiEndpoints.authLogout,
      data: refreshToken == null ? null : {'RefreshToken': refreshToken},
    );
  }

  /// Backend `/auth/refresh` — eski refresh token revoke edilir, yeni access+refresh döner.
  Future<Map<String, dynamic>> refresh(String refreshToken) async {
    final response = await _apiClient.dio.post(
      ApiEndpoints.authRefresh,
      data: {'RefreshToken': refreshToken},
    );
    return response.data as Map<String, dynamic>;
  }

  // PUT /users/{userId} — ad, telefon veya yeniSifre güncellenebilir
  Future<User> updateProfile(String userId, UserUpdateDto dto) async {
    final response = await _apiClient.dio.put(
      ApiEndpoints.user(userId),
      data: dto.toJson(),
    );
    return User.fromJson(response.data as Map<String, dynamic>);
  }

  // DELETE /users/{userId}
  Future<void> deleteAccount(String userId) async {
    await _apiClient.dio.delete(ApiEndpoints.user(userId));
  }

  // POST /auth/forgot-password — sıfırlama tokeni döner
  Future<Map<String, dynamic>> forgotPassword(String email) async {
    final response = await _apiClient.dio.post(
      ApiEndpoints.authForgotPassword,
      data: {'Email': email},
    );
    return response.data as Map<String, dynamic>;
  }

  // POST /auth/reset-password — yeni şifre belirler
  Future<void> resetPassword(String token, String yeniSifre) async {
    await _apiClient.dio.post(
      ApiEndpoints.authResetPassword,
      data: {'Token': token, 'YeniSifre': yeniSifre},
    );
  }
}
