import '../../core/constants/app_strings.dart';
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
      _userId = data['_id'] as String?;
      _registerSuccess = true;
      setSuccess();
    } on Exception catch (e) {
      setError(_friendlyMessage(e));
    }
  }

  String _friendlyMessage(Exception e) {
    final msg = e.toString().toLowerCase();
    if (msg.contains('conflict') || msg.contains('409')) {
      return 'Bu e-posta veya telefon zaten kayıtlı.';
    }
    if (msg.contains('network') || msg.contains('connection')) {
      return AppStrings.errorNetwork;
    }
    return AppStrings.errorGeneric;
  }
}
