// PUT /auth/profile — sadece telefon güncelleniyor (developer.md + AuthController)
class UserUpdateDto {
  final String? phone;

  const UserUpdateDto({this.phone});

  Map<String, dynamic> toJson() => {
        if (phone != null) 'Phone': phone,
      };
}
