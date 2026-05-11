// Backend RegisterRequest: {Ad, Email, Phone, Sifre, Cinsiyet?, DogumTarihi?}
class UserRegisterDto {
  final String name;
  final String email;
  final String phone;
  final String password;
  final String? gender;
  final String? birthDate;

  const UserRegisterDto({
    required this.name,
    required this.email,
    required this.phone,
    required this.password,
    this.gender,
    this.birthDate,
  });

  Map<String, dynamic> toJson() => {
        'Ad': name,
        'Email': email,
        'Phone': phone,
        'Sifre': password,
        if (gender != null) 'Cinsiyet': gender,
        if (birthDate != null) 'DogumTarihi': birthDate,
      };
}
