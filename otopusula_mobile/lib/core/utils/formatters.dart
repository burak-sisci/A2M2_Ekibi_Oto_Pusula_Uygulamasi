import 'package:intl/intl.dart';

class Formatters {
  Formatters._();

  static final _priceFormat = NumberFormat.currency(
    locale: 'tr_TR',
    symbol: '₺',
    decimalDigits: 0,
  );

  static final _dateFormat = DateFormat('dd MMM yyyy', 'tr_TR');

  // ₺850.000
  static String price(num value) => _priceFormat.format(value);

  // 15 Şub 2026
  static String date(DateTime dt) => _dateFormat.format(dt);

  // 2 saat önce / 3 gün önce
  static String relativeDate(DateTime dt) {
    final diff = DateTime.now().difference(dt);
    if (diff.inMinutes < 1) return 'Az önce';
    if (diff.inMinutes < 60) return '${diff.inMinutes} dakika önce';
    if (diff.inHours < 24) return '${diff.inHours} saat önce';
    if (diff.inDays < 7) return '${diff.inDays} gün önce';
    if (diff.inDays < 30) return '${(diff.inDays / 7).floor()} hafta önce';
    if (diff.inDays < 365) return '${(diff.inDays / 30).floor()} ay önce';
    return '${(diff.inDays / 365).floor()} yıl önce';
  }

  // 45.000 km
  static String km(int value) =>
      '${NumberFormat('#,###', 'tr_TR').format(value)} km';

  // Yüzde gösterimi: 0.87 → %87
  static String percent(double value) =>
      '%${(value * 100).toStringAsFixed(0)}';
}
