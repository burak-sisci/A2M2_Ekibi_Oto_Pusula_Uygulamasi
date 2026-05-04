# OtoPusula Mobil Frontend — Geliştirici Rehberi (developer.md)

Bu doküman, OtoPusula mobil uygulamasının Flutter ile geliştirilecek frontend katmanının mimari, klasör yapısı, bağımlılık kuralları ve component hiyerarşisini tanımlar. Bu rehber bir CodeBuilder uygulamasının kod üretirken referans alacağı tek doğrulama kaynağıdır. Backend mimarisi (mikroservisler, MongoDB, Redis, RabbitMQ vb.) frontend için şeffaftır; uygulama yalnızca `openapi.yaml` dosyasında tanımlı REST endpoint'leri üzerinden iletişim kurar.

---

## 1. Teknoloji Yığını

| Katman | Seçim | Gerekçe |
|---|---|---|
| Framework | Flutter (stable channel, Dart 3.x) | Tek kod tabanıyla iOS/Android |
| Mimari | MVVM (Model–View–ViewModel) | View'ı durum yönetiminden ayırır, test edilebilir |
| Durum yönetimi | `provider` (ChangeNotifier tabanlı) | MVVM ile doğal uyum, minimum bağımlılık |
| HTTP | `dio` | Interceptor desteği (JWT, hata yönetimi) |
| Yerel depolama | `flutter_secure_storage` (token), `shared_preferences` (tercihler) | Güvenli token saklama |
| Routing | `go_router` | Bildirimsel rota tanımı, deep-link uyumlu |
| Form doğrulama | Yerel `Validator` sınıfı (eklenti yok) | Bağımlılığı en aza indirme |
| Tarih biçimleme | `intl` | OpenAPI `date-time` alanları için |
| Resim önbelleği | `cached_network_image` | İlan fotoğrafları için |

**Bağımlılık ilkesi:** Yukarıdaki listenin dışında paket eklenmez. Her yeni paket önerisi bu dosyada gerekçelendirilmeden eklenemez.

---

## 2. Klasör Yapısı (Mecburi)

Proje kökünde **yalnızca** aşağıdaki üst düzey klasörler ve dosyalar bulunur:

```
otopusula_mobile/
├── lib/
├── assets/
├── test/
├── pubspec.yaml
└── README.md
```

`lib/` altındaki yapı MVVM disiplinini takip eder:

