import '../../core/network/api_client.dart';
import '../../core/constants/api_endpoints.dart';
import '../../core/constants/app_constants.dart';
import '../dto/comment_add_dto.dart';
import '../dto/comment_update_dto.dart';
import '../models/comment.dart';
import '../models/share_link.dart';

class CommentRepository {
  final ApiClient _apiClient;

  CommentRepository({required ApiClient apiClient}) : _apiClient = apiClient;

  Future<List<Comment>> listCarComments(
    String carId, {
    int page = 1,
    int limit = AppConstants.defaultPageSize,
  }) async {
    final response = await _apiClient.dio.get(
      ApiEndpoints.carComments(carId),
      queryParameters: {'page': page, 'limit': limit},
    );
    final data = response.data;
    final List<dynamic> items =
        data is List ? data : ((data as Map<String, dynamic>)['items'] ?? data['data'] ?? []);
    return items.map((e) => Comment.fromJson(e as Map<String, dynamic>)).toList();
  }

  Future<Comment> addComment(String carId, CommentAddDto dto) async {
    final response = await _apiClient.dio.post(
      ApiEndpoints.carComments(carId),
      data: dto.toJson(),
    );
    return Comment.fromJson(response.data as Map<String, dynamic>);
  }

  Future<Comment> updateComment(String commentId, CommentUpdateDto dto) async {
    final response = await _apiClient.dio.put(
      ApiEndpoints.comment(commentId),
      data: dto.toJson(),
    );
    return Comment.fromJson(response.data as Map<String, dynamic>);
  }

  Future<void> deleteComment(String commentId) async {
    await _apiClient.dio.delete(ApiEndpoints.comment(commentId));
  }

  Future<ShareLink> getShareLink(String carId) async {
    final response = await _apiClient.dio.get(ApiEndpoints.carShare(carId));
    return ShareLink.fromJson(response.data as Map<String, dynamic>);
  }

  // TODO: Backend'de henüz implement edilmedi.
  // Endpoint eklenince: POST /comments/:commentId/like
  Future<void> likeComment(String commentId) async {
    await _apiClient.dio.post(ApiEndpoints.commentLike(commentId));
  }

  // TODO: Backend'de henüz implement edilmedi.
  // Endpoint eklenince: DELETE /comments/:commentId/like
  Future<void> unlikeComment(String commentId) async {
    await _apiClient.dio.delete(ApiEndpoints.commentLike(commentId));
  }
}
