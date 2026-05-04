class Validators {
  Validators._();

  static final _emailRegex = RegExp(r'^[\w.+-]+@[\w-]+\.[a-zA-Z]{2,}$');
  static final _phoneRegex = RegExp(r'^\+?[0-9]{10,13}$');
  // En az 8 karakter, bir büyük, bir küçük, bir rakam
  static final _passwordRegex = RegExp(r'^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$');

  static String? required(String? value, [String fieldName = 'Bu alan']) {
    if (value == null || value.trim().isEmpty) return '$fieldName zorunludur.';
    return null;
  }

  static String? email(String? value) {
    final req = required(value, 'E-posta');
    if (req != null) return req;
    if (!_emailRegex.hasMatch(value!.trim())) return 'Geçerli bir e-posta adresi girin.';
    return null;
  }

  static String? phone(String? value) {
    final req = required(value, 'Telefon');
    if (req != null) return req;
    if (!_phoneRegex.hasMatch(value!.trim())) return 'Geçerli bir telefon numarası girin.';
    return null;
  }

  static String? password(String? value) {
    final req = required(value, 'Şifre');
    if (req != null) return req;
    if (!_passwordRegex.hasMatch(value!)) {
      return 'Şifre en az 8 karakter, bir büyük harf, bir küçük harf ve bir rakam içermelidir.';
    }
    return null;
  }

  static String? identifier(String? value) {
    if (value == null || value.trim().isEmpty) return 'E-posta veya telefon zorunludur.';
    final v = value.trim();
    if (_emailRegex.hasMatch(v) || _phoneRegex.hasMatch(v)) return null;
    return 'Geçerli bir e-posta veya telefon numarası girin.';
  }

  static String? positiveInt(String? value, [String fieldName = 'Bu alan']) {
    final req = required(value, fieldName);
    if (req != null) return req;
    final parsed = int.tryParse(value!.trim());
    if (parsed == null || parsed <= 0) return '$fieldName pozitif bir sayı olmalıdır.';
    return null;
  }

  static String? minLength(String? value, int min, [String fieldName = 'Bu alan']) {
    final req = required(value, fieldName);
    if (req != null) return req;
    if (value!.trim().length < min) return '$fieldName en az $min karakter olmalıdır.';
    return null;
  }
}
