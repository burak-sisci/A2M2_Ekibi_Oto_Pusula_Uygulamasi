# OtoPusula

> **Not:** Araç Alım-Satım ve Yapay Zeka Destekli Fiyat Tahmin Platformu

---

## 📱 Mobil Uygulama — İndir

| Kaynak | Link | Önerilen Cihaz |
|--------|------|----------------|
| 🟢 **Google Drive** | [app-release.apk](https://drive.google.com/uc?export=download&id=1soD1iLwStdSldp0vOjDJJi5zpcPIXryM) | **Tüm Android cihazlar** (önerilen) |
| 📦 **GitHub Release** | [app-release.apk](https://github.com/burak-sisci/A2M2_Ekibi_Oto_Pusula_Uygulamasi/releases/latest/download/app-release.apk) | Samsung, Pixel, OnePlus, Honor |
| 🗜️ **ZIP (MIUI/Xiaomi)** | [otopusula-v1.0.0.zip](https://github.com/burak-sisci/A2M2_Ekibi_Oto_Pusula_Uygulamasi/releases/latest/download/otopusula-v1.0.0.zip) | Xiaomi (MIUI APK kısıtlamasını aşar) |

**Kurulum:** APK'yı indir → Ayarlar → Güvenlik → "Bilinmeyen kaynaklardan kuruluma izin ver" → APK'ya dokun → Yükle.

> İlk istek backend cold start nedeniyle ~30 sn sürebilir (Render free tier). Sonraki istekler anında.

---

## Proje Hakkında

![Ürün Tanıtım Görseli](Product.png)

**Proje Tanımı:** > OtoPusula, ikinci el araç piyasasındaki belirsizlikleri ortadan kaldırmak ve kullanıcılarına en doğru veriyi sunmak amacıyla geliştirilmiş, yapay zeka destekli bir otomotiv e-ticaret platformudur. Standart bir ilan sitesinin ötesine geçen OtoPusula, geliştirdiğimiz makine öğrenmesi (ML) algoritmaları sayesinde araçların teknik özelliklerine göre anlık "Piyasa Değer Tahmini" yapar. Kullanıcılar, gelişmiş filtreleme seçenekleriyle aradıkları aracı kolayca bulabilir, "Alınacaklar" veya "Kıyaslanacaklar" gibi özel listeler oluşturabilir ve araçlar hakkında yorum yaparak sosyal bir etkileşim kurabilirler. 

**Proje Kategorisi:** > Otomotiv, E-Ticaret, Yapay Zeka (AI), Finansal Teknoloji

---

## Proje Linkleri (Canlı)

| Servis | URL |
|--------|-----|
| **REST API (Swagger)** | https://otopusula-backend.onrender.com/swagger |
| **REST API base URL** | https://otopusula-backend.onrender.com |
| **Makine Öğrenmesi (FastAPI)** | https://burak-sisci-otopusula-ml.hf.space |
| **Notification Servisi** | https://burak-sisci-otopusula-notification.hf.space |
| **AI Model Repository** | https://huggingface.co/burak-sisci/otopusula-model |
| **Mobil APK (Android)** | https://github.com/burak-sisci/A2M2_Ekibi_Oto_Pusula_Uygulamasi/releases/latest |

> ⏱ Backend ve HF servisleri free tier'da idle uyumasından sonra ilk istekte ~30 sn cold start yapabilir; sonraki istekler anlık.

---

## Proje Ekibi

**Grup Adı:** > A2M2

**Ekip Üyeleri:** 

- Burak Şişci 
- Mehmet Uludağ 
- Anıl Elmaz 
- Mehmet Öz 

---

## Dokümantasyon

Projenin teknik detaylarına ve geliştirme süreçlerine aşağıdaki linklerden erişebilirsiniz:

1. [Gereksinim Analizi](Gereksinim-Analizi.md)
2. [Veritabanı Şeması](docs/Veritabani-Semasi.md)
3. [REST API Tasarımı](API-Tasarimi.md)
4. [REST API](Rest-API.md)
5. [Web Front-End](WebFrontEnd.md)
6. [Mobil Front-End](MobilFrontEnd.md)
7. [Mobil Backend](MobilBackEnd.md)
8. [Video Sunum](Sunum.md)

### Deploy & DevOps

- 🚀 [Bulut Deploy Rehberi](docs/DEPLOY-PUBLIC.md) — Atlas + Render + HF Spaces + GitHub Release ile sıfırdan public yayın
- 🤖 [CI/CD (GitHub Actions)](docs/CICD.md) — backend, notification, mobile build pipeline'ları
- 🗺️ [Mobil Roadmap](ROADMAP_MobilCanliya.md) — fazlı kalkınma planı

---

## Teknoloji Stack

| Katman | Teknoloji |
|--------|-----------|
| **Web Frontend** | React 19, React Router 7, Axios, Tailwind CSS |
| **Mobil** | Flutter (Dart), MVVM, Dio, GoRouter, Firebase Messaging |
| **Backend** | ASP.NET Core 10, C# |
| **Mimari** | Modular Monolith, Clean Architecture, CQRS (MediatR) |
| **Veritabanı** | MongoDB Atlas |
| **Cache / Oturum** | Upstash Redis (JWT blacklist + refresh token store) |
| **Mesaj Kuyruğu** | CloudAMQP RabbitMQ |
| **Notification Service** | .NET Worker (RabbitMQ consumer) + FirebaseAdmin (FCM) + MailKit (SMTP) |
| **Push Notification** | Firebase Cloud Messaging (FCM) |
| **E-posta** | Gmail SMTP (App Password) |
| **Kimlik Doğrulama** | JWT Bearer + Refresh Token Rotation |
| **Şifre Güvenliği** | BCrypt |
| **API Dokümantasyonu** | Swagger / OpenAPI |
| **ML Modeli** | Python, scikit-learn (RandomForest pipeline ~1.1 GB), FastAPI |
| **Backend Deployment** | Docker → Render (free web service) |
| **ML & Notification Deployment** | Docker → Hugging Face Spaces |
| **CI/CD** | GitHub Actions (3 workflow: backend/notification/mobile) |
| **APK Dağıtım** | GitHub Releases + Google Drive mirror |

### Mimari (Canlı)

```
APK indiren herhangi bir Android cihaz
        │ HTTPS
        ▼
otopusula-backend.onrender.com        (Render free)
        ├─→ MongoDB Atlas                            ✅
        ├─→ Upstash Redis (JWT + refresh tokens)     ✅
        ├─→ CloudAMQP RabbitMQ                       ✅
        │       ↓
        │   burak-sisci-otopusula-notification.hf.space  (HF Spaces)
        │       ├─→ Firebase Cloud Messaging  → APK
        │       └─→ Gmail SMTP                → mail
        │
        └─→ burak-sisci-otopusula-ml.hf.space           (HF Spaces)
                ↑
        huggingface.co/burak-sisci/otopusula-model (1.1 GB, LFS)
```

---

## Yerel Kurulum

### Gereksinimler

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (preview)
- [Node.js 18+](https://nodejs.org/)
- [Python 3.11+](https://www.python.org/)
- [Flutter 3.x](https://docs.flutter.dev/get-started/install) (mobil için)
- [Git](https://git-scm.com/) ve [Git LFS](https://git-lfs.com/)
- [MongoDB Community Server](https://www.mongodb.com/try/download/community) (yerel test için) veya MongoDB Atlas hesabı

### 1. Projeyi Klonla

```bash
git clone https://github.com/burak-sisci/A2M2_Ekibi_Oto_Pusula_Uygulamasi.git
cd A2M2_Ekibi_Oto_Pusula_Uygulamasi
```

### 2. Backend'i Çalıştır

```bash
cd backend/backend.API
dotnet restore
dotnet run
```

Backend `http://localhost:8080` adresinde çalışır (PORT env'i ile değiştirilebilir). Swagger: `http://localhost:8080/swagger`.

### 3. Web Frontend'i Çalıştır

```bash
cd frontend
npm install
npm start
```

Frontend `http://localhost:3000` adresinde çalışır.

### 4. ML Model Servisini Çalıştır

```bash
cd ML_Model_V4
python -m venv venv
./venv/Scripts/activate    # Windows
pip install -r requirements.txt
uvicorn api:app --host 0.0.0.0 --port 8000
```

ML servisi `http://localhost:8000`. Swagger: `http://localhost:8000/docs`.

> **Not:** `araba_modeli.pkl` (~1.1 GB) Git LFS ile yönetilir. Yoksa `git lfs pull` çalıştır veya HF Hub'dan indir: `huggingface_hub.hf_hub_download(repo_id='burak-sisci/otopusula-model', filename='araba_modeli.pkl')`.

### 5. Notification Service'i Çalıştır

```bash
cd notification-service
dotnet restore
dotnet run
```

Notification Service `http://localhost:7860` adresinde çalışır. RabbitMQ ve Firebase credentials'a ihtiyaç duyar (env değişkenleri tablosuna bak).

### 6. Mobil Uygulamayı Çalıştır

```bash
cd otopusula_mobile
flutter pub get
# Telefon USB ile bağlı + USB debugging açık
adb reverse tcp:8080 tcp:8080   # backend'e telefonun localhost'undan erişim
flutter run --dart-define=API_BASE_URL=http://localhost:8080
```

### Ortam Değişkenleri

#### Backend (`backend/backend.API/.env`)

| Değişken | Açıklama |
|----------|----------|
| `CONNECTION_STRING` | MongoDB bağlantı stringi (Atlas veya `mongodb://localhost:27017`) |
| `DATABASE_NAME` | Veritabanı adı (örn: `projctdb`) |
| `JWT_SECRET` | JWT imzalama anahtarı (≥32 karakter) |
| `ISSUER` / `AUDIENCE` | JWT issuer/audience |
| `ACCESS_TOKEN_MINUTES` | Access token ömrü (varsayılan: 15) |
| `REFRESH_TOKEN_DAYS` | Refresh token ömrü (varsayılan: 30) |
| `REDIS_CONNECTION_STRING` | Redis URL (StackExchange.Redis formatı) |
| `RABBITMQ_CONNECTION_STRING` | RabbitMQ AMQP URL |
| `FASTAPI_BASE_URL` | ML servis URL'i (`http://localhost:8000` veya HF Spaces URL) |
| `FRONTEND_URL` | Web frontend URL (şifre sıfırlama mailinde kullanılır) |
| `PORT` | Backend portu (Render `10000`, lokal `8080`) |

#### Notification Service (`notification-service/.env`)

| Değişken | Açıklama |
|----------|----------|
| `RABBITMQ_CONNECTION_STRING` | RabbitMQ AMQP URL |
| `CONNECTION_STRING` | MongoDB URL |
| `DATABASE_NAME` | Veritabanı adı |
| `FIREBASE_CREDENTIALS_PATH` | Service account JSON yolu (yerelde `firebase-credentials.json`) |
| `FIREBASE_CREDENTIALS_B64` | Production'da JSON'un base64'ü |
| `SMTP_HOST` / `SMTP_PORT` | SMTP sunucu (örn: `smtp.gmail.com` / `587`) |
| `SMTP_USER` / `SMTP_PASS` | Gmail App Password |
| `SMTP_FROM` | Gönderen adres |
| `PORT` | HTTP health endpoint portu (HF Spaces: `7860`) |

#### Frontend (`frontend/.env`)

| Değişken | Açıklama |
|----------|----------|
| `REACT_APP_API_URL` | Backend URL (yerel: `http://localhost:8080`, canlı: `https://otopusula-backend.onrender.com`) |

#### Mobil (build-time `--dart-define`)

| Değişken | Açıklama |
|----------|----------|
| `API_BASE_URL` | Backend URL (yerel `http://10.0.2.2:8080` emülatör için, canlı release: `https://otopusula-backend.onrender.com`) |

---

## Canlı Deploy

Projeyi sıfırdan public bir kuruluma çevirmek için adım adım rehber: [docs/DEPLOY-PUBLIC.md](docs/DEPLOY-PUBLIC.md).

Özet:
1. MongoDB Atlas M0/Flex cluster
2. Upstash Redis + CloudAMQP RabbitMQ (mevcut free tier hesaplar)
3. Backend → Render (`render.yaml` blueprint ile tek tıkla)
4. ML & Notification → Hugging Face Spaces (Docker)
5. Mobil APK rebuild + GitHub Release yayınlama