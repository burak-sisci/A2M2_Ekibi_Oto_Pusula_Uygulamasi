class PricePredictDto {
  final String brand;
  final String model;
  final int year;
  final int km;
  final String fuelType;
  final String gearType;

  const PricePredictDto({
    required this.brand,
    required this.model,
    required this.year,
    required this.km,
    required this.fuelType,
    required this.gearType,
  });

  Map<String, dynamic> toJson() => {
        'brand': brand,
        'model': model,
        'year': year,
        'km': km,
        'fuelType': fuelType,
        'gearType': gearType,
      };
}
