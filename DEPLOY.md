# Oto Pusula - Railway Deployment Rehberi

Bu rehber, Oto Pusula projesini Railway platformunda canlıya almak için adım adım talimatlar içerir.

---

## Ön Gereksinimler

- [GitHub](https://github.com) hesabı
- [Railway](https://railway.app) hesabı (GitHub ile giriş yapılır)
- [Git](https://git-scm.com/downloads) ve [Git LFS](https://git-lfs.com/) yüklü
- Projenin tamamı lokalde çalışır durumda

---

## Adım 1: GitHub Repository Oluşturma

### 1.1 GitHub'da yeni repo oluştur

1. https://github.com/new adresine git
2. Repository adı: `oto-pusula` (veya istediğin isim)
3. **Private** olarak oluştur (credentials güvenliği için)
4. "Create repository" tıkla

### 1.2 Projeyi GitHub'a pushla

Terminal'de proje dizinine git ve sırayla çalıştır:

```bash
cd C:\Users\YOGA\Desktop\frontend-deneme

# İlk commit
git add .
git commit -m "Initial commit: Oto Pusula full-stack application"

# GitHub remote ekle (kendi kullanıcı adını yaz)
git remote add origin https://github.com/KULLANICI_ADIN/oto-pusula.git

# Push (LFS dosyaları otomatik yüklenir)
git branch -M main
git push -u origin main
```

> **Not:** `araba_modeli.pkl` dosyası (1.1 GB) Git LFS ile takip ediliyor. Push sırasında bu dosya LFS sunucusuna yüklenir, biraz zaman alabilir.

---

## Adım 2: Railway Hesabı ve Proje Kurulumu

### 2.1 Railway'e giriş yap

1. https://railway.app adresine git
2. **"Login with GitHub"** ile giriş yap
3. Dashboard'a yönlendirileceksin

### 2.2 Yeni proje oluştur

1. Dashboard'da **"New Project"** butonuna tıkla
2. **"Empty Project"** seç

---

## Adım 3: ML Model Servisini Deploy Et (İlk Bu!)

ML servisi bağımsız çalışır, önce onu kurmak en mantıklısı.

### 3.1 Servis ekle

1. Proje dashboard'unda **"New"** → **"GitHub Repo"** tıkla
2. Repo'nu seç (`oto-pusula`)
3. **"Add Root Directory"** → `ML_Model_V4` yaz
4. Deploy başlayacak

### 3.2 Environment Variables ayarla

Servis sayfasında **"Variables"** sekmesine git, şunları ekle:

| Değişken | Değer |
|----------|-------|
| `PORT` | `8000` |

### 3.3 Domain oluştur

1. **"Settings"** → **"Networking"** bölümüne git
2. **"Generate Domain"** tıkla
3. Verilen URL'yi not al (örn: `ml-model-production-xxxx.up.railway.app`)

> **Önemli:** Bu URL'yi bir sonraki adımda backend'e vereceksin.

---

## Adım 4: Backend Servisini Deploy Et

### 4.1 Servis ekle

1. Aynı proje içinde **"New"** → **"GitHub Repo"** tıkla
2. Aynı repo'yu seç
3. **"Add Root Directory"** → `backend` yaz

### 4.2 Environment Variables ayarla

**"Variables"** sekmesinde şunları ekle:

| Değişken | Değer |
|----------|-------|
| `CONNECTION_STRING` | `mongodb+srv://projectdb:1024@projecluster.9oq0six.mongodb.net/?appName=arabaproje` |
| `DATABASE_NAME` | `projctdb` |
| `JWT_SECRET` | `cedpkvnpgfsmgjbimfvmkwqeqddcfvngf!!!!gklmn` |
| `ISSUER` | `Sahibinden.com` |
| `AUDIENCE` | `Sahibinden.com` |
| `EXPIRYMINUTES` | `60` |
| `REDIS_CONNECTION_STRING` | `redis-12197.c322.us-east-1-2.ec2.cloud.redislabs.com:12197,password=AvRIdMYdM9FGtfPXiPzn7twLBwMB49Ub,ssl=True,abortConnect=False` |
| `FASTAPI_BASE_URL` | `https://ML-SERVIS-DOMAININ.up.railway.app` |
| `ASPNETCORE_ENVIRONMENT` | `Production` |

> **Önemli:** `FASTAPI_BASE_URL` değerine Adım 3'te oluşturduğun ML servis domain'ini yaz!

### 4.3 Domain oluştur

1. **"Settings"** → **"Networking"** → **"Generate Domain"**
2. Verilen URL'yi not al (örn: `backend-production-xxxx.up.railway.app`)

---

## Adım 5: Frontend Servisini Deploy Et

### 5.1 Servis ekle

1. **"New"** → **"GitHub Repo"** → aynı repo
2. **"Add Root Directory"** → `frontend` yaz

### 5.2 Build Arguments ayarla

**"Variables"** sekmesinde:

| Değişken | Değer |
|----------|-------|
| `REACT_APP_API_URL` | `https://BACKEND-DOMAININ.up.railway.app` |

> **Önemli:** Bu değere Adım 4'te oluşturduğun backend domain'ini yaz!

**Not:** React uygulamalarında environment variable'lar build sırasında bake edilir. Bu yüzden bu değişkeni **build argument** olarak da eklememiz gerekiyor:

1. **"Settings"** sekmesine git
2. **"Build"** bölümünde **"Docker Build Arguments"** kısmına:
   - `REACT_APP_API_URL` = `https://BACKEND-DOMAININ.up.railway.app`

### 5.3 Domain oluştur

1. **"Settings"** → **"Networking"** → **"Generate Domain"**
2. Bu senin sitenin ana adresi olacak! 🎉

---

## Adım 6: Doğrulama

Deploy tamamlandıktan sonra her servisi kontrol et:

### ML Model Kontrolü
```
https://ML-DOMAININ.up.railway.app/docs
```
FastAPI Swagger arayüzü açılmalı.

### Backend Kontrolü
```
https://BACKEND-DOMAININ.up.railway.app/health
```
`{"status":"healthy"}` yanıtı dönmeli.

```
https://BACKEND-DOMAININ.up.railway.app/swagger
```
Swagger UI açılmalı.

### Frontend Kontrolü
```
https://FRONTEND-DOMAININ.up.railway.app
```
Oto Pusula ana sayfası açılmalı.

---

## Sorun Giderme

### Build hatası alıyorum
- **"Deployments"** sekmesinden build log'larını kontrol et
- .NET 10 preview image'ı bulunamıyorsa, Railway build ortamının güncel olduğundan emin ol

### ML model yüklenemiyor / Memory hatası
- ML model 1.1 GB RAM kullanıyor. Railway Hobby planında ($5/ay) servise ayrılan RAM'i artır:
  - **"Settings"** → **"Resources"** → Memory'yi en az **2 GB** yap

### Frontend backend'e bağlanamıyor
- `REACT_APP_API_URL` değerinin doğru olduğundan emin ol
- URL'nin `https://` ile başladığından emin ol
- Backend servisinin çalıştığını `/health` endpoint'inden kontrol et

### CORS hatası
- Backend zaten `AllowAnyOrigin` ayarlı, bu bir sorun olmamalı
- Eğer hala hata varsa, tarayıcı konsolundan tam hata mesajını kontrol et

### Redis bağlantı hatası
- Redis cloud instance'ın çalıştığından emin ol
- SSL ayarının doğru olduğunu kontrol et

---

## Mimari Özet

```
┌─────────────────┐     ┌──────────────────┐     ┌─────────────────┐
│    Frontend      │────▶│     Backend      │────▶│    ML Model     │
│  (React/Nginx)  │     │  (ASP.NET Core)  │     │    (FastAPI)    │
│   Port: 80      │     │   Port: 8080     │     │   Port: 8000    │
└─────────────────┘     └──────────────────┘     └─────────────────┘
                              │       │
                              ▼       ▼
                        ┌─────────┐ ┌───────┐
                        │ MongoDB │ │ Redis │
                        │ (Atlas) │ │(Cloud)│
                        └─────────┘ └───────┘
```

---

## Maliyet

- **Railway Trial:** İlk $5 kredi ücretsiz
- **Railway Hobby:** $5/ay (3 servisi çalıştırmak için yeterli)
- **MongoDB Atlas:** Ücretsiz tier (512 MB)
- **Redis Cloud:** Ücretsiz tier (30 MB)

> **İpucu:** Kullanmadığın zamanlarda servisleri "Sleep" moduna alarak kredi tasarrufu yapabilirsin.
