class AppConstants {
  AppConstants._();

  // Ortam bazlı API adresi — varsayılan Android emülatör adresidir.
  // Android emülatör : flutter run (varsayılan)
  // iOS simülatör    : flutter run --dart-define=API_BASE_URL=http://localhost:8081
  // Fiziksel cihaz   : flutter run --dart-define=API_BASE_URL=http://192.168.x.x:8081
  // Production       : flutter run --dart-define=API_BASE_URL=https://api.otopusula.com
  static const baseUrl = String.fromEnvironment(
    'API_BASE_URL',
    defaultValue: 'http://10.0.2.2:8081',
  );

  // Sayfalandırma
  static const defaultPageSize = 10;
  static const maxPageSize = 50;
  static const paginationScrollThreshold = 200.0; // px kala sonraki sayfa

  // Araç ilanı görsel sınırları (developer.md §12)
  static const maxCarImages = 8;
  static const maxImageSizeBytes = 5 * 1024 * 1024; // 5 MB

  // Boşluk skalası (8pt grid)
  static const space4 = 4.0;
  static const space8 = 8.0;
  static const space12 = 12.0;
  static const space16 = 16.0;
  static const space24 = 24.0;
  static const space32 = 32.0;

  // Köşe yuvarlama
  static const radiusCard = 8.0;
  static const radiusSheet = 12.0;
  static const radiusPill = 999.0;

  // GoRouter segment sabiti (app.dart'ta iç rotada kullanılır)
  static const carsSegment = 'cars';

  // Minimum dokunma hedefi (dp)
  static const minTapTarget = 48.0;
}
