# Oto Pusula - Yerelde Calistirma Kilavuzu

Bu dokuman, projeyi sifirdan yerelde calistirmak icin gereken tum adimlari icerir.
Hicbir on bilgi gerektirmeden, adim adim takip ederek projeyi ayaga kaldirabilirsiniz.

---

## Proje Yapisi

```
oto-pusula/
├── frontend/        → React 19 (Kullanici arayuzu)         → Port 3000
├── backend/         → ASP.NET Core 10 (API sunucusu)       → Port 5078
├── ML_Model_V4/     → FastAPI + scikit-learn (AI modeli)    → Port 8000
└── dataset/         → Veri seti (arabam_merged.json)
```

Sistem uc katmandan olusur. Istek akisi:

```
Tarayici (React :3000)  →  .NET API (:5078)  →  FastAPI (:8000)
```

---

## On Gereksinimler

Asagidaki yazilimlarin bilgisayarinizda kurulu olmasi gerekir:

| Yazilim       | Minimum Versiyon | Indirme Adresi                          |
|---------------|------------------|-----------------------------------------|
| Node.js       | 18+              | https://nodejs.org                      |
| Python        | 3.12+            | https://www.python.org/downloads        |
| .NET SDK      | 10.0             | https://dotnet.microsoft.com/download   |
| Git           | herhangi         | https://git-scm.com                     |

### Kurulu olup olmadigini kontrol etmek:

```bash
node --version
npm --version
python --version
dotnet --version
git --version
```

Her komut bir versiyon numarasi dondurmelidir. Dondurmuyorsa o yazilim kurulu degildir.

---

## Adim 1: Projeyi Indirin

Eger projeyi henuz indirmediyseniz:

```bash
git clone <REPO_URL>
cd oto-pusula
```

Eger zip olarak aldiysiniz, klasoru acin ve terminal ile o klasore gidin.

---

## Adim 2: ML Model Kurulumu (FastAPI)

ML modeli, arac fiyat tahmini yapan yapay zeka servisidir.

### 2.1 Terminal acin ve ML klasorune gidin:

```bash
cd ML_Model_V4
```

### 2.2 Python sanal ortam (venv) olusturun:

```bash
python -m venv venv
```

### 2.3 Sanal ortami aktif edin:

**Windows (CMD):**
```bash
venv\Scripts\activate
```

**Windows (PowerShell):**
```powershell
.\venv\Scripts\Activate.ps1
```

**Windows (Git Bash):**
```bash
source venv/Scripts/activate
```

**Linux / Mac:**
```bash
source venv/bin/activate
```

> Basarili olursa terminal satirinin basinda `(venv)` yazar.

### 2.4 Gerekli paketleri yukleyin:

```bash
pip install -r requirements.txt
```

> Bu islem birkaC dakika surebilir. Hata alirssaniz `pip install --upgrade pip` komutunu calistirip tekrar deneyin.

### 2.5 Model dosyasini kontrol edin:

`ML_Model_V4/` klasorunde `araba_modeli.pkl` dosyasi olmalidir.
Yoksa modeli egitmek icin:

```bash
python train_model.py
```

> Bu komut `dataset/arabam_merged.json` dosyasini okuyarak modeli egitir ve `araba_modeli.pkl` olusturur.

### 2.6 FastAPI sunucusunu baslatin:

```bash
uvicorn api:app --host 0.0.0.0 --port 8000 --reload
```

### 2.7 Calistigini dogrulayin:

Tarayicida asagidaki adresi acin:

```
http://localhost:8000/docs
```

Swagger arayuzu aciliyorsa ML model basariyla calisiyor demektir.

> **BU TERMINALI KAPATMAYIN.** Yeni bir terminal acarak sonraki adima gecin.

---

## Adim 3: Backend Kurulumu (.NET)

Backend, kullanici yonetimi, ilan islemleri ve ML modeline baglanti saglayan API'dir.

### 3.1 Yeni bir terminal acin ve backend klasorune gidin:

```bash
cd backend/backend.API
```

### 3.2 Bagimliliklari yukleyin:

```bash
dotnet restore
```

### 3.3 Ortam degiskenleri (.env) kontrolu:

`backend/backend.API/.env` dosyasinin var oldugunu ve asagidaki degiskenleri icerdigini dogrulayin:

```
CONNECTION_STRING="mongodb+srv://..."
JWT_SECRET="..."
REDIS_CONNECTION_STRING="..."
```

> Bu dosya zaten projede mevcuttur. Eger silindiyse, bir takim arkadasinizdan edinin.

### 3.4 Backend sunucusunu baslatin:

