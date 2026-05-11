import 'package:dio/dio.dart';
import '../../core/constants/app_strings.dart';
import '../../core/network/exceptions.dart';
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

  Future<void> createList(String name) async {
    try {
      final newList = await _listRepository.createList(ListCreateDto(name: name));
      _lists.add(newList);
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

  Future<void> deleteList(String listId) async {
    try {
      await _listRepository.deleteList(listId);
      _lists.removeWhere((l) => l.id == listId);
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
      NetworkException() => AppStrings.errorNetwork,
      ServerException() => 'Sunucu hatası. Lütfen daha sonra tekrar deneyin.',
      _ => e.message,
    };
  }
}
