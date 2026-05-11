class User {
  final String id;
  final String name;
  final String email;
  final String phone;
  final String? gender;
  final String? birthDate;
  final int listingCount;
  final DateTime createdAt;
  final DateTime? updatedAt;

  const User({
    required this.id,
    required this.name,
    required this.email,
    required this.phone,
    this.gender,
    this.birthDate,
    this.listingCount = 0,
    required this.createdAt,
    this.updatedAt,
  });

  // Backend: GET /users/{id} → {id, ad, email, telefon, cinsiyet, dogumTarihi, kayitTarihi}
  // Backend: PUT /users/{id} → {id, ad, email, telefon}
  factory User.fromJson(Map<String, dynamic> json) => User(
        id: json['id'] as String? ?? json['kullaniciId'] as String? ?? '',
        name: json['ad'] as String? ?? '',
        email: json['email'] as String? ?? '',
        phone: json['phone'] as String? ?? json['telefon'] as String? ?? '',
        gender: json['cinsiyet'] as String?,
        birthDate: json['dogumTarihi'] as String?,
        listingCount: (json['ilanSayisi'] as num?)?.toInt() ?? 0,
        createdAt: json['createdAt'] != null
            ? DateTime.parse(json['createdAt'] as String)
            : json['olusturulmaTarihi'] != null
                ? DateTime.parse(json['olusturulmaTarihi'] as String)
                : DateTime.now(),
        updatedAt: json['updatedAt'] != null
            ? DateTime.parse(json['updatedAt'] as String)
            : null,
      );

  Map<String, dynamic> toJson() => {
        'id': id,
        'ad': name,
        'email': email,
        'telefon': phone,
        if (gender != null) 'cinsiyet': gender,
        if (birthDate != null) 'dogumTarihi': birthDate,
        'kayitTarihi': createdAt.toIso8601String(),
        if (updatedAt != null) 'guncelleme': updatedAt!.toIso8601String(),
      };

  User copyWith({
    String? id,
    String? name,
    String? email,
    String? phone,
    String? gender,
    String? birthDate,
    int? listingCount,
    DateTime? createdAt,
    DateTime? updatedAt,
  }) =>
      User(
        id: id ?? this.id,
        name: name ?? this.name,
        email: email ?? this.email,
        phone: phone ?? this.phone,
        gender: gender ?? this.gender,
        birthDate: birthDate ?? this.birthDate,
        listingCount: listingCount ?? this.listingCount,
        createdAt: createdAt ?? this.createdAt,
        updatedAt: updatedAt ?? this.updatedAt,
      );
}
