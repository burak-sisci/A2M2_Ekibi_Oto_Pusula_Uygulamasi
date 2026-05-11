import 'package:dio/dio.dart';
import '../../core/constants/app_strings.dart';
import '../../core/network/exceptions.dart';
import '../../data/dto/user_update_dto.dart';
import '../../data/models/user.dart';
import '../../data/repositories/auth_repository.dart';
import '../../data/repositories/user_repository.dart';
import '../base_view_model.dart';

class ProfileViewModel extends BaseViewModel {
  final UserRepository _userRepository;
  final AuthRepository _authRepository;
  final String userId;

  User? _user;
  bool _updateSuccess = false;
  bool _deleteSuccess = false;

  User? get user => _user;
  bool get updateSuccess => _updateSuccess;
  bool get deleteSuccess => _deleteSuccess;

  void resetDeleteSuccess() {
    _deleteSuccess = false;
  }

  ProfileViewModel({
    required UserRepository userRepository,
    required AuthRepository authRepository,
    required this.userId,
  })  : _userRepository = userRepository,
        _authRepository = authRepository;

  Future<void> load() async {
    setLoading();
    try {
      _user = await _userRepository.getUser(userId);
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

  Future<void> updatePhone(String phone) async {
    setLoading();
    _updateSuccess = false;
    try {
      _user = await _authRepository.updateProfile(
        userId,
        UserUpdateDto(telefon: phone),
      );
      _updateSuccess = true;
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

  Future<void> deleteAccount() async {
    setLoading();
    _deleteSuccess = false;
    try {
      await _authRepository.deleteAccount(userId);
      _deleteSuccess = true;
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

  String _friendlyApiMessage(ApiException e) {
    return switch (e) {
      NotFoundException() => AppStrings.errorNotFound,
      NetworkException() => AppStrings.errorNetwork,
      ServerException() => 'Sunucu hatası. Lütfen daha sonra tekrar deneyin.',
      _ => e.message,
    };
  }
}
