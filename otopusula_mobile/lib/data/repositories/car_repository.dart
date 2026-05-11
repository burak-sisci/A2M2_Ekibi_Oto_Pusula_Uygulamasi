import 'package:dio/dio.dart';
import '../../core/network/api_client.dart';
import '../../core/constants/api_endpoints.dart';
import '../../core/constants/app_constants.dart';
import '../dto/car_create_dto.dart';
import '../models/car.dart';

class CarFilterParams {
  final String? brand;
  final String? model;
  final int? minPrice;
  final int? maxPrice;
  final int? minKm;
  final int? maxKm;
  final String? fuelType;
  final String? gearType;
  final int? year;
  final String? city;
  final int page;
  final int limit;

  const CarFilterParams({
    this.brand,
    this.model,
    this.minPrice,
    this.maxPrice,
    this.minKm,
    this.maxKm,
    this.fuelType,
    this.gearType,
    this.year,
    this.city,
    this.page = 1,
    this.limit = AppConstants.defaultPageSize,
  });

  // Backend: GET /cars?limit=&offset=&brand=&model=&location=&minPrice=&maxPrice=&minKm=&maxKm=&fuelType=&gearType=&year=
  Map<String, dynamic> toQueryParameters() => {
        if (brand != null) 'brand': brand,
        if (model != null) 'model': model,
        if (minPrice != null) 'minPrice': minPrice.toString(),
        if (maxPrice != null) 'maxPrice': maxPrice.toString(),
        if (minKm != null) 'minKm': minKm.toString(),
        if (maxKm != null) 'maxKm': maxKm.toString(),
        if (fuelType != null) 'fuelType': fuelType,
        if (gearType != null) 'gearType': gearType,
        if (year != null) 'year': year.toString(),
        if (city != null) 'location': city,
        'limit': limit.toString(),
        'offset': ((page - 1) * limit).toString(),
      };
}

class CarRepository {
  final ApiClient _apiClient;

  CarRepository({required ApiClient apiClient}) : _apiClient = apiClient;

  Future<List<Car>> listCars(CarFilterParams params) async {
    final response = await _apiClient.dio.get(
      ApiEndpoints.cars,
      queryParameters: params.toQueryParameters(),
    );
    final data = response.data;
    // Backend PagedResult veya düz liste dönebilir; her ikisini de destekle
    final List<dynamic> items =
        data is List ? data : ((data as Map<String, dynamic>)['items'] ?? data['data'] ?? []);
    return items.map((e) => Car.fromJson(e as Map<String, dynamic>)).toList();
  }

  Future<Car> getCar(String carId) async {
    final response = await _apiClient.dio.get(ApiEndpoints.car(carId));
    return Car.fromJson(response.data as Map<String, dynamic>);
  }

  Future<Car> createCar(CarCreateDto dto, List<String> uploadedUrls) async {
    if (dto.imagePaths.length > AppConstants.maxCarImages) {
      throw ArgumentError('En fazla ${AppConstants.maxCarImages} fotoğraf yüklenebilir.');
    }
    final response = await _apiClient.dio.post(
      ApiEndpoints.cars, 
      data: dto.toJson(uploadedUrls)
    );
    return Car.fromJson(response.data as Map<String, dynamic>);
  }

  Future<Car> updateCar(String carId, Map<String, dynamic> fields) async {
    final response = await _apiClient.dio.put(ApiEndpoints.car(carId), data: fields);
    return Car.fromJson(response.data as Map<String, dynamic>);
  }

  Future<String> uploadImage(String imagePath) async {
    final formData = FormData.fromMap({
      'file': await MultipartFile.fromFile(imagePath),
    });
    final response = await _apiClient.dio.post('/api/upload/image', data: formData);
    return response.data['url'] as String;
  }

  Future<void> deleteCar(String carId) async {
    await _apiClient.dio.delete(ApiEndpoints.car(carId));
  }
}
