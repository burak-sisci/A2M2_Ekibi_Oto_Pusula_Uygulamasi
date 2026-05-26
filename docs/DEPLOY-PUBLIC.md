# OtoPusula — Bulut Deploy Rehberi (Ücretsiz)

> Bu rehber sıfırdan canlı, public bir OtoPusula kurulumunu anlatır. APK kuran herkes (USB bağlantı gerekmeden) uygulamayı kullanabilir.

## Mimari

```
APK (Play Store / GitHub Release)
        │ HTTPS
        ▼
otopusula-backend.onrender.com   ←  Render Web Service (Docker)
        ├─→ MongoDB Atlas M0 (free)
        ├─→ Upstash Redis (free, mevcut)
        ├─→ CloudAMQP RabbitMQ (free, mevcut)
        └─→ otopusula-ml HF Space (free)
                  ↓
        otopusula-notification HF Space (free)
        (RabbitMQ consumer + FCM + SMTP)
```

| Bileşen | Platform | Limit |
|---------|----------|-------|
| Backend | Render Free Web | 750 h/ay, 15 dk idle sonra uyur, ~30 sn cold start |
| Notification Service | Hugging Face Spaces (Docker) | 16 GB RAM, CPU 2 core, ücretsiz |
| ML Service | Hugging Face Spaces (Docker) | 16 GB RAM (1.1 GB model rahat sığar) |
| MongoDB | Atlas M0/Flex | 5 GB |
| Redis | Upstash | 10k komut/gün |
| RabbitMQ | CloudAMQP Little Lemur | 1M mesaj/ay |
| APK paylaşımı | GitHub Releases | Sınırsız (≤2 GB asset) |

---

## Adım 1 — MongoDB Atlas M0

1. https://www.mongodb.com/cloud/atlas/register
2. Google Sign-in (`buraksisci32@gmail.com`).
3. **M0 (Free Forever)** seç. Gizliyse "Flex" de free tier'dadır.
4. Region: **Frankfurt** veya **Ireland**.
5. Cluster name: `otopusula`.
6. Database user: `otopusula` / `OtoPusula2026`.
7. Network Access: **`0.0.0.0/0`**.
8. Connect → Drivers → C#/.NET → connection string kopyala.

**Çıktı:** `mongodb+srv://otopusula:OtoPusula2026@<cluster>.mongodb.net/?retryWrites=true&w=majority`

---

## Adım 2 — Backend → Render

1. https://render.com → GitHub ile signup.
2. **New** → **Blueprint** → repo'yu seç → `render.yaml` otomatik algılanır.
3. **Environment Group** ekranında secret'ları gir:

| Key | Değer |
|-----|-------|
| `CONNECTION_STRING` | Atlas connection string (Adım 1) |
| `JWT_SECRET` | `cedpkvnpgfsmgjbimfvmkwqeqddcfvngf!!!!gklmn` |
| `REDIS_CONNECTION_STRING` | `central-crappie-132911.upstash.io:6379,user=default,password=...,ssl=True,abortConnect=False` |
| `RABBITMQ_CONNECTION_STRING` | `amqps://vcgiczeh:...@cow.rmq2.cloudamqp.com/vcgiczeh` |
| `FASTAPI_BASE_URL` | (Adım 3'te HF URL gelince ekle) |

4. **Apply** → ilk build ~10 dk.
5. URL: `https://otopusula-backend.onrender.com`

**Test:** `curl https://otopusula-backend.onrender.com/health` → `{"durum":"saglikli"}`

---

## Adım 3 — ML Service → Hugging Face Spaces

1. https://huggingface.co → signup (Google).
2. **New Space** → name: `otopusula-ml`, SDK: **Docker**, **Public**, **Create**.
3. Yerelde:
   ```bash
   cd ML_Model_V4
   git init
   huggingface-cli login
   huggingface-cli lfs-enable-largefiles .
   git remote add hf https://huggingface.co/spaces/<username>/otopusula-ml
   git lfs track "*.pkl"
   git add .gitattributes Dockerfile README.md requirements.txt api.py train_model.py araba_modeli.pkl
   git commit -m "Initial ML space"
   git push hf main
   ```
4. HF Spaces otomatik build (~10-15 dk, model 1.1 GB upload + image build).
5. URL: `https://<username>-otopusula-ml.hf.space`

**Test:** `curl https://<username>-otopusula-ml.hf.space/health`

**Render'a geri dön** → backend env → `FASTAPI_BASE_URL` = HF URL.

---

## Adım 4 — Notification Service → Hugging Face Spaces

1. **New Space** → name: `otopusula-notification`, SDK: **Docker**, **Public**.
2. Yerelde:
   ```bash
   cd notification-service
   git init
   git remote add hf https://huggingface.co/spaces/<username>/otopusula-notification
   # firebase-credentials.json olmamalı (secret'tan gelecek)
   git add .
   git commit -m "Initial notification space"
   git push hf main
   ```
3. **Settings → Repository secrets:**

| Key | Değer |
|-----|-------|
| `RABBITMQ_CONNECTION_STRING` | CloudAMQP URL |
| `CONNECTION_STRING` | Atlas URL |
| `DATABASE_NAME` | `projctdb` |
| `FIREBASE_CREDENTIALS_B64` | `firebase-credentials.json` base64 |
| `SMTP_HOST` | `smtp.gmail.com` |
| `SMTP_PORT` | `587` |
| `SMTP_USER` | `buraksisci32@gmail.com` |
| `SMTP_PASS` | Gmail App Password |
| `SMTP_FROM` | `OtoPusula <buraksisci32@gmail.com>` |

**Test:** `curl https://<username>-otopusula-notification.hf.space/health`

---

## Adım 5 — Mobile APK (public backend ile)

```powershell
cd otopusula_mobile
flutter build apk --release --dart-define=API_BASE_URL=https://otopusula-backend.onrender.com
```

APK: `build/app/outputs/flutter-apk/app-release.apk` (~53 MB, imzalı).

---

## Adım 6 — APK GitHub Release

```powershell
cd C:\Users\YOGA\Desktop\A2M2_Ekibi_Oto_Pusula_Uygulamasi-main
gh release create v1.0.0 `
  --title "OtoPusula v1.0.0" `
  --notes "Mobil sürüm 1.0 — backend Render, ML/Notification HF Spaces" `
  otopusula_mobile/build/app/outputs/flutter-apk/app-release.apk
```

**Public link:** `https://github.com/burak-sisci/A2M2_Ekibi_Oto_Pusula_Uygulamasi/releases/latest`

Direkt APK download:
```
https://github.com/burak-sisci/A2M2_Ekibi_Oto_Pusula_Uygulamasi/releases/latest/download/app-release.apk
```

QR kod istersen: https://www.qr-code-generator.com/ → link yapıştır.

---

## Telefon (kullanıcı tarafı)

1. Link/QR → APK indir.
2. Ayarlar → Güvenlik → **Bilinmeyen kaynaklardan kuruluma izin ver**.
3. APK'yi aç → **Yükle**.
4. Uygulamayı aç → kayıt ol → kullan.

**USB bağlantısı gerekmez** — APK içinde `https://otopusula-backend.onrender.com` baked.

---

## Cold Start Uyarısı

Render free tier 15 dk idle sonra uyur. Demo öncesi 30 sn önce backend'i bir kez "ısıt":
```bash
curl https://otopusula-backend.onrender.com/health
```
İlk istek ~30 sn, sonraki istekler hızlı.
