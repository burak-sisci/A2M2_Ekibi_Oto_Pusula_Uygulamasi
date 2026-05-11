// Backend LoginRequest: {Identifier, Sifre} — Identifier e-posta veya telefon olabilir
class UserLoginDto {
  final String identifier;
  final String password;

  const UserLoginDto({required this.identifier, required this.password});

  Map<String, dynamic> toJson() => {
        'Identifier': identifier,
        'Sifre': password,
      };
}
