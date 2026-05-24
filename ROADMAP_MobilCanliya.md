# OtoPusula Mobil — Canlıya Çıkış Yol Haritası (Revize v2)

> **Hedef:** Web tarafı çalışan OtoPusula uygulamasının mobil (Flutter) sürümünü gerçek telefonda, canlı backend ile çalışır hâle getirmek; eksik backend parçalarını (refresh token + devices + Notification Service) tamamlamak; Redis + Docker + CI/CD + push notification + email altyapısını kurup demo'ya hazırlamak.
>
> **Repo'da zaten BİTMİŞ olanlar (atlanacak):**
> - Mobil FrontEnd (Flutter MVVM, `otopusula_mobile/lib`)
> - REST API + UI bağlantısı (api_client + endpoint katmanı)
> - RabbitMQ **publisher** tarafı (`backend.API/Shared/Messaging/RabbitMqPublisher.cs`)
> - Backend Dockerfile + tam stack `docker-compose.yaml` (Mongo + Redis + RabbitMQ + AI + Backend)
> - Redis JWT blacklist (`RedisTokenBlacklist.cs`)
>
> **Bu roadmap'in kapsadığı eksikler:** Refresh token · Devices endpoint'leri · Notification Service (.NET Worker) · FCM · SMTP · Managed Redis · Fly.io deploy · CI/CD · İmzalı APK · Demo · Gerçek telefon testi.
>
> **Tahmini toplam süre:** 12–15 iş günü (tek geliştirici).

---

## Hedef Mimari

```
┌──────────────────┐        HTTPS         ┌────────────────────────┐
│  Flutter APK     │ ───────────────────▶ │  Backend (Fly.io)      │
│  (Android)       │                      │  ASP.NET Core 10       │
│  + FCM           │                      │  /auth, /cars, ...     │
└────────┬─────────┘                      │  /auth/refresh (YENİ)  │
         │ Push                           │  /devices/* (YENİ)     │
         │ (FCM)                          └─────┬─────────┬────────┘
         │                                      │         │
         │                          ┌───────────┘         └──────────┐
         │                          ▼                                ▼
         │                ┌──────────────────┐            ┌────────────────────┐
         │                │  MongoDB Atlas   │            │  RabbitMQ          │
         │                │  + devices       │            │  (CloudAMQP)       │
         │                │  collection      │            │  • notifications.  │
         │                └──────────────────┘            │    comments        │
         │                                                │  • emails.outbound │
         │                ┌──────────────────┐            └─────────┬──────────┘
         │                │  Redis (Upstash) │                      │
         │                │  • JWT blacklist │                      ▼
         │                │  • refresh tokens│         ┌────────────────────────┐
         │                └──────────────────┘         │  Notification Service  │
         │                                             │  (.NET 10 Worker — YENİ)│
         └─────────────── FCM ────────── Firebase ◀────│  • RabbitMQ consumer   │
                                                       │  • FcmSender           │
                                                       │  • SmtpSender (MailKit)│
                                                       └────────────┬───────────┘
                                                                    ▼
                                                        ┌─────────────────────┐
                                                        │  SMTP (Gmail/Brevo) │
                                                        └─────────────────────┘
```

---

## Fazlar — Genel Bakış

| Faz | İş | Süre | Önkoşul |
|-----|----|------|---------|
| 0 | Hazırlık (hesap açma, baseline test) | 0.5 gün | — |
| 1 | Managed Redis (Upstash) | 0.5–1 gün | — |
| **1.5** | **Backend: refresh token + devices endpoint'leri** | **1.5 gün** | 1 |
| **2.5** | **Notification Service (.NET Worker + FCM + SMTP)** | **2 gün** | 1.5 |
| 3 | Fly.io deploy (backend + notification) | 1.5 gün | 1.5, 2.5 |
| **3.5** | **Mobilde Firebase / FCM entegrasyonu** | **1 gün** | 3 |
| 4 | CI/CD (GitHub Actions) | 1.5–2 gün | 3 |
| 5 | İmzalı release APK + 3 telefonda test | 1.5–2 gün | 3.5 |
| 6 | Demo hazırlığı | 1 gün | 5 |
| 7 | Sunum sonrası cleanup | 0.5 gün | 6 |

