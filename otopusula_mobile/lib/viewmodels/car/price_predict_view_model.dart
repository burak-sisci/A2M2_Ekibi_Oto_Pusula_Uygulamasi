import '../../core/constants/app_strings.dart';
import '../../data/dto/price_predict_dto.dart';
import '../../data/models/price_predict.dart';
import '../../data/repositories/ai_repository.dart';
import '../base_view_model.dart';

class PricePredictViewModel extends BaseViewModel {
  final AiRepository _aiRepository;

  PricePredict? _result;
  PricePredict? get result => _result;

  PricePredictViewModel({required AiRepository aiRepository})
      : _aiRepository = aiRepository;

  Future<void> predict({
    required String brand,
    required String model,
    required int year,
    required int km,
    required String fuelType,
    required String gearType,
  }) async {
    setLoading();
    _result = null;
    try {
      _result = await _aiRepository.predictPrice(
        PricePredictDto(
          brand: brand,
          model: model,
          year: year,
          km: km,
          fuelType: fuelType,
          gearType: gearType,
        ),
      );
      setSuccess();
    } on Exception catch (e) {
      setError(_friendlyMessage(e));
    }
  }

  String _friendlyMessage(Exception e) {
    final msg = e.toString().toLowerCase();
    if (msg.contains('network') || msg.contains('connection')) return AppStrings.errorNetwork;
    if (msg.contains('500') || msg.contains('server')) return 'Yapay zeka servisi şu an kullanılamıyor.';
    return AppStrings.errorGeneric;
  }
}