```bash
dotnet run
```

Asagidaki gibi bir cikti gormelisiniz:

```
Now listening on: http://localhost:5078
```

### 3.5 Calistigini dogrulayin:

Tarayicida asagidaki adresi acin:

```
http://localhost:5078/swagger
```

Swagger arayuzu gorunuyorsa backend calisiyor demektir.

> **BU TERMINALI DE KAPATMAYIN.** Ucuncu bir terminal acarak devam edin.

---

## Adim 4: Frontend Kurulumu (React)

Frontend, kullanicinin tarayicida gordugu arayuzdur.

### 4.1 Ucuncu bir terminal acin ve frontend klasorune gidin:

```bash
cd frontend
```

### 4.2 Bagimliliklari yukleyin:

```bash
npm install
```

> Bu islem ilk seferde 1-3 dakika surebilir.

### 4.3 Gelistirme sunucusunu baslatin:

```bash
npm start
```

Tarayiciniz otomatik olarak acilacaktir. Acilmazsa:

```
http://localhost:3000
```

---

## Ozet: Tam Calistirma Sirasi

**3 ayri terminal** acin. Her birinde sirasiyla:

### Terminal 1 - ML Model:
```bash
cd ML_Model_V4
venv\Scripts\activate
uvicorn api:app --host 0.0.0.0 --port 8000 --reload
```

### Terminal 2 - Backend:
```bash
cd backend/backend.API
dotnet run
```

### Terminal 3 - Frontend:
```bash
cd frontend
npm start
```

### Baslatma sirasi onemlidir:
1. Oncelikle ML Model (port 8000)
2. Sonra Backend (port 5078)
3. En son Frontend (port 3000)

---

## Sayfa Adresleri

| Sayfa              | Adres                                |
|--------------------|--------------------------------------|
| Anasayfa           | http://localhost:3000                |
| Arac Ilanlari      | http://localhost:3000/cars           |
| Fiyat Tahmini      | http://localhost:3000/prediction     |
| Ilan Ver           | http://localhost:3000/cars/new       |
| Giris Yap          | http://localhost:3000/login          |
| Kayit Ol           | http://localhost:3000/register       |
| FastAPI Swagger    | http://localhost:8000/docs           |
| .NET Swagger       | http://localhost:5078/swagger        |

---

## Durdurma

Her terminalde `Ctrl + C` tusuna basarak ilgili sunucuyu durdurabilirsiniz.

---

## Sik Karsilasilan Sorunlar

### "python bulunamadi" veya "'python' is not recognized"
Python kurulu degil veya PATH'e eklenmemis. Python kurulumunda "Add to PATH" secenegini isaretleyin.

### "pip install" sirasinda hata aliorum
```bash
python -m pip install --upgrade pip
pip install -r requirements.txt
```

### "venv\Scripts\activate cannot be loaded" (PowerShell)
PowerShell script politikasi kapatmaktadir. Cozum:
```powershell
Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned
```

### "dotnet bulunamadi"
.NET SDK 10.0 kurulu degil. https://dotnet.microsoft.com/download adresinden indirin.

### "npm install" cok uzun suruyor veya hata veriyor
Node.js versiyonunuzu kontrol edin (`node --version`). 18 altindaysa guncelleyin.
```bash
npm cache clean --force
npm install
```

### Frontend aciliyor ama veri gelmiyor
Backend (port 5078) calismiyor olabilir. Terminal 2'yi kontrol edin.

### Fiyat tahmini calismıyor
1. ML Model (port 8000) calisıyor mu kontrol edin
2. Backend (port 5078) calisıyor mu kontrol edin
3. `http://localhost:8000/docs` adresinden FastAPI'yi dogrudan test edin

### "Model dosyasi bulunamadi" hatasi
`ML_Model_V4/araba_modeli.pkl` dosyasi eksik. Modeli egitmek icin:
```bash
cd ML_Model_V4
venv\Scripts\activate
python train_model.py
```

### Port zaten kullaniliyor hatasi
Ilgili portu kullanan programi kapatin veya:
```bash
# Windows - 8000 portunu kullanan islemi bul:
netstat -ano | findstr :8000
# Cikan PID numarasi ile islemi kapat:
taskkill /PID <PID_NUMARASI> /F
```

---

## Hizli Baslangic (Tek Bakista)

```bash
# Terminal 1
cd ML_Model_V4 && venv\Scripts\activate && uvicorn api:app --host 0.0.0.0 --port 8000 --reload

# Terminal 2
cd backend/backend.API && dotnet run

# Terminal 3
cd frontend && npm start
```

Tarayicida → http://localhost:3000