> **Not — Docker fazı yok:** `backend/docker-compose.yaml` zaten Mongo + Redis + RabbitMQ + AI + Backend'i kapsıyor. Bu compose dosyasına sadece Faz 2.5'te `notification-service` eklenecek. Sıfırdan Docker fazı çalışmasına gerek yok.

```
Faz 0 ─▶ Faz 1 (Redis) ─▶ Faz 1.5 (refresh+devices) ─▶ Faz 2.5 (Notification Svc) ─▶ Faz 3 (Deploy)
                                                                                          │
                                                                                          ▼
                                                                                     Faz 3.5 (Mobil FCM)
                                                                                          │
                                                                                          ▼
                                                                Faz 4 (CI/CD) ─▶ Faz 5 (APK) ─▶ Faz 6 (Demo)
```

---

## FAZ 0 — Hazırlık ve Doğrulama (0.5 gün)

**Amaç:** Tüm hesapların açık, mevcut kodun ne durumda olduğunun net olduğunu garanti altına almak.

### 0.1 Üçüncü taraf hesaplar
- [ ] **CloudAMQP** — Little Lemur free tier RabbitMQ instance. Bağlantı string'i: `amqps://user:pass@host/vhost`.
- [ ] **Firebase** — Yeni proje "OtoPusula" + Android app, package: `com.a2m2.otopusula` (Faz 3.5'te mobilde aynı id kullanılacak). `google-services.json` indir. Project Settings → Service Accounts → Generate new private key → JSON indir (Notification Service kullanacak).
- [ ] **Upstash Redis** — EU region free tier. TLS string al.
- [ ] **SMTP** — Gmail App Password (500 mail/gün) veya Brevo free tier (300 mail/gün, daha iyi deliverability).
- [ ] **Fly.io** — `flyctl auth login`. Kredi kartı + $5/ay free credit.

### 0.2 Baseline doğrulama
- `release/mobile-v1` branch'i aç.
- Backend `localhost:5078`, ML `:8000`, mobil emülatör → kayıt + giriş + araç listele + yorum like + fiyat tahmini akışı geçsin.
- Bilinen mobil kısıtlar listesi: `image_picker` yok, like/unlike stub vb. — demo senaryosundan çıkarılacaklar işaretlensin.

### Çıktı
- 5 hesap, branch, `docs/baseline-test-report.md`.

---

## FAZ 1 — Managed Redis (Upstash) (0.5–1 gün)

**Amaç:** Production Redis'i ayağa kaldırmak; mevcut JWT blacklist'in (`RedisTokenBlacklist.cs`) ve Faz 1.5'te eklenecek refresh token store'unun aynı instance'ı kullanması.

### 1.1 Upstash kurulumu (30 dk)
- Yeni database (EU region, TLS enabled).
- Connection string formatını al: `rediss://default:<pass>@<host>:<port>` veya StackExchange.Redis formatı: `<host>:<port>,password=<pass>,ssl=True,abortConnect=False`.

### 1.2 Yerel doğrulama (1–2 saat)
- `appsettings.Production.json` veya env `REDIS_CONNECTION_STRING` ile backend'i ayağa kaldır.
- Login → logout → aynı token'la istek → 401 (JWT blacklist çalıştığını gör).
- Upstash konsolundan `blacklist:*` key'ini gör.

### 1.3 Fail-over davranışı dokümante et
- Redis erişilemezse: blacklist sorgusu fail-open + warning log; login/logout fail-closed.

### Çıktı
- Upstash bağlantı string'i (Fly secret'a yazılacak), `docs/redis-runbook.md`.

---

## FAZ 1.5 — Backend: Refresh Token + Devices (1.5 gün)

**Amaç:** Mobil uzun oturum için refresh token akışı; push notification için cihaz FCM token kaydı.

> **Bu faz olmadan demo'da kullanıcı her 15 dk login ekranına atılır + push notification hedef cihazı bulamaz.**

### 1.5.1 Refresh token domain + store (3–4 saat)

