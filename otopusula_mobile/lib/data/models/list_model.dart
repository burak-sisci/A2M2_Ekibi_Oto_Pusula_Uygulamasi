// 'List' Dart anahtar kelimesi olduğu için dosya ve sınıf adı 'UserList'
import 'car.dart';

class UserList {
  final String id;
  final String name;
  final bool isDefault;
  // Detay görünümde repository tarafından doldurulan tam ilan nesneleri
  final List<Car> cars;
  // GET /lists/{id} yanıtından gelen ham ilan ID listesi
  final List<String> carIds;
  // Özet yanıtlarda (ilanSayisi) gelen ilan sayısı
  final int carCount;
  final DateTime? createdAt;
  final DateTime? updatedAt;

  const UserList({
    required this.id,
    required this.name,
    required this.isDefault,
    this.cars = const [],
    this.carIds = const [],
    this.carCount = 0,
    this.createdAt,
    this.updatedAt,
  });

  int get totalCars =>
      carCount > 0 ? carCount : cars.isNotEmpty ? cars.length : carIds.length;

  // Backend yanıt formatları:
  //   GET /users/{id}/lists  → [{id, ad, varsayilan, ilanSayisi, kayitTarihi}]
  //   GET /lists/{id}        → {id, ad, varsayilan, ilanlar:[ids], kayitTarihi, guncelleme}
  //   POST/PUT /lists        → {id, ad, varsayilan, ilanSayisi, kayitTarihi, guncelleme}
  factory UserList.fromJson(Map<String, dynamic> json) {
    final ilanlar =
        (json['ilanlar'] as List<dynamic>? ?? []).cast<String>();
    final ilanSayisi = json['ilanSayisi'] as int? ?? ilanlar.length;

    return UserList(
      id: json['id'] as String,
      name: json['ad'] as String? ?? '',
      isDefault: json['varsayilan'] as bool? ?? false,
      carIds: ilanlar,
      carCount: ilanSayisi,
      createdAt: json['kayitTarihi'] != null
          ? DateTime.parse(json['kayitTarihi'] as String)
          : null,
      updatedAt: json['guncelleme'] != null
          ? DateTime.parse(json['guncelleme'] as String)
          : null,
    );
  }

  Map<String, dynamic> toJson() => {
        'id': id,
        'ad': name,
        'varsayilan': isDefault,
        'ilanlar': carIds,
        'ilanSayisi': carCount,
        if (createdAt != null) 'kayitTarihi': createdAt!.toIso8601String(),
        if (updatedAt != null) 'guncelleme': updatedAt!.toIso8601String(),
      };

  UserList copyWith({
    String? id,
    String? name,
    bool? isDefault,
    List<Car>? cars,
    List<String>? carIds,
    int? carCount,
    DateTime? createdAt,
    DateTime? updatedAt,
  }) =>
      UserList(
        id: id ?? this.id,
        name: name ?? this.name,
        isDefault: isDefault ?? this.isDefault,
        cars: cars ?? this.cars,
        carIds: carIds ?? this.carIds,
        carCount: carCount ?? this.carCount,
        createdAt: createdAt ?? this.createdAt,
        updatedAt: updatedAt ?? this.updatedAt,
      );
}
