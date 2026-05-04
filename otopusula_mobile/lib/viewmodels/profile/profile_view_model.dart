import '../../core/constants/app_strings.dart';
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
    } on Exception catch (e) {
      setError(_friendlyMessage(e));
    }
  }

  Future<void> updatePhone(String phone) async {
    setLoading();
    _updateSuccess = false;
    try {
      _user = await _authRepository.updateProfile(phone);
      _updateSuccess = true;
      setSuccess();
    } on Exception catch (e) {
      setError(_friendlyMessage(e));
    }
  }

  Future<void> deleteAccount() async {
    setLoading();
    _deleteSuccess = false;
    try {
      await _authRepository.deleteAccount(userId);
      _deleteSuccess = true;
      setSuccess();
    } on Exception catch (e) {
      setError(_friendlyMessage(e));
    }
  }

  String _friendlyMessage(Exception e) {
    final msg = e.toString().toLowerCase();
    if (msg.contains('network') || msg.contains('connection')) return AppStrings.errorNetwork;
    if (msg.contains('404') || msg.contains('notfound')) return AppStrings.errorNotFound;
    return AppStrings.errorGeneric;
  }
}
