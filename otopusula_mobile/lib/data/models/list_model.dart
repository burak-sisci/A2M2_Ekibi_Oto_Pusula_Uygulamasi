// 'List' Dart anahtar kelimesi olduğu için dosya ve sınıf adı 'UserList' (developer.md §2)
import 'car.dart';

class UserList {
  final String id;
  final String userId;
  final String name;
  final bool isDefault;
  final List<Car> cars;
  final DateTime createdAt;
  final DateTime updatedAt;

  const UserList({
    required this.id,
    required this.userId,
    required this.name,
    required this.isDefault,
    required this.cars,
    required this.createdAt,
    required this.updatedAt,
  });

  factory UserList.fromJson(Map<String, dynamic> json) => UserList(
        id: json['_id'] as String,
        userId: json['userId'] as String,
        name: json['name'] as String,
        isDefault: json['isDefault'] as bool? ?? false,
        cars: (json['cars'] as List<dynamic>? ?? [])
            .map((e) => Car.fromJson(e as Map<String, dynamic>))
            .toList(),
        createdAt: DateTime.parse(json['createdAt'] as String),
        updatedAt: DateTime.parse(json['updatedAt'] as String),
      );

  Map<String, dynamic> toJson() => {
        '_id': id,
        'userId': userId,
        'name': name,
        'isDefault': isDefault,
        'cars': cars.map((c) => c.toJson()).toList(),
        'createdAt': createdAt.toIso8601String(),
        'updatedAt': updatedAt.toIso8601String(),
      };

  UserList copyWith({
    String? id,
    String? userId,
    String? name,
    bool? isDefault,
    List<Car>? cars,
    DateTime? createdAt,
    DateTime? updatedAt,
  }) =>
      UserList(
        id: id ?? this.id,
        userId: userId ?? this.userId,
        name: name ?? this.name,
        isDefault: isDefault ?? this.isDefault,
        cars: cars ?? this.cars,
        createdAt: createdAt ?? this.createdAt,
        updatedAt: updatedAt ?? this.updatedAt,
      );
}
