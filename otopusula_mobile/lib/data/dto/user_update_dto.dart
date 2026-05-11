// PUT /users/{userId} — Ad, Telefon ve YeniSifre güncellenebilir
class UserUpdateDto {
  final String? ad;
  final String? telefon;
  final String? yeniSifre;

  const UserUpdateDto({this.ad, this.telefon, this.yeniSifre});

  Map<String, dynamic> toJson() => {
        if (ad != null) 'Ad': ad,
        if (telefon != null) 'Telefon': telefon,
        if (yeniSifre != null) 'YeniSifre': yeniSifre,
      };
}
