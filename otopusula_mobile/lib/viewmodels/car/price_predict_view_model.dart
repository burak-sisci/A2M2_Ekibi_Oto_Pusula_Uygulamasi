import 'package:dio/dio.dart';
import '../../core/constants/app_strings.dart';
import '../../core/network/exceptions.dart';
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
    String aracDurumu = 'İkinciEl',
    String cekis = 'ÖndenÇekiş',
    double ortalamaYakitTuketim = 0.0,
    int yakitDeposu = 0,
    Map<String, String> boyaliDegisen = const {},
  }) async {
    setLoading();
    _result = null;
    try {
      _result = await _aiRepository.predictPrice(
        PricePredictDto(
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
          aracDurumu: aracDurumu,
          cekis: cekis,
          ortalamaYakitTuketim: ortalamaYakitTuketim,
          yakitDeposu: yakitDeposu,
          boyaliDegisen: boyaliDegisen,
        ),
      );
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

  String _friendlyApiMessage(ApiException e) {
    return switch (e) {
      BadRequestException() => 'Eksik veya hatalı bilgi girdiniz.',
      NetworkException() => AppStrings.errorNetwork,
      ServerException() => 'Yapay zeka servisi şu an kullanılmıyor.',
      _ => e.message,
    };
  }
}
