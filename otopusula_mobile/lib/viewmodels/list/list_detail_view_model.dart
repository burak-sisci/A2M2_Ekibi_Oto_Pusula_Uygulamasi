import 'package:dio/dio.dart';
import '../../core/constants/app_strings.dart';
import '../../core/network/exceptions.dart';
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

  Future<void> rename(String newName) async {
    if (_userList == null) return;
    final oldCars = _userList!.cars;
    final oldCarIds = _userList!.carIds;
    try {
      final updated = await _listRepository.updateList(
        listId,
        ListUpdateDto(name: newName),
      );
      // DTO araç listesi içermez; eski verileri koru
      _userList = updated.copyWith(cars: oldCars, carIds: oldCarIds);
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

  Future<void> removeCarFromList(String carId) async {
    _removeSuccess = false;
    try {
      await _listRepository.removeCarFromList(listId, carId);
      _userList = _userList?.copyWith(
        cars: _userList!.cars.where((c) => c.id != carId).toList(),
        carIds: _userList!.carIds.where((id) => id != carId).toList(),
      );
      _removeSuccess = true;
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

  String _friendlyApiMessage(ApiException e) {
    return switch (e) {
      NotFoundException() => AppStrings.errorNotFound,
      ForbiddenException() => AppStrings.errorForbidden,
      NetworkException() => AppStrings.errorNetwork,
      ServerException() => 'Sunucu hatası. Lütfen daha sonra tekrar deneyin.',
      _ => e.message,
    };
  }
}
