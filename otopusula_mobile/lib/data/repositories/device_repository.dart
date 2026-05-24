import '../../core/network/api_client.dart';
import '../../core/constants/api_endpoints.dart';

/// FCM push notification cihaz yönetimi.
/// Backend `/devices/register` ve `/devices/unregister` endpoint'leriyle konuşur.
class DeviceRepository {
  final ApiClient _apiClient;

  DeviceRepository({required ApiClient apiClient}) : _apiClient = apiClient;

  Future<void> register(String fcmToken, {String platform = 'android'}) async {
    await _apiClient.dio.post(
      ApiEndpoints.devicesRegister,
      data: {'FcmToken': fcmToken, 'Platform': platform},
    );
  }

  Future<void> unregister(String fcmToken) async {
    await _apiClient.dio.delete(
      ApiEndpoints.devicesUnregister,
      data: {'FcmToken': fcmToken},
    );
  }
}