```
lib/
├── main.dart                     # Uygulama giriş noktası
├── app.dart                      # MaterialApp + GoRouter kurulumu, tema sağlayıcı
│
├── core/                         # Çapraz kesen yardımcılar (UI'a bağımlı değildir)
│   ├── constants/
│   │   ├── api_endpoints.dart    # openapi.yaml ile birebir eşleşen sabitler
│   │   └── app_constants.dart    # Sayfa boyutları, regex'ler vb.
│   ├── network/
│   │   ├── api_client.dart       # Dio yapılandırması
│   │   ├── auth_interceptor.dart # JWT'yi her isteğe ekler
│   │   └── error_interceptor.dart# 401/403/404 → ApiException eşleme
│   ├── storage/
│   │   └── token_storage.dart    # flutter_secure_storage sarıcısı
│   ├── theme/
│   │   ├── app_theme.dart        # Light/Dark ThemeData
│   │   ├── app_colors.dart       # Figma kit token'ları
│   │   └── app_text_styles.dart  # Tipografi token'ları
│   ├── router/
│   │   └── app_router.dart       # Tüm rotalar tek dosyada
│   └── utils/
│       ├── validators.dart       # Email/telefon/şifre doğrulayıcıları
│       └── formatters.dart       # TL fiyat, tarih biçimleme
│
├── data/                         # OpenAPI'a karşılık gelen veri katmanı
│   ├── models/                   # JSON ↔ Dart dönüşümü (immutable)
│   │   ├── user.dart
│   │   ├── car.dart
│   │   ├── list_model.dart       # `List` Dart anahtar kelimesi olduğu için
│   │   ├── comment.dart
│   │   ├── price_predict.dart
│   │   └── share_link.dart
│   ├── dto/                      # İstek gövdeleri (input şemaları)
│   │   ├── user_register_dto.dart
│   │   ├── user_login_dto.dart
│   │   ├── car_create_dto.dart
│   │   └── ...
│   └── repositories/             # ViewModel'lerin tek erişim noktası
│       ├── auth_repository.dart  # /auth/* endpoint'leri
│       ├── user_repository.dart  # /users/* endpoint'leri
│       ├── car_repository.dart   # /cars/* endpoint'leri
│       ├── ai_repository.dart    # /cars/predict-price
│       ├── list_repository.dart  # /lists/* endpoint'leri
│       └── comment_repository.dart # /cars/.../comments + /comments/*
│
├── viewmodels/                   # Her View için bir ViewModel (ChangeNotifier)
│   ├── base_view_model.dart      # ortak loading/error yönetimi
│   ├── auth/
│   │   ├── login_view_model.dart
│   │   ├── register_view_model.dart
│   │   └── auth_session_view_model.dart  # uygulama yaşam süresi boyunca tek
│   ├── car/
│   │   ├── car_list_view_model.dart
│   │   ├── car_detail_view_model.dart
│   │   ├── car_create_view_model.dart
│   │   └── price_predict_view_model.dart
│   ├── list/
│   │   ├── user_lists_view_model.dart
│   │   └── list_detail_view_model.dart
│   ├── comment/
│   │   └── car_comments_view_model.dart
│   └── profile/
│       └── profile_view_model.dart
│
└── view/                         # UI katmanı (yalnız sunum)
    ├── pages/                    # Tam ekran sayfalar (rota hedefleri)
    │   ├── splash_page.dart
    │   ├── auth/
    │   │   ├── login_page.dart
    │   │   └── register_page.dart
    │   ├── home/
    │   │   └── home_page.dart    # Alt sekme kabuğu
    │   ├── car/
    │   │   ├── car_list_page.dart
    │   │   ├── car_detail_page.dart
    │   │   ├── car_create_page.dart
    │   │   └── price_predict_page.dart
    │   ├── list/
    │   │   ├── lists_page.dart
    │   │   └── list_detail_page.dart
    │   ├── comment/
    │   │   └── comments_page.dart
    │   └── profile/
    │       ├── profile_page.dart
    │       └── edit_profile_page.dart
    │
    └── widgets/                  # Yeniden kullanılabilir parçalar
        ├── common/               # Uygulama genelinde nötr widget'lar
        │   ├── primary_button.dart
        │   ├── secondary_button.dart
        │   ├── app_text_field.dart
        │   ├── app_dropdown.dart
        │   ├── loading_indicator.dart
        │   ├── error_view.dart
        │   ├── empty_state.dart
        │   └── confirm_dialog.dart
        ├── car/                  # Yalnız ilanlarla ilgili
        │   ├── car_card.dart
        │   ├── car_filter_sheet.dart
        │   ├── car_image_carousel.dart
        │   └── car_spec_row.dart
        ├── comment/
        │   ├── comment_tile.dart
        │   └── comment_input_bar.dart
        └── list/
            ├── list_card.dart
            └── add_to_list_sheet.dart
```

**Kural:** `assets/` altında `images/`, `icons/`, `fonts/` alt klasörleri açılır. `pubspec.yaml` bu yolları açıkça bildirir.

---

## 3. Katmanlar Arası Bağımlılık Kuralları

Bu kurallar **CodeBuilder için bağlayıcıdır** ve ihlali kabul edilemez.

```
view ──► viewmodels ──► data/repositories ──► data/models, core/network
   ▲                          │
   │                          ▼
   └──────── core (theme, utils, validators, router) ◄────────
```

1. **View, asla repository veya `Dio` çağırmaz.** Yalnızca ViewModel ile konuşur. View içinde `import 'package:dio/...'` görülürse hatadır.
2. **ViewModel, asla Widget import etmez.** `material.dart`'tan yalnızca `ChangeNotifier` ve temel sınıflar (örn. `TextEditingController`) kullanır; `BuildContext` parametresi almaz, `Navigator` çağırmaz. Yönlendirme kararını `bool` veya `enum` durum üzerinden View tetikler.
3. **Repository, asla ViewModel veya View bilmez.** Yalnız `ApiClient` ve `Model`/`DTO` ile çalışır.
4. **Model'ler immutable'dır** (`final` alanlar, `copyWith` ile güncellenir). `fromJson` / `toJson` el ile yazılır; runtime kod üretimi (build_runner, json_serializable) **kullanılmaz** — bağımlılığı azaltmak için.
5. **`core/`, hiçbir üst katmana bağımlı olamaz.** Saf Dart yardımcıları + tema.
6. **Tek yön:** `view → viewmodel → repository`. Geri yönde import yasaktır.