**Yeni dosyalar:**
- `Modules/Auth/Domain/RefreshToken.cs` — `Id` (Guid), `UserId`, `ExpiresAt`, `DeviceInfo`.
- `Modules/Auth/Application/IRefreshTokenStore.cs`:
  - `SaveAsync(RefreshToken)` → Redis `refresh:{userId}:{tokenId}` key'i, TTL = `REFRESH_TOKEN_DAYS` (default 30).
  - `GetAsync(userId, tokenId)`, `RevokeAsync(userId, tokenId)`, `RevokeAllForUserAsync(userId)` (pattern scan ile).
- `Modules/Auth/Infrastructure/RedisRefreshTokenStore.cs` — `IConnectionMultiplexer` üzerinden.

**Değişiklik:**
- `Shared/Security/JwtTokenGenerator.cs` — access token süresi `ACCESS_TOKEN_MINUTES` (default 15) env'inden okusun. `GenerateRefreshToken(userId, deviceInfo)` metodu eklensin: 32 byte random + base64.

### 1.5.2 Endpoint'ler (3–4 saat)

```
POST /auth/refresh
Body: { "refreshToken": "..." }
Response: { "accessToken": "...", "refreshToken": "...", "accessExpiresIn": 900 }
→ Token rotation: eski refresh revoke + yeni refresh dön.
```

```
POST /auth/logout (mevcut güncellenecek)
Authorization: Bearer <access>
Body: { "refreshToken": "..." }   # opsiyonel
→ JTI blacklist'e ek olarak refresh varsa revoke.
```

**Login response güncellemesi:**
```json
{ "userId": "...", "email": "...", "accessToken": "...", "refreshToken": "...", "accessExpiresIn": 900 }
```
Geriye uyumluluk: `token` alanı bir süre `accessToken` ile aynı değerde kalsın (web FE kırılmasın).

### 1.5.3 Device (FCM token) yönetimi (3–4 saat)

**Yeni:**
- `Modules/Auth/Domain/Device.cs` — `Id` (ObjectId), `UserId`, `FcmToken`, `Platform="android"`, `LastSeenAt`.
- MongoDB yeni koleksiyon `devices`, unique index `(UserId, FcmToken)`.
- `IDeviceRepository` + `MongoDeviceRepository`.

**Endpoint'ler:**
```
POST   /devices/register      Bearer + { "fcmToken", "platform" }  → upsert
DELETE /devices/unregister    Bearer + { "fcmToken" }              → sil
```

### 1.5.4 Yeni env değişkenleri (backend)
```
ACCESS_TOKEN_MINUTES=15
REFRESH_TOKEN_DAYS=30
```
(EXPIRYMINUTES geriye uyumluluk için kalabilir ama yeni kod `ACCESS_TOKEN_MINUTES` okuyacak.)

### 1.5.5 Test (Postman)
- [ ] `/auth/login` → `accessToken` + `refreshToken` dönüyor mu?
- [ ] `/auth/refresh` → yeni access + **yeni** refresh dönüyor mu (rotation)?
- [ ] Eski refresh ile tekrar refresh → 401.
- [ ] `/devices/register` → MongoDB `devices` koleksiyonunda kayıt var mı?
- [ ] `/devices/unregister` → kayıt siliniyor mu?

### Çıktı
- 2 yeni endpoint + 2 yeni domain + Redis refresh store + devices koleksiyonu.
- Burak Şişci (Auth modülü sahibi) bu fazı yapar.

---

## FAZ 2.5 — Notification Service (.NET Worker) (2 gün)

**Amaç:** RabbitMQ'ya düşen `comment.created` ve `email.send` mesajlarını tüketip FCM ve SMTP'ye gönderen worker servisi.

> **Mevcut publisher var, tüketici yok.** Bu olmadan push bildirim ve şifre sıfırlama maili çalışmaz.

### 2.5.1 Proje oluşturma (30 dk)

```
cd <repo-root>
dotnet new worker -n NotificationService -o notification-service
cd notification-service
dotnet add package RabbitMQ.Client
dotnet add package FirebaseAdmin
dotnet add package MailKit
dotnet add package MongoDB.Driver
dotnet add package DotNetEnv
```

