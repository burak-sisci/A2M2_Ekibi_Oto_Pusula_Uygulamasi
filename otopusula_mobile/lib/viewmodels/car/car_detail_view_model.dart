import '../../core/constants/app_strings.dart';
import '../../data/models/car.dart';
import '../../data/models/share_link.dart';
import '../../data/repositories/car_repository.dart';
import '../../data/repositories/comment_repository.dart';
import '../base_view_model.dart';

class CarDetailViewModel extends BaseViewModel {
  final CarRepository _carRepository;
  final CommentRepository _commentRepository;
  final String carId;

  Car? _car;
  ShareLink? _shareLink;
  bool _deleteSuccess = false;

  Car? get car => _car;
  ShareLink? get shareLink => _shareLink;
  bool get deleteSuccess => _deleteSuccess;

  CarDetailViewModel({
    required CarRepository carRepository,
    required CommentRepository commentRepository,
    required this.carId,
  })  : _carRepository = carRepository,
        _commentRepository = commentRepository;

  Future<void> load() async {
    setLoading();
    try {
      _car = await _carRepository.getCar(carId);
      setSuccess();
    } on Exception catch (e) {
      setError(_friendlyMessage(e));
    }
  }

  Future<void> fetchShareLink() async {
    try {
      _shareLink = await _commentRepository.getShareLink(carId);
      notifyListeners();
    } on Exception {
      // Paylaşım linki arka planda; hata kullanıcıya iletilebilir
    }
  }

  Future<void> deleteCar() async {
    setLoading();
    _deleteSuccess = false;
    try {
      await _carRepository.deleteCar(carId);
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
    if (msg.contains('403') || msg.contains('forbidden')) return AppStrings.errorForbidden;
    return AppStrings.errorGeneric;
  }
}
