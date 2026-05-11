import 'package:otopusula_mobile/data/models/car.dart';
import 'package:otopusula_mobile/data/models/comment.dart';
import 'package:otopusula_mobile/data/models/list_model.dart';
import 'package:otopusula_mobile/data/models/price_predict.dart';
import 'package:otopusula_mobile/data/models/share_link.dart';
import 'package:otopusula_mobile/data/models/user.dart';

class TestData {
  TestData._();

  // Backend GET /users/{id} yanıtı
  static final userJson = {
    'id': 'user123',
    'ad': 'Test Kullanıcı',
    'email': 'test@example.com',
    'telefon': '+905551234567',
    'cinsiyet': 'Erkek',
    'dogumTarihi': '1998-05-15',
    'kayitTarihi': '2025-01-10T09:00:00Z',
  };

  // Backend GET /cars/{id} yanıtı (camelCase Turkish field names)
  static final carJson = {
    'id': 'car456',
    'ilanSahibi': 'user123',
    'marka': 'Toyota',
    'seri': 'Corolla',
    'model': 'Corolla',
    'yil': 2020,
    'kilometre': 45000,
    'yakitTipi': 'Benzin',
    'vitesTipi': 'Otomatik',
    'kasaTipi': 'Sedan',
    'renk': 'Beyaz',
    'fiyat': 850000,
    'konum': 'İstanbul, Kadıköy',
    'resimler': ['https://storage.example.com/img1.jpg'],
    'boyaliDegisen': {
      'motorKaputu': 'Değişmiş',
      'tavan': 'Orijinal',
    },
    'agirHasarKaydi': false,
    'takasaUygun': true,
    'kimden': 'Sahibinden',
    'ilanTarihi': '2026-02-15T10:00:00Z',
  };

  // Backend GET /cars/{id}/comments yanıtı
  static final commentJson = {
    'id': 'comment789',
    'userId': 'user123',
    'carId': 'car456',
    'content': 'Bu araç hâlâ satılık mı?',
    'likeCount': 3,
    'likedByUsers': ['user001', 'user002'],
    'createdAt': '2026-03-06T12:30:00Z',
  };

  // Backend GET /lists/{id} yanıtı — ilanlar: [ids]
  static final listJson = {
    'id': 'list001',
    'ad': 'Favoriler',
    'varsayilan': true,
    'ilanlar': ['car456'],
    'kayitTarihi': '2026-01-20T08:00:00Z',
    'guncelleme': '2026-03-05T11:00:00Z',
  };

  // Backend GET /users/{id}/lists yanıtı — ilanSayisi
  static final listSummaryJson = {
    'id': 'list001',
    'ad': 'Favoriler',
    'varsayilan': true,
    'ilanSayisi': 2,
    'kayitTarihi': '2026-01-20T08:00:00Z',
  };

  // Backend POST /api/Prediction/predict yanıtı
  static final pricePredictJson = {
    'durum': 'Başarılı',
    'tahminSonucu': {
      'fiyatEtiketi': '875000',
      'birim': 'TL',
    },
  };

  // Backend GET /cars/{carId}/share yanıtı (Türkçe camelCase alan adları)
  static final shareLinkJson = {
    'kisaUrl': 'https://otopusula.app/s/aBc123',
    'orijinalUrl': 'https://otopusula.app/cars/car456',
    'gecerlilikSonu': '2026-06-07T14:00:00Z',
  };

  // Model örnekleri
  static User get user => User.fromJson(userJson);
  static Car get car => Car.fromJson(carJson);
  static Comment get comment => Comment.fromJson(commentJson);
  static UserList get userList => UserList.fromJson(listJson);
  static PricePredict get pricePredict => PricePredict.fromJson(pricePredictJson);
  static ShareLink get shareLink => ShareLink.fromJson(shareLinkJson);
}
