import '../../lib/data/models/car.dart';
import '../../lib/data/models/comment.dart';
import '../../lib/data/models/list_model.dart';
import '../../lib/data/models/price_predict.dart';
import '../../lib/data/models/share_link.dart';
import '../../lib/data/models/user.dart';

class TestData {
  TestData._();

  static final userJson = {
    '_id': 'user123',
    'name': 'Test Kullanıcı',
    'email': 'test@example.com',
    'phone': '+905551234567',
    'gender': 'Erkek',
    'birthDate': '1998-05-15',
    'createdAt': '2025-01-10T09:00:00Z',
    'updatedAt': '2026-03-07T14:30:00Z',
  };

  static final carJson = {
    '_id': 'car456',
    'userId': 'user123',
    'brand': 'Toyota',
    'model': 'Corolla',
    'year': 2020,
    'km': 45000,
    'fuelType': 'Benzin',
    'gearType': 'Otomatik',
    'price': 850000,
    'location': {'city': 'İstanbul', 'district': 'Kadıköy'},
    'description': 'Kazasız, hasarsız araç.',
    'images': ['https://storage.example.com/img1.jpg'],
    'damageInfo': ['Kaput Değişen'],
    'createdAt': '2026-02-15T10:00:00Z',
    'updatedAt': '2026-03-01T16:45:00Z',
  };

  static final commentJson = {
    '_id': 'comment789',
    'userId': 'user123',
    'carId': 'car456',
    'content': 'Bu araç hâlâ satılık mı?',
    'createdAt': '2026-03-06T12:30:00Z',
  };

  static final listJson = {
    '_id': 'list001',
    'userId': 'user123',
    'name': 'Favoriler',
    'isDefault': true,
    'cars': [carJson],
    'createdAt': '2026-01-20T08:00:00Z',
    'updatedAt': '2026-03-05T11:00:00Z',
  };

  static final pricePredictJson = {
    'estimatedPrice': 875000,
    'priceRange': {'min': 820000, 'max': 930000},
    'confidence': 0.87,
    'generatedAt': '2026-03-07T14:00:00Z',
  };

  static final shareLinkJson = {
    'shortUrl': 'https://otopusula.app/s/aBc123',
    'originalUrl': 'https://otopusula.app/cars/car456',
    'expiresAt': '2026-06-07T14:00:00Z',
  };

  // Model örnekleri
  static User get user => User.fromJson(userJson);
  static Car get car => Car.fromJson(carJson);
  static Comment get comment => Comment.fromJson(commentJson);
  static UserList get userList => UserList.fromJson(listJson);
  static PricePredict get pricePredict => PricePredict.fromJson(pricePredictJson);
  static ShareLink get shareLink => ShareLink.fromJson(shareLinkJson);
}
