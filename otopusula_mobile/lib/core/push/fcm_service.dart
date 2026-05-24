import 'package:firebase_core/firebase_core.dart';
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/foundation.dart';
import '../../data/repositories/device_repository.dart';

/// Firebase Cloud Messaging entegrasyonu.
/// - Uygulama açılışında Firebase.initializeApp() çağrılır (main.dart).
/// - Login sonrası registerForUser() çağrılır → izin alır, token alır, backend'e kaydeder.
/// - Logout öncesi unregisterCurrentToken() çağrılır.
class FcmService {
  final DeviceRepository _deviceRepository;
  String? _currentToken;

  FcmService({required DeviceRepository deviceRepository})
      : _deviceRepository = deviceRepository;

  /// Firebase'i başlat. main.dart'tan tek seferlik çağrılır.
  static Future<void> initFirebase() async {
    try {
      await Firebase.initializeApp();
      debugPrint('[FCM] Firebase initialized.');
    } catch (e) {
      debugPrint('[FCM] Firebase init hatası: $e');
    }
  }

  /// Login sonrası: izin iste, token al, backend'e kaydet, token yenileme dinleyicisi kur.
  Future<void> registerForUser() async {
    try {
      final messaging = FirebaseMessaging.instance;

      // Android 13+ runtime izni + iOS izin akışı
      final settings = await messaging.requestPermission(alert: true, badge: true, sound: true);
      debugPrint('[FCM] Notification permission: ${settings.authorizationStatus}');

      if (settings.authorizationStatus == AuthorizationStatus.denied) {
        debugPrint('[FCM] Bildirim izni reddedildi.');
        return;
      }

      final token = await messaging.getToken();
      if (token == null) {
        debugPrint('[FCM] getToken null döndü.');
        return;
      }

      _currentToken = token;
      debugPrint('[FCM] FCM token (suffix): ...${token.substring(token.length - 12)}');

      await _deviceRepository.register(token);
      debugPrint('[FCM] Backend /devices/register OK.');

      // Token yenilendiğinde otomatik tekrar kaydet
      messaging.onTokenRefresh.listen((newToken) async {
        _currentToken = newToken;
        try {
          await _deviceRepository.register(newToken);
          debugPrint('[FCM] Token yenilendi, backend güncellendi.');
        } catch (e) {
          debugPrint('[FCM] Token yenileme kaydı başarısız: $e');
        }
      });

      // Foreground bildirim akışı
      FirebaseMessaging.onMessage.listen(_handleForegroundMessage);
    } catch (e) {
      debugPrint('[FCM] registerForUser hatası: $e');
    }
  }

  /// Logout öncesi: backend'den cihazı sil, ardından FCM token'ı geçersiz kıl.
  Future<void> unregisterCurrentToken() async {
    final token = _currentToken ?? await FirebaseMessaging.instance.getToken();
    if (token == null) return;
    try {
      await _deviceRepository.unregister(token);
      debugPrint('[FCM] Backend /devices/unregister OK.');
    } catch (e) {
      debugPrint('[FCM] unregister hatası: $e');
    }
    try {
      await FirebaseMessaging.instance.deleteToken();
    } catch (e) {
      debugPrint('[FCM] deleteToken hatası: $e');
    }
    _currentToken = null;
  }

  void _handleForegroundMessage(RemoteMessage message) {
    final n = message.notification;
    debugPrint('[FCM] Foreground bildirim: ${n?.title} — ${n?.body}');
    // İlerleyen sürümlerde flutter_local_notifications ile ekrana banner basılabilir.
  }
}
