import 'package:dio/dio.dart';
import '../../core/constants/app_strings.dart';
import '../../core/network/exceptions.dart';
import '../../data/dto/user_login_dto.dart';
import '../../data/repositories/auth_repository.dart';
import '../base_view_model.dart';

class LoginViewModel extends BaseViewModel {
  final AuthRepository _authRepository;

  // View bu alanları okur; callback ile navigate eder
  bool _loginSuccess = false;
  String? _token;
  String? _userId;

  bool get loginSuccess => _loginSuccess;
  String? get token => _token;
  String? get userId => _userId;

  LoginViewModel({required AuthRepository authRepository})
      : _authRepository = authRepository;

  Future<void> login(String identifier, String password) async {
    setLoading();
    _loginSuccess = false;
    try {
      final data = await _authRepository.login(
        UserLoginDto(identifier: identifier, password: password),
      );
      _token = data['token'] as String?;
      // Backend: {kullaniciId, email, ad, token}
      _userId = data['kullaniciId'] as String?
          ?? data['userId'] as String?
          ?? data['_id'] as String?;
      _loginSuccess = true;
      setSuccess();
    } on DioException catch (e) {
      // ErrorInterceptor, DioException.error alanına ApiException koyar
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
      UnauthorizedException() || BadRequestException() =>
        'E-posta/telefon veya şifre hatalı.',
      NetworkException() => AppStrings.errorNetwork,
      ServerException() => 'Sunucu hatası. Lütfen daha sonra tekrar deneyin.',
      _ => e.message,
    };
  }
}
