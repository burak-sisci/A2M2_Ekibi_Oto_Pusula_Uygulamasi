import '../../core/constants/app_strings.dart';
import '../../data/dto/list_update_dto.dart';
import '../../data/models/list_model.dart';
import '../../data/repositories/list_repository.dart';
import '../base_view_model.dart';

class ListDetailViewModel extends BaseViewModel {
  final ListRepository _listRepository;
  final String listId;

  UserList? _userList;
  bool _removeSuccess = false;

  UserList? get userList => _userList;
  bool get removeSuccess => _removeSuccess;

  ListDetailViewModel({
    required ListRepository listRepository,
    required this.listId,
  }) : _listRepository = listRepository;

  Future<void> load() async {
    setLoading();
    try {
      _userList = await _listRepository.getList(listId);
      setSuccess();
    } on Exception catch (e) {
      setError(_friendlyMessage(e));
    }
  }

  Future<void> rename(String newName) async {
    if (_userList == null) return;
    try {
      final updated = await _listRepository.updateList(
        listId,
        ListUpdateDto(name: newName),
      );
      _userList = updated;
      notifyListeners();
    } on Exception catch (e) {
      setError(_friendlyMessage(e));
    }
  }

  Future<void> removeCarFromList(String carId) async {
    _removeSuccess = false;
    try {
      await _listRepository.removeCarFromList(listId, carId);
      _userList = _userList?.copyWith(
        cars: _userList!.cars.where((c) => c.id != carId).toList(),
      );
      _removeSuccess = true;
      notifyListeners();
    } on Exception catch (e) {
      setError(_friendlyMessage(e));
    }
  }

  String _friendlyMessage(Exception e) {
    final msg = e.toString().toLowerCase();
    if (msg.contains('network') || msg.contains('connection')) return AppStrings.errorNetwork;
    if (msg.contains('403')) return AppStrings.errorForbidden;
    return AppStrings.errorGeneric;
  }
}
