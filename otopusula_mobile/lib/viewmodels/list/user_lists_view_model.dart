import '../../core/constants/app_strings.dart';
import '../../data/dto/list_create_dto.dart';
import '../../data/models/list_model.dart';
import '../../data/repositories/list_repository.dart';
import '../../data/repositories/user_repository.dart';
import '../base_view_model.dart';

class UserListsViewModel extends BaseViewModel {
  final UserRepository _userRepository;
  final ListRepository _listRepository;
  final String userId;

  final List<UserList> _lists = [];
  List<UserList> get lists => List.unmodifiable(_lists);

  UserListsViewModel({
    required UserRepository userRepository,
    required ListRepository listRepository,
    required this.userId,
  })  : _userRepository = userRepository,
        _listRepository = listRepository;

  Future<void> load() async {
    setLoading();
    try {
      final result = await _userRepository.getUserLists(userId);
      _lists
        ..clear()
        ..addAll(result);
      setSuccess();
    } on Exception catch (e) {
      setError(_friendlyMessage(e));
    }
  }

  Future<void> createList(String name) async {
    try {
      final newList = await _listRepository.createList(ListCreateDto(name: name));
      _lists.add(newList);
      notifyListeners();
    } on Exception catch (e) {
      setError(_friendlyMessage(e));
    }
  }

  Future<void> deleteList(String listId) async {
    try {
      await _listRepository.deleteList(listId);
      _lists.removeWhere((l) => l.id == listId);
      notifyListeners();
    } on Exception catch (e) {
      setError(_friendlyMessage(e));
    }
  }

  String _friendlyMessage(Exception e) {
    final msg = e.toString().toLowerCase();
    if (msg.contains('network') || msg.contains('connection')) return AppStrings.errorNetwork;
    return AppStrings.errorGeneric;
  }
}