### 2.5.2 RabbitMQ topology declare (1 saat)

Notification Service ilk açılışta declare etsin (idempotent):
- Exchange `otopusula` (topic, durable)
- Queue `notifications.comments` (durable) ← bind: `comment.created`
- Queue `emails.outbound` (durable) ← bind: `email.send`
- DLQ: `notifications.comments.dlq`, `emails.outbound.dlq`

> **Önemli:** Backend publisher tarafı zaten exchange'i declare ediyor olabilir — kodu oku, çakışma olmasın. Backend publisher değişmediği sürece queue/binding declare'i Notification Service'in işi.

### 2.5.3 Backend → publisher event'leri (eklenmemişse ekle)

`backend.API/Shared/Messaging/RabbitMqPublisher.cs` mevcut, **ama** event yayınlama çağrıları henüz akışa girmiş olmayabilir. Kontrol et:
- `CreateCommentCommandHandler` içinde `comment.created` yayını var mı? Yoksa ekle (sahip: Mehmet Öz koordinasyonu).
- `ForgotPasswordCommandHandler` içinde `email.send` yayını var mı? Yoksa ekle.

**Event DTO'ları** (notification-service ve backend ikisinde de aynı POCO):
```csharp
public class CommentCreatedEvent {
    public string CommentId, CarId, CarOwnerId, AuthorUserId, AuthorEmail,
                  CarBrand, CarModel, Text;
    public DateTime CreatedAt;
}
public class EmailSendEvent {
    public string To, TemplateId;
    public Dictionary<string,string> Variables;
}
```

### 2.5.4 CommentNotificationWorker (3–4 saat)

```
1. notifications.comments'a abone (manual ack, prefetch=10)
2. Mesaj → CommentCreatedEvent deserialize
3. AuthorUserId == CarOwnerId → ack + skip (self-comment)
4. MongoDB devices'tan UserId == CarOwnerId olanları çek
5. Her FCM token için FcmSender.SendAsync:
   - title: "Yeni yorum: {CarBrand} {CarModel}"
   - body: Text (max 100 karakter)
6. Başarı → ack | Hata → 3 retry sonra DLQ
```

### 2.5.5 EmailWorker + template (2 saat)

```
1. emails.outbound'a abone
2. EmailSendEvent deserialize
3. TemplateId'ye göre subject + HTML üret (Templates/password-reset.html, basit string.Replace)
4. SmtpSender.SendAsync(to, subject, html)
```

`Templates/password-reset.html`:
```html
<h2>OtoPusula — Şifre Sıfırlama</h2>
<p>Merhaba {{UserName}},</p>
<p><a href="{{ResetUrl}}">Şifremi Sıfırla</a> (15 dakika geçerli)</p>
```
`.csproj` içinde `<Content Update="Templates\**" CopyToOutputDirectory="Always" />`.

### 2.5.6 FcmSender + SmtpSender (2 saat)

**FcmSender:** `FirebaseApp.Create` ile service account JSON yükle. `Unregistered` hatasında MongoDB'den token'ı sil.

**SmtpSender:** MailKit. Gmail için `smtp.gmail.com:587`, app password.

### 2.5.7 docker-compose'a ekleme (30 dk)

`backend/docker-compose.yaml` içine yeni service:
```yaml
notification:
  build:
    context: ../notification-service
    dockerfile: Dockerfile
  container_name: otopusula-notification
  environment:
    RABBITMQ_CONNECTION_STRING: "amqp://guest:guest@rabbitmq:5672/"
    CONNECTION_STRING: "mongodb://mongodb:27017/${DATABASE_NAME}"
    DATABASE_NAME: ${DATABASE_NAME}
    SMTP_HOST: ${SMTP_HOST}
    SMTP_PORT: ${SMTP_PORT}
    SMTP_USER: ${SMTP_USER}
    SMTP_PASS: ${SMTP_PASS}
    SMTP_FROM: ${SMTP_FROM}
    FIREBASE_CREDENTIALS_PATH: /app/firebase-credentials.json
  volumes:
    - ../notification-service/firebase-credentials.json:/app/firebase-credentials.json:ro
  depends_on:
    rabbitmq: { condition: service_healthy }
    mongodb: { condition: service_healthy }
  networks: [otopusula-net]
  restart: unless-stopped
```

