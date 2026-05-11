import 'package:dio/dio.dart';
import '../../core/constants/app_constants.dart';
import '../../core/constants/app_strings.dart';
import '../../core/network/exceptions.dart';
import '../../data/dto/comment_add_dto.dart';
import '../../data/dto/comment_update_dto.dart';
import '../../data/models/comment.dart';
import '../../data/repositories/comment_repository.dart';
import '../base_view_model.dart';

class CarCommentsViewModel extends BaseViewModel {
  final CommentRepository _commentRepository;
  final String carId;

  final List<Comment> _comments = [];
  int _currentPage = 1;
  bool _hasMore = true;
  bool _isFetchingMore = false;
  String? _operationError;

  List<Comment> get comments => List.unmodifiable(_comments);
  bool get hasMore => _hasMore;
  bool get isFetchingMore => _isFetchingMore;
  // Beğeni/güncelleme gibi ikincil işlemlerde snackbar için kullanılır
  String? get operationError => _operationError;

  CarCommentsViewModel({
    required CommentRepository commentRepository,
    required this.carId,
  }) : _commentRepository = commentRepository;

  void clearOperationError() {
    _operationError = null;
  }

  Future<void> load() async {
    _currentPage = 1;
    _hasMore = true;
    _comments.clear();
    setLoading();
    try {
      final result = await _commentRepository.listCarComments(carId);
      _comments.addAll(result);
      _hasMore = result.length == AppConstants.defaultPageSize;
      setSuccess();
    } on DioException catch (e) {
      final apiError = e.error;
      if (apiError is ApiException) {
        setError(_friendlyApiMessage(apiError));
      } else {
        setError(AppStrings.errorGeneric);
      }
    } on ApiException catch (e) {
      setError(_friendlyApiMessage(e));
    } on Exception catch (_) {
      setError(AppStrings.errorNetwork);
    }
  }

  Future<void> loadMore() async {
    if (!_hasMore || _isFetchingMore) return;
    _isFetchingMore = true;
    notifyListeners();
    try {
      _currentPage++;
      final result = await _commentRepository.listCarComments(
        carId,
        page: _currentPage,
      );
      _comments.addAll(result);
      _hasMore = result.length == AppConstants.defaultPageSize;
    } on Exception {
      _currentPage--;
    } finally {
      _isFetchingMore = false;
      notifyListeners();
    }
  }

  Future<void> addComment(String text) async {
    try {
      final comment = await _commentRepository.addComment(
        carId,
        CommentAddDto(content: text),
      );
      _comments.insert(0, comment);
      notifyListeners();
    } on DioException catch (e) {
      final apiError = e.error;
      if (apiError is ApiException) {
        setError(_friendlyApiMessage(apiError));
      } else {
        setError(AppStrings.errorGeneric);
      }
    } on ApiException catch (e) {
      setError(_friendlyApiMessage(e));
    } on Exception catch (_) {
      setError(AppStrings.errorNetwork);
    }
  }

  Future<void> updateComment(String commentId, String newText) async {
    try {
      final updated = await _commentRepository.updateComment(
        commentId,
        CommentUpdateDto(content: newText),
      );
      final idx = _comments.indexWhere((c) => c.id == commentId);
      if (idx != -1) {
        _comments[idx] = updated;
        notifyListeners();
      }
    } on DioException catch (e) {
      final apiError = e.error;
      _operationError = apiError is ApiException
          ? _friendlyApiMessage(apiError)
          : AppStrings.errorGeneric;
      notifyListeners();
    } on ApiException catch (e) {
      _operationError = _friendlyApiMessage(e);
      notifyListeners();
    } on Exception catch (_) {
      _operationError = AppStrings.errorNetwork;
      notifyListeners();
    }
  }

  Future<void> deleteComment(String commentId) async {
    try {
      await _commentRepository.deleteComment(commentId);
      _comments.removeWhere((c) => c.id == commentId);
      notifyListeners();
    } on DioException catch (e) {
      final apiError = e.error;
      _operationError = apiError is ApiException
          ? _friendlyApiMessage(apiError)
          : AppStrings.errorGeneric;
      notifyListeners();
    } on ApiException catch (e) {
      _operationError = _friendlyApiMessage(e);
      notifyListeners();
    } on Exception catch (_) {
      _operationError = AppStrings.errorNetwork;
      notifyListeners();
    }
  }

  // POST /comments/{id}/like — likedByUsers optimistik güncellenir
  Future<void> likeComment(String commentId, String? currentUserId) async {
    try {
      final result = await _commentRepository.likeComment(commentId);
      final likeCount = result['begeniSayisi'] as int? ?? 0;
      final idx = _comments.indexWhere((c) => c.id == commentId);
      if (idx != -1) {
        final updatedUsers = currentUserId != null &&
                !_comments[idx].likedByUsers.contains(currentUserId)
            ? [..._comments[idx].likedByUsers, currentUserId]
            : _comments[idx].likedByUsers;
        _comments[idx] = _comments[idx].copyWith(
          likeCount: likeCount,
          likedByUsers: updatedUsers,
        );
        notifyListeners();
      }
    } on DioException catch (e) {
      final apiError = e.error;
      _operationError = apiError is ApiException
          ? _friendlyApiMessage(apiError)
          : AppStrings.errorGeneric;
      notifyListeners();
    } on Exception catch (_) {
      _operationError = 'Beğeni işlemi başarısız.';
      notifyListeners();
    }
  }

  // DELETE /comments/{id}/like — likedByUsers optimistik güncellenir
  Future<void> unlikeComment(String commentId, String? currentUserId) async {
    try {
      final result = await _commentRepository.unlikeComment(commentId);
      final likeCount = result['begeniSayisi'] as int? ?? 0;
      final idx = _comments.indexWhere((c) => c.id == commentId);
      if (idx != -1) {
        final updatedUsers = currentUserId != null
            ? _comments[idx]
                .likedByUsers
                .where((id) => id != currentUserId)
                .toList()
            : _comments[idx].likedByUsers;
        _comments[idx] = _comments[idx].copyWith(
          likeCount: likeCount,
          likedByUsers: updatedUsers,
        );
        notifyListeners();
      }
    } on DioException catch (e) {
      final apiError = e.error;
      _operationError = apiError is ApiException
          ? _friendlyApiMessage(apiError)
          : AppStrings.errorGeneric;
      notifyListeners();
    } on Exception catch (_) {
      _operationError = 'Beğeni geri alma başarısız.';
      notifyListeners();
    }
  }

  String _friendlyApiMessage(ApiException e) {
    return switch (e) {
      ForbiddenException() => AppStrings.errorForbidden,
      NetworkException() => AppStrings.errorNetwork,
      ServerException() => 'Sunucu hatası. Lütfen daha sonra tekrar deneyin.',
      _ => e.message,
    };
  }
}