---

## 4. MVVM Sözleşmesi

Her ViewModel `BaseViewModel`'den miras alır:

```dart
enum ViewState { idle, loading, success, error }

abstract class BaseViewModel extends ChangeNotifier {
  ViewState _state = ViewState.idle;
  String? _errorMessage;

  ViewState get state => _state;
  String? get errorMessage => _errorMessage;
  bool get isLoading => _state == ViewState.loading;

  @protected
  void setState(ViewState s, {String? error}) {
    _state = s;
    _errorMessage = error;
    notifyListeners();
  }
}
```

**View tarafı şablonu** (her sayfa bunu uygular):

```dart
ChangeNotifierProvider(
  create: (_) => CarListViewModel(carRepository: context.read())..load(),
  child: Consumer<CarListViewModel>(
    builder: (_, vm, __) {
      switch (vm.state) {
        case ViewState.loading: return const LoadingIndicator();
        case ViewState.error:   return ErrorView(message: vm.errorMessage!);
        case ViewState.success: return _CarListBody(vm: vm);
        case ViewState.idle:    return const SizedBox.shrink();
      }
    },
  ),
);
```

**Bağımlılık enjeksiyonu:** Repository'ler `MultiProvider` içinde `app.dart` köküne yerleştirilir. ViewModel'ler `context.read<XRepository>()` ile bunları alır. Service locator paketi (get_it vb.) **kullanılmaz**.

---

## 5. OpenAPI ↔ Repository Eşlemesi

Her endpoint kümesi tek bir repository'e gider. Tüm metot imzaları `openapi.yaml` ile birebir uyumludur. Tutarsızlık varsa OpenAPI esastır.

| OpenAPI tag | Repository | Temel metotlar |
|---|---|---|
| Kimlik | `AuthRepository` | `register`, `login`, `logout` |
| Kullanıcılar | `UserRepository` | `getUser`, `updateUser`, `deleteUser`, `getUserLists`, `getUserComments` |
| İlanlar | `CarRepository` | `createCar`, `listCars`, `getCar`, `updateCar`, `deleteCar` |
| YapayZeka | `AiRepository` | `predictPrice` |
| Listeler | `ListRepository` | `createList`, `getList`, `updateList`, `deleteList`, `addCarToList`, `removeCarFromList` |
| Yorumlar | `CommentRepository` | `listCarComments`, `addComment`, `getShareLink`, `updateComment`, `deleteComment`, `likeComment`, `unlikeComment` |

**Hata sözleşmesi:** Repository hata fırlatır, durum kodunu şu istisnalara çevirir: `BadRequestException` (400), `UnauthorizedException` (401), `ForbiddenException` (403), `NotFoundException` (404), `ConflictException` (409), `ServerException` (5xx), `NetworkException` (offline). ViewModel bu istisnaları yakalar ve mesajı `errorMessage`'a yazar.

**Auth akışı:** `AuthInterceptor` her isteğe `Authorization: Bearer <token>` ekler. 401 dönerse `AuthSessionViewModel` token'ı temizler ve `/login` rotasına itme bayrağı set eder.

---

## 6. Component Hiyerarşisi (Minimum Bağımlılık Kuralı)

Hiyerarşinin amacı: bir widget yalnızca **kendi seviyesinin altındaki** widget'ları import edebilir. Üst seviye widget'a yukarı bağımlılık yasaktır.

