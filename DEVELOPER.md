# OtoPusula — Geliştirici Dökümanı

> Bu döküman, projeye yeni katılan geliştiricilerin sistemi hızlıca anlaması ve geliştirmeye başlaması için hazırlanmıştır.

---

## İçindekiler

1. [Projeye Genel Bakış](#1-projeye-genel-bakış)
2. [Canlı Linkler](#2-canlı-linkler)
3. [Sistem Mimarisi](#3-sistem-mimarisi)
4. [Klasör Yapısı](#4-klasör-yapısı)
5. [Teknoloji Stack](#5-teknoloji-stack)
6. [Modüller ve Sorumluluklar](#6-modüller-ve-sorumluluklar)
7. [Veritabanı Şeması](#7-veritabanı-şeması)
8. [API Endpoint Referansı](#8-api-endpoint-referansı)
9. [Kimlik Doğrulama Sistemi](#9-kimlik-doğrulama-sistemi)
10. [Ortam Değişkenleri](#10-ortam-değişkenleri)
11. [Yerel Geliştirme Ortamı Kurulumu](#11-yerel-geliştirme-ortamı-kurulumu)
12. [Deployment (Render + HF Spaces)](#12-deployment-render--hf-spaces)
13. [Ekip ve Modül Sahipliği](#13-ekip-ve-modül-sahipliği)

---

## 1. Projeye Genel Bakış

**OtoPusula**, ikinci el araç alım-satım platformudur. Standart bir ilan sitesinin ötesinde, yapay zeka destekli **araç fiyat tahmini** sunar.

**Temel özellikler:**
- Kullanıcı kayıt/giriş/çıkış (JWT tabanlı)
- Araç ilanı oluşturma, listeleme, filtreleme
- İlanlara yorum yapma
- Kişisel liste oluşturma (Favoriler vb.)
- Makine öğrenmesi ile araç piyasa değeri tahmini

---

## 2. Canlı Linkler

| Servis | URL |
|--------|-----|
| REST API | `https://otopusula-backend.onrender.com` |
| Swagger (API Dökümantasyonu) | `https://otopusula-backend.onrender.com/swagger` |
| Health Check | `https://otopusula-backend.onrender.com/health` |
| ML Model (FastAPI) | `https://burak-sisci-otopusula-ml.hf.space` |
| ML Model Health | `https://burak-sisci-otopusula-ml.hf.space/health` |
| Notification Service | `https://burak-sisci-otopusula-notification.hf.space` |
| ML Model Repo (HF) | `https://huggingface.co/burak-sisci/otopusula-model` |
| Web Frontend | _şu an cloud'da değil — yerel kurulum (`npm start` → `http://localhost:3000`)_ |
| Mobil APK (release) | `https://github.com/burak-sisci/A2M2_Ekibi_Oto_Pusula_Uygulamasi/releases/latest` |

> Backend ve HF servisleri free tier'da idle uyumasından sonra ilk istekte ~30 sn cold start yapabilir.

---

## 3. Sistem Mimarisi

```
┌──────────────────────────────────────────────────────────┐
│           KULLANICI (Web Browser veya Android APK)       │
└───────────────────────────┬──────────────────────────────┘
                            │ HTTPS
                            ▼
┌──────────────────────────────────────────────────────────┐
│           Backend  (ASP.NET Core 10 — Render)            │
│             otopusula-backend.onrender.com               │
│                                                          │
│  ┌──────────┐ ┌────────┐ ┌────────┐ ┌──────┐ ┌───────┐  │
│  │  Auth +  │ │  Cars  │ │ Lists  │ │Comms │ │Predict│  │
│  │ Devices  │ │ Module │ │ Module │ │Module│ │ Proxy │  │
│  └──────────┘ └────────┘ └────────┘ └──────┘ └───┬───┘  │
└──────┬─────────────┬───────────┬──────┬────────────┼─────┘
       │             │           │      │            │
       ▼             ▼           ▼      ▼            ▼
   ┌────────┐  ┌──────────┐ ┌──────┐ ┌────────┐ ┌────────────┐
   │MongoDB │  │ Upstash  │ │Cloud │ │ Notif. │ │ ML Model   │
   │ Atlas  │  │  Redis   │ │ AMQP │ │  HF    │ │ (FastAPI)  │
   │        │  │          │ │      │ │ Space  │ │ HF Space   │
   │ users  │  │ JWT      │ │ msg  │ │  ↓     │ │            │
   │ cars   │  │ blacklist│ │ queue│ │ FCM    │ │ Random     │
   │ comm.  │  │ refresh  │ │      │ │ +      │ │ Forest     │
   │ devices│  │ tokens   │ │      │ │ SMTP   │ │ Pipeline   │
   └────────┘  └──────────┘ └──────┘ └────────┘ └────────────┘
```

Public Web Frontend henüz cloud'a alınmamıştır; mobil uygulama (Android APK) tüm fonksiyonları kapsar.

### Mimari Desenler

| Desen | Açıklama |
|-------|----------|
| **Modular Monolith** | Tek deployable unit, modüller arası net sınırlar |
| **Clean Architecture** | Domain → Application → Infrastructure → Presentation katmanları |
| **CQRS** | Command (yazma) ve Query (okuma) ayrımı, MediatR ile |
| **Repository Pattern** | Veri erişimi soyutlanmış, test edilebilir |
| **Event-Driven** | Kullanıcı silme cascade için MediatR notification |

---

## 4. Klasör Yapısı

```
A2M2_Ekibi_Oto_Pusula_Uygulamasi/
│
├── backend/
│   └── backend.API/
│       ├── Program.cs                  # Uygulama giriş noktası, tüm DI kayıtları
│       ├── appsettings.json
│       ├── Modules/
│       │   ├── Auth/
│       │   │   ├── Application/        # Command'lar, Query'ler, Interface'ler
│       │   │   ├── Domain/             # User entity
│       │   │   └── Infrastructure/     # MongoDB repo, BCrypt, Redis blacklist
│       │   ├── Cars/
│       │   │   ├── Application/
│       │   │   ├── Domain/
│       │   │   └── Infrastructure/
│       │   ├── Comments/
│       │   │   ├── Application/
│       │   │   ├── Domain/
│       │   │   └── Infrastructure/
│       │   ├── Lists/
│       │   │   ├── Application/
│       │   │   ├── Domain/
│       │   │   └── Infrastructure/
│       │   └── Prediction/
│       │       ├── Application/        # IPredictionService interface
│       │       └── Infrastructure/     # FastAPI'ye HTTP çağrısı
│       ├── Presentation/
│       │   ├── Controllers/            # AuthController, CarsController vb.
│       │   └── Middlewares/            # JwtAuthMiddleware, GlobalExceptionHandler
│       └── Shared/
│           ├── Database/               # MongoDbContext, MongoTransactionManager
│           ├── Security/               # JwtTokenGenerator
│           └── Events/                 # UserDeletedEvent (cross-module)
│
├── frontend/
│   ├── public/
│   └── src/
│       ├── api/                        # Her modül için ayrı Axios çağrıları
│       │   ├── axiosInstance.js        # Base URL ve interceptor ayarları
│       │   ├── authApi.js
│       │   ├── carApi.js
│       │   ├── commentApi.js
│       │   ├── listApi.js
│       │   └── predictionApi.js
│       ├── components/                 # UI bileşenleri (modüle göre gruplu)
│       ├── pages/                      # Sayfa bileşenleri
│       ├── context/                    # React Context (auth state vb.)
│       └── constants/
│
├── ML_Model_V4/                        # Python + FastAPI ML servisi
│   ├── api.py                          # FastAPI uygulama giriş noktası
│   ├── araba_modeli.pkl                # Eğitilmiş RandomForest modeli (1.1 GB, Git LFS)
│   └── requirements.txt
│
├── postman/                            # Postman koleksiyonu
│   └── collections/OtoPusula API/
│       ├── Kimlik/                     # Auth endpoint testleri
│       ├── İlanlar/                    # Cars endpoint testleri
│       ├── Listeler/
│       ├── Yorumlar/
│       └── YapayZeka/
│
├── docs/
│   └── Veritabani-Semasi.md
│
├── Burak-Sisci/                        # Üye görev ve dokümantasyonları
├── Anil-Elmaz/
├── Mehmet-Oz/
├── Mehmet-Uludag/
│
├── README.md
├── DEVELOPER.md                        # Bu dosya
└── DEPLOY.md                           # Railway deployment rehberi
```

---

## 5. Teknoloji Stack

### Backend

| Teknoloji | Versiyon | Kullanım Amacı |
|-----------|----------|----------------|
| ASP.NET Core | 10 | Web API framework |
| C# | 13 | Backend dili |
| MongoDB.Driver | Son | Veritabanı sürücüsü |
| MediatR | Son | CQRS pattern, event-driven mimari |
| BCrypt.Net | Son | Şifre hashleme |
| System.IdentityModel.Tokens.Jwt | Son | JWT token üretimi ve doğrulama |
| StackExchange.Redis | Son | Token kara liste (logout mekanizması) |
| DotNetEnv | Son | `.env` dosyasından ortam değişkeni okuma |
| Swagger / Swashbuckle | Son | API dokümantasyonu |

### Frontend

| Teknoloji | Versiyon | Kullanım Amacı |
|-----------|----------|----------------|
| React | 19 | UI framework |
| React Router | 7 | Client-side routing |
| Axios | Son | HTTP istekleri |
| Tailwind CSS | 3 | Utility-first stil sistemi |
| React Toastify | Son | Bildirim mesajları |
| React Icons | Son | İkon kütüphanesi |

### ML Modeli

| Teknoloji | Kullanım Amacı |
|-----------|----------------|
| Python 3.11+ | ML servis dili |
| FastAPI | REST API sunucusu (ML için) |
| scikit-learn | RandomForest modeli |
| Uvicorn | ASGI sunucu |
| Pandas / NumPy | Veri işleme |

### Altyapı

| Servis | Kullanım Amacı |
|--------|----------------|
| Railway | Cloud deployment (3 servis: Backend, Frontend, ML) |
| Docker | Konteynerizasyon |
| Nginx | Frontend için statik dosya sunucu |
| MongoDB Atlas | Ücretsiz tier bulut veritabanı (512 MB) |
| Redis Cloud | Ücretsiz tier önbellekleme (30 MB) |
| Git LFS | Büyük dosya takibi (araba_modeli.pkl 1.1 GB) |

---

## 6. Modüller ve Sorumluluklar

### Modül A — Kullanıcı & Kimlik Yönetimi (`Auth`)
**Sorumlu:** Burak Şişci

| Endpoint | Metot | Açıklama |
|----------|-------|----------|
| `/auth/register` | POST | Yeni kullanıcı kaydı |
| `/auth/login` | POST | Kullanıcı girişi, JWT döner |
| `/auth/logout` | POST | Oturumu sonlandırır, token kara listeye eklenir |
| `/auth/profile` | PUT | Profil güncelleme (telefon) |
| `/auth/{id}` | DELETE | Hesap silme + cascade delete |
| `/auth/forgot-password` | POST | Şifre sıfırlama tokeni üretir |
| `/auth/reset-password` | POST | Yeni şifre belirleme |
| `/users/{userId}` | GET | Profil görüntüleme |

---

### Modül B — İlan Yönetimi (`Cars`)
**Sorumlu:** Anıl Elmaz

| Endpoint | Metot | Açıklama |
|----------|-------|----------|
| `/cars` | GET | Araç ilanlarını listele (filtreli) |
| `/cars` | POST | Yeni ilan oluştur |
| `/cars/{id}` | GET | İlan detayı |
| `/cars/{id}` | PUT | İlan güncelle |
| `/cars/{id}` | DELETE | İlan sil |

---

### Modül C — Listeler & Favoriler (`Lists`)
**Sorumlu:** Mehmet Uludağ

| Endpoint | Metot | Açıklama |
|----------|-------|----------|
| `/lists` | GET | Kullanıcının listelerini getir |
| `/lists` | POST | Yeni liste oluştur |
| `/lists/{id}/items` | POST | Listeye araç ekle |
| `/lists/{id}/items/{carId}` | DELETE | Listeden araç çıkar |

---

### Modül D — Yorumlar (`Comments`)
**Sorumlu:** Mehmet Öz

| Endpoint | Metot | Açıklama |
|----------|-------|----------|
| `/cars/{carId}/comments` | GET | İlana yapılan yorumları getir |
| `/comments` | POST | Yorum ekle |
| `/comments/{id}` | PUT | Yorum güncelle |
| `/comments/{id}` | DELETE | Yorum sil |

---

### Modül E — Fiyat Tahmini (`Prediction`)
**Sorumlu:** Tüm ekip (ML servisi Burak tarafından entegre edildi)

| Endpoint | Metot | Açıklama |
|----------|-------|----------|
| `/prediction` | POST | Araç özelliklerine göre piyasa değeri tahmini |

---

## 7. Veritabanı Şeması

Veritabanı: **MongoDB Atlas** — Koleksiyon ismi: `projctdb`

### `users` Koleksiyonu
```json
{
  "_id": "ObjectId (otomatik)",
  "email": "String (zorunlu, benzersiz)",
  "phone": "String (zorunlu, benzersiz)",
  "passwordHash": "String (BCrypt ile hashlenmiş)",
  "createdAt": "DateTime",
  "resetToken": "String? (şifre sıfırlama, opsiyonel)",
  "resetTokenExpires": "DateTime? (opsiyonel)"
}
```

> **Not:** Email ve Phone alanları MongoDB'de `unique index` ile korunmaktadır.

### `cars` Koleksiyonu
```json
{
  "_id": "ObjectId",
  "userId": "ObjectId (referans: users)",
  "brand": "String (zorunlu — örn: Toyota)",
  "model": "String (zorunlu — örn: Corolla)",
  "year": "Number (zorunlu)",
  "km": "Number (zorunlu)",
  "fuelType": "String (Benzin | Dizel | LPG | Elektrik | Hibrit)",
  "gearType": "String (Manuel | Otomatik | Yarı Otomatik)",
  "price": "Number (zorunlu)",
  "description": "String",
  "images": ["String (URL)"],
  "location": { "city": "String", "district": "String" },
  "damageInfo": ["String"],
  "createdAt": "DateTime",
  "updatedAt": "DateTime"
}
```

### `lists` Koleksiyonu
```json
{
  "_id": "ObjectId",
  "userId": "ObjectId (referans: users)",
  "name": "String (örn: 'Favoriler')",
  "isDefault": "Boolean (true = Favoriler listesi)",
  "cars": ["ObjectId (referans: cars)"],
  "createdAt": "DateTime",
  "updatedAt": "DateTime"
}
```

> **Önemli:** Yeni kullanıcı kaydında `isDefault: true` olan **Favoriler** listesi otomatik oluşturulur. Bu işlem MongoDB transaction içinde yapılır (atomik).

### `comments` Koleksiyonu
```json
{
  "_id": "ObjectId",
  "userId": "ObjectId (referans: users)",
  "carId": "ObjectId (referans: cars)",
  "text": "String (zorunlu)",
  "createdAt": "DateTime",
  "updatedAt": "DateTime"
}
```

---

## 8. API Endpoint Referansı

**Base URL:** `https://otopusula-backend.onrender.com`

**Tüm isteklerde:**
- Content-Type: `application/json`
- Korunan endpointlerde: `Authorization: Bearer <token>`

### Hata Kodları

| HTTP Kodu | Durum | Açıklama |
|-----------|-------|----------|
| 200 | OK | Başarılı |
| 201 | Created | Kaynak oluşturuldu |
| 400 | Bad Request | Geçersiz istek / iş kuralı hatası |
| 401 | Unauthorized | Token eksik, geçersiz veya kara listede |
| 404 | Not Found | Kaynak bulunamadı |
| 422 | Unprocessable Entity | Validasyon hatası |
| 500 | Internal Server Error | Sunucu hatası |

### Örnek: Register
```http
POST /auth/register
Content-Type: application/json

{
  "email": "kullanici@example.com",
  "phone": "05551234567",
  "password": "Sifre1234!"
}

# Başarılı yanıt (201):
{
  "userId": "...",
  "email": "kullanici@example.com",
  "token": "eyJhbGci..."
}
```

### Örnek: Login
```http
POST /auth/login
Content-Type: application/json

{
  "email": "kullanici@example.com",
  "password": "Sifre1234!"
}

# Başarılı yanıt (200):
{
  "userId": "...",
  "email": "kullanici@example.com",
  "token": "eyJhbGci..."
}
```

### Örnek: Korunan Endpoint
```http
POST /auth/logout
Authorization: Bearer eyJhbGci...

# Başarılı yanıt (200):
{ "message": "Başarıyla çıkış yapıldı" }
```

> Swagger arayüzünden tüm endpointleri interaktif test edebilirsiniz:
> `https://otopusula-backend.onrender.com/swagger`

---

## 9. Kimlik Doğrulama Sistemi

### Akış

```
Kullanıcı                Backend                  Redis
   │                        │                       │
   │──── POST /auth/login ──▶│                       │
   │                        │── JWT üret (HMAC-256) │
   │◀── { token, userId } ──│                       │
   │                        │                       │
   │── GET /users/:id ──────▶│                       │
   │   Authorization: Bearer │── JTI kontrol ───────▶│
   │                        │◀── "kara listede değil"│
   │◀── Kullanıcı bilgisi ──│                       │
   │                        │                       │
   │── POST /auth/logout ───▶│                       │
   │   Authorization: Bearer │── JTI ekle (TTL ile) ▶│
   │◀── 200 OK ─────────────│                       │
   │                        │                       │
   │── GET /users/:id ──────▶│                       │
   │   (eski token ile)      │── JTI kontrol ───────▶│
   │                        │◀── "kara listede!" ────│
   │◀── 401 Unauthorized ───│                       │
```

### JWT Token İçeriği
```json
{
  "sub": "userId",
  "email": "kullanici@example.com",
  "jti": "benzersiz-guid (logout için)",
  "nameid": "userId",
  "exp": 1234567890
}
```

### Token Süresi
- **Varsayılan:** 60 dakika (`EXPIRYMINUTES` env var ile değiştirilebilir)
- **ClockSkew:** Sıfır (hassas süre kontrolü)

### Kara Liste Mekanizması (Redis)
- Logout olan tokenın `jti` değeri Redis'e yazılır
- Redis key formatı: `blacklist:{jti}`
- TTL: Tokenın kalan geçerlilik süresi kadar (otomatik silinir)
- Her istekte `JwtAuthMiddleware` Redis'i sorgular

---

## 10. Ortam Değişkenleri

### Backend (`.env` dosyası veya Railway Variables)

| Değişken | Örnek Değer | Açıklama |
|----------|-------------|----------|
| `CONNECTION_STRING` | `mongodb+srv://user:pass@cluster.mongodb.net/` | MongoDB bağlantı adresi |
| `DATABASE_NAME` | `projctdb` | MongoDB veritabanı adı |
| `JWT_SECRET` | `gizli-anahtar-min-32-karakter` | JWT imzalama anahtarı |
| `ISSUER` | `OtoPusula` | JWT issuer |
| `AUDIENCE` | `OtoPusula` | JWT audience |
| `EXPIRYMINUTES` | `60` | Token geçerlilik süresi (dakika) |
| `REDIS_CONNECTION_STRING` | `host:port,password=xxx,ssl=True` | Redis bağlantı adresi |
| `FASTAPI_BASE_URL` | `https://burak-sisci-otopusula-ml.hf.space` | ML servis URL'i |
| `PORT` | `8080` | Backend dinleme portu |
| `ASPNETCORE_ENVIRONMENT` | `Production` | Ortam modu |

### Frontend (`.env` dosyası veya Railway Build Args)

| Değişken | Örnek Değer | Açıklama |
|----------|-------------|----------|
| `REACT_APP_API_URL` | `https://otopusula-backend.onrender.com` | Backend API adresi |

> **Önemli:** React'te environment variable'lar build sırasında kod içine gömülür. URL değiştiyse yeniden build gerekir.

### ML Model

| Değişken | Değer | Açıklama |
|----------|-------|----------|
| `PORT` | `8000` | FastAPI dinleme portu |

---

## 11. Yerel Geliştirme Ortamı Kurulumu

### Gereksinimler

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)
- [Python 3.11+](https://www.python.org/)
- [Git](https://git-scm.com/) + [Git LFS](https://git-lfs.com/)

### Adım 1: Projeyi Klonla

```bash
git clone https://github.com/burak-sisci/A2M2_Ekibi_Oto_Pusula_Uygulamasi.git
cd A2M2_Ekibi_Oto_Pusula_Uygulamasi
git lfs pull   # ML model dosyasını indir (1.1 GB)
```

### Adım 2: Backend Kurulumu

```bash
cd backend/backend.API
```

`.env` dosyası oluştur:
```
CONNECTION_STRING=mongodb+srv://...
DATABASE_NAME=projctdb
JWT_SECRET=yerel-test-icin-herhangi-bir-uzun-anahtar
ISSUER=OtoPusula
AUDIENCE=OtoPusula
EXPIRYMINUTES=60
REDIS_CONNECTION_STRING=...
FASTAPI_BASE_URL=http://localhost:8000
```

```bash
dotnet restore
dotnet run
# → http://localhost:5078
# → http://localhost:5078/swagger
```

### Adım 3: Frontend Kurulumu

```bash
cd frontend
```

`.env` dosyası oluştur:
```
REACT_APP_API_URL=http://localhost:5078
```

```bash
npm install
npm start
# → http://localhost:3000
```

### Adım 4: ML Model Servisi (Opsiyonel)

```bash
cd ML_Model_V4
pip install -r requirements.txt
uvicorn api:app --host 0.0.0.0 --port 8000
# → http://localhost:8000/docs
```

### Middleware Sırası (Backend)

Program.cs içinde pipeline sırası şöyledir — bu sıra kritiktir:

```
ExceptionHandler → Swagger → CORS → StaticFiles → Authentication → Authorization → Controllers
```

---

## 12. Deployment (Render + HF Spaces)

Tüm detaylar için: [docs/DEPLOY-PUBLIC.md](docs/DEPLOY-PUBLIC.md)

### Mimari Özet

Tüm servisler ücretsiz tier'larda dağıtılmıştır:

| Servis | Platform | URL |
|--------|----------|-----|
| Backend (ASP.NET Core 10) | **Render** (Docker web service) | `https://otopusula-backend.onrender.com` |
| ML Model (FastAPI) | **Hugging Face Spaces** (Docker) | `https://burak-sisci-otopusula-ml.hf.space` |
| Notification Service (.NET Worker) | **Hugging Face Spaces** (Docker) | `https://burak-sisci-otopusula-notification.hf.space` |
| ML Model dosyası (1.1 GB) | **Hugging Face Models** | `huggingface.co/burak-sisci/otopusula-model` |
| MongoDB | **MongoDB Atlas** | M0/Flex cluster |
| Redis | **Upstash** | Free 10k req/gün |
| RabbitMQ | **CloudAMQP** | Little Lemur free |
| Mobil APK | **GitHub Releases + Google Drive mirror** | [v1.0.0](https://github.com/burak-sisci/A2M2_Ekibi_Oto_Pusula_Uygulamasi/releases/latest) |

### CI/CD

Her servis için ayrı GitHub Actions workflow'u tanımlanmıştır. Detay: [docs/CICD.md](docs/CICD.md).

### Sağlık Kontrolü

```bash
# Backend
curl https://otopusula-backend.onrender.com/health
# Beklenen: {"durum":"saglikli"}

# ML
curl https://burak-sisci-otopusula-ml.hf.space/health
# Beklenen: {"status":"healthy"}

# Notification
curl https://burak-sisci-otopusula-notification.hf.space/health
# Beklenen: {"status":"healthy"}
```

### Önemli Notlar

- ML model dosyası `araba_modeli.pkl` **1.1 GB**'dır. HF Models repo'sunda saklanır; Spaces build-time'da `huggingface_hub.hf_hub_download` ile indirir (Spaces 1 GB storage limitini aşar).
- Render free tier 15 dk idle sonra uyur, ilk istek ~30 sn cold start yapar.
- HF Spaces free tier'da 16 GB RAM tahsis edilir — 1.1 GB model rahat çalışır.
- Notification Service `firebase-credentials.json`'u secret olarak `FIREBASE_CREDENTIALS_B64` env değişkeninden okur (HF Spaces Settings → Repository secrets).

---

## 13. Ekip ve Modül Sahipliği

| Üye | Modül | Branch |
|-----|-------|--------|
| **Burak Şişci** | Auth / Kullanıcı & Kimlik Yönetimi | `burak-sisci` |
| **Anıl Elmaz** | Cars / İlan Yönetimi | `anil-elmaz` |
| **Mehmet Uludağ** | Lists / Listeler & Favoriler | `mehmet-uludag` |
| **Mehmet Öz** | Comments / Yorumlar | `mehmet-oz` |

### Git Akışı

```bash
# Yeni özellik geliştirirken:
git checkout -b kendi-branch-adin
git add dosya-adi
git commit -m "feat: açıklama"
git push origin kendi-branch-adin
```

> **Kural:** Her üye kendi branch'ına commit ve push yapmalıdır.

---

*Son güncelleme: Nisan 2026*
