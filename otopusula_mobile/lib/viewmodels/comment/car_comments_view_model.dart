import '../../core/constants/app_constants.dart';
import '../../core/constants/app_strings.dart';
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

  List<Comment> get comments => List.unmodifiable(_comments);
  bool get hasMore => _hasMore;
  bool get isFetchingMore => _isFetchingMore;

  CarCommentsViewModel({
    required CommentRepository commentRepository,
    required this.carId,
  }) : _commentRepository = commentRepository;

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
    } on Exception catch (e) {
      setError(_friendlyMessage(e));
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
    } on Exception catch (e) {
      setError(_friendlyMessage(e));
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
    } on Exception catch (e) {
      setError(_friendlyMessage(e));
    }
  }

  Future<void> deleteComment(String commentId) async {
    try {
      await _commentRepository.deleteComment(commentId);
      _comments.removeWhere((c) => c.id == commentId);
      notifyListeners();
    } on Exception catch (e) {
      setError(_friendlyMessage(e));
    }
  }

  // TODO: Backend endpoint bekliyor — likeComment / unlikeComment (developer.md §5 tablosu)
  Future<void> likeComment(String commentId) async {
    try {
      await _commentRepository.likeComment(commentId);
    } on Exception catch (_) {
      // Hata sessizce yutulur; backend hazır olunca state yönetimi eklenecek
    }
  }

  Future<void> unlikeComment(String commentId) async {
    try {
      await _commentRepository.unlikeComment(commentId);
    } on Exception catch (_) {}
  }

  String _friendlyMessage(Exception e) {
    final msg = e.toString().toLowerCase();
    if (msg.contains('network') || msg.contains('connection')) return AppStrings.errorNetwork;
    if (msg.contains('403')) return AppStrings.errorForbidden;
    return AppStrings.errorGeneric;
  }
}
