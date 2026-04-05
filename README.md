# OtoPusula

> **Not:** Araç Alım-Satım ve Yapay Zeka Destekli Fiyat Tahmin Platformu

---

## Proje Hakkında

![Ürün Tanıtım Görseli](Product.png)

**Proje Tanımı:** > OtoPusula, ikinci el araç piyasasındaki belirsizlikleri ortadan kaldırmak ve kullanıcılarına en doğru veriyi sunmak amacıyla geliştirilmiş, yapay zeka destekli bir otomotiv e-ticaret platformudur. Standart bir ilan sitesinin ötesine geçen OtoPusula, geliştirdiğimiz makine öğrenmesi (ML) algoritmaları sayesinde araçların teknik özelliklerine göre anlık "Piyasa Değer Tahmini" yapar. Kullanıcılar, gelişmiş filtreleme seçenekleriyle aradıkları aracı kolayca bulabilir, "Alınacaklar" veya "Kıyaslanacaklar" gibi özel listeler oluşturabilir ve araçlar hakkında yorum yaparak sosyal bir etkileşim kurabilirler. 

**Proje Kategorisi:** > Otomotiv, E-Ticaret, Yapay Zeka (AI), Finansal Teknoloji

---

## Proje Linkleri

- **REST API Adresi:** `https://backend-production-f151.up.railway.app/swagger`
- **Web Frontend Adresi:** `https://a2m2ekibiotopusulauygulamasi-production.up.railway.app/`
- **Makine Öğrenmesi Modeli Adresi:** `https://ml-model-production-8caa.up.railway.app/`

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

---

## Teknoloji Stack

| Katman | Teknoloji |
|--------|-----------|
| **Frontend** | React 19, React Router 7, Axios, Tailwind CSS |
| **Backend** | ASP.NET Core 10, C# |
| **Mimari** | Modular Monolith, Clean Architecture, CQRS (MediatR) |
| **Veritabanı** | MongoDB (Atlas) |
| **Cache / Oturum** | Redis (Cloud) |
| **Kimlik Dogrulama** | JWT Bearer Token |
| **Sifre Guvenligi** | BCrypt |
| **API Dokumantasyonu** | Swagger / OpenAPI |
| **ML Modeli** | Python, scikit-learn (RandomForest), FastAPI |
| **Deployment** | Railway, Docker, Nginx |

---

## Kurulum ve Baslangic

### Gereksinimler

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)
- [Python 3.11+](https://www.python.org/)
- [Git](https://git-scm.com/) ve [Git LFS](https://git-lfs.com/)

### 1. Projeyi Klonla

```bash
git clone https://github.com/burak-sisci/A2M2_Ekibi_Oto_Pusula_Uygulamasi.git
cd A2M2_Ekibi_Oto_Pusula_Uygulamasi
```

### 2. Backend'i Calistir

```bash
cd backend/backend.API
dotnet restore
dotnet run
```

Backend varsayilan olarak `http://localhost:5078` adresinde calisir. Swagger arayuzu icin: `http://localhost:5078/swagger`

### 3. Frontend'i Calistir

```bash
cd frontend
npm install
npm start
```

Frontend varsayilan olarak `http://localhost:3000` adresinde calisir.

### 4. ML Model Servisini Calistir (Opsiyonel)

```bash
cd ML_Model_V4
pip install -r requirements.txt
uvicorn api:app --host 0.0.0.0 --port 8000
```

ML servisi `http://localhost:8000` adresinde calisir. Swagger: `http://localhost:8000/docs`

### Ortam Degiskenleri

Backend icin `backend/backend.API/.env` dosyasinda asagidaki degiskenler tanimli olmalidir:

| Degisken | Aciklama |
|----------|----------|
| `CONNECTION_STRING` | MongoDB baglanti adresi |
| `DATABASE_NAME` | MongoDB veritabani adi |
| `JWT_SECRET` | JWT token sifreleme anahtari |
| `ISSUER` | JWT issuer |
| `AUDIENCE` | JWT audience |
| `EXPIRYMINUTES` | Token gecerlilik suresi (dakika) |
| `REDIS_CONNECTION_STRING` | Redis baglanti adresi |
| `FASTAPI_BASE_URL` | ML model servis adresi (opsiyonel) |

Frontend icin `frontend/.env` dosyasinda:

| Degisken | Aciklama |
|----------|----------|
| `REACT_APP_API_URL` | Backend API adresi (varsayilan: `http://localhost:5078`) |