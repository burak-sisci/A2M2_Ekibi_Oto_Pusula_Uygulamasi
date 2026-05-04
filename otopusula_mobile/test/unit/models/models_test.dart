import 'package:flutter_test/flutter_test.dart';
import '../../../lib/data/models/car.dart';
import '../../../lib/data/models/comment.dart';
import '../../../lib/data/models/list_model.dart';
import '../../../lib/data/models/price_predict.dart';
import '../../../lib/data/models/share_link.dart';
import '../../../lib/data/models/user.dart';
import '../../helpers/test_data.dart';

void main() {
  group('User fromJson/toJson', () {
    test('round-trip', () {
      final user = User.fromJson(TestData.userJson);
      expect(user.id, 'user123');
      expect(user.name, 'Test Kullanıcı');
      expect(user.email, 'test@example.com');
      final json = user.toJson();
      expect(json['_id'], user.id);
    });
  });

  group('Car fromJson/toJson', () {
    test('round-trip', () {
      final car = Car.fromJson(TestData.carJson);
      expect(car.id, 'car456');
      expect(car.brand, 'Toyota');
      expect(car.price, 850000);
      expect(car.location.city, 'İstanbul');
      final json = car.toJson();
      expect(json['brand'], 'Toyota');
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
      final json = comment.toJson();
      expect(json['content'], comment.content);
    });
  });

  group('UserList fromJson/toJson', () {
    test('round-trip ve iç car parse', () {
      final list = UserList.fromJson(TestData.listJson);
      expect(list.id, 'list001');
      expect(list.cars.length, 1);
      expect(list.cars.first.brand, 'Toyota');
    });
  });

  group('PricePredict fromJson', () {
    test('parse', () {
      final p = PricePredict.fromJson(TestData.pricePredictJson);
      expect(p.estimatedPrice, 875000);
      expect(p.priceRange.min, 820000);
      expect(p.confidence, closeTo(0.87, 0.001));
    });
  });

  group('ShareLink fromJson', () {
    test('parse', () {
      final s = ShareLink.fromJson(TestData.shareLinkJson);
      expect(s.shortUrl, 'https://otopusula.app/s/aBc123');
    });
  });
}
