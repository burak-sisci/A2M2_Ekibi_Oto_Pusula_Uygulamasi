import 'package:dio/dio.dart';
import 'package:image_picker/image_picker.dart';
import '../../core/constants/app_constants.dart';
import '../../core/constants/app_strings.dart';
import '../../core/network/exceptions.dart';
import '../../data/dto/car_create_dto.dart';
import '../../data/models/car.dart';
import '../../data/repositories/car_repository.dart';
import '../base_view_model.dart';

class CarCreateViewModel extends BaseViewModel {
  final CarRepository _carRepository;
  final ImagePicker _imagePicker = ImagePicker();

  Car? _createdCar;
  final List<String> _imageUrls = [];
  bool _isUploading = false;

  Car? get createdCar => _createdCar;
  List<String> get imageUrls => List.unmodifiable(_imageUrls);
  bool get canAddMoreImages => _imageUrls.length < AppConstants.maxCarImages;
  bool get isUploading => _isUploading;

  CarCreateViewModel({required CarRepository carRepository})
      : _carRepository = carRepository;

  Future<void> pickAndUploadImage() async {
    if (!canAddMoreImages) return;
    final picked = await _imagePicker.pickImage(
      source: ImageSource.gallery,
      imageQuality: 85,
    );
    if (picked == null) return;

    _isUploading = true;
    notifyListeners();
    try {
      final url = await _carRepository.uploadImage(picked.path);
      _imageUrls.add(url);
    } on Exception {
      // Yükleme hatası sessizce geçilir; kullanıcı tekrar deneyebilir
    } finally {
      _isUploading = false;
      notifyListeners();
    }
  }

  void removeImageUrl(int index) {
    if (index < 0 || index >= _imageUrls.length) return;
    _imageUrls.removeAt(index);
    notifyListeners();
  }

  Future<void> createCar({
    required String brand,
    String seri = '',
    required String model,
    required int year,
    required int km,
    required String fuelType,
    required String gearType,
    String kasaTipi = 'Sedan',
    String renk = '',
    double motorHacmi = 0.0,
    int motorGucu = 0,
    String cekis = 'ÖndenÇekiş',
    String aracDurumu = 'İkinciEl',
    double ortalamaYakitTuketim = 0.0,
    int yakitDeposu = 0,
    bool agirHasarKaydi = false,
    bool takasaUygun = false,
    String kimden = 'Sahibinden',
    required int price,
    required String city,
    required String district,
    required String description,
    Map<String, String> boyaliDegisen = const {},
  }) async {
    setLoading();
    try {
      final dto = CarCreateDto(
        brand: brand,
        seri: seri,
        model: model,
        year: year,
        km: km,
        fuelType: fuelType,
        gearType: gearType,
        kasaTipi: kasaTipi,
        renk: renk,
        motorHacmi: motorHacmi,
        motorGucu: motorGucu,
        cekis: cekis,
        aracDurumu: aracDurumu,
        ortalamaYakitTuketim: ortalamaYakitTuketim,
        yakitDeposu: yakitDeposu,
        agirHasarKaydi: agirHasarKaydi,
        takasaUygun: takasaUygun,
        kimden: kimden,
        price: price,
        city: city,
        district: district,
        description: description,
        boyaliDegisen: boyaliDegisen,
        imagePaths: List.from(_imageUrls),
      );
      _createdCar = await _carRepository.createCar(dto, List.from(_imageUrls));
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
    } on Exception catch (e) {
      final msg = e.toString().toLowerCase();
      if (msg.contains('5 mb') || msg.contains('sınırını')) {
        setError(e.toString());
      } else {
        setError(AppStrings.errorNetwork);
      }
    }
  }

  String _friendlyApiMessage(ApiException e) {
    return switch (e) {
      BadRequestException() => 'Eksik veya hatalı bilgi girdiniz.',
      NetworkException() => AppStrings.errorNetwork,
      ServerException() => 'Sunucu hatası. Lütfen daha sonra tekrar deneyin.',
      _ => e.message,
    };
  }
}
