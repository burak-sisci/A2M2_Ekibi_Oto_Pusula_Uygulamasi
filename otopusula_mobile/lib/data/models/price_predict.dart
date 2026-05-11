// Backend: {durum, tahminSonucu: {fiyatEtiketi, birim}}
class PricePredict {
  final String status;
  final String priceLabel;
  final String unit;

  const PricePredict({
    required this.status,
    required this.priceLabel,
    required this.unit,
  });

  factory PricePredict.fromJson(Map<String, dynamic> json) {
    final tahmin = json['tahminSonucu'] as Map<String, dynamic>? ?? {};
    return PricePredict(
      status: json['durum'] as String? ?? '',
      priceLabel: tahmin['fiyatEtiketi'] as String? ?? '',
      unit: tahmin['birim'] as String? ?? 'TL',
    );
  }

  Map<String, dynamic> toJson() => {
        'durum': status,
        'tahminSonucu': {
          'fiyatEtiketi': priceLabel,
          'birim': unit,
        },
      };

  PricePredict copyWith({String? status, String? priceLabel, String? unit}) =>
      PricePredict(
        status: status ?? this.status,
        priceLabel: priceLabel ?? this.priceLabel,
        unit: unit ?? this.unit,
      );
}
