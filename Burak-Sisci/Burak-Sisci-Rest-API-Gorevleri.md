**API Test Videosu:** [Youtube Link](https://youtu.be/sZ2mSsM0JjU)

# Burak Sisci - REST API Gorevleri

## Modul A: Kullanici ve Kimlik Yonetimi (Backend)

---

### 1. Proje Altyapisi ve Yapilandirma

**Sorumluluk:** Projenin backend iskeletinin kurulmasi, veritabani baglantisi ve temel yapilandirmalarin yapilmasi.

**Yapilan Isler:**

- **Program.cs** dosyasinda tum uygulama yapilandirmasi:
  - MongoDB baglantisi (`IMongoClient` singleton kaydi, `CONNECTION_STRING` environment variable)
  - `MongoDbContext` ve `MongoTransactionManager` singleton servisleri
  - JSON serialization ayarlari (camelCase, case-insensitive, enum string converter)
  - CORS politikasi (AllowAnyOrigin, AllowAnyMethod, AllowAnyHeader)
  - MediatR entegrasyonu (`RegisterServicesFromAssembly`)
  - Middleware pipeline sirasi (ExceptionHandler > CORS > StaticFiles > Authentication > Authorization > Controllers)
  - Auth modulu servis kayitlari (IUserRepository, IPasswordHasher, ITokenBlacklist, RegisterUserCommand, LoginUserCommand, LogoutUserCommand)

- **Shared/Database/MongoDbContext.cs:**
  - MongoDB veritabani baglanti yoneticisi
  - `GetCollection<T>(string name)` metodu ile koleksiyon erisimi
  - `DATABASE_NAME` konfigurasyonundan veritabani adi okuma

- **Shared/Database/MongoTransactionManager.cs:**
  - MongoDB transaction yonetimi
  - `ExecuteInTransactionAsync()` metodu: session olusturma, transaction baslat/commit/abort
  - Generic ve non-generic versiyonlar
  - Kullanim yeri: Kayit sirasinda kullanici + varsayilan liste atomik olusturma

**Ilgili Dosyalar:**
- `backend/backend.API/Program.cs`
- `backend/backend.API/Shared/Database/MongoDbContext.cs`
- `backend/backend.API/Shared/Database/MongoTransactionManager.cs`

---

### 2. JWT Authentication ve Authorization Altyapisi

**Sorumluluk:** JWT token uretimi, dogrulama, kara liste mekanizmasi ve ASP.NET Core authentication pipeline kurulumu.

**Yapilan Isler:**

- **Shared/Security/JwtTokenGenerator.cs:**
  - `GenerateToken(string userId, string email)` - JWT token uretimi
    - Claims: Sub (userId), Email, Jti (benzersiz GUID), NameIdentifier (userId)
    - HMAC-SHA256 imzalama algoritmasi
    - Environment variable'lardan konfigurasyonlar: `JWT_SECRET`, `ISSUER`, `AUDIENCE`, `EXPIRYMINUTES` (varsayilan: 60 dk)
  - `GetJtiFromToken(string token)` - Token'dan JTI cikartma (logout icin)
  - `GetRemainingExpiry(string token)` - Kalan gecerlilik suresi hesaplama

- **Program.cs JWT Yapilandirmasi:**
  - `AddAuthentication(JwtBearerDefaults.AuthenticationScheme)` ile JWT eklenmesi
  - TokenValidationParameters: ValidateIssuer, ValidateAudience, ValidateIssuerSigningKey, ValidateLifetime
  - `ClockSkew = TimeSpan.Zero` ile hassas sure dogrulamasi

- **Redis Token Kara Liste (RedisTokenBlacklist.cs):**
  - `AddAsync(string jti, TimeSpan expiry)` - Token JTI'sini Redis'e ekleme (TTL ile)
  - `IsBlacklistedAsync(string jti)` - Token'in kara listede olup olmadigini kontrol
  - Redis key formati: `"blacklist:{jti}"`
  - Otomatik TTL ile suresi dolan tokenlar temizlenir

- **Program.cs Redis Yapilandirmasi:**
  - `REDIS_CONNECTION_STRING` environment variable kontrolu
  - `ConfigurationOptions` ile guvenli baslama (AbortOnConnectFail = false)
  - Baglanti zaman asimi: 10 saniye (ConnectTimeout, SyncTimeout)

- **JwtAuthMiddleware.cs:**
  - Her istekte Authorization header kontrolu
  - Bearer token'dan JTI cikartma
  - Redis kara liste sorgusu - 401 donusu (logout edilmis tokenlar icin)
  - Swagger endpoint'leri bypass

**Ilgili Dosyalar:**
- `backend/backend.API/Shared/Security/JwtTokenGenerator.cs`
- `backend/backend.API/Modules/Auth/Infrastructure/RedisTokenBlacklist.cs`
- `backend/backend.API/Presentation/Middlewares/JwtAuthMiddleware.cs`

---

### 3. Kullanici Domain Modeli

**Sorumluluk:** Kullanici varlik sinifinin tanimlanmasi.

**Yapilan Isler:**

- **Domain/User.cs:**
  - `Id` (string) - MongoDB ObjectId, otomatik uretilir
  - `Email` (string) - Kullanici e-posta adresi
  - `Phone` (string) - Telefon numarasi
  - `PasswordHash` (string) - BCrypt ile hashlanmis sifre
  - `CreatedAt` (DateTime) - Hesap olusturma tarihi
  - `ResetToken` (string?) - Sifre sifirlama tokeni (opsiyonel)
  - `ResetTokenExpires` (DateTime?) - Token gecerlilik suresi (opsiyonel)

**Ilgili Dosya:**
- `backend/backend.API/Modules/Auth/Domain/User.cs`

---

### 4. Kullanici Repository Katmani

**Sorumluluk:** MongoDB ile kullanici veri erisim islemleri.

**Yapilan Isler:**

- **IUserRepository.cs (Interface):**
  - `GetByEmailAsync(string email)` - Email ile kullanici sorgulama
  - `GetByIdAsync(string id)` - ID ile kullanici sorgulama
  - `ExistsByEmailAsync(string email)` - Email mevcut mu kontrolu
  - `ExistsByPhoneAsync(string phone)` - Telefon mevcut mu kontrolu
  - `CreateAsync(User user, IClientSessionHandle? session)` - Kullanici olusturma (transaction destekli)
  - `GetByResetTokenAsync(string token)` - Reset token ile kullanici bulma
  - `UpdateAsync(string id, User user)` - Kullanici guncelleme
  - `DeleteAsync(string id)` - Kullanici silme

- **MongoUserRepository.cs (Implementation):**
  - MongoDB koleksiyonu: `"users"`
  - Benzersiz index: Email ve Phone alanlari uzerinde unique index
  - Tum repository metodlarinin MongoDB sorgulari ile implementasyonu

**Ilgili Dosyalar:**
- `backend/backend.API/Modules/Auth/Application/IUserRepository.cs`
- `backend/backend.API/Modules/Auth/Infrastructure/MongoUserRepository.cs`

---

### 5. Sifre Hashleme Servisi

**Sorumluluk:** Kullanici sifrelerinin guvenli sekilde hashlenmesi ve dogrulanmasi.

**Yapilan Isler:**

- **IPasswordHasher.cs (Interface):**
  - `Hash(string password)` - Sifre hashleme
  - `Verify(string password, string hash)` - Sifre dogrulama

- **BCryptPasswordHasher.cs (Implementation):**
  - BCrypt.Net NuGet paketi kullanimi
  - Otomatik salt uretimi ile hashleme
  - Guvenli sifre karsilastirma

**Ilgili Dosyalar:**
- `backend/backend.API/Modules/Auth/Application/IPasswordHasher.cs`
- `backend/backend.API/Modules/Auth/Infrastructure/BCryptPasswordHasher.cs`

---

### 6. Kayit (Register) Islemi

**Sorumluluk:** Yeni kullanici kaydinin olusturulmasi.

**Yapilan Isler:**

- **RegisterUserCommand.cs:**
  - Girdi: `RegisterRequest(Email, Phone, Password)`
  - Cikti: `RegisterResult(UserId, Email, Token)`
  - Is mantigi:
    1. Email benzersizlik kontrolu (zaten varsa hata)
    2. Telefon benzersizlik kontrolu
    3. Sifre hashleme (BCrypt)
    4. MongoDB transaction icinde: kullanici olusturma + varsayilan "Favoriler" listesi olusturma
    5. JWT token uretimi
    6. Sonuc donusu

- **AuthController.cs - POST /auth/register:**
  - Status 201 Created donusu
  - Hata durumlarinda uygun HTTP status kodlari

**Ilgili Dosyalar:**
- `backend/backend.API/Modules/Auth/Application/RegisterUserCommand.cs`
- `backend/backend.API/Presentation/Controllers/AuthController.cs`

---

### 7. Giris (Login) Islemi

**Sorumluluk:** Kullanici kimlik dogrulamasi ve token uretimi.

**Yapilan Isler:**

- **LoginUserCommand.cs:**
  - Girdi: `LoginRequest(Email, Password)`
  - Cikti: `LoginResult(UserId, Email, Token)`
  - Is mantigi:
    1. Email ile kullanici bulma (bulunamazsa UnauthorizedAccessException)
    2. Sifre dogrulama (eslesme yoksa UnauthorizedAccessException)
    3. JWT token uretimi
    4. Sonuc donusu

- **AuthController.cs - POST /auth/login:**
  - Status 200 OK donusu
  - 401 Unauthorized (hatali kimlik bilgileri)

**Ilgili Dosyalar:**
- `backend/backend.API/Modules/Auth/Application/LoginUserCommand.cs`
- `backend/backend.API/Presentation/Controllers/AuthController.cs`

---

### 8. Cikis (Logout) Islemi

**Sorumluluk:** Aktif oturumun guvenli sekilde sonlandirilmasi.

**Yapilan Isler:**

- **LogoutUserCommand.cs:**
  - Girdi: JWT token (Authorization header'dan)
  - Is mantigi:
    1. Token'dan JTI (JWT ID) cikartma
    2. Kalan gecerlilik suresi hesaplama
    3. JTI'yi Redis kara listeye ekleme (TTL ile)
  - Sonuc: Token artik gecersiz, sonraki istekler 401 donecek

- **AuthController.cs - POST /auth/logout:**
  - `[Authorize]` attribute ile korunmus endpoint
  - Bearer token header'dan alinir
  - Status 200 OK donusu

**Ilgili Dosyalar:**
- `backend/backend.API/Modules/Auth/Application/LogoutUserCommand.cs`
- `backend/backend.API/Presentation/Controllers/AuthController.cs`

---

### 9. Profil Guncelleme Islemi

**Sorumluluk:** Kullanici profil bilgilerinin guncellenmesi.

**Yapilan Isler:**

- **UpdateProfileCommand.cs (MediatR Command):**
  - `UpdateProfileCommand(UserId, Phone?) : IRequest<bool>`
  - Handler: kullanici bulma, telefon guncelleme, kaydetme

- **AuthController.cs - PUT /auth/profile:**
  - `[Authorize]` attribute ile korunmus
  - UserId JWT claims'den alinir (`ClaimTypes.NameIdentifier`)
  - Girdi: `UpdateProfileRequestDto(Phone?)`
  - Status 200 OK veya 400 Bad Request

**Ilgili Dosyalar:**
- `backend/backend.API/Modules/Auth/Application/UpdateProfileCommand.cs`
- `backend/backend.API/Modules/Auth/Application/UpdateProfileCommandHandler.cs`
- `backend/backend.API/Presentation/Controllers/AuthController.cs`

---

### 10. Hesap Silme ve Cascade Delete

**Sorumluluk:** Kullanici hesabinin ve iliskili tum verilerin silinmesi.

**Yapilan Isler:**

- **DeleteUserCommand.cs (MediatR Command):**
  - `DeleteUserCommand(UserId) : IRequest<bool>`

- **DeleteUserCommandHandler.cs:**
  - Kullaniciyi veritabanindan siler
  - Basari durumunda `UserDeletedEvent` MediatR notification yayinlar
  - Diger moduller (Cars, Comments) bu event'i dinleyerek cascade silme yapar

- **UserDeletedEvent.cs (Shared Event):**
  - `UserDeletedEvent(UserId) : INotification`
  - Yayinlanma: DeleteUserCommandHandler
  - Dinleyiciler: Cars modulu (kullanicinin ilanlarini siler), Comments modulu (kullanicinin yorumlarini siler)

- **AuthController.cs - DELETE /auth/{id}:**
  - Status 200 OK veya 404 Not Found

**Ilgili Dosyalar:**
- `backend/backend.API/Modules/Auth/Application/DeleteUserCommand.cs`
- `backend/backend.API/Modules/Auth/Application/DeleteUserCommandHandler.cs`
- `backend/backend.API/Shared/Events/UserDeletedEvent.cs`
- `backend/backend.API/Presentation/Controllers/AuthController.cs`

---

### 11. Sifremi Unuttum ve Sifre Sifirlama

**Sorumluluk:** Sifre kurtarma akisinin uygulanmasi.

**Yapilan Isler:**

- **AuthController.cs - POST /auth/forgot-password:**
  - Girdi: `ForgotPasswordRequest(Email)`
  - Is mantigi:
    1. Email ile kullanici bulma
    2. Rastgele Base64 reset token uretimi
    3. Token gecerlilik suresi: 1 saat
    4. Token ve gecerlilik suresini veritabanina kaydetme
  - Cikti: `{ message, resetToken }`

- **AuthController.cs - POST /auth/reset-password:**
  - Girdi: `ResetPasswordRequest(Token, NewPassword)`
  - Is mantigi:
    1. Reset token ile kullanici bulma
    2. Token suresi dolmus mu kontrolu
    3. Yeni sifre hashleme (BCrypt)
    4. ResetToken ve ResetTokenExpires alanlarini temizleme
    5. Kullaniciyi guncelleme
  - Cikti: `{ message }`

**Ilgili Dosya:**
- `backend/backend.API/Presentation/Controllers/AuthController.cs`

---

### 12. Global Hata Yonetimi

**Sorumluluk:** Uygulamada olusabilecek hatalarin merkezi olarak yakalanmasi ve uygun HTTP yanit kodlariyla donulmesi.

**Yapilan Isler:**

- **GlobalExceptionHandler.cs:**
  - `InvalidOperationException` → 400 Bad Request
  - `UnauthorizedAccessException` → 401 Unauthorized
  - `ArgumentException` → 422 Unprocessable Entity
  - `KeyNotFoundException` → 404 Not Found
  - Diger hatalar → 500 Internal Server Error
  - ProblemDetails formati ile standart hata yanitlari

**Ilgili Dosya:**
- `backend/backend.API/Presentation/Middlewares/GlobalExceptionHandler.cs`

---

### Kullanilan Teknolojiler ve Kutuphaneler

| Teknoloji | Kullanim Alani |
|-----------|----------------|
| ASP.NET Core 10 | Web API framework |
| MongoDB.Driver | Veritabani erisimi |
| BCrypt.Net | Sifre hashleme |
| System.IdentityModel.Tokens.Jwt | JWT token islemleri |
| StackExchange.Redis | Token kara liste |
| MediatR | CQRS ve event-driven mimari |
| DotNetEnv | Environment variable yonetimi |

---

### API Endpoint Ozeti

| Metod | Endpoint | Yetki | Aciklama |
|-------|----------|-------|----------|
| POST | /auth/register | Hayir | Yeni kullanici kaydi |
| POST | /auth/login | Hayir | Kullanici girisi |
| POST | /auth/logout | Evet | Oturum sonlandirma |
| PUT | /auth/profile | Evet | Profil guncelleme |
| DELETE | /auth/{id} | Hayir | Hesap silme |
| POST | /auth/forgot-password | Hayir | Sifre sifirlama talebi |
| POST | /auth/reset-password | Hayir | Yeni sifre belirleme |
