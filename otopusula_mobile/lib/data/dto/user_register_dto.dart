class UserRegisterDto {
  final String name;
  final String email;
  final String phone;
  final String password;
  final String? gender;
  final String? birthDate; // ISO date string: "1998-05-15"

  const UserRegisterDto({
    required this.name,
    required this.email,
    required this.phone,
    required this.password,
    this.gender,
    this.birthDate,
  });

  Map<String, dynamic> toJson() => {
        'name': name,
        'email': email,
        'phone': phone,
        'password': password,
        if (gender != null) 'gender': gender,
        if (birthDate != null) 'birthDate': birthDate,
      };
}
