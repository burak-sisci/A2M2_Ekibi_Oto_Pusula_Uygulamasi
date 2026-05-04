import '../../core/network/api_client.dart';
import '../../core/constants/api_endpoints.dart';
import '../dto/user_login_dto.dart';
import '../dto/user_register_dto.dart';
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

  Future<void> logout() async {
    await _apiClient.dio.post(ApiEndpoints.authLogout);
  }

  Future<User> updateProfile(String phone) async {
    final response = await _apiClient.dio.put(
      ApiEndpoints.authProfile,
      data: {'Phone': phone},
    );
    return User.fromJson(response.data as Map<String, dynamic>);
  }

  Future<void> deleteAccount(String userId) async {
    await _apiClient.dio.delete(ApiEndpoints.authDelete(userId));
  }
}
