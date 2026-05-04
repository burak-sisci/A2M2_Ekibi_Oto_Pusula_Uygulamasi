import 'package:flutter_test/flutter_test.dart';
import '../../../lib/core/utils/validators.dart';

void main() {
  group('Validators.email', () {
    test('geçerli e-posta kabul edilir', () {
      expect(Validators.email('burak@example.com'), isNull);
    });
    test('geçersiz e-posta reddedilir', () {
      expect(Validators.email('gecersiz'), isNotNull);
    });
    test('boş değer reddedilir', () {
      expect(Validators.email(''), isNotNull);
    });
  });

  group('Validators.phone', () {
    test('geçerli telefon kabul edilir', () {
      expect(Validators.phone('+905551234567'), isNull);
    });
    test('kısa numara reddedilir', () {
      expect(Validators.phone('123'), isNotNull);
    });
  });

  group('Validators.password', () {
    test('güçlü şifre kabul edilir', () {
      expect(Validators.password('Gizli1234!'), isNull);
    });
    test('büyük harf olmadan reddedilir', () {
      expect(Validators.password('gizli1234!'), isNotNull);
    });
    test('rakam olmadan reddedilir', () {
      expect(Validators.password('GizliAbcd!'), isNotNull);
    });
    test('8 karakterden kısa reddedilir', () {
      expect(Validators.password('Gizli1!'), isNotNull);
    });
  });

  group('Validators.identifier', () {
    test('e-posta kabul edilir', () {
      expect(Validators.identifier('burak@example.com'), isNull);
    });
    test('telefon kabul edilir', () {
      expect(Validators.identifier('+905551234567'), isNull);
    });
    test('geçersiz değer reddedilir', () {
      expect(Validators.identifier('abc'), isNotNull);
    });
  });

  group('Validators.positiveInt', () {
    test('pozitif sayı kabul edilir', () {
      expect(Validators.positiveInt('100'), isNull);
    });
    test('sıfır reddedilir', () {
      expect(Validators.positiveInt('0'), isNotNull);
    });
    test('metin reddedilir', () {
      expect(Validators.positiveInt('abc'), isNotNull);
    });
  });
}
