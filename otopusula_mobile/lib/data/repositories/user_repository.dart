import '../../core/network/api_client.dart';
import '../../core/constants/api_endpoints.dart';
import '../models/comment.dart';
import '../models/list_model.dart';
import '../models/user.dart';

class UserRepository {
  final ApiClient _apiClient;

  UserRepository({required ApiClient apiClient}) : _apiClient = apiClient;

  Future<User> getUser(String userId) async {
    final response = await _apiClient.dio.get(ApiEndpoints.user(userId));
    return User.fromJson(response.data as Map<String, dynamic>);
  }

  Future<List<UserList>> getUserLists(String userId) async {
    final response = await _apiClient.dio.get(ApiEndpoints.userLists(userId));
    final list = response.data as List<dynamic>;
    return list.map((e) => UserList.fromJson(e as Map<String, dynamic>)).toList();
  }

  Future<List<Comment>> getUserComments(String userId) async {
    final response = await _apiClient.dio.get(ApiEndpoints.userComments(userId));
    final list = response.data as List<dynamic>;
    return list.map((e) => Comment.fromJson(e as Map<String, dynamic>)).toList();
  }
}
