import '../../core/constants/app_constants.dart';
import '../../core/constants/app_strings.dart';
import '../../data/dto/car_create_dto.dart';
import '../../data/models/car.dart';
import '../../data/repositories/car_repository.dart';
import '../base_view_model.dart';

class CarCreateViewModel extends BaseViewModel {
  final CarRepository _carRepository;

  Car? _createdCar;
  // TODO: image_picker paketi onaylı listede değil.
  // Dosya yolları; tam akış için image_picker eklendiğinde bu liste doldurulacak.
  final List<String> _imagePaths = [];

  Car? get createdCar => _createdCar;
  List<String> get imagePaths => List.unmodifiable(_imagePaths);
  bool get canAddMoreImages => _imagePaths.length < AppConstants.maxCarImages;

  CarCreateViewModel({required CarRepository carRepository})
      : _carRepository = carRepository;

  void addImagePath(String path) {
    if (_imagePaths.length >= AppConstants.maxCarImages) return;
    _imagePaths.add(path);
    notifyListeners();
  }

  void removeImagePath(int index) {
    if (index < 0 || index >= _imagePaths.length) return;
    _imagePaths.removeAt(index);
    notifyListeners();
  }

  Future<void> createCar({
    required String brand,
    required String model,
    required int year,
    required int km,
    required String fuelType,
    required String gearType,
    required int price,
    required String city,
    required String district,
    required String description,
    List<String> damageInfo = const [],
  }) async {
    setLoading();
    try {
      final dto = CarCreateDto(
        brand: brand,
        model: model,
        year: year,
        km: km,
        fuelType: fuelType,
        gearType: gearType,
        price: price,
        city: city,
        district: district,
        description: description,
        damageInfo: damageInfo,
        imagePaths: List.from(_imagePaths),
      );
      _createdCar = await _carRepository.createCar(dto);
      setSuccess();
    } on Exception catch (e) {
      setError(_friendlyMessage(e));
    }
  }

  String _friendlyMessage(Exception e) {
    final msg = e.toString().toLowerCase();
    if (msg.contains('network') || msg.contains('connection')) return AppStrings.errorNetwork;
    if (msg.contains('5 mb') || msg.contains('sınırını')) return e.toString();
    return AppStrings.errorGeneric;
  }
}