`.gitignore`'a ekle:
```
notification-service/firebase-credentials.json
otopusula_mobile/android/app/google-services.json
```

### 2.5.8 Notification Service Dockerfile

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

> `firebase-credentials.json` image'a kopyalanmaz, volume veya base64-env-to-file ile gelir.

### 2.5.9 Test
- [ ] `docker compose up -d` → 4 servis (mongo, redis, rabbitmq, backend, notification) yeşil.
- [ ] RabbitMQ UI (http://localhost:15672, guest/guest) → exchange + queue + binding görünüyor.
- [ ] Postman: `/comments` POST → notification worker log'unda "Comment X processed, sent to N devices".
- [ ] Postman: `/auth/forgot-password` → gmail/brevo inbox'ında mail (spam dahil kontrol).

### Çıktı
- `notification-service/` klasörü + Dockerfile + compose'a eklendi.

---

## FAZ 3 — Fly.io Deploy (1.5 gün)

**Amaç:** İki canlı app: `otopusula-backend` (HTTP) + `otopusula-notification` (worker).

> Memory'deki not: ML servisi RAM nedeniyle Fly'a alınmıyor. Demo süresince ML yerelde Docker'da koşacak ve `FASTAPI_BASE_URL` geçici (örn. Hugging Face Spaces veya geliştirici tunnel) olabilir.

### 3.1 Backend (1 saat + ilk deploy 30 dk)

```
cd backend/backend.API
fly launch --no-deploy --name otopusula-backend --region fra
```

`fly.toml` özet:
```toml
app = "otopusula-backend"
primary_region = "fra"
[build]
  dockerfile = "Dockerfile"
[http_service]
  internal_port = 8080
  force_https = true
  auto_stop_machines = "stop"
  auto_start_machines = true
  min_machines_running = 0
[[vm]]
  cpu_kind = "shared"
  cpus = 1
  memory_mb = 512
```

Secrets:
```
fly secrets set \
  CONNECTION_STRING="mongodb+srv://..." \
  DATABASE_NAME="projctdb" \
  JWT_SECRET="..." \
  ISSUER="OtoPusula" AUDIENCE="OtoPusula" \
  ACCESS_TOKEN_MINUTES="15" REFRESH_TOKEN_DAYS="30" \
  REDIS_CONNECTION_STRING="<upstash>" \
  RABBITMQ_CONNECTION_STRING="<cloudamqp>" \
  FASTAPI_BASE_URL="<ml URL>" \
  FRONTEND_URL="https://otopusula-web.fly.dev" \
  ASPNETCORE_ENVIRONMENT="Production" \
  -a otopusula-backend

fly deploy -a otopusula-backend
```

Doğrulama: `https://otopusula-backend.fly.dev/health` → 200, `/swagger` → açık.

### 3.2 Notification Service (1 saat)

```
cd notification-service
fly launch --no-deploy --name otopusula-notification --region fra
```

`fly.toml` (HTTP yok, sadece worker):
```toml
app = "otopusula-notification"
primary_region = "fra"
[build]
  dockerfile = "Dockerfile"
[[vm]]
  cpu_kind = "shared"
  cpus = 1
  memory_mb = 256
```

Firebase credentials base64 olarak secret'a:
```powershell
$b64 = [Convert]::ToBase64String([IO.File]::ReadAllBytes("firebase-credentials.json"))
fly secrets set FIREBASE_CREDENTIALS_B64="$b64" -a otopusula-notification
```

`Program.cs` başına ekle:
```csharp
var b64 = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_B64");
if (!string.IsNullOrEmpty(b64))
    File.WriteAllBytes("firebase-credentials.json", Convert.FromBase64String(b64));
```

Kalan secrets (RABBITMQ, MongoDB, SMTP) `fly secrets set` ile. Sonra `fly deploy`.

> Worker için `min_machines_running = 1` koy (cold start istemiyoruz — bildirimler sürekli akabilir).

### 3.3 ML servisi
- Demo süresince geliştirici makinesinde Docker'da `ai-service` ayakta.
- Geliştirici makinesi sunum günü açık olacaksa OK; aksi hâlde geçici olarak Render free tier veya Hugging Face Spaces'e deploy et (1.1 GB model dikkat — image upload 5 GB limitli).

### Çıktı
- `https://otopusula-backend.fly.dev` canlı.
- `otopusula-notification` worker ayakta.

---

## FAZ 3.5 — Mobilde Firebase / FCM (1 gün)

**Amaç:** Mobil uygulama FCM token alıp backend'e kaydetsin; bildirim gelsin.

### 3.5.1 Paketler (`otopusula_mobile/pubspec.yaml`)
```yaml
dependencies:
  firebase_core: ^3.6.0
  firebase_messaging: ^15.1.0
```
`flutter pub get`.

### 3.5.2 Android konfig
- Firebase Console'dan indirilen `google-services.json` → `otopusula_mobile/android/app/google-services.json`.
- `otopusula_mobile/android/build.gradle` ve `android/app/build.gradle` içinde Google services plugin satırları (Firebase docs'un verdiği iki satırlık apply'lar).
- `AndroidManifest.xml`:
  ```xml
  <uses-permission android:name="android.permission.INTERNET" />
  <uses-permission android:name="android.permission.POST_NOTIFICATIONS" />
  ```
- `applicationId` = `com.a2m2.otopusula` (Firebase'deki package ile birebir).

### 3.5.3 `lib/core/push/fcm_service.dart` (yeni)

```dart
class FcmService {
  final ApiClient api;
  FcmService(this.api);

  Future<void> initAndRegister() async {
    await Firebase.initializeApp();
    final messaging = FirebaseMessaging.instance;
    await messaging.requestPermission();
    final token = await messaging.getToken();
    if (token == null) return;
    await api.dio.post('/devices/register',
        data: {'fcmToken': token, 'platform': 'android'});
    messaging.onTokenRefresh.listen((t) async {
      await api.dio.post('/devices/register',
          data: {'fcmToken': t, 'platform': 'android'});
    });
    FirebaseMessaging.onMessage.listen(_handleForeground);
  }

  void _handleForeground(RemoteMessage msg) {
    // in-app banner veya flutter_local_notifications
  }
}
```

### 3.5.4 Login akışına bağlama
- Login başarılıysa `FcmService.initAndRegister()` çağır.
- Logout'ta `messaging.deleteToken()` + backend `DELETE /devices/unregister`.

### 3.5.5 Refresh token akışı (Dio interceptor)
- API_BASE_URL artık `https://otopusula-backend.fly.dev`.
- Access token expire (401) → `/auth/refresh` çağır → yeni access + refresh sakla → orijinal isteği tekrarla.
- `flutter_secure_storage` ile sakla (paket yoksa ekle).

### 3.5.6 Test (emülatör + gerçek cihaz)
- [ ] Login sonrası MongoDB `devices` koleksiyonunda yeni kayıt.
- [ ] Firebase Console → Cloud Messaging → "Send test message" → cihazda bildirim çıktı.
- [ ] Başka kullanıcı bu cihaz sahibinin ilanına yorum yazınca bildirim geldi.
- [ ] 15 dk bekle → access expire → otomatik refresh → kullanıcı login'e atılmadı.

### Çıktı
- Mobilde FCM aktif + refresh interceptor.

---

## FAZ 4 — CI/CD (1.5–2 gün)

**Amaç:** PR'da test, `main` push'unda otomatik Fly deploy + APK artifact.

### 4.1 `.github/workflows/backend.yml`
- Triggers: `push` main + `pull_request`, paths `backend/**`.
- Steps: setup-dotnet@v4 (10.0.x) → restore → build Release → test → (main'de) `superfly/flyctl-actions/setup-flyctl@master` + `fly deploy -a otopusula-backend`.

### 4.2 `.github/workflows/notification.yml`
- Aynı yapı, paths `notification-service/**`, target `otopusula-notification`.

### 4.3 `.github/workflows/mobile.yml`
- subosito/flutter-action@v2 (stable) → `pub get` → `flutter analyze` → `flutter test` → `flutter build apk --release --dart-define=API_BASE_URL=https://otopusula-backend.fly.dev` → `upload-artifact@v4`.
- `google-services.json` base64'lü secret'tan decode et.

### 4.4 `.github/workflows/mobile-release.yml`
- Trigger: tag `mobile-v*`.
- Build APK + `softprops/action-gh-release@v2` ile GitHub Releases'e yükle.

### 4.5 Secrets
```
FLY_API_TOKEN
GOOGLE_SERVICES_JSON_B64
ANDROID_KEYSTORE_BASE64 (Faz 5'te)
ANDROID_KEYSTORE_PASSWORD, ANDROID_KEY_ALIAS, ANDROID_KEY_PASSWORD
API_BASE_URL=https://otopusula-backend.fly.dev
```

### 4.6 Branch koruması
- `main`'e direkt push kapalı; PR + green CI zorunlu.

### Çıktı
- 4 workflow + branch protection.

---

## FAZ 5 — İmzalı APK + Telefonda Çalıştırma (1.5–2 gün)

### 5.1 Android keystore (2 saat)
```
keytool -genkey -v -keystore otopusula-release.keystore \
  -alias otopusula -keyalg RSA -keysize 2048 -validity 10000
```
`otopusula_mobile/android/key.properties`:
```
storePassword=...
keyPassword=...
keyAlias=otopusula
storeFile=../otopusula-release.keystore
```
`android/app/build.gradle` → release signing config (key.properties'i oku).
Keystore'u **commit etme**, base64'lü hâli GitHub secret'a.

### 5.2 App ikonu + isim (1 saat)
- `flutter_launcher_icons` paketi ile OtoPusula logosundan ikon.
- `applicationId = com.a2m2.otopusula`, `android:label="OtoPusula"`.

### 5.3 Release APK (30 dk)
```
flutter build apk --release \
  --dart-define=API_BASE_URL=https://otopusula-backend.fly.dev
```
Çıktı: `build/app/outputs/flutter-apk/app-release.apk` (~25–40 MB).

### 5.4 3 telefonda smoke test (3–4 saat)
- Android 11, 13, 14.
- "Bilinmeyen kaynaklara izin ver" → kur.
- Test matrisi:

| # | Senaryo | Beklenen |
|---|---------|----------|
| 1 | İlk açılış | <2 sn |
| 2 | Kayıt → Favoriler oto-oluştu | Lists endpoint dönüyor |
| 3 | Araç listele + scroll | Pagination çalışıyor |
| 4 | Araç detay → yoruma like | Optimistic UI |
| 5 | Fiyat tahmini | ML cevap dönüyor |
| 6 | **2. cihazdan yorum at → 1. cihaza push gelir** | Bildirim ekrana düşüyor |
| 7 | **15 dk bekle → istek at → otomatik refresh** | Login'e atmıyor |
| 8 | Logout → eski token tekrar dene | 401 (Redis blacklist) |
| 9 | **Şifre sıfırlama** | Mail inbox'ta |
| 10 | Uçak modu → uygulama açma | "Bağlantı yok" mesajı |

### Çıktı
- `app-release.apk` imzalı + 3 cihaz raporu.

---

## FAZ 6 — Demo Hazırlığı (1 gün)

### 6.1 Demo scripti (yazılı)
1. **Hook (30 sn):** Cebindeki AI araç asistanı.
2. **Kayıt + giriş (45 sn).**
3. **Araç keşfi (60 sn):** Liste → filtre → detay.
4. **Push notification "wow" (60 sn):** 2. telefondan yorum → 1. telefonda bildirim popup.
5. **Fiyat tahmini (60 sn):** ML cevabı.
6. **Mimari slaytı (90 sn):** modüler monolit + Mongo + Redis + RabbitMQ + Notification Service + FCM.
7. **Q&A.**

### 6.2 Seed verisi (2–3 saat)
- `scripts/seed-demo.js` (mongosh) — 20–30 gerçekçi araç + 5 kullanıcı + yorumlar.
- Sunum öncesi tek komutla DB sıfırla + seed.

### 6.3 Yedek planlar

| Risk | Yedek |
|------|-------|
| İnternet kopması | Sunum laptop'ta Docker'da tam stack, mobil hotspot |
| Fly cold start | Sunum 5 dk öncesi `/health` ısıtması, notification için `min_machines_running=1` zaten ayarlı |
| ML servisi yavaş/down | Bir tahmin önceden cache, slaytta sonucu göster |
| Push gelmiyor | Firebase Console'dan manuel test message yedek demo |
| APK kurulmuyor | Yedek telefonda önceden kurulu APK |
| MongoDB Atlas IP allowlist | `0.0.0.0/0` sunum süresince açık |
| CloudAMQP free tier limit | Demo öncesi mesaj sayacı kontrol |

### 6.4 Çıktılar
- 8–10 slayt deck (`docs/Sunum.md` güncelle).
- Mimari diyagram PNG.
- 2 dk ekran kaydı (yedek).
- QR kod → APK download (GitHub Releases).

---

## FAZ 7 — Sunum Sonrası Cleanup (0.5 gün, opsiyonel)

- `fly scale count 0` (maliyet kontrolü).
- Upstash + CloudAMQP free tier'da kalsın.
- GitHub release tag `v1.0-mobile-demo` + APK.
- `docs/POST_DEMO.md`: bilinen eksikler (iOS, in-app messaging, image upload, gerçek like/unlike) → v2 backlog.

---

## Toplam Süre

| Faz | Süre |
|-----|------|
| 0 — Hazırlık | 0.5 gün |
| 1 — Upstash Redis | 0.5–1 gün |
| **1.5 — Refresh + Devices** | **1.5 gün** |
| **2.5 — Notification Service** | **2 gün** |
| 3 — Fly.io deploy | 1.5 gün |
| **3.5 — Mobil FCM** | **1 gün** |
| 4 — CI/CD | 1.5–2 gün |
| 5 — APK + telefon | 1.5–2 gün |
| 6 — Demo | 1 gün |
| 7 — Cleanup (ops.) | 0.5 gün |
| **Toplam** | **12–14.5 gün** |

---

## Geliştiricinin Cevaplaması Gereken Karar Noktaları

1. **SMTP sağlayıcı:** Gmail App Password mı (kolay, spam riski), Brevo mu (300/gün, daha iyi deliverability)?
2. **ML servisi demo'da nerede koşacak?** Geliştirici laptop'u, Render, Hugging Face Spaces?
3. **iOS demo'ya dahil mi?** (Öneri: hayır, sadece Android.)
4. **Demo veritabanı:** Mevcut Atlas instance mı, yeni demo instance mı?
5. **CI'de test coverage zorunluluğu?** (Öneri: ilk sürümde sadece `analyze` + `build`, test threshold yok.)
6. **Backend publisher event çağrıları gerçekten eklendi mi?** Mehmet Öz (Comments) + Burak Şişci (Auth) ile teyit.

---

## Modül Sahipleri ve Sorumluluklar

| Modül | Sahip | Bu roadmap'te dokunulacak yer |
|-------|-------|------------------------------|
| Auth | Burak Şişci | Faz 1.5 (refresh + devices), Faz 2.5 publisher (email) |
| Cars | Anıl Elmaz | — |
| Lists | Mehmet Uludağ | — |
| Comments | Mehmet Öz | Faz 2.5 publisher (`comment.created`) çağrısı kontrolü |
| Notification Service (YENİ) | Burak Şişci (öneri) | Faz 2.5 |
| DevOps (Docker/Fly/CI) | Burak Şişci | Faz 3, 4 |
| Mobil | Tüm ekip | Faz 3.5, 5 |

> Modül sınırlarına saygı: Auth dışındaki modüllerde publisher çağrısı eklenecekse sahibiyle senkronize ol.

---

*Revize tarihi: 2026-05-21 — v2*
