import '../../core/network/api_client.dart';
import '../../core/constants/api_endpoints.dart';
import '../dto/price_predict_dto.dart';
import '../models/price_predict.dart';

class AiRepository {
  final ApiClient _apiClient;

  AiRepository({required ApiClient apiClient}) : _apiClient = apiClient;

  Future<PricePredict> predictPrice(PricePredictDto dto) async {
    final response = await _apiClient.dio.post(
      ApiEndpoints.predictPrice,
      data: dto.toJson(),
    );
    return PricePredict.fromJson(response.data as Map<String, dynamic>);
  }
}
