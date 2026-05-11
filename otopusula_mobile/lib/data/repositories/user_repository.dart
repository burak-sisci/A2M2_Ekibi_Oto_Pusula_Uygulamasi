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

  // GET /users/{userId}/lists → [{id, ad, varsayilan, ilanSayisi, kayitTarihi}]
  Future<List<UserList>> getUserLists(String userId) async {
    final response = await _apiClient.dio.get(ApiEndpoints.userLists(userId));
    final list = response.data as List<dynamic>;
    return list.map((e) => UserList.fromJson(e as Map<String, dynamic>)).toList();
  }

  // GET /users/{userId}/comments?limit=10&offset=0
  Future<List<Comment>> getUserComments(
    String userId, {
    int limit = 10,
    int offset = 0,
  }) async {
    final response = await _apiClient.dio.get(
      ApiEndpoints.userComments(userId),
      queryParameters: {'limit': limit, 'offset': offset},
    );
    final data = response.data;
    final List<dynamic> items = data is List
        ? data
        : ((data as Map<String, dynamic>)['items'] ?? data['data'] ?? []);
    return items.map((e) => Comment.fromJson(e as Map<String, dynamic>)).toList();
  }
}
