import 'package:dio/dio.dart';
import '../../core/constants/app_strings.dart';
import '../../core/network/exceptions.dart';
import '../../data/dto/user_register_dto.dart';
import '../../data/repositories/auth_repository.dart';
import '../base_view_model.dart';

class RegisterViewModel extends BaseViewModel {
  final AuthRepository _authRepository;

  bool _registerSuccess = false;
  String? _token;
  String? _userId;

  bool get registerSuccess => _registerSuccess;
  String? get token => _token;
  String? get userId => _userId;

  RegisterViewModel({required AuthRepository authRepository})
      : _authRepository = authRepository;

  Future<void> register({
    required String name,
    required String email,
    required String phone,
    required String password,
    String? gender,
    String? birthDate,
  }) async {
    setLoading();
    _registerSuccess = false;
    try {
      final data = await _authRepository.register(
        UserRegisterDto(
          name: name,
          email: email,
          phone: phone,
          password: password,
          gender: gender,
          birthDate: birthDate,
        ),
      );
      _token = data['token'] as String?;
      // Backend: {kullaniciId, email, ad, token}
      _userId = data['kullaniciId'] as String?
          ?? data['userId'] as String?
          ?? data['_id'] as String?;
      _registerSuccess = true;
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
      ConflictException() => 'Bu e-posta veya telefon zaten kayıtlı.',
      NetworkException() => AppStrings.errorNetwork,
      BadRequestException() => 'Girdiğiniz bilgileri kontrol edin.',
      ServerException() => 'Sunucu hatası. Lütfen daha sonra tekrar deneyin.',
      _ => e.message,
    };
  }
}