```
Seviye 0 — Atomik (core/theme'den başka import yok)
  PrimaryButton, SecondaryButton, AppTextField, AppDropdown,
  LoadingIndicator, ErrorView, EmptyState, AppChip, AppBadge

Seviye 1 — Birleşik (yalnız Seviye 0 + core'u kullanır)
  ConfirmDialog, AppTextField + Validator, CarSpecRow,
  CommentInputBar, CarImageCarousel

Seviye 2 — Etki alanı kartları (Seviye 0–1 kullanır)
  CarCard, ListCard, CommentTile

Seviye 3 — Bölmeler / Sheet'ler (Seviye 0–2 kullanır)
  CarFilterSheet, AddToListSheet

Seviye 4 — Sayfa gövdeleri (Seviye 0–3 kullanır)
  CarListPage, CarDetailPage, ListsPage, CommentsPage, ...

Seviye 5 — Kabuk (Seviye 4'ü kullanır)
  HomePage (BottomNavigationBar ile sekme kabuğu)
```

**Kural:** `CarCard` (S2), `PrimaryButton` (S0) kullanabilir; tersi yasaktır. Sayfa içi kompozisyon her zaman aşağı doğru akar.

---

## 7. Tema ve UI Token'ları (Figma Kit Eşlemesi)

Figma "Mobile Apps – Prototyping Kit" component setinden token'lar `core/theme/` içine sabit olarak çıkarılır. CodeBuilder, Figma'da gördüğü ham renk veya boyutu **doğrudan widget'a yazmaz**; `AppColors` ve `AppTextStyles`'ten okur.

```dart
// app_colors.dart
class AppColors {
  static const primary       = Color(0xFF1F6FEB);
  static const primaryDark   = Color(0xFF1A5BC4);
  static const surface       = Color(0xFFFFFFFF);
  static const surfaceMuted  = Color(0xFFF5F6F8);
  static const textPrimary   = Color(0xFF11181C);
  static const textSecondary = Color(0xFF687076);
  static const border        = Color(0xFFE6E8EB);
  static const success       = Color(0xFF1A8754);
  static const warning       = Color(0xFFF0A924);
  static const danger        = Color(0xFFD93B3B);
}

// app_text_styles.dart — Figma type scale
class AppTextStyles {
  static const h1     = TextStyle(fontSize: 28, fontWeight: FontWeight.w600, height: 1.2);
  static const h2     = TextStyle(fontSize: 22, fontWeight: FontWeight.w600, height: 1.25);
  static const h3     = TextStyle(fontSize: 18, fontWeight: FontWeight.w500, height: 1.3);
  static const body   = TextStyle(fontSize: 15, fontWeight: FontWeight.w400, height: 1.5);
  static const small  = TextStyle(fontSize: 13, fontWeight: FontWeight.w400, height: 1.4);
  static const button = TextStyle(fontSize: 15, fontWeight: FontWeight.w500, height: 1.2);
}
```

