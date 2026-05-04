import '../../core/constants/app_strings.dart';
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
      _userId = data['userId'] as String? ?? data['_id'] as String?;
      _loginSuccess = true;
      setSuccess();
    } on Exception catch (e) {
      setError(_friendlyMessage(e));
    }
  }

  String _friendlyMessage(Exception e) {
    final msg = e.toString().toLowerCase();
    if (msg.contains('network') || msg.contains('connection')) {
      return AppStrings.errorNetwork;
    }
    if (msg.contains('401') || msg.contains('unauthorized')) {
      return 'E-posta/telefon veya şifre hatalı.';
    }
    return AppStrings.errorGeneric;
  }
}
