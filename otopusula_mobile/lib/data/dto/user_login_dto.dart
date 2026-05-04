class UserLoginDto {
  // E-posta veya telefon numarası
  final String identifier;
  final String password;

  const UserLoginDto({required this.identifier, required this.password});

  Map<String, dynamic> toJson() => {
        'identifier': identifier,
        'password': password,
      };
}
