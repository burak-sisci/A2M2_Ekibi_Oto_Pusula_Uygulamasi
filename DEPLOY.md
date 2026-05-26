# Deploy Rehberi

> ⚠️ **Bu dosyanın eski Railway rehberi artık geçerli değil.** Railway hesabı kapatıldığı için proje **Render + Hugging Face Spaces + MongoDB Atlas** mimarisine taşındı.

## 🚀 Güncel deploy rehberi

Sıfırdan public bir kuruluma çıkmak için: **[docs/DEPLOY-PUBLIC.md](docs/DEPLOY-PUBLIC.md)**

Özet:

| Bileşen | Platform | Ücret |
|---------|----------|-------|
| Backend (ASP.NET Core) | Render Web Service (Docker) | Free 750h/ay |
| ML Service (FastAPI) | Hugging Face Spaces (Docker) | Free, sınırsız |
| Notification Service (.NET Worker) | Hugging Face Spaces (Docker) | Free, sınırsız |
| ML Model (1.1 GB) | Hugging Face Models repo | Free |
| MongoDB | MongoDB Atlas M0/Flex | Free |
| Redis | Upstash | Free 10k req/gün |
| RabbitMQ | CloudAMQP Little Lemur | Free 1M msg/ay |
| Mobil APK | GitHub Releases + Google Drive mirror | Free |

## 🔁 CI/CD

GitHub Actions workflow'ları: **[docs/CICD.md](docs/CICD.md)** — backend, notification, mobile için ayrı pipeline'lar.

## 📱 Mobil yayın

GitHub Release v1.0.0 + Google Drive mirror — [Releases sayfası](https://github.com/burak-sisci/A2M2_Ekibi_Oto_Pusula_Uygulamasi/releases/latest).

---

## Eski Railway rehberi (referans)

> Aşağıdaki içerik Railway hesabı kapatılmadan önceki eski rehberdir. Tarihsel referans olarak korunmuştur, **kullanmayın**. Yeni deploy için yukarıdaki linklere bakın.

<details>
<summary>Eski Railway rehberi (genişletmek için tıkla)</summary>

> Bu eski rehber `git log --follow DEPLOY.md` ile geçmişten görüntülenebilir. Repo geçmişinde commit `73ce290` ve öncesi.

</details>
