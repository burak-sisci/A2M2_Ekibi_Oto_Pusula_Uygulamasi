# Burak Şişci'nin Mobil Frontend Görevleri

**Mobile Front-end Demo Videosu:** [Link buraya eklenecek](https://example.com)

> **Modül A — Kullanıcı ve Kimlik Yönetimi** kapsamındaki 6 endpoint için Flutter mobil arayüz tasarımı ve implementasyonu.

---

## 1. Üye Olma (Kayıt) Ekranı
- **API Endpoint:** `POST /auth/register`
- **Görev:** Kullanıcı kayıt işlemi için mobil ekran tasarımı ve implementasyonu
- **UI Bileşenleri:**
  - Ad (`Ad`) input alanı
  - Email input alanı (`TextField` keyboardType: emailAddress)
  - Telefon numarası input alanı (`+90 5XX XXX XX XX` maskeli)
  - Şifre input alanı (obscureText: true, görünür/gizli toggle)
  - Şifre tekrar input alanı (doğrulama için)
  - Cinsiyet seçimi (`DropdownButton`: Erkek / Kadın / BelirtmekIstemiyorum)
  - Doğum tarihi seçici (`showDatePicker`)
  - "Kayıt Ol" butonu (`ElevatedButton`, full-width)
  - "Zaten hesabınız var mı? Giriş Yap" link (`TextButton`)
  - `CircularProgressIndicator` (kayıt işlemi sırasında)
- **Form Validasyonu:**
  - Email format kontrolü (real-time, regex)
  - Şifre güvenlik kuralları (min 8 karakter, büyük/küçük harf, rakam)
  - Şifre eşleşme kontrolü
  - Ad ve telefon boş olamaz kontrolü
  - Tüm alanlar geçerli olmadan "Kayıt Ol" butonu disabled
- **Kullanıcı Deneyimi:**
  - `FormField.errorText` ile alan altında hata mesajı
  - Başarılı kayıt sonrası `SnackBar` ile success mesajı, GoRouter ile Login sayfasına yönlendirme
  - Hata durumlarında kullanıcı dostu mesajlar (409 Conflict → "Bu email/telefon zaten kullanılıyor", 400 → alan hatası)
  - `FocusScope.of(context).unfocus()` ile keyboard dismiss
  - `SingleChildScrollView` kullanımı (klavye açıldığında içerik gizlenmesin)
- **Teknik Detaylar:**
  - Platform: **Flutter (Dart)** — Material 3
  - State management: **Provider** + `BaseViewModel` (`RegisterViewModel`)
  - Navigation: **GoRouter** (`AppRoutes.register` → `AppRoutes.login`)
  - HTTP: **Dio** + `AuthRepository.register(UserRegisterDto)`
  - Accessibility: `Semantics` widget'ları, label'lar

## 2. Kullanıcı Girişi (Login) Ekranı
- **API Endpoint:** `POST /auth/login`
- **Görev:** Kullanıcı giriş ekranı tasarımı ve implementasyonu (email veya telefon ile)
- **UI Bileşenleri:**
  - "Email veya Telefon" tek input alanı (`identifier`)
  - Şifre input alanı (obscureText)
  - "Giriş Yap" butonu
  - "Şifremi Unuttum?" link
  - "Hesabın yok mu? Kayıt Ol" link
  - Logo + uygulama başlığı
  - `CircularProgressIndicator` (login sırasında)
- **Form Validasyonu:**
  - `identifier` alanı email mi telefon mu otomatik algılanır
  - Şifre min uzunluk kontrolü
  - Tüm alanlar doldurulmadan buton disabled
- **Kullanıcı Deneyimi:**
  - Başarılı giriş → `AuthSessionViewModel.setAuthenticated(token, userId, refreshToken)` → GoRouter `AppRoutes.home`'a yönlendirir
  - 401 Unauthorized → "E-posta/telefon veya şifre hatalı"
  - Network hatası → "Bağlantı yok, daha sonra deneyin"
- **Teknik Detaylar:**
  - `LoginViewModel` (`ChangeNotifier`) — `loginSuccess`, `token`, `refreshToken`, `userId` state'leri
  - **Refresh token** response'tan alınır → `TokenStorage.saveRefreshToken` ile `flutter_secure_storage`'a yazılır
  - **AuthInterceptor** otomatik 401 → `/auth/refresh` rotation akışını yönetir
  - Login sonrası **FcmService.registerForUser()** çağrısı (cihaz FCM token'ı backend'e kaydedilir)

## 3. Profil Görüntüleme Ekranı
- **API Endpoint:** `GET /users/{userId}`
- **Görev:** Kullanıcı profil bilgilerini görüntüleme ekranı tasarımı ve implementasyonu
- **UI Bileşenleri:**
  - Profil avatar alanı (`CircleAvatar`, ilk harf placeholder)
  - Ad ve email (büyük başlık)
  - Telefon numarası (`ListTile`, ikonlu)
  - Cinsiyet, doğum tarihi (varsa, ikonlu)
  - Kayıt tarihi (`olusturulmaTarihi`)
  - "Profili Düzenle" butonu (`ElevatedButton.icon`)
  - "Çıkış Yap" butonu
  - "Hesabı Sil" butonu (kırmızı, `OutlinedButton` destructive)
  - `RefreshIndicator` (pull-to-refresh)
- **Kullanıcı Deneyimi:**
  - Loading state → `Shimmer` veya `CircularProgressIndicator`
  - Empty/Error state → "Yüklenemedi, tekrar dene" butonu
  - Smooth scroll (`CustomScrollView` + `SliverAppBar`)
- **Teknik Detaylar:**
  - `ProfileViewModel` — `User` modelini `UserRepository.getById(userId)` ile çeker
  - `AuthInterceptor` Bearer token'ı otomatik ekler
  - Token expire olursa refresh akışı transparently çalışır
  - Image caching: gelecekte avatar URL'i eklendiğinde `cached_network_image`

## 4. Profil Düzenleme Ekranı
- **API Endpoint:** `PUT /users/{userId}`
- **Görev:** Kullanıcı profil bilgilerini düzenleme ekranı
- **UI Bileşenleri:**
  - Ad input alanı (mevcut değerle dolu)
  - Telefon input alanı (mevcut değerle dolu, format maskesi)
  - "Yeni Şifre" input alanı (boş bırakılırsa değiştirilmez)
  - "Yeni Şifre Tekrar" input alanı
  - "Kaydet" butonu (`AppBar.actions` sağ üst veya alt FAB)
  - "İptal" butonu (sol üst leading)
- **Form Validasyonu:**
  - Telefon format kontrolü
  - Şifre alanları doldurulmuşsa min 8 karakter + eşleşme kontrolü
  - Değişiklik yapılmadan "Kaydet" disabled (`_isDirty` flag)
- **Kullanıcı Deneyimi:**
  - Optimistic update — kaydet sonrası UI hemen güncellenir
  - Başarı → `SnackBar` "Profil güncellendi"
  - Hata → değişiklik geri alınır + error mesaj
  - İptal'e basıldığında değişiklik varsa onay dialog'u (`showDialog`)
- **Teknik Detaylar:**
  - `EditProfileViewModel` — `_initialValues` vs `_currentValues` karşılaştırması (`isDirty`)
  - `UserUpdateDto` (sadece değişen alanlar)
  - `AuthRepository.updateProfile(userId, dto)`
  - `WillPopScope` ile unsaved changes uyarısı

## 5. Hesap Silme Akışı
- **API Endpoint:** `DELETE /users/{userId}`
- **Görev:** Kullanıcı hesabını silme işlemi için UI akışı
- **UI Bileşenleri:**
  - "Hesabı Sil" butonu (profilde, kırmızı, ikonlu)
  - İlk uyarı `AlertDialog` ("Bu işlem geri alınamaz")
  - Şifre doğrulama (opsiyonel ikinci dialog)
  - Son onay `AlertDialog` ("Eminim, sil")
- **Kullanıcı Deneyimi:**
  - Destructive action görsel uyarıları (kırmızı renk, uyarı ikonu)
  - Açık ve net mesajlar
  - İptal seçeneği her zaman mevcut
  - Silme sırasında loading
  - Başarılı silme → `AuthSessionViewModel.logout()` → Login ekranına yönlendirme (nav stack temizlenir)
- **Akış Adımları:**
  1. Profil ekranında "Hesabı Sil" butonuna tıklama
  2. İlk uyarı dialog'u
  3. Onaylandığında şifre re-entry
  4. Son onay dialog'u
  5. `AuthRepository.deleteAccount(userId)` çağrısı
  6. Başarılı → logout + Login ekranına nav reset
- **Teknik Detaylar:**
  - `showDialog<bool>` chain ile multi-step flow
  - Cascade delete: kullanıcı silinince yorumlar / listeler / cihazlar da MediatR `UserDeletedEvent` ile temizlenir (backend tarafı; mobil sadece sonucu gösterir)
  - `GoRouter.go(AppRoutes.login)` + `_authSession.handleSessionExpired()`

## 6. Çıkış (Logout) Akışı
- **API Endpoint:** `POST /auth/logout`
- **Görev:** Kullanıcı oturumunu sonlandırma
- **UI Bileşenleri:**
  - "Çıkış Yap" butonu (profil sayfasında, `ListTile` veya `OutlinedButton`)
  - Onay dialog'u ("Çıkış yapmak istediğinizden emin misiniz?")
  - `CircularProgressIndicator` (logout sırasında)
- **Kullanıcı Deneyimi:**
  - Çıkış başarılıysa otomatik Login ekranına yönlendirme
  - Backend hatası olsa bile yerel token temizlenir (`TokenStorage.clearAll`)
  - `SnackBar`: "Görüşmek üzere"
- **Akış:**
  1. Butona tıklama → onay dialog'u
  2. `FcmService.unregisterCurrentToken()` → backend'den cihaz silinir + Firebase token deleteToken
  3. `AuthRepository.logout(refreshToken)` çağrısı → backend JWT blacklist + refresh revoke
  4. `TokenStorage.clearAll` → secure storage temizlenir
  5. `GoRouter.go(AppRoutes.login)` + stack reset
- **Teknik Detaylar:**
  - `AuthSessionViewModel.logout()` orkestre eder
  - JWT JTI Redis blacklist'e yazılır (kalan TTL kadar)
  - Refresh token Redis'ten silinir
  - Hata olsa bile `_clearSession()` `finally` bloğunda çalışır (kullanıcı her zaman çıkış yapabilir)

---

## Ortak Teknik Notlar (Tüm Ekranlar)

- **Mimari:** MVVM (`lib/viewmodels/auth/`, `lib/view/pages/auth/`, `lib/view/pages/profile/`)
- **HTTP Layer:** Dio + `AuthInterceptor` + `ErrorInterceptor` + `LogInterceptor` (debug)
- **Token Yönetimi:** `flutter_secure_storage` (release) + in-memory map (debug)
- **Refresh Token Rotation:** `AuthInterceptor` 401'i yakalar → `/auth/refresh` → orijinal isteği tekrarlar
- **Navigation:** GoRouter declarative routing, `_authSession` `refreshListenable` olarak kullanılır
- **Form Yapısı:** `Form` + `GlobalKey<FormState>` + `TextFormField.validator`
