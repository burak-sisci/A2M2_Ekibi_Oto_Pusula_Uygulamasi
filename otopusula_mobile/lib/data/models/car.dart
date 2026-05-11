class CarLocation {
  final String city;
  final String district;

  const CarLocation({required this.city, required this.district});

  // Backend konum alanı "Şehir, İlçe" formatında tek string döner
  factory CarLocation.fromKonum(String konum) {
    final idx = konum.indexOf(', ');
    if (idx == -1) return CarLocation(city: konum, district: '');
    return CarLocation(
      city: konum.substring(0, idx),
      district: konum.substring(idx + 2),
    );
  }

  factory CarLocation.fromJson(Map<String, dynamic> json) => CarLocation(
        city: json['city'] as String? ?? '',
        district: json['district'] as String? ?? '',
      );

  Map<String, dynamic> toJson() => {'city': city, 'district': district};

  String toKonum() => district.isEmpty ? city : '$city, $district';

  CarLocation copyWith({String? city, String? district}) =>
      CarLocation(city: city ?? this.city, district: district ?? this.district);
}

class Car {
  final String id;
  final String userId;
  final String brand;
  final String series;
  final String model;
  final int year;
  final int km;
  final String fuelType;
  final String gearType;
  final String bodyType;
  final String color;
  final int price;
  final CarLocation location;
  final String description;
  final List<String> images;
  final List<String> damageInfo;
  final bool heavilyDamaged;
  final bool tradeEligible;
  final String sellerType;
  final DateTime createdAt;
  final DateTime? updatedAt;

  const Car({
    required this.id,
    required this.userId,
    required this.brand,
    this.series = '',
    required this.model,
    required this.year,
    required this.km,
    required this.fuelType,
    required this.gearType,
    this.bodyType = '',
    this.color = '',
    required this.price,
    required this.location,
    this.description = '',
    required this.images,
    this.damageInfo = const [],
    this.heavilyDamaged = false,
    this.tradeEligible = false,
    this.sellerType = '',
    required this.createdAt,
    this.updatedAt,
  });

  // Backend: {id, marka, seri, model, yil, fiyat, kilometre, vitesTipi, yakitTipi,
  //           kasaTipi, renk, ilanSahibi, konum, resimler, boyaliDegisen,
  //           agirHasarKaydi, takasaUygun, kimden, ilanTarihi}
  factory Car.fromJson(Map<String, dynamic> json) {
    final konum = json['konum'] as String? ?? '';
    final location = CarLocation.fromKonum(konum);

    // boyaliDegisen: Orijinal olmayan panelleri hasar listesine çevir
    final boyali = json['boyaliDegisen'] as Map<String, dynamic>?;
    final damageInfo = <String>[];
    if (boyali != null) {
      boyali.forEach((panel, status) {
        if (status != null && status.toString() != 'Orijinal') {
          damageInfo.add('$panel: $status');
        }
      });
    }

    return Car(
      id: json['id'] as String,
      userId: json['ilanSahibi'] as String? ?? '',
      brand: json['marka'] as String? ?? '',
      series: json['seri'] as String? ?? '',
      model: json['model'] as String? ?? '',
      year: (json['yil'] as num?)?.toInt() ?? 0,
      km: (json['kilometre'] as num?)?.toInt() ?? 0,
      fuelType: json['yakitTipi'] as String? ?? '',
      gearType: json['vitesTipi'] as String? ?? '',
      bodyType: json['kasaTipi'] as String? ?? '',
      color: json['renk'] as String? ?? '',
      price: (json['fiyat'] as num?)?.toInt() ?? 0,
      location: location,
      description: json['aciklama'] as String? ?? '',
      images: (json['resimler'] as List<dynamic>? ?? []).cast<String>(),
      damageInfo: damageInfo,
      heavilyDamaged: json['agirHasarKaydi'] as bool? ?? false,
      tradeEligible: json['takasaUygun'] as bool? ?? false,
      sellerType: json['kimden'] as String? ?? '',
      createdAt: json['ilanTarihi'] != null
          ? DateTime.parse(json['ilanTarihi'] as String)
          : DateTime.now(),
      updatedAt: json['guncellemeTarihi'] != null
          ? DateTime.parse(json['guncellemeTarihi'] as String)
          : null,
    );
  }

  Map<String, dynamic> toJson() => {
        'id': id,
        'ilanSahibi': userId,
        'marka': brand,
        'seri': series,
        'model': model,
        'yil': year,
        'kilometre': km,
        'yakitTipi': fuelType,
        'vitesTipi': gearType,
        'kasaTipi': bodyType,
        'renk': color,
        'fiyat': price,
        'konum': location.toKonum(),
        'aciklama': description,
        'resimler': images,
        'agirHasarKaydi': heavilyDamaged,
        'takasaUygun': tradeEligible,
        'kimden': sellerType,
        'ilanTarihi': createdAt.toIso8601String(),
      };

  Car copyWith({
    String? id,
    String? userId,
    String? brand,
    String? series,
    String? model,
    int? year,
    int? km,
    String? fuelType,
    String? gearType,
    String? bodyType,
    String? color,
    int? price,
    CarLocation? location,
    String? description,
    List<String>? images,
    List<String>? damageInfo,
    bool? heavilyDamaged,
    bool? tradeEligible,
    String? sellerType,
    DateTime? createdAt,
    DateTime? updatedAt,
  }) =>
      Car(
        id: id ?? this.id,
        userId: userId ?? this.userId,
        brand: brand ?? this.brand,
        series: series ?? this.series,
        model: model ?? this.model,
        year: year ?? this.year,
        km: km ?? this.km,
        fuelType: fuelType ?? this.fuelType,
        gearType: gearType ?? this.gearType,
        bodyType: bodyType ?? this.bodyType,
        color: color ?? this.color,
        price: price ?? this.price,
        location: location ?? this.location,
        description: description ?? this.description,
        images: images ?? this.images,
        damageInfo: damageInfo ?? this.damageInfo,
        heavilyDamaged: heavilyDamaged ?? this.heavilyDamaged,
        tradeEligible: tradeEligible ?? this.tradeEligible,
        sellerType: sellerType ?? this.sellerType,
        createdAt: createdAt ?? this.createdAt,
        updatedAt: updatedAt ?? this.updatedAt,
      );
}
