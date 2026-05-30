# Burak Şişci'nin Mobil Backend Görevleri

**Mobil Front-end ile Back-end Bağlanmış Test Videosu:** [Link buraya eklenecek](https://example.com)

> **Modül A — Kullanıcı ve Kimlik Yönetimi** kapsamındaki 6 endpoint için Flutter ↔ ASP.NET Core 10 backend bağlantı katmanı.

---

## 1. Üye Olma (Kayıt) Servisi
- **API Endpoint:** `POST /auth/register`
- **Görev:** Mobil uygulamada kullanıcı kayıt işlemini gerçekleştiren servis entegrasyonu
- **İşlevler:**
  - Kullanıcı bilgilerini toplama (`ad`, `email`, `phone`, `sifre`, opsiyonel `cinsiyet`/`dogumTarihi`)
  - Form validasyonu (email regex, telefon formatı, şifre gücü)
  - `Dio.post('/auth/register', data: dto.toJson())` ile API'ye POST gönderme
  - Başarılı kayıt → Login ekranına yönlendirme (`GoRouter`)
  - Hata yakalama (`DioException`):
    - `409 Conflict` → "Bu email/telefon zaten kullanılıyor"
    - `400 BadRequest` → alan validasyon mesajları (`errors` payload)
    - `NetworkException` → "Bağlantı yok"
- **Teknik Detaylar:**
  - HTTP Client: **Dio 5.7** + `BaseOptions(baseUrl: AppConstants.baseUrl)`
  - Request/Response model: `UserRegisterDto` + `User`
  - Repository: `AuthRepository.register(UserRegisterDto)`
  - State management: `RegisterViewModel` (`BaseViewModel` extends `ChangeNotifier`)
  - Loading state: `setLoading()` / `setSuccess()` / `setError(msg)`
  - Error interceptor (`ErrorInterceptor`) → `DioException.error` içine `ApiException` atar

## 2. Kullanıcı Girişi Servisi
- **API Endpoint:** `POST /auth/login`
- **Görev:** Email veya telefon ile giriş işlemi + JWT + Refresh token saklama
- **İşlevler:**
  - `identifier` (email veya telefon) ve `sifre` ile request
  - Response'tan `accessToken`, `refreshToken`, `kullaniciId`, `accessExpiresIn` parse
  - Token'ları `flutter_secure_storage` ile güvenli saklama (`TokenStorage.saveToken`, `saveRefreshToken`)
  - `AuthSessionViewModel.setAuthenticated(...)` ile global oturum state'i güncelleme
  - Başarılı login sonrası `FcmService.registerForUser()` çağrısı (`POST /devices/register`)
- **Teknik Detaylar:**
  - `LoginViewModel.login(identifier, password)`
  - `UserLoginDto(identifier, password).toJson()` request body
  - Geri uyumluluk: response'taki `token` alanı `accessToken`'la aynı (web tarafı için)
  - Error handling: 401 → "kimlik bilgileri hatalı", 5xx → "sunucu hatası"
  - SecureStorage debug modda in-memory map'e fallback (Android Keystore timeout sorununa çözüm)

## 3. Profil Bilgilerini Görüntüleme Servisi
- **API Endpoint:** `GET /users/{userId}`
- **Görev:** Kullanıcı profil bilgilerini API'den çekip mobil uygulamada gösterme
- **İşlevler:**
  - JWT token ile authenticated request (`Authorization: Bearer <accessToken>`)
  - `Dio.get('/users/{userId}')` → `User.fromJson(response.data)`
  - 401 olursa `AuthInterceptor` otomatik `/auth/refresh` çağırır, isteği tekrarlar
  - Refresh başarısız olursa `onSessionExpired` callback → `AuthSessionViewModel.handleSessionExpired()` → Login ekranına yönlendirme
- **Teknik Detaylar:**
  - `AuthInterceptor.onRequest` her isteğe `Authorization` header'ı ekler
  - `UserRepository.getById(userId)`
  - Cache stratejisi: gelecekte `shared_preferences` ile son profili offline gösterme
  - Error: 401 (refresh tetiklenir), 404 (kullanıcı silinmiş) → logout

## 4. Profil Bilgilerini Güncelleme Servisi
- **API Endpoint:** `PUT /users/{userId}`
- **Görev:** Kullanıcı profil bilgilerini güncelleme
- **İşlevler:**
  - Profil düzenleme ekranından gelen `UserUpdateDto` (sadece değişen alanlar)
  - `Dio.put('/users/{userId}', data: dto.toJson())`
  - Başarılı update sonrası ViewModel state güncellenir, ProfilePage refresh
  - Optimistic update: kaydet butonuna basıldığında UI hemen güncellenir, hata olursa rollback
- **Teknik Detaylar:**
  - `AuthRepository.updateProfile(userId, dto)` → `User` döner
  - Partial update: `UserUpdateDto.toJson()` null olmayan alanları gönderir
  - Telefon değişikliği → 409 (zaten kullanılıyor) hatası mümkün
  - Şifre değişikliği opsiyonel (`yeniSifre` alanı dolu ise hash'lenir)
  - Bearer token otomatik eklenir

## 5. Hesap Silme Servisi
- **API Endpoint:** `DELETE /users/{userId}`
- **Görev:** Kullanıcı hesabını ve ilişkili tüm verileri silme
- **İşlevler:**
  - Multi-step onay dialog'undan sonra `AuthRepository.deleteAccount(userId)` çağrısı
  - Backend tarafında MediatR `UserDeletedEvent` ile cascade delete:
    - Kullanıcının ilanları → silinir
    - Yorumları → silinir
    - Listeleri (`lists` koleksiyonu) → silinir
    - Cihazları (`devices` koleksiyonu) → silinir
  - Başarılı silme → `_clearSession()` → Login ekranı
- **Teknik Detaylar:**
  - `AuthInterceptor` Bearer token otomatik
  - Cleanup işlemleri:
    - `TokenStorage.clearAll()` (access + refresh + userId secure storage)
    - `SharedPreferences.clear()` (offline cache varsa)
    - `FcmService.unregisterCurrentToken()` (Firebase token deleteToken)
  - Hata durumu: 401 (refresh dene), 403 (başkasının hesabı, mobil için imkansız), 404 (zaten silinmiş)
  - `GoRouter.go(AppRoutes.login)` ile navigation reset

## 6. Çıkış (Logout) Servisi
- **API Endpoint:** `POST /auth/logout`
- **Görev:** Oturum sonlandırma — JWT blacklist + Refresh token revoke + FCM cihaz silme
- **İşlevler:**
  - `AuthRepository.logout(refreshToken: refresh)` çağrısı
    - Backend: access token JTI → Redis `blacklist:{jti}` (kalan TTL)
    - Backend: refresh token → Redis'ten sil (`refresh:{userId}:{tokenId}`)
  - `FcmService.unregisterCurrentToken()`:
    - `DELETE /devices/unregister` ile backend'den cihaz sil
    - `FirebaseMessaging.instance.deleteToken()` ile FCM token iptal
  - Local cleanup: `TokenStorage.clearAll()`
  - Sunucu hatası olsa bile (offline durum) yerel oturum temizlenir (`finally` bloğu)
- **Teknik Detaylar:**
  - `AuthSessionViewModel.logout()` orkestrasyon noktası
  - Sıralama önemli:
    1. FCM unregister (henüz token var)
    2. Backend logout
    3. Local clear
    4. Navigation
  - 401 olsa bile `_clearSession()` çağrılır — kullanıcı her durumda çıkış yapabilir

---

## Ortak Teknik Notlar

- **API Base URL:** `https://otopusula-backend.onrender.com` (canlı), `http://10.0.2.2:8080` (emülatör), `http://localhost:8080` (USB + `adb reverse`)
- **Authentication:** JWT Bearer + Refresh Token Rotation (15 dk access + 30 gün refresh)
- **Token Storage:** `flutter_secure_storage` (Android Keystore — release), in-memory map (debug)
- **HTTP Client:** Dio 5.7
  - `AuthInterceptor` → Bearer token + 401 refresh akışı
  - `ErrorInterceptor` → `DioException` → `ApiException` mapping
  - `LogInterceptor` → debug build'de request/response log
- **State Management:** Provider 6.1 + `ChangeNotifier` (BaseViewModel)
- **Navigation:** GoRouter 14
- **Repository Pattern:** `AuthRepository`, `UserRepository` — `ApiClient`'a dependency injection
