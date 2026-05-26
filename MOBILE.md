# OtoPusula — Mobil Genişletme Geliştirici Dokümanı

> Bu doküman, mevcut web platformunun mobil (Android) sürümünü canlıya almak için **baştan sona** atılması gereken adımları sıralı şekilde içerir. Mevcut backend'e dokunmadan **yan servisler ekleyerek** ilerlenir.

---

## İçindekiler

1. [Hedef ve Kapsam (MVP)](#1-hedef-ve-kapsam-mvp)
2. [Yeni Sistem Mimarisi](#2-yeni-sistem-mimarisi)
3. [Teknoloji Stack (Mobil Genişletme)](#3-teknoloji-stack-mobil-genişletme)
4. [Yeni Bileşenler ve Sorumluluklar](#4-yeni-bileşenler-ve-sorumluluklar)
5. [Repo Yapısı (Hedef)](#5-repo-yapısı-hedef)
6. [FAZ 0 — Hazırlık ve Hesap Açma](#faz-0--hazırlık-ve-hesap-açma)
7. [FAZ 1 — Backend Eklemeleri (mevcut API'ye)](#faz-1--backend-eklemeleri-mevcut-apiye)
8. [FAZ 2 — RabbitMQ Altyapısı](#faz-2--rabbitmq-altyapısı)
9. [FAZ 3 — Notification Service (.NET Worker)](#faz-3--notification-service-net-worker)
10. [FAZ 4 — Firebase / FCM Kurulumu](#faz-4--firebase--fcm-kurulumu)
11. [FAZ 5 — Email Kuyruğu (Şifre Sıfırlama)](#faz-5--email-kuyruğu-şifre-sıfırlama)
12. [FAZ 6 — Flutter Mobil Uygulama](#faz-6--flutter-mobil-uygulama)
13. [FAZ 7 — Docker (Tüm Servisler)](#faz-7--docker-tüm-servisler)
14. [FAZ 8 — CI/CD (GitHub Actions)](#faz-8--cicd-github-actions)
15. [FAZ 9 — Cep Telefonunda Çalıştırma](#faz-9--cep-telefonunda-çalıştırma)
16. [Ortam Değişkenleri (Tam Liste)](#ortam-değişkenleri-tam-liste)
17. [Test Stratejisi](#test-stratejisi)
18. [Sorun Giderme](#sorun-giderme)
19. [Hosting Stratejisi (Fly.io)](#hosting-stratejisi-flyio)
20. [Post-MVP Roadmap](#post-mvp-roadmap)

---

## 1. Hedef ve Kapsam (MVP)

**Hedef:** Mevcut web platformunun fonksiyonlarına Android cihazlardan da erişilebilmesi; ek olarak yorum bildirimleri için **push notification** ve şifre sıfırlama için **email kuyruğu** mekanizmalarının kurulması.

**MVP'ye dahil:**
- Flutter Android uygulaması (iOS yok)
- Tüm mevcut REST endpoint'lerinin mobilden tüketilmesi
- Mobil için **refresh token** desteği (Redis tabanlı)
- RabbitMQ: iki kuyruk — `notifications.comments`, `emails.outbound`
- Yeni **Notification Service** (.NET worker) — RabbitMQ tüketicisi + FCM/SMTP gönderici
- Push notification: **sadece "ilanına yorum geldi"** tetikleyicisi
- Tüm servislerin Docker'a alınması
- GitHub Actions CI/CD pipeline'ları
- USB debugging veya release APK ile telefonda çalıştırma

**MVP dışı (sonraya):**
- iOS desteği
- Çoklu push trigger (fiyat düşüşü, yeni ilan vb.)
- In-app messaging
- App Store yayını

> **Hosting durumu (Nisan 2026):** Mevcut Railway servisleri kapatıldı; Backend / Frontend / ML şu an **canlıda değil**. Mobil MVP geliştirme süresince yerel `docker compose` üzerinden ve telefonun bilgisayarla aynı Wi-Fi'da olduğu setup'tan ilerlenecek. Canlıya çıkış zamanı geldiğinde **tüm servisler Fly.io'ya** deploy edilecektir (bkz. [Hosting Stratejisi](#hosting-stratejisi-flyio)).

---

## 2. Yeni Sistem Mimarisi

> URL'ler Fly.io'ya deploy aşamasında atanacaktır (örn. `otopusula-backend.fly.dev`). Geliştirme sürecinde tüm servisler `docker compose` ile yerelde çalışır.

```
┌─────────────────────┐    ┌─────────────────────────┐
│  Web Frontend       │    │  Flutter App (Android)  │
│  (mevcut, React)    │    │  YENİ                   │
└──────────┬──────────┘    └────────────┬────────────┘
           │ HTTPS                       │ HTTPS
           └──────────────┬──────────────┘
                          ▼
            ┌──────────────────────────────────┐
            │   Backend API (ASP.NET Core 10)  │
            │   mevcut + yeni endpoint'ler:    │
            │   • POST /auth/refresh           │
            │   • POST /devices/register       │
            │   • POST /devices/unregister     │
            │   Yorum oluştuğunda → RabbitMQ   │
            │   /forgot-password   → RabbitMQ  │
            └────────┬───────────────┬─────────┘
                     │               │
            ┌────────▼─────┐   ┌─────▼──────────────┐
            │  MongoDB     │   │   RabbitMQ         │ ← YENİ
            │  Atlas       │   │   (CloudAMQP)      │
            │  (mevcut)    │   │   • notifications. │
            │              │   │     comments       │
            │  + devices   │   │   • emails.        │
            │    koleksy.  │   │     outbound       │
            └──────────────┘   └──────────┬─────────┘
                     ▲                    │
                     │                    ▼
            ┌────────┴───────┐   ┌────────────────────┐
            │   Redis        │   │ Notification       │ ← YENİ
            │   (mevcut +    │   │ Service (.NET 10   │
            │   refresh      │   │ BackgroundService) │
            │   tokens)      │   │ • RabbitMQ tüket.  │
            └────────────────┘   │ • FCM gönderim     │
                                 │ • SMTP gönderim    │
                                 └─────────┬──────────┘
                                           │
                          ┌────────────────┴────────┐
                          ▼                         ▼
               ┌──────────────────┐      ┌────────────────┐
               │ Firebase Cloud   │      │ SMTP Provider  │
               │ Messaging (FCM)  │      │ (Gmail/SendGrid│
               │ (Android push)   │      │  free tier)    │
               └──────────────────┘      └────────────────┘
```

**Etkileşim akışları:**

**(A) Yorum bildirimi:**
1. Mobil/Web kullanıcı yorum yapar → `POST /comments`
2. Backend yorumu kaydeder + RabbitMQ'ya `comment.created` mesajını `notifications.comments` kuyruğuna yayınlar
3. Notification Service mesajı tüketir → ilan sahibinin `devices` kayıtlarındaki FCM token'ları çeker
4. FCM API'ye gönderim yapar → Android cihazda bildirim belirir

**(B) Şifre sıfırlama:**
1. Kullanıcı `POST /auth/forgot-password` çağırır
2. Backend reset token üretir + RabbitMQ'ya `email.send` mesajını `emails.outbound` kuyruğuna yayınlar
3. Notification Service tüketir → SMTP ile email gönderir

**(C) Refresh token:**
1. Login'de backend hem **access token** (kısa, 15 dk) hem **refresh token** (uzun, 30 gün) döner
2. Refresh token Redis'te `refresh:{userId}:{tokenId}` key'iyle saklanır
3. Access süresi dolunca mobil `POST /auth/refresh` ile yeni access alır
4. Logout'ta refresh token Redis'ten silinir

---

## 3. Teknoloji Stack (Mobil Genişletme)

| Katman | Teknoloji | Amaç |
|--------|-----------|------|
| Mobil UI | Flutter 3.x (Dart) | Android uygulama |
| Mobil HTTP | `dio` paketi | REST çağrıları + interceptor (refresh token) |
| Mobil State | `flutter_riverpod` | State management |
| Mobil Storage | `flutter_secure_storage` | Token saklama |
| Push (mobil) | `firebase_messaging` | FCM token alma + bildirim dinleme |
| Mesaj kuyruğu | RabbitMQ (CloudAMQP free tier) | Async iş kuyruğu |
| Notification svc | .NET 10 Worker Service | RabbitMQ tüketici |
| RabbitMQ client | `RabbitMQ.Client` (NuGet) | Backend ve worker için |
| FCM gönderim | `FirebaseAdmin` (NuGet) | Server-side push |
| Email gönderim | `MailKit` (NuGet) | SMTP |
| Email provider | Gmail SMTP veya Brevo (Sendinblue) free tier | Email yollama |
| Konteyner | Docker + docker-compose | Yerel ortam |
| CI/CD | GitHub Actions | Build, test, deploy |
| APK build | Flutter `flutter build apk --release` | Telefonda çalıştırma |

---

## 4. Yeni Bileşenler ve Sorumluluklar

| Bileşen | Yeni mi? | Sorumlu | Sahiplik |
|---------|----------|---------|----------|
| `mobile/` (Flutter) | YENİ | Tüm ekip | Ortak |
| `backend.API` Auth eklemeleri (refresh, devices) | Eklendi | Burak Şişci | Auth modülü |
| `backend.API` RabbitMQ publisher | Eklendi | İlgili modül sahibi | Comments + Auth |
| `notification-service/` | YENİ | Burak Şişci (öneri) | Yeni modül |
| `docker-compose.yml` | YENİ | Burak Şişci | DevOps |
| `.github/workflows/` | YENİ | Burak Şişci | DevOps |

---

## 5. Repo Yapısı (Hedef)

```
A2M2_Ekibi_Oto_Pusula_Uygulamasi/
├── backend/
│   └── backend.API/                    (mevcut, eklemeler yapılacak)
├── frontend/                           (mevcut, dokunulmaz)
├── ML_Model_V4/                        (mevcut, dokunulmaz)
├── mobile/                             ← YENİ (Flutter projesi)
│   ├── android/
│   ├── lib/
│   │   ├── main.dart
│   │   ├── core/
│   │   │   ├── api/                    (dio instance, interceptor)
│   │   │   ├── auth/                   (token storage, refresh logic)
│   │   │   └── push/                   (FCM init, token register)
│   │   ├── features/
│   │   │   ├── auth/
│   │   │   ├── cars/
│   │   │   ├── lists/
│   │   │   ├── comments/
│   │   │   └── prediction/
│   │   └── shared/
│   ├── pubspec.yaml
│   ├── android/app/google-services.json (Firebase, .gitignore'a)
│   └── Dockerfile                      (CI build için, opsiyonel)
├── notification-service/               ← YENİ
│   ├── NotificationService.csproj
│   ├── Program.cs
│   ├── Workers/
│   │   ├── CommentNotificationWorker.cs
│   │   └── EmailWorker.cs
│   ├── Services/
│   │   ├── FcmSender.cs
│   │   └── SmtpSender.cs
│   ├── Models/
│   │   └── (event DTO'lar — backend ile paylaşılan)
│   └── Dockerfile
├── docker-compose.yml                  ← YENİ (yerel ortam: backend+rabbit+redis+notif)
├── docker-compose.prod.yml             ← YENİ (Railway için referans)
├── .github/
│   └── workflows/                      ← YENİ
│       ├── backend-ci.yml
│       ├── notification-service-ci.yml
│       ├── mobile-ci.yml
│       └── mobile-apk-release.yml
├── DEVELOPER.md                        (mevcut)
├── DEPLOY.md                           (mevcut, güncellenecek)
└── MOBILE.md                           (bu dosya)
```

---

## FAZ 0 — Hazırlık ve Hesap Açma

> Tek seferlik, kod yazmadan önce yapılması gerekenler.

### 0.1 Üçüncü taraf hesaplar
- [ ] **CloudAMQP** hesabı aç → "Little Lemur" free tier instance oluştur (1M mesaj/ay, 20 connection). Bağlantı string'i şu formatta gelir: `amqps://user:pass@host/vhost`
- [ ] **Firebase** hesabı aç → Yeni proje oluştur ("OtoPusula") → Android app ekle:
  - Package name: `com.otopusula.app` (mobil tarafında bu ismi kullanacaksın)
  - `google-services.json` indir → `mobile/android/app/` altına koy
  - Firebase Console → **Project Settings → Service Accounts → Generate new private key** → JSON indir (Notification Service kullanacak)
- [ ] **SMTP sağlayıcı** seç:
  - **Gmail SMTP** (App password ile, ücretsiz, 500 mail/gün limit) — MVP için yeterli
  - veya **Brevo** (eski Sendinblue) free tier — 300 mail/gün, profesyonel
- [ ] **Fly.io** hesabı aç (canlıya alma fazına gelene kadar opsiyonel — bkz. [Hosting Stratejisi](#hosting-stratejisi-flyio)). Kayıt sonrası kredi kartı doğrulaması istenir; "Hobby Plan" ($1.94/ay'a kadar küçük makineler ücretsiz kullanım kredisi içinde).

### 0.2 Yerel araçlar
- [ ] **Flutter SDK 3.x** kur → `flutter doctor` temiz çıkmalı
- [ ] **Android Studio** + Android SDK + emulator (en az Android 11 / API 30)
- [ ] **Java 17** (Flutter Android build için gerekli)
- [ ] **Docker Desktop** kurulu ve çalışıyor olmalı
- [ ] **.NET 10 SDK** zaten var (backend için)
- [ ] **flyctl** CLI (deploy fazına gelene kadar opsiyonel): `iwr https://fly.io/install.ps1 -useb | iex` (PowerShell)

### 0.3 Doğrulama checklist
```bash
flutter doctor -v          # Tüm satırlar ✓ (chrome dışında)
docker --version           # 24.x+
dotnet --version           # 10.x
adb devices                # Telefon/emulator listesi
```

---

## FAZ 1 — Backend Eklemeleri (mevcut API'ye)

> Mevcut `backend/backend.API` projesine yapılacak eklemeler. Hiçbir mevcut endpoint kırılmayacak.

### 1.1 Refresh token desteği

**Yeni dosyalar / değişiklikler:**

`Modules/Auth/Domain/RefreshToken.cs`:
```csharp
public class RefreshToken
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string UserId { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public string DeviceInfo { get; set; } = "";
}
```

`Modules/Auth/Application/Interfaces/IRefreshTokenStore.cs`:
- `Task SaveAsync(RefreshToken t)` → Redis'e `refresh:{userId}:{tokenId}` key'iyle TTL'li yaz
- `Task<RefreshToken?> GetAsync(string userId, string tokenId)` → Redis'ten oku
- `Task RevokeAsync(string userId, string tokenId)` → Redis'ten sil
- `Task RevokeAllForUserAsync(string userId)` → `refresh:{userId}:*` pattern'inde sil (logout-all)

`Shared/Security/JwtTokenGenerator.cs` güncelleme:
- Access token süresi `EXPIRYMINUTES`'tan **15 dakikaya** düşürülecek (yeni env: `ACCESS_TOKEN_MINUTES=15`)
- Yeni metot: `GenerateRefreshToken(userId, deviceInfo)` → opaque random string (32 byte base64) + Redis'e yaz, 30 gün TTL (yeni env: `REFRESH_TOKEN_DAYS=30`)

**Yeni endpoint'ler:**

```http
POST /auth/refresh
Body: { "refreshToken": "..." }
Response: { "accessToken": "...", "refreshToken": "..." }   # rotation
```

```http
POST /auth/logout
Authorization: Bearer <access>
Body: { "refreshToken": "..." }      # opsiyonel, body yoksa sadece JTI blacklist
```

**Login response güncellemesi:**
```json
{
  "userId": "...",
  "email": "...",
  "accessToken": "...",
  "refreshToken": "...",
  "accessExpiresIn": 900
}
```

> **Geriye dönük uyumluluk:** `token` alanı response'ta `accessToken` ile aynı değerle bir süre daha tutulabilir; web frontend'i kırılmasın.

### 1.2 Device (FCM token) yönetimi

`Modules/Auth/Domain/Device.cs`:
```csharp
public class Device
{
    public ObjectId Id { get; set; }
    public string UserId { get; set; } = default!;
    public string FcmToken { get; set; } = default!;
    public string Platform { get; set; } = "android";
    public DateTime LastSeenAt { get; set; } = DateTime.UtcNow;
}
```

MongoDB'de yeni koleksiyon: **`devices`** — `(userId, fcmToken)` üzerinde unique index.

**Yeni endpoint'ler:**

```http
POST /devices/register
Authorization: Bearer <access>
Body: { "fcmToken": "...", "platform": "android" }
→ Aynı token varsa upsert, yoksa insert
```

```http
DELETE /devices/unregister
Authorization: Bearer <access>
Body: { "fcmToken": "..." }
```

### 1.3 RabbitMQ publisher

NuGet paketi ekle:
```bash
dotnet add backend/backend.API package RabbitMQ.Client
```

`Shared/Messaging/IEventPublisher.cs`:
```csharp
public interface IEventPublisher
{
    Task PublishAsync<T>(string exchange, string routingKey, T payload, CancellationToken ct = default);
}
```

`Shared/Messaging/RabbitMqPublisher.cs` — `IConnection` singleton ile, channel'ı her publish'te aç/kapa veya channel pool kullan. MVP için her publish'te `using var channel = _connection.CreateModel();` yeterli.

`Program.cs` DI kayıtları:
```csharp
builder.Services.AddSingleton<IConnection>(sp => {
    var factory = new ConnectionFactory {
        Uri = new Uri(Environment.GetEnvironmentVariable("RABBITMQ_URL")!)
    };
    return factory.CreateConnection("backend-api");
});
builder.Services.AddSingleton<IEventPublisher, RabbitMqPublisher>();
```

**Topology (publisher tarafında declare edilmesi):**
- Exchange: `otopusula` (type: `topic`, durable: true)
- Routing key'ler:
  - `comment.created`
  - `email.send`

### 1.4 Yorum oluşturma akışına publisher entegrasyonu

`Modules/Comments/Application/Commands/CreateCommentCommandHandler.cs` içinde yorum kaydedildikten sonra:

```csharp
await _eventPublisher.PublishAsync("otopusula", "comment.created", new CommentCreatedEvent {
    CommentId = comment.Id,
    CarId = comment.CarId,
    CarOwnerId = car.UserId,        // bildirim alacak kişi
    AuthorUserId = comment.UserId,
    AuthorEmail = author.Email,
    CarBrand = car.Brand,
    CarModel = car.Model,
    Text = comment.Text,
    CreatedAt = comment.CreatedAt
});
```

> **Self-comment kuralı:** Kullanıcı kendi ilanına yorum yaparsa bildirim gönderme. Bu kontrolü Notification Service tarafında `if (AuthorUserId == CarOwnerId) return;` ile yap.

### 1.5 Şifre sıfırlamaya publisher entegrasyonu

`Modules/Auth/Application/Commands/ForgotPasswordCommandHandler.cs` içinde reset token üretildikten sonra:

```csharp
await _eventPublisher.PublishAsync("otopusula", "email.send", new EmailSendEvent {
    To = user.Email,
    TemplateId = "password-reset",
    Variables = new Dictionary<string, string> {
        ["ResetUrl"] = $"{frontendUrl}/reset-password?token={resetToken}",
        ["UserName"] = user.Email
    }
});
```

### 1.6 Yeni env değişkenleri (backend)

`.env` dosyasına ekle:
```
ACCESS_TOKEN_MINUTES=15
REFRESH_TOKEN_DAYS=30
RABBITMQ_URL=amqps://user:pass@host/vhost
FRONTEND_URL=https://otopusula-backend.onrender.com
```

### 1.7 Test
- [ ] Postman: `/auth/login` → access + refresh dönüyor mu?
- [ ] Postman: `/auth/refresh` → yeni access + yeni refresh dönüyor mu? (rotation)
- [ ] Postman: `/devices/register` → MongoDB `devices` koleksiyonunda kayıt var mı?
- [ ] Yorum POST sonrası RabbitMQ Web UI'da (`https://<cloudamqp>/#/queues`) `notifications.comments` kuyruğunda mesaj birikiyor mu? (Faz 2'den sonra tüketici alacak.)

---

## FAZ 2 — RabbitMQ Altyapısı

### 2.1 CloudAMQP topology kurulumu

CloudAMQP web arayüzü → "RabbitMQ Manager" → şunları el ile oluştur (veya Notification Service ilk açılışta declare etsin — önerilen):

**Exchange:**
- `otopusula` — `topic`, `durable: true`

**Queue'lar:**
- `notifications.comments` — `durable: true`
- `emails.outbound` — `durable: true`
- `notifications.comments.dlq` — dead letter (mesaj 3 kez başarısız olursa)
- `emails.outbound.dlq` — dead letter

**Binding'ler:**
- `otopusula` → `notifications.comments` (routing key: `comment.created`)
- `otopusula` → `emails.outbound` (routing key: `email.send`)

### 2.2 Mesaj contract'ları (paylaşılan DTO)

İki çözüm:
- **(a) Kopyala-yapıştır:** Backend ve Notification Service her ikisinde de aynı POCO sınıfları
- **(b) Shared library:** `OtoPusula.Contracts` adında ayrı bir class library

MVP için **(a)** yeterli. Class library ileride çıkarılır.

`CommentCreatedEvent.cs`:
```csharp
public class CommentCreatedEvent
{
    public string CommentId { get; set; } = "";
    public string CarId { get; set; } = "";
    public string CarOwnerId { get; set; } = "";
    public string AuthorUserId { get; set; } = "";
    public string AuthorEmail { get; set; } = "";
    public string CarBrand { get; set; } = "";
    public string CarModel { get; set; } = "";
    public string Text { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}
```

`EmailSendEvent.cs`:
```csharp
public class EmailSendEvent
{
    public string To { get; set; } = "";
    public string TemplateId { get; set; } = "";  // password-reset, welcome, ...
    public Dictionary<string, string> Variables { get; set; } = new();
}
```

JSON serileştirme — `System.Text.Json` ile.

---

## FAZ 3 — Notification Service (.NET Worker)

### 3.1 Proje oluşturma

```bash
cd <repo-root>
dotnet new worker -n NotificationService -o notification-service
cd notification-service
dotnet add package RabbitMQ.Client
dotnet add package FirebaseAdmin
dotnet add package MailKit
dotnet add package MongoDB.Driver        # device token'ları okumak için
dotnet add package DotNetEnv
```

### 3.2 Program.cs iskelet

```csharp
using DotNetEnv;
Env.Load();

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IConnection>(sp => {
    var factory = new ConnectionFactory {
        Uri = new Uri(Environment.GetEnvironmentVariable("RABBITMQ_URL")!),
        DispatchConsumersAsync = true
    };
    return factory.CreateConnection("notification-service");
});

builder.Services.AddSingleton<IMongoDatabase>(sp => {
    var client = new MongoClient(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
    return client.GetDatabase(Environment.GetEnvironmentVariable("DATABASE_NAME"));
});

builder.Services.AddSingleton<FcmSender>();
builder.Services.AddSingleton<SmtpSender>();

builder.Services.AddHostedService<CommentNotificationWorker>();
builder.Services.AddHostedService<EmailWorker>();

var host = builder.Build();
host.Run();
```

### 3.3 CommentNotificationWorker akışı

```
1. notifications.comments kuyruğuna abone ol (manual ack, prefetch=10)
2. Mesaj geldiğinde:
   2a. JSON deserialize → CommentCreatedEvent
   2b. AuthorUserId == CarOwnerId ise ack ve geç (self-comment)
   2c. MongoDB'den devices koleksiyonunda userId == CarOwnerId olanları çek
   2d. Her FCM token için FcmSender.SendAsync(token, title, body) çağır
       - title: "Yeni yorum: {CarBrand} {CarModel}"
       - body: Text (max 100 karakter, sonu "...")
   2e. Tüm token'lar için gönderim bittikten sonra ack
   2f. Hata olursa: 3 kez retry (delivery count header), sonra DLQ'ya nack
```

### 3.4 FcmSender.cs

```csharp
public class FcmSender
{
    public FcmSender()
    {
        FirebaseApp.Create(new AppOptions {
            Credential = GoogleCredential.FromFile("firebase-credentials.json")
        });
    }

    public async Task SendAsync(string fcmToken, string title, string body, IDictionary<string,string>? data = null)
    {
        var message = new Message {
            Token = fcmToken,
            Notification = new Notification { Title = title, Body = body },
            Data = data
        };
        try {
            await FirebaseMessaging.DefaultInstance.SendAsync(message);
        } catch (FirebaseMessagingException ex) when (ex.MessagingErrorCode == MessagingErrorCode.Unregistered) {
            // Geçersiz token → MongoDB'den sil
        }
    }
}
```

`firebase-credentials.json` → Faz 0'da indirdiğin service account JSON'u. **Asla git'e commit etme**, `.gitignore`'a ekle. Production'da Railway secret olarak dosya mount edilir veya base64 env'den dosyaya yazılır.

### 3.5 EmailWorker akışı

```
1. emails.outbound kuyruğuna abone ol
2. Mesaj geldiğinde:
   2a. EmailSendEvent deserialize
   2b. TemplateId'ye göre subject + html üret (basit string.Replace ile değişken yerleştir)
   2c. SmtpSender.SendAsync(to, subject, html)
   2d. Ack veya DLQ
```

### 3.6 SmtpSender.cs (MailKit ile)

Env değişkenleri: `SMTP_HOST`, `SMTP_PORT`, `SMTP_USER`, `SMTP_PASS`, `SMTP_FROM`.

Gmail için: `smtp.gmail.com:587`, `SMTP_PASS` = Google Account → Security → App Password ile üretilen 16 karakterlik şifre.

### 3.7 Notification Service env değişkenleri

```
RABBITMQ_URL=amqps://...
CONNECTION_STRING=mongodb+srv://...
DATABASE_NAME=projctdb
FIREBASE_CREDENTIALS_PATH=firebase-credentials.json
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USER=otopusula.app@gmail.com
SMTP_PASS=xxxxxxxxxxxxxxxx
SMTP_FROM=OtoPusula <noreply@otopusula.app>
```

### 3.8 Test
- [ ] Worker'ı yerel başlat: `dotnet run`
- [ ] Backend'den yorum oluştur
- [ ] Worker log'unda "Comment {id} processed, sent to N devices" görüyor musun?
- [ ] Test cihazında bildirim geliyor mu? (Faz 6 sonrası)

---

## FAZ 4 — Firebase / FCM Kurulumu

### 4.1 Firebase Console adımları (Faz 0'da hesap açtıktan sonra)
- [ ] Project Settings → Cloud Messaging → API "Enabled" olmalı
- [ ] Project Settings → Service Accounts → Generate new private key → indir → `notification-service/firebase-credentials.json` olarak kaydet
- [ ] **`.gitignore`** içine ekle:
  ```
  notification-service/firebase-credentials.json
  mobile/android/app/google-services.json
  ```

### 4.2 Mobil tarafta entegrasyon (özet — detayı Faz 6'da)
- `pubspec.yaml`:
  ```yaml
  firebase_core: ^3.x
  firebase_messaging: ^15.x
  ```
- `mobile/android/build.gradle` ve `mobile/android/app/build.gradle` içinde Google services plugin satırları (Firebase Console'un verdiği talimatları takip et)
- App açılışında:
  ```dart
  await Firebase.initializeApp();
  final fcmToken = await FirebaseMessaging.instance.getToken();
  // Login sonrası: POST /devices/register
  ```

### 4.3 Test bildirimi gönderimi
Firebase Console → Cloud Messaging → "Send your first message" → cihaz token'ı ile manuel deneme yap. Bildirim gelirse pipeline temiz.

---

## FAZ 5 — Email Kuyruğu (Şifre Sıfırlama)

Faz 3'te `EmailWorker` zaten yazıldı. Bu fazda template'leri ve test akışını tamamlayalım.

### 5.1 Email template'leri

`notification-service/Templates/password-reset.html`:
```html
<!DOCTYPE html>
<html>
<body style="font-family: sans-serif;">
  <h2>OtoPusula — Şifre Sıfırlama</h2>
  <p>Merhaba {{UserName}},</p>
  <p>Şifrenizi sıfırlamak için aşağıdaki bağlantıya tıklayın (15 dakika geçerli):</p>
  <p><a href="{{ResetUrl}}">Şifremi Sıfırla</a></p>
  <p>Bu isteği siz yapmadıysanız bu emaili görmezden gelebilirsiniz.</p>
</body>
</html>
```

`Templates/` klasörü `.csproj` içinde `<Content CopyToOutputDirectory="Always" />` olarak işaretlenmeli.

### 5.2 Template engine (basit)
MVP için `string.Replace("{{UserName}}", value)` yeterli. İleride Scriban veya Razor kullanılabilir.

### 5.3 Test
- [ ] `/auth/forgot-password` çağır → 1-2 sn içinde inbox'a email düşüyor mu?
- [ ] Spam klasörünü kontrol et (Gmail'de SPF/DKIM olmadığı için spam'e düşebilir — Brevo daha iyi sonuç verir)

---

## FAZ 6 — Flutter Mobil Uygulama

### 6.1 Proje oluşturma

```bash
cd <repo-root>
flutter create --org com.otopusula --project-name otopusula mobile
cd mobile
```

### 6.2 Bağımlılıklar (`pubspec.yaml`)

```yaml
dependencies:
  flutter:
    sdk: flutter
  dio: ^5.4.0
  flutter_riverpod: ^2.5.0
  flutter_secure_storage: ^9.2.0
  go_router: ^14.0.0
  firebase_core: ^3.6.0
  firebase_messaging: ^15.1.0
  intl: ^0.19.0
  cached_network_image: ^3.3.0
  flutter_dotenv: ^5.1.0       # config için
  image_picker: ^1.1.0          # ilan eklemek için
```

### 6.3 Klasör yapısı (`lib/`)

```
lib/
├── main.dart
├── core/
│   ├── api/
│   │   ├── api_client.dart          (dio instance + interceptor)
│   │   └── api_endpoints.dart       (string sabitleri)
│   ├── auth/
│   │   ├── auth_repository.dart
│   │   ├── token_storage.dart       (flutter_secure_storage wrapper)
│   │   └── auth_interceptor.dart    (refresh token logic)
│   ├── push/
│   │   ├── fcm_service.dart
│   │   └── notification_handler.dart
│   └── routing/
│       └── app_router.dart
├── features/
│   ├── auth/
│   │   ├── data/
│   │   ├── domain/
│   │   └── presentation/
│   │       ├── login_screen.dart
│   │       ├── register_screen.dart
│   │       └── forgot_password_screen.dart
│   ├── cars/
│   │   └── presentation/
│   │       ├── car_list_screen.dart
│   │       ├── car_detail_screen.dart
│   │       └── add_car_screen.dart
│   ├── lists/
│   ├── comments/
│   └── prediction/
└── shared/
    ├── widgets/
    └── theme/
```

### 6.4 API client (`core/api/api_client.dart`)

```dart
class ApiClient {
  late final Dio dio;
  ApiClient(TokenStorage storage) {
    dio = Dio(BaseOptions(
      baseUrl: const String.fromEnvironment('API_URL',
        defaultValue: 'https://otopusula-backend.onrender.com'),
      connectTimeout: const Duration(seconds: 10),
      receiveTimeout: const Duration(seconds: 15),
    ));
    dio.interceptors.add(AuthInterceptor(dio, storage));
  }
}
```

### 6.5 Auth interceptor (refresh akışı)

```
1. Her isteğe Authorization: Bearer {access} ekle
2. 401 dönerse:
   2a. Token storage'dan refreshToken al
   2b. POST /auth/refresh çağır
   2c. Başarılıysa yeni access + refresh sakla, orijinal isteği tekrarla
   2d. Başarısızsa logout (storage temizle, login sayfasına yönlendir)
3. Eşzamanlı 401'ler için tek refresh çağrısı (lock/mutex)
```

### 6.6 Token storage

```dart
class TokenStorage {
  final _storage = const FlutterSecureStorage();
  static const _accessKey = 'access_token';
  static const _refreshKey = 'refresh_token';

  Future<void> save(String access, String refresh) async {
    await _storage.write(key: _accessKey, value: access);
    await _storage.write(key: _refreshKey, value: refresh);
  }
  Future<String?> readAccess() => _storage.read(key: _accessKey);
  Future<String?> readRefresh() => _storage.read(key: _refreshKey);
  Future<void> clear() => _storage.deleteAll();
}
```

### 6.7 FCM entegrasyonu (`core/push/fcm_service.dart`)

```dart
class FcmService {
  final ApiClient api;
  FcmService(this.api);

  Future<void> initAndRegister() async {
    await Firebase.initializeApp();
    final messaging = FirebaseMessaging.instance;

    // Android 13+ için bildirim izni
    await messaging.requestPermission();

    // Token al
    final token = await messaging.getToken();
    if (token == null) return;

    // Backend'e kaydet (login sonrası çağrılmalı)
    await api.dio.post('/devices/register', data: {
      'fcmToken': token,
      'platform': 'android',
    });

    // Token yenilendiğinde
    messaging.onTokenRefresh.listen((newToken) async {
      await api.dio.post('/devices/register', data: {
        'fcmToken': newToken,
        'platform': 'android',
      });
    });

    // Foreground bildirim
    FirebaseMessaging.onMessage.listen(_handleForeground);
  }

  void _handleForeground(RemoteMessage msg) {
    // local notification göster veya in-app banner
  }
}
```

### 6.8 AndroidManifest izinleri (`mobile/android/app/src/main/AndroidManifest.xml`)

```xml
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.POST_NOTIFICATIONS" />
```

### 6.9 Ekran listesi (MVP)
- [ ] Splash + auto-login (storage'da access varsa direkt ana ekran)
- [ ] Login
- [ ] Register
- [ ] Forgot password
- [ ] Reset password (deep link ile email'deki linkten gelir — post-MVP, MVP'de web'e yönlendir yeterli)
- [ ] Car list (filtreli)
- [ ] Car detail (resimler, yorumlar, fiyat tahmini butonu)
- [ ] Add car
- [ ] My lists
- [ ] List detail
- [ ] My profile (logout, hesap silme)
- [ ] Comment add (car detail içinde)

### 6.10 Build ve çalıştırma
```bash
cd mobile
flutter pub get
flutter run                                  # debug
flutter build apk --release                  # release APK
# çıktı: build/app/outputs/flutter-apk/app-release.apk
```

---

## FAZ 7 — Docker (Tüm Servisler)

### 7.1 Backend Dockerfile (zaten var, kontrol et)

`backend/Dockerfile` — multi-stage:
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY backend.API/*.csproj backend.API/
RUN dotnet restore backend.API/backend.API.csproj
COPY backend.API/. backend.API/
RUN dotnet publish backend.API/backend.API.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "backend.API.dll"]
```

### 7.2 Notification Service Dockerfile

`notification-service/Dockerfile`:
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "NotificationService.dll"]
```

> **Önemli:** `firebase-credentials.json` Dockerfile'da kopyalanmamalı. Production'da volume veya base64 env-to-file ile geçirilir.

### 7.3 docker-compose.yml (yerel ortam)

```yaml
version: "3.9"
services:
  redis:
    image: redis:7-alpine
    ports: ["6379:6379"]

  rabbitmq:
    image: rabbitmq:3.13-management
    ports: ["5672:5672", "15672:15672"]
    environment:
      RABBITMQ_DEFAULT_USER: dev
      RABBITMQ_DEFAULT_PASS: dev

  backend:
    build: ./backend
    ports: ["5078:8080"]
    env_file: ./backend/backend.API/.env.docker
    depends_on: [redis, rabbitmq]

  notification:
    build: ./notification-service
    env_file: ./notification-service/.env.docker
    volumes:
      - ./notification-service/firebase-credentials.json:/app/firebase-credentials.json:ro
    depends_on: [rabbitmq]
```

`.env.docker` dosyalarında:
- `REDIS_CONNECTION_STRING=redis:6379`
- `RABBITMQ_URL=amqp://dev:dev@rabbitmq:5672`

(MongoDB Atlas zaten cloud'da, container içinde değil.)

### 7.4 Yerel ortamı ayağa kaldırma
```bash
docker compose up -d
docker compose logs -f notification
```

RabbitMQ Management UI: http://localhost:15672 (dev/dev)

---

## FAZ 8 — CI/CD (GitHub Actions)

### 8.1 Genel strateji
- Her servis için **ayrı workflow**, sadece o servisin path'i değiştiğinde tetiklenir (`paths:` filter ile)
- `main`'e push → otomatik Railway deploy (Railway zaten Git push'ta otomatik build eder, ama biz **test + lint**'i GitHub'da çalıştırırız)
- Mobil için: APK build → GitHub Releases'e artifact yükle

### 8.2 `.github/workflows/backend-ci.yml`

```yaml
name: Backend CI
on:
  push:
    branches: [main]
    paths: ['backend/**', '.github/workflows/backend-ci.yml']
  pull_request:
    paths: ['backend/**']

jobs:
  build-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'
      - run: dotnet restore backend/backend.API/backend.API.csproj
      - run: dotnet build backend/backend.API/backend.API.csproj --no-restore -c Release
      - run: dotnet test backend/  # test projeleri eklenince
```

### 8.3 `.github/workflows/notification-service-ci.yml`

Yapı backend ile aynı — `paths: ['notification-service/**']`.

### 8.4 `.github/workflows/mobile-ci.yml`

```yaml
name: Mobile CI
on:
  push:
    branches: [main]
    paths: ['mobile/**']
  pull_request:
    paths: ['mobile/**']

jobs:
  analyze-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: subosito/flutter-action@v2
        with:
          flutter-version: '3.x'
      - run: cd mobile && flutter pub get
      - run: cd mobile && flutter analyze
      - run: cd mobile && flutter test
```

### 8.5 `.github/workflows/mobile-apk-release.yml`

```yaml
name: Mobile APK Release
on:
  push:
    tags: ['mobile-v*']

jobs:
  build-apk:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-java@v4
        with: { distribution: 'temurin', java-version: '17' }
      - uses: subosito/flutter-action@v2
        with: { flutter-version: '3.x' }

      - name: Decode google-services.json
        run: echo "${{ secrets.GOOGLE_SERVICES_JSON_B64 }}" | base64 -d > mobile/android/app/google-services.json

      - run: cd mobile && flutter pub get
      - run: cd mobile && flutter build apk --release --dart-define=API_URL=${{ secrets.API_URL }}

      - uses: softprops/action-gh-release@v2
        with:
          files: mobile/build/app/outputs/flutter-apk/app-release.apk
          tag_name: ${{ github.ref_name }}
```

**GitHub Repo Secrets'a ekle:**
- `GOOGLE_SERVICES_JSON_B64` — `base64 -i mobile/android/app/google-services.json`
- `API_URL` — Fly.io deploy sonrasında atanan adres (örn. `https://otopusula-backend.fly.dev`). Geliştirme APK'ları için yerel IP kullanılır (bkz. Faz 9).

> **APK signing:** MVP için debug keystore yeterli (kişisel telefonda kurulur). Store yayını için `upload-keystore.jks` üretip CI'da Base64 secret olarak inject edilir.

### 8.6 Fly.io deployment otomasyonu

> **Önerilen yaklaşım — manuel deploy:** MVP boyunca her servis için bir kez `flyctl launch` çalıştırılır, sonrasında `flyctl deploy` ile elle güncelleme yapılır. Otomatik deploy CI ekleme kararı canlıya çıkış kararıyla birlikte verilir.

Otomatik deploy istenirse her workflow'a şu job eklenir:

```yaml
deploy:
  needs: build-test
  runs-on: ubuntu-latest
  if: github.ref == 'refs/heads/main'
  steps:
    - uses: actions/checkout@v4
    - uses: superfly/flyctl-actions/setup-flyctl@master
    - run: flyctl deploy --remote-only --config notification-service/fly.toml
      env:
        FLY_API_TOKEN: ${{ secrets.FLY_API_TOKEN }}
```

Hosting detayları için [Hosting Stratejisi (Fly.io)](#hosting-stratejisi-flyio) bölümüne bak.

---

## FAZ 9 — Cep Telefonunda Çalıştırma

### 9.1 Geliştirme sırasında (USB debugging — en hızlı)

Tek seferlik:
- [ ] Telefonda **Geliştirici Seçenekleri**'ni aç (Ayarlar → Telefon hakkında → Yapı numarası 7 kez tıkla)
- [ ] **USB hata ayıklama**'yı aç
- [ ] Telefonu USB ile bağla → bilgisayardan "izin ver" pop-up'ı onayla

Çalıştırma:
```bash
cd mobile
adb devices                # cihaz listede mi?
flutter devices            # Flutter görüyor mu?
flutter run                # cihazda otomatik kurar ve başlatır
# hot reload: r tuşu, hot restart: R tuşu
```

### 9.2 Yerel backend'e telefondan bağlanma (canlı yayın yokken)

Backend canlıda olmadığı için telefon, **bilgisayarın yerel IP'sine** bağlanır. İkisi de **aynı Wi-Fi**'de olmalı.

Bilgisayarın yerel IP'sini bul:
```powershell
ipconfig | findstr IPv4    # örn: 192.168.1.42
```

Backend'i tüm interface'lerde dinleyecek şekilde başlat (sadece `localhost` değil):
```bash
docker compose up -d        # backend 0.0.0.0:5078 (compose ile)
```

veya `dotnet run` kullanıyorsan: `dotnet run --urls "http://0.0.0.0:5078"`

Windows Firewall 5078 portunu telefondan gelen isteklere açık tutmalı (ilk çalıştırmada Windows pop-up çıkar — "Özel ağ" seç).

Flutter çalıştırırken IP'yi geç:
```bash
flutter run --dart-define=API_URL=http://192.168.1.42:5078
```

> **Not:** HTTP (HTTPS değil) kullandığın için Android `network_security_config.xml` içinde `cleartextTrafficPermitted="true"` olmalı (debug build'de varsayılan açık, release'de kapalı).

### 9.3 Release APK ile (canlıya alındığında)

```bash
flutter build apk --release \
  --dart-define=API_URL=https://otopusula-backend.fly.dev
```

APK dosyasını telefona aktar (Google Drive, WhatsApp, USB kopya...) → telefonda **"Bilinmeyen kaynaklara izin ver"** açıkken kur.

### 9.4 Emulator ile (telefon yoksa)
```bash
flutter emulators                  # liste
flutter emulators --launch <id>
flutter run
```

### 9.5 İlk kurulumda doğrulama checklist'i
- [ ] Login ekranı açılıyor
- [ ] Register → MongoDB'de kullanıcı oluşuyor
- [ ] Login sonrası ana ekrana geçiyor
- [ ] İlanlar listeleniyor (gerçek backend datası)
- [ ] FCM token backend'e kaydedildi mi (`devices` koleksiyonu)
- [ ] Başka bir kullanıcı bu kullanıcının ilanına yorum yazdığında **bildirim geliyor mu?**
- [ ] Şifre sıfırlama → email kutusuna düşüyor mu?
- [ ] 15 dk sonra access token expire olunca refresh otomatik çalışıyor mu? (log'lardan kontrol)

---

## Ortam Değişkenleri (Tam Liste)

### Backend (`backend/backend.API/.env`)
```
# Mevcut
CONNECTION_STRING=mongodb+srv://...
DATABASE_NAME=projctdb
JWT_SECRET=...
ISSUER=OtoPusula
AUDIENCE=OtoPusula
REDIS_CONNECTION_STRING=...
FASTAPI_BASE_URL=https://burak-sisci-otopusula-ml.hf.space
PORT=8080
ASPNETCORE_ENVIRONMENT=Production

# Mobil için yeni
ACCESS_TOKEN_MINUTES=15
REFRESH_TOKEN_DAYS=30
RABBITMQ_URL=amqps://user:pass@host/vhost
FRONTEND_URL=http://localhost:3000        # canlı yayın yokken; Fly.io sonrası https://otopusula-web.fly.dev
```

### Notification Service (`notification-service/.env`)
```
RABBITMQ_URL=amqps://user:pass@host/vhost
CONNECTION_STRING=mongodb+srv://...
DATABASE_NAME=projctdb
FIREBASE_CREDENTIALS_PATH=firebase-credentials.json
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USER=otopusula.app@gmail.com
SMTP_PASS=xxxxxxxxxxxxxxxx
SMTP_FROM=OtoPusula <noreply@otopusula.app>
```

### Mobile (`mobile/.env` — `flutter_dotenv` ile, ya da `--dart-define`)
```
# Geliştirme (yerel backend, telefon aynı Wi-Fi'de):
API_URL=http://192.168.1.42:5078

# Canlı (Fly.io deploy sonrası):
# API_URL=https://otopusula-backend.fly.dev
```

### GitHub Secrets
- `GOOGLE_SERVICES_JSON_B64`
- `FIREBASE_CREDENTIALS_JSON_B64` (notification-service deploy için, opsiyonel)
- `API_URL`
- `FLY_API_TOKEN` (Fly.io otomatik deploy için — `flyctl auth token` ile alınır)

---

## Test Stratejisi

| Katman | Test türü | Araç |
|--------|-----------|------|
| Backend yeni endpoint'ler | Postman koleksiyonu (zaten var, genişlet) | Postman |
| RabbitMQ akışı | Manuel: yorum at → kuyruğu izle → Notification log'una bak | RabbitMQ UI |
| FCM | Firebase Console "Send test message" | Firebase Console |
| Email | `forgot-password` çağır + inbox kontrol | Gmail |
| Mobil unit | `flutter test` | flutter_test |
| Mobil widget | Login + Car list smoke testi | flutter_test |
| End-to-end | Manuel checklist (Faz 9.4) | El ile |

MVP'de otomatik E2E yok. İleride Patrol veya `integration_test` paketi eklenir.

---

## Sorun Giderme

| Belirti | Olası neden | Çözüm |
|---------|-------------|-------|
| Mobil login 401 | Backend `Authorization` middleware sırası | Faz 1.6 — pipeline sırası kontrol |
| FCM bildirimi gelmiyor | `google-services.json` yanlış package | Firebase Console'daki package = `mobile/android/app/build.gradle` `applicationId` |
| Bildirim foreground'da yok | Android default davranış | `flutter_local_notifications` ile manuel göster (post-MVP) |
| RabbitMQ connection refused | CloudAMQP TLS port (5671 vs 5672) | URI `amqps://` (TLS) kullan |
| Refresh token sürekli 401 | Redis'te key yok / expire | `EXPIREAT` doğru, `REFRESH_TOKEN_DAYS` env tanımlı mı? |
| Email spam'e düşüyor | Gmail SPF eksik | Brevo'ya geç veya domain SPF kaydı |
| `flutter run` cihazı görmüyor | USB debugging kapalı | Geliştirici seçenekleri → USB debugging |
| APK kurulurken "App not installed" | Eski versiyonla imza çakışması | Mevcut uygulamayı kaldır, yeniden kur |

---

## Hosting Stratejisi (Fly.io)

> Mevcut Railway servisleri kapatıldı. MVP geliştirme süresince **canlı host yok** — her şey yerelde `docker compose` ile çalışır. Canlıya çıkış kararı verildiğinde aşağıdaki plan uygulanır.

### Hangi servis Fly.io'da

| Servis | Fly.io app adı (öneri) | Kaynak gereksinimi | Notlar |
|--------|------------------------|---------------------|--------|
| Backend (`backend.API`) | `otopusula-backend` | `shared-cpu-1x`, 512 MB | Mevcut Dockerfile yeterli |
| Notification Service | `otopusula-notification` | `shared-cpu-1x`, 256 MB | Public port yok (sadece outbound) |
| Frontend (React) | `otopusula-web` | `shared-cpu-1x`, 256 MB | Nginx ile statik servis |
| ML Model | **Fly.io'da host edilmeyecek** | — | 1.1 GB model + 2 GB RAM ihtiyacı Fly.io free kredisini aşar; ihtiyaç anında yerel veya başka platform |

| Servis | Yer |
|--------|-----|
| MongoDB | MongoDB Atlas (mevcut, ücretsiz) |
| Redis | Redis Cloud free tier (mevcut) **veya** Fly.io'nun Upstash entegrasyonu (`flyctl ext redis create`) |
| RabbitMQ | CloudAMQP free tier (Fly.io'da host edilmez) |

### Fly.io kurulum adımları (servis başına bir kez)

#### Backend için
```bash
cd backend
flyctl auth login
flyctl launch --no-deploy --name otopusula-backend
# çıktı: backend/fly.toml
```

`backend/fly.toml` örnek:
```toml
app = "otopusula-backend"
primary_region = "fra"   # Frankfurt — Türkiye'ye en yakın

[build]
  dockerfile = "Dockerfile"

[http_service]
  internal_port = 8080
  force_https = true
  auto_stop_machines = "stop"     # trafik yoksa durur (cold start var)
  auto_start_machines = true
  min_machines_running = 0

[[vm]]
  cpu_kind = "shared"
  cpus = 1
  memory_mb = 512
```

Env secret'ları yükle:
```bash
flyctl secrets set \
  CONNECTION_STRING="mongodb+srv://..." \
  DATABASE_NAME="projctdb" \
  JWT_SECRET="..." \
  ISSUER="OtoPusula" \
  AUDIENCE="OtoPusula" \
  ACCESS_TOKEN_MINUTES="15" \
  REFRESH_TOKEN_DAYS="30" \
  REDIS_CONNECTION_STRING="..." \
  RABBITMQ_URL="amqps://..." \
  FRONTEND_URL="https://otopusula-web.fly.dev" \
  -a otopusula-backend

flyctl deploy -a otopusula-backend
```

#### Notification Service için
```bash
cd notification-service
flyctl launch --no-deploy --name otopusula-notification
```

`notification-service/fly.toml` — **dikkat: HTTP service yok**, sadece worker:
```toml
app = "otopusula-notification"
primary_region = "fra"

[build]
  dockerfile = "Dockerfile"

# http_service bölümü YOK — bu sadece worker

[[vm]]
  cpu_kind = "shared"
  cpus = 1
  memory_mb = 256

[processes]
  worker = "dotnet NotificationService.dll"
```

`firebase-credentials.json` Fly.io secret'a yüklenmek için base64 encode edilir:
```bash
$b64 = [Convert]::ToBase64String([IO.File]::ReadAllBytes("firebase-credentials.json"))
flyctl secrets set FIREBASE_CREDENTIALS_B64="$b64" -a otopusula-notification
```

`notification-service/Program.cs` başlangıcına ekle:
```csharp
var b64 = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_B64");
if (!string.IsNullOrEmpty(b64)) {
    File.WriteAllBytes("firebase-credentials.json", Convert.FromBase64String(b64));
}
```

Diğer secret'lar (RabbitMQ, MongoDB, SMTP) `flyctl secrets set` ile yüklenir.

```bash
flyctl deploy -a otopusula-notification
```

#### Frontend için
```bash
cd frontend
flyctl launch --no-deploy --name otopusula-web
```

Frontend Dockerfile'ında `REACT_APP_API_URL` build argument'ı `https://otopusula-backend.fly.dev` olarak verilir:
```bash
flyctl deploy -a otopusula-web --build-arg REACT_APP_API_URL=https://otopusula-backend.fly.dev
```

### Maliyet tahmini

Fly.io "Hobby Plan" — kredi kartı zorunlu, ay başına $5 ücretsiz kredi:
- 3 servis × shared-cpu-1x × 256–512 MB ≈ ay başına ~$3–5
- `auto_stop_machines = "stop"` ile trafik yoksa makine durur (~$0)
- Cold start ~5 sn (push notification için sorun değil çünkü uyumayan worker hep ayakta tutulmalı: notification-service için `min_machines_running = 1` yap)

**Gerçek aylık maliyet beklentisi (MVP trafik):** $0–3.

### Loglar ve gözlemleme
```bash
flyctl logs -a otopusula-backend
flyctl logs -a otopusula-notification
flyctl status -a otopusula-backend
flyctl ssh console -a otopusula-backend     # makineye SSH
```

### Yerel-üretim eşitliği
- **Yerel:** `docker compose up` → tüm servisler localhost'ta
- **Üretim:** Fly.io'da 3 ayrı app → MongoDB Atlas + CloudAMQP + Redis Cloud
- **MongoDB ve RabbitMQ aynı bulutta** kalır — yerel ve prod arasında URL hariç fark yok

---

## Post-MVP Roadmap

Bu MVP tamamlandıktan sonra eklenebilecekler:

1. **iOS desteği** — Apple Developer hesabı + APNs + TestFlight
2. **Çoklu push trigger:**
   - "Listendeki ilanın fiyatı düştü"
   - "Yeni ilan: aradığın markada"
3. **Kullanıcı bildirim tercihleri** — bildirim aç/kapa ayarı
4. **In-app notification merkezi** — geçmiş bildirimler ekranı
5. **Deep linking** — email'deki reset link doğrudan uygulamayı açsın
6. **Image upload** — şu an URL ile, ileride S3/Cloudinary ile dosya yükleme
7. **Offline mode** — `hive` veya `isar` ile cache
8. **Push notification için outbox pattern** — DB transaction + RabbitMQ tutarlılığı
9. **OpenTelemetry** — backend + notification-service distributed tracing
10. **App Store / Play Store** yayını

---

*Son güncelleme: Nisan 2026 — MVP planlama dokümanı*
