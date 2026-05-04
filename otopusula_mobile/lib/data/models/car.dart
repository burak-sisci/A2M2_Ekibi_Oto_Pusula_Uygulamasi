class CarLocation {
  final String city;
  final String district;

  const CarLocation({required this.city, required this.district});

  factory CarLocation.fromJson(Map<String, dynamic> json) => CarLocation(
        city: json['city'] as String,
        district: json['district'] as String,
      );

  Map<String, dynamic> toJson() => {'city': city, 'district': district};

  CarLocation copyWith({String? city, String? district}) =>
      CarLocation(city: city ?? this.city, district: district ?? this.district);
}

class Car {
  final String id;
  final String userId;
  final String brand;
  final String model;
  final int year;
  final int km;
  final String fuelType;
  final String gearType;
  final int price;
  final CarLocation location;
  final String description;
  final List<String> images;
  final List<String> damageInfo;
  final DateTime createdAt;
  final DateTime updatedAt;

  const Car({
    required this.id,
    required this.userId,
    required this.brand,
    required this.model,
    required this.year,
    required this.km,
    required this.fuelType,
    required this.gearType,
    required this.price,
    required this.location,
    required this.description,
    required this.images,
    required this.damageInfo,
    required this.createdAt,
    required this.updatedAt,
  });

  factory Car.fromJson(Map<String, dynamic> json) => Car(
        id: json['_id'] as String,
        userId: json['userId'] as String? ?? '',
        brand: json['brand'] as String,
        model: json['model'] as String,
        year: json['year'] as int,
        km: json['km'] as int,
        fuelType: json['fuelType'] as String,
        gearType: json['gearType'] as String,
        price: json['price'] as int,
        location: CarLocation.fromJson(json['location'] as Map<String, dynamic>),
        description: json['description'] as String? ?? '',
        images: (json['images'] as List<dynamic>? ?? []).cast<String>(),
        damageInfo: (json['damageInfo'] as List<dynamic>? ?? []).cast<String>(),
        createdAt: DateTime.parse(json['createdAt'] as String),
        updatedAt: DateTime.parse(json['updatedAt'] as String),
      );

  Map<String, dynamic> toJson() => {
        '_id': id,
        'userId': userId,
        'brand': brand,
        'model': model,
        'year': year,
        'km': km,
        'fuelType': fuelType,
        'gearType': gearType,
        'price': price,
        'location': location.toJson(),
        'description': description,
        'images': images,
        'damageInfo': damageInfo,
        'createdAt': createdAt.toIso8601String(),
        'updatedAt': updatedAt.toIso8601String(),
      };

  Car copyWith({
    String? id,
    String? userId,
    String? brand,
    String? model,
    int? year,
    int? km,
    String? fuelType,
    String? gearType,
    int? price,
    CarLocation? location,
    String? description,
    List<String>? images,
    List<String>? damageInfo,
    DateTime? createdAt,
    DateTime? updatedAt,
  }) =>
      Car(
        id: id ?? this.id,
        userId: userId ?? this.userId,
        brand: brand ?? this.brand,
        model: model ?? this.model,
        year: year ?? this.year,
        km: km ?? this.km,
        fuelType: fuelType ?? this.fuelType,
        gearType: gearType ?? this.gearType,
        price: price ?? this.price,
        location: location ?? this.location,
        description: description ?? this.description,
        images: images ?? this.images,
        damageInfo: damageInfo ?? this.damageInfo,
        createdAt: createdAt ?? this.createdAt,
        updatedAt: updatedAt ?? this.updatedAt,
      );
}