**Boşluk skalası:** 4 / 8 / 12 / 16 / 24 / 32 (Figma 8pt grid'iyle uyumlu). Bunun dışında değer kullanılmaz.
**Köşe yuvarlama:** 8 (kart), 12 (sheet), 999 (chip/pill).
**Karanlık mod** zorunludur — `ThemeData.dark()` üzerine aynı token isimleriyle override edilir.

---

## 8. Rotalar ve Navigasyon

Tüm rotalar `core/router/app_router.dart` içinde tek dosyada tanımlanır. Rotalar JWT durumuna göre `redirect` kullanır.

| Yol | Sayfa | Auth gerekli |
|---|---|---|
| `/splash` | SplashPage | Hayır |
| `/login` | LoginPage | Hayır |
| `/register` | RegisterPage | Hayır |
| `/home` | HomePage (sekme kabuğu) | Evet |
| `/cars` | CarListPage | Evet |
| `/cars/:id` | CarDetailPage | Evet |
| `/cars/new` | CarCreatePage | Evet |
| `/cars/predict` | PricePredictPage | Evet |
| `/cars/:id/comments` | CommentsPage | Evet |
| `/lists` | ListsPage | Evet |
| `/lists/:id` | ListDetailPage | Evet |
| `/profile` | ProfilePage | Evet |
| `/profile/edit` | EditProfilePage | Evet |

`HomePage` alt sekmeleri (BottomNavigationBar): **İlanlar / Listelerim / Fiyat Tahmini / Profil**.

---

## 9. Test Stratejisi (`test/` klasörü)

```
test/
├── unit/
│   ├── viewmodels/        # Her ViewModel için durum geçişi testi
│   ├── repositories/      # MockClient ile HTTP davranışı
│   ├── models/            # fromJson / toJson round-trip
│   └── utils/             # Validators, formatters
├── widget/                # Atomik ve etki alanı widget'ları
└── helpers/
    ├── mocks.dart         # Repository mock'ları (mocktail)
    └── test_data.dart     # Sabit JSON örnekleri
```

**Asgari kapsam (MVP):** Auth akışı, Car listele/detay/oluştur, Liste ekle/çıkar, Yorum ekle/beğen. ViewModel'lerin mutlaka unit testi olur; UI testleri sadece kritik kullanıcı akışları için.

---

## 10. Yerelleştirme ve Format Kuralları

- Uygulama dili: **Türkçe (tek dil)**. Metinler şimdilik string sabitleri olarak `core/constants/app_strings.dart`'ta toplanır; çoklu dil eklenirse `flutter_localizations` + ARB'ye geçilir.
- Para birimi: `intl` ile `tr_TR` locale, sembol `₺`, binlik ayraç nokta. Örn: `₺850.000`.
- Tarih: `dd MMM yyyy` (Örn: `15 Şub 2026`). Liste sayfalarında ek olarak göreceli tarih (`2 saat önce`).

---

## 11. Performans ve Erişilebilirlik

- Liste sayfaları **sayfalandırma** kullanır (`?page=&limit=`). Sonsuz kaydırma için `ScrollController` eşiği 200 px kala bir sonraki sayfayı çağırır.
- `cached_network_image` her uzak görsel için zorunludur. Yer tutucu olarak `surfaceMuted` rengi.
- Tüm dokunma hedefleri ≥ 48×48 dp. `Semantics` etiketleri ikon-only butonlar için zorunlu.
- Dinamik yazı tipi ölçeklemesi (`MediaQuery.textScaler`) kırılmamalı; sayfalar 130 % ölçekte denenir.

---

## 12. Gizlilik ve Güvenlik

- JWT yalnız `flutter_secure_storage` ile saklanır. `shared_preferences` kullanılmaz.
- Şifre giriş alanları `obscureText: true` ve `enableSuggestions: false`.
- Loglama: `dio` interceptor yalnız debug build'de payload yazar; release build'de header/body sızdırmaz.
- Görsel yükleme `multipart/form-data` (CarInput); maksimum 8 fotoğraf, dosya başı 5 MB sınırı istemcide kontrol edilir.

---

## 13. CodeBuilder Davranış Kuralları

CodeBuilder bu rehberi okurken aşağıdaki kararları **sormadan** uygular:

1. Belirsiz endpoint sözleşmesi → OpenAPI kazanır.
2. Belirsiz görsel detay → Figma kit'inin en yakın 8pt-uyumlu varyantı kullanılır.
3. Yeni bir paket önerisi düşündüğünde → eklemez, mevcut yığınla çözer veya bir TODO bırakır.
4. Bir widget hangi seviyede konumlanmalı belirsizse → daha düşük seviyeyi seçer.
5. ViewModel'de UI/Navigator çağrısı yazma içgüdüsü gelirse → durum bayrağı (`enum`) üretir, View okur.
6. Hata mesajları kullanıcıya **Türkçe** ve genel ifadelerle gösterilir; sunucu mesajı yalnız debug log'a düşer.
7. Test dosyası eksikse → ViewModel veya repository üretirken eşleniğini de oluşturur.

---

## 14. MVP Sırası (Önerilen Geliştirme Akışı)

1. `core/` (theme, network, storage, router iskeleti)
2. `data/models` + `data/dto` (OpenAPI'dan birebir)
3. `AuthRepository` + `AuthSessionViewModel` + Login/Register sayfaları
4. `HomePage` sekme kabuğu + `CarRepository` + `CarListPage` + `CarDetailPage`
5. `CarCreatePage` + `AiRepository` + `PricePredictPage`
6. `ListRepository` + `ListsPage` + `ListDetailPage` + `AddToListSheet`
7. `CommentRepository` + `CommentsPage` + beğen/geri al
8. `ProfilePage` + `EditProfilePage` + hesap silme
9. Test kapsamının doldurulması, erişilebilirlik geçişi, release optimizasyonu.

---

**Tek doğrulama kaynağı sıralaması (anlaşmazlıkta):**
`openapi.yaml` › `developer.md` (bu dosya) › Figma kit › CodeBuilder yorumu.
