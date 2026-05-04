import 'dart:io';
import 'package:dio/dio.dart';

class CarCreateDto {
  final String brand;
  final String model;
  final int year;
  final int km;
  final String fuelType;
  final String gearType;
  final int price;
  final String city;
  final String district;
  final String description;
  final List<String> damageInfo;
  // Yerel dosya yolları — max 8, 5 MB/dosya (developer.md §12)
  // TODO: image_picker paketi onaylı listede değil; UI'da dosya yolu manuel set edilir
  final List<String> imagePaths;

  const CarCreateDto({
    required this.brand,
    required this.model,
    required this.year,
    required this.km,
    required this.fuelType,
    required this.gearType,
    required this.price,
    required this.city,
    required this.district,
    required this.description,
    this.damageInfo = const [],
    this.imagePaths = const [],
  });

  Future<FormData> toFormData() async {
    final formData = FormData();
    formData.fields.addAll([
      MapEntry('brand', brand),
      MapEntry('model', model),
      MapEntry('year', year.toString()),
      MapEntry('km', km.toString()),
      MapEntry('fuelType', fuelType),
      MapEntry('gearType', gearType),
      MapEntry('price', price.toString()),
      MapEntry('location', '{"city":"$city","district":"$district"}'),
      MapEntry('description', description),
      MapEntry('damageInfo', damageInfo.join(',')),
    ]);

    for (final path in imagePaths) {
      final file = File(path);
      final sizeBytes = await file.length();
      if (sizeBytes > 5 * 1024 * 1024) {
        throw ArgumentError('Fotoğraf 5 MB sınırını aşıyor: $path');
      }
      formData.files.add(
        MapEntry('images', await MultipartFile.fromFile(path)),
      );
    }
    return formData;
  }
}
