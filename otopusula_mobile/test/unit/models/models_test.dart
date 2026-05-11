import 'package:flutter_test/flutter_test.dart';
import 'package:otopusula_mobile/data/models/car.dart';
import 'package:otopusula_mobile/data/models/comment.dart';
import 'package:otopusula_mobile/data/models/list_model.dart';
import 'package:otopusula_mobile/data/models/price_predict.dart';
import 'package:otopusula_mobile/data/models/share_link.dart';
import 'package:otopusula_mobile/data/models/user.dart';
import '../../helpers/test_data.dart';

void main() {
  group('User fromJson/toJson', () {
    test('round-trip', () {
      final user = User.fromJson(TestData.userJson);
      expect(user.id, 'user123');
      expect(user.name, 'Test Kullanıcı');
      expect(user.email, 'test@example.com');
      expect(user.phone, '+905551234567');
      expect(user.gender, 'Erkek');
      final json = user.toJson();
      expect(json['id'], user.id);
      expect(json['ad'], user.name);
      expect(json['telefon'], user.phone);
    });
  });

  group('Car fromJson/toJson', () {
    test('round-trip', () {
      final car = Car.fromJson(TestData.carJson);
      expect(car.id, 'car456');
      expect(car.brand, 'Toyota');
      expect(car.price, 850000);
      expect(car.km, 45000);
      expect(car.fuelType, 'Benzin');
      expect(car.gearType, 'Otomatik');
      expect(car.location.city, 'İstanbul');
      expect(car.location.district, 'Kadıköy');
      // boyaliDegisen: Değişmiş panel damageInfo'ya eklenmeli
      expect(car.damageInfo, contains('motorKaputu: Değişmiş'));
      final json = car.toJson();
      expect(json['marka'], 'Toyota');
      expect(json['fiyat'], 850000);
      expect(json['konum'], 'İstanbul, Kadıköy');
    });

    test('copyWith', () {
      final car = Car.fromJson(TestData.carJson);
      final updated = car.copyWith(price: 900000);
      expect(updated.price, 900000);
      expect(updated.brand, car.brand);
    });
  });

  group('Comment fromJson/toJson', () {
    test('round-trip', () {
      final comment = Comment.fromJson(TestData.commentJson);
      expect(comment.id, 'comment789');
      expect(comment.content, 'Bu araç hâlâ satılık mı?');
      expect(comment.likeCount, 3);
      expect(comment.likedByUsers, hasLength(2));
      final json = comment.toJson();
      expect(json['content'], comment.content);
      expect(json['likeCount'], 3);
    });
  });

  group('UserList fromJson', () {
    test('ilanlar listesini carIds olarak parse eder', () {
      final list = UserList.fromJson(TestData.listJson);
      expect(list.id, 'list001');
      expect(list.name, 'Favoriler');
      expect(list.isDefault, true);
      expect(list.carIds, contains('car456'));
      expect(list.totalCars, 1);
    });

    test('ilanSayisi ile özet yanıtı parse eder', () {
      final list = UserList.fromJson(TestData.listSummaryJson);
      expect(list.id, 'list001');
      expect(list.carCount, 2);
      expect(list.totalCars, 2);
    });
  });

  group('PricePredict fromJson', () {
    test('parse', () {
      final p = PricePredict.fromJson(TestData.pricePredictJson);
      expect(p.status, 'Başarılı');
      expect(p.priceLabel, '875000');
      expect(p.unit, 'TL');
    });
  });

  group('ShareLink fromJson', () {
    test('parse', () {
      final s = ShareLink.fromJson(TestData.shareLinkJson);
      expect(s.shortUrl, 'https://otopusula.app/s/aBc123');
    });
  });
}
