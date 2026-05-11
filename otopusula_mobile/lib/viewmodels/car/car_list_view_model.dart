import 'package:dio/dio.dart';
import '../../core/constants/app_constants.dart';
import '../../core/constants/app_strings.dart';
import '../../core/network/exceptions.dart';
import '../../data/models/car.dart';
import '../../data/repositories/car_repository.dart';
import '../base_view_model.dart';

class CarListViewModel extends BaseViewModel {
  final CarRepository _carRepository;

  final List<Car> _cars = [];
  CarFilterParams _filter = const CarFilterParams();
  int _currentPage = 1;
  bool _hasMore = true;
  bool _isFetchingMore = false;

  List<Car> get cars => List.unmodifiable(_cars);
  bool get hasMore => _hasMore;
  bool get isFetchingMore => _isFetchingMore;
  CarFilterParams get filter => _filter;

  CarListViewModel({required CarRepository carRepository})
      : _carRepository = carRepository;

  Future<void> load([CarFilterParams? filter]) async {
    _filter = filter ?? const CarFilterParams();
    _currentPage = 1;
    _hasMore = true;
    _cars.clear();
    setLoading();
    try {
      final result = await _carRepository.listCars(_filter);
      _cars.addAll(result);
      _hasMore = result.length == AppConstants.defaultPageSize;
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

  Future<void> loadMore() async {
    if (!_hasMore || _isFetchingMore) return;
    _isFetchingMore = true;
    notifyListeners();
    try {
      _currentPage++;
      final result = await _carRepository.listCars(
        CarFilterParams(
          brand: _filter.brand,
          model: _filter.model,
          minPrice: _filter.minPrice,
          maxPrice: _filter.maxPrice,
          minKm: _filter.minKm,
          maxKm: _filter.maxKm,
          fuelType: _filter.fuelType,
          gearType: _filter.gearType,
          year: _filter.year,
          city: _filter.city,
          page: _currentPage,
          limit: _filter.limit,
        ),
      );
      _cars.addAll(result);
      _hasMore = result.length == AppConstants.defaultPageSize;
    } on Exception {
      _currentPage--;
    } finally {
      _isFetchingMore = false;
      notifyListeners();
    }
  }

  Future<void> applyFilter(CarFilterParams filter) => load(filter);

  String _friendlyApiMessage(ApiException e) {
    return switch (e) {
      NetworkException() => AppStrings.errorNetwork,
      ServerException() => 'Sunucu hatası. Lütfen daha sonra tekrar deneyin.',
      _ => e.message,
    };
  }
}
