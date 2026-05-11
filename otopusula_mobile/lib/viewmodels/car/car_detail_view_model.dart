import 'package:dio/dio.dart';
import '../../core/constants/app_strings.dart';
import '../../core/network/exceptions.dart';
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

  void resetDeleteSuccess() {
    _deleteSuccess = false;
  }

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

  String? _shareLinkError;
  String? get shareLinkError => _shareLinkError;

  Future<void> fetchShareLink() async {
    _shareLinkError = null;
    try {
      _shareLink = await _commentRepository.getShareLink(carId);
      notifyListeners();
    } on DioException catch (e) {
      final apiError = e.error;
      _shareLinkError = apiError is ApiException
          ? _friendlyApiMessage(apiError)
          : 'Paylaşım linki alınamadı.';
      notifyListeners();
    } on Exception {
      _shareLinkError = 'Paylaşım linki alınamadı.';
      notifyListeners();
    }
  }

  Future<void> deleteCar() async {
    setLoading();
    _deleteSuccess = false;
    try {
      await _carRepository.deleteCar(carId);
      _deleteSuccess = true;
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
      NotFoundException() => AppStrings.errorNotFound,
      ForbiddenException() => AppStrings.errorForbidden,
      NetworkException() => AppStrings.errorNetwork,
      ServerException() => 'Sunucu hatası. Lütfen daha sonra tekrar deneyin.',
      _ => e.message,
    };
  }
}
