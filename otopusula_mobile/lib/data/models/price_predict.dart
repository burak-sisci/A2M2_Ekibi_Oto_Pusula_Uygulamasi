class PriceRange {
  final int min;
  final int max;

  const PriceRange({required this.min, required this.max});

  factory PriceRange.fromJson(Map<String, dynamic> json) => PriceRange(
        min: json['min'] as int,
        max: json['max'] as int,
      );

  Map<String, dynamic> toJson() => {'min': min, 'max': max};

  PriceRange copyWith({int? min, int? max}) =>
      PriceRange(min: min ?? this.min, max: max ?? this.max);
}

class PricePredict {
  final int estimatedPrice;
  final PriceRange priceRange;
  final double confidence;
  final DateTime generatedAt;

  const PricePredict({
    required this.estimatedPrice,
    required this.priceRange,
    required this.confidence,
    required this.generatedAt,
  });

  factory PricePredict.fromJson(Map<String, dynamic> json) => PricePredict(
        estimatedPrice: json['estimatedPrice'] as int,
        priceRange: PriceRange.fromJson(json['priceRange'] as Map<String, dynamic>),
        confidence: (json['confidence'] as num).toDouble(),
        generatedAt: DateTime.parse(json['generatedAt'] as String),
      );

  Map<String, dynamic> toJson() => {
        'estimatedPrice': estimatedPrice,
        'priceRange': priceRange.toJson(),
        'confidence': confidence,
        'generatedAt': generatedAt.toIso8601String(),
      };

  PricePredict copyWith({
    int? estimatedPrice,
    PriceRange? priceRange,
    double? confidence,
    DateTime? generatedAt,
  }) =>
      PricePredict(
        estimatedPrice: estimatedPrice ?? this.estimatedPrice,
        priceRange: priceRange ?? this.priceRange,
        confidence: confidence ?? this.confidence,
        generatedAt: generatedAt ?? this.generatedAt,
      );
}
