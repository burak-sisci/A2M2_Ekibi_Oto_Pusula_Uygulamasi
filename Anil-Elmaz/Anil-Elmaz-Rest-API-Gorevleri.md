**API Test Videosu:** [Youtube Link](https://youtu.be/EV1uHv6Xy6I)

# Anil Elmaz - REST API Gorevleri

## Modul B: Arac Ilanlari ve Yapay Zeka (Backend + ML)

---

### 1. Arac Domain Modeli ve Enum Tanimlari

**Sorumluluk:** Arac varlik sinifinin ve ilgili enum tiplerinin tanimlanmasi.

**Yapilan Isler:**

- **Domain/Car.cs:**
  - 26 alanli arac entity sinifi:
    - `Id` (string) - MongoDB ObjectId
    - `Marka` (string) - Marka (ornek: BMW, Mercedes)
    - `Seri` (string) - Seri (ornek: 3 Serisi, C Serisi)
    - `Model` (string) - Model (ornek: 320i, C200)
    - `Yil` (int) - Uretim yili
    - `Fiyat` (decimal) - Satis fiyati (TL)
    - `Kilometre` (int) - Kilometre
    - `VitesTipi` (enum) - Vites tipi
    - `YakitTipi` (enum) - Yakit tipi
    - `KasaTipi` (enum) - Kasa tipi
    - `Renk` (string) - Renk
    - `MotorHacmi` (double) - Motor hacmi (cc)
    - `MotorGucu` (int) - Motor gucu (HP)
    - `Cekis` (enum) - Cekis turu
    - `AracDurumu` (enum) - Arac durumu
    - `OrtalamaYakitTuketim` (double) - Ortalama yakit tuketimi (L/100km)
    - `YakitDeposu` (int) - Yakit deposu kapasitesi (L)
    - `AgirHasarKaydi` (bool) - Agir hasar kaydi var mi
    - `TakasaUygun` (bool) - Takasa uygun mu
    - `Kimden` (enum) - Satici tipi
    - `Resimler` (List<string>) - Gorsel URL listesi
    - `Konum` (string) - Arac konumu (il)
    - `IlanSahibi` (string) - Ilan sahibi kullanici ID'si
    - `BoyaliDegisen` (BoyaliveDegisen) - Panel boya/degisim detaylari
    - `IlanTarihi` (DateTime) - Ilan tarihi

  - **BoyaliveDegisen sinifi (13 panel alani):**
    - SagArkaCamurluk, ArkaKaput, SolArkaCamurluk
    - SagArkaKapi, SagOnKapi, Tavan, SolArkaKapi, SolOnKapi
    - SagOnCamurluk, MotorKaputu, SolOnCamurluk
    - OnTampon, ArkaTampon
    - Her panel icin deger: Orijinal, Boyali, Degismis

  - **Enum Tanimlari:**
    - `PanelStatus`: Orijinal, Boyali, Degismis
    - `VitesTipi`: Duz, Otomatik, YariOtomatik
    - `YakitTipi`: Benzin, Dizel, Elektrik, Hibrit, LPG, Benzin_LPG
    - `KasaTipi`: Sedan, Hatchback, SUV, Crossover, Coupe, Cabrio, StationWagon, Minivan, Pickup, Van
    - `CekisTuru`: OndenCekis, ArkadanItis, DortCeker, DortcarpiDort
    - `AracDurumu`: Sifir, IkinciEl
    - `Kimden`: Sahibinden, Galeriden

**Ilgili Dosya:**
- `backend/backend.API/Modules/Cars/Domain/Car.cs`

---

### 2. Arac Repository Katmani

**Sorumluluk:** MongoDB ile arac veri erisim islemleri.

**Yapilan Isler:**

- **ICarRepository.cs (Interface):**
  - `GetAllAsync(CarsFilter filter, PaginationParameters pagination)` - Filtrelenebilir ve sayfalanabilir ilan listesi
  - `GetByIdAsync(string id)` - Tekil ilan sorgulama
  - `CreateAsync(Car car)` - Yeni ilan olusturma
  - `UpdateAsync(string id, Car car)` - Ilan guncelleme
  - `DeleteAsync(string id)` - Ilan silme
  - `DeleteAllByOwnerIdAsync(string ownerId)` - Kullanicinin tum ilanlarini silme (cascade delete)

- **CarsFilter record (Filtreleme parametreleri):**
  - Metin filtreler: Marka, Seri, Model, Konum, Renk
  - Aralik filtreler: MinFiyat/MaxFiyat, MinKilometre/MaxKilometre, MinYil/MaxYil, MinMotorGucu/MaxMotorGucu
  - Enum filtreler: VitesTipi, YakitTipi, KasaTipi, Cekis, AracDurumu, Kimden
  - Boolean filtreler: AgirHasarKaydi, TakasaUygun

- **MongoCarRepository.cs (Implementation):**
  - MongoDB koleksiyonu: `"cars"`
  - Bilesk index: Marka, Seri, Model, Yil, Fiyat, Kilometre, YakitTipi, VitesTipi, KasaTipi, Cekis, Konum
  - Metin alanlari icin case-insensitive regex filtreleme
  - Sayisal alanlar icin aralik filtreleme
  - Sayfalama: Skip/Take ile MongoDB sorgusu

**Ilgili Dosyalar:**
- `backend/backend.API/Modules/Cars/Application/ICarRepository.cs`
- `backend/backend.API/Modules/Cars/Infrastructure/MongoCarRepository.cs`

---

### 3. Ilan Listeleme ve Filtreleme (GET /cars)

**Sorumluluk:** Araclarin filtrelenerek listelenmesi.

**Yapilan Isler:**

- **GetCarsQuery.cs:**
  - `ExecuteAsync(CarsFilter filter, PaginationParameters pagination)` - Filtrelenebilir ilan listesi
  - Sayfalama destegi: limit (varsayilan 20) ve offset (varsayilan 0)
  - Donus tipi: `PagedResult<Car>` (items + totalCount)

- **CarsController.cs - GET /cars:**
  - Query parametreleri: limit, offset, brand, seri, images, model, location, minPrice, maxPrice
  - Status 200 OK ile sayfalanmis arac listesi

**Ilgili Dosyalar:**
- `backend/backend.API/Modules/Cars/Application/GetCarsQuery.cs`
- `backend/backend.API/Presentation/Controllers/CarsController.cs`

---

### 4. Yeni Ilan Ekleme (POST /cars)

**Sorumluluk:** Sisteme yeni arac ilani eklenmesi.

**Yapilan Isler:**

- **AddCarCommand.cs:**
  - `ExecuteAsync(AddCarRequest request, string ilanSahibi)` - Yeni ilan olusturma
  - AddCarRequest: Tum arac alanlari (marka, seri, model, yil, fiyat, km, resimler, panel durumlari vb.)
  - IlanSahibi JWT claims'den alinir
  - IlanTarihi otomatik ayarlanir

- **CarsController.cs - POST /cars:**
  - UserId JWT claims'den cikarilir
  - Status 201 Created donusu

**Ilgili Dosyalar:**
- `backend/backend.API/Modules/Cars/Application/GetCarsQuery.cs` (AddCarCommand burada tanimli)
- `backend/backend.API/Presentation/Controllers/CarsController.cs`

---

### 5. Ilan Guncelleme (PUT /cars/{id})

**Sorumluluk:** Mevcut ilanin guncellenmesi.

**Yapilan Isler:**

- **UpdateCarCommand.cs:**
  - `ExecuteAsync(string id, UpdateCarRequest request)` - Ilan guncelleme
  - Guncellenebilen alanlar: Marka, Resimler, Model, Yil, Fiyat, Konum

- **CarsController.cs - PUT /cars/{id}:**
  - Sahiplik kontrolu: `IlanSahibi == userId` (JWT'den)
  - Sahip degilse 403 Forbid donusu
  - Basarili guncelleme icin Status 200 OK

**Ilgili Dosyalar:**
- `backend/backend.API/Modules/Cars/Application/GetCarsQuery.cs` (UpdateCarCommand burada tanimli)
- `backend/backend.API/Presentation/Controllers/CarsController.cs`

---

### 6. Ilan Silme (DELETE /cars/{id})

**Sorumluluk:** Ilanin sistemden silinmesi ve iliskili verilerin temizlenmesi.

**Yapilan Isler:**

- **DeleteCarCommand.cs (MediatR Command):**
  - `DeleteCarCommand(CarId) : IRequest<bool>`

- **DeleteCarCommandHandler.cs:**
  - Ilani veritabanindan siler
  - `CarDeletedEvent` MediatR notification yayinlar
  - Comments modulu bu event'i dinleyerek ilana ait yorumlari siler

- **CarsController.cs - DELETE /cars/{id}:**
  - Status 200 OK veya 404 Not Found

**Ilgili Dosyalar:**
- `backend/backend.API/Modules/Cars/Application/DeleteCarCommand.cs`
- `backend/backend.API/Modules/Cars/Application/DeleteCarCommandHandler.cs`
- `backend/backend.API/Presentation/Controllers/CarsController.cs`

---

### 7. Cascade Delete Event Handler'lari

**Sorumluluk:** Kullanici silindignde iliskili ilanlarin otomatik temizlenmesi.

**Yapilan Isler:**

- **UserDeletedEventHandler.cs (Cars Modulu):**
  - `UserDeletedEvent` dinleyicisi
  - Kullanici silindiginde `DeleteAllByOwnerIdAsync(userId)` cagrisi
  - Kullanicinin tum ilanlari otomatik silinir

**Ilgili Dosya:**
- `backend/backend.API/Modules/Cars/Application/EventHandler/UserDeletedEventHandler.cs`

---

### 8. Gorsel Yukleme Servisi (Upload API)

**Sorumluluk:** Ilan gorsellerinin sunucuya yuklenmesi.

**Yapilan Isler:**

- **UploadController.cs - POST /api/upload/image:**
  - Multipart form-data ile dosya alma
  - Desteklenen formatlar: .jpg, .jpeg, .png, .webp, .gif
  - Maksimum dosya boyutu: 5MB
  - Benzersiz dosya adi: `{Guid}{uzanti}` formati
  - Kayit yolu: `wwwroot/uploads/`
  - Donus: `{ url: "{scheme}://{host}/uploads/{fileName}" }`
  - Dosya tipi ve boyut validasyonu

**Ilgili Dosya:**
- `backend/backend.API/Presentation/Controllers/UploadController.cs`

---

### 9. Fiyat Tahmini Domain Modeli

**Sorumluluk:** ML model yanit yapisinin tanimlanmasi.

**Yapilan Isler:**

- **PricePredictionResponse.cs:**
  - `Durum` (string) - Islem durumu ("basarili")
  - `TahminSonucu` (TahminSonucu) - Tahmin sonucu nesnesi
    - `FiyatEtiketi` (string) - Formatlanmis fiyat ("XXX.XXX,XX TL")
    - `Birim` (string) - Para birimi ("TL")

**Ilgili Dosya:**
- `backend/backend.API/Modules/Prediction/Domain/PricePredictionResponse.cs`

---

### 10. Fiyat Tahmini Servis Katmani

**Sorumluluk:** Backend ile FastAPI ML servisi arasindaki iletisimin saglanmasi.

**Yapilan Isler:**

- **IPredictionService.cs (Interface):**
  - `PredictAsync(Car car)` - Arac bilgileriyle fiyat tahmini alma

- **PredictionService.cs (Implementation):**
  - HttpClient ile FastAPI `/predict` endpoint'ine POST istegi
  - C# Car nesnesinden Python beklentisine uygun JSON payload olusturma:
    - snake_case alan adlari (ilan_tarihi, vites_tipi, yakit_tipi vb.)
    - Motor hacmi formatlamasi: "X cc"
    - Motor gucu formatlamasi: "X hp"
    - Boolean alanlari Turkce stringe cevirme: "Evet" / "Hayir"
    - 13 panel alani icin PanelStatus enum → string donusumu
  - Yanit deserialization: `JsonNamingPolicy.SnakeCaseLower`
  - Hata yonetimi ve loglama

- **PredictPriceQuery.cs (MediatR Query):**
  - `PredictPriceQuery(Car) : IRequest<PricePredictionResponse>`

- **PredictPriceQueryHandler.cs:**
  - MediatR handler, `IPredictionService.PredictAsync()` cagirisi

- **PredictionController.cs - POST /api/prediction/predict:**
  - Girdi: Car nesnesi (full entity)
  - MediatR uzerinden PredictPriceQuery gonderme
  - Status 200 OK ile tahmin sonucu veya 400 BadRequest

- **Program.cs HttpClient yapilandirmasi:**
  - `FASTAPI_BASE_URL` environment variable (varsayilan: `http://127.0.0.1:8000`)
  - Timeout: 30 saniye

**Ilgili Dosyalar:**
- `backend/backend.API/Modules/Prediction/Application/IPredictionService.cs`
- `backend/backend.API/Modules/Prediction/Infrastructure/PredictionService.cs`
- `backend/backend.API/Modules/Prediction/Application/PredictPriceQuery.cs`
- `backend/backend.API/Modules/Prediction/Application/PredictPriceQueryHandler.cs`
- `backend/backend.API/Presentation/Controllers/PredictionController.cs`

---

### 11. Makine Ogrenmesi Modeli (FastAPI Servisi)

**Sorumluluk:** Arac fiyat tahmini yapan ML modelinin gelistirilmesi ve API olarak sunulmasi.

**Yapilan Isler:**

- **ML_Model_V4/api.py (FastAPI Sunucusu):**
  - Uygulama adi: "Oto Pusula Fiyat Tahmini API"
  - Lazy model yukleme: Ilk istek geldiginde `araba_modeli.pkl` dosyasindan model yuklenir
  - **GET /health:** Saglik kontrolu endpoint'i
  - **POST /predict:** Fiyat tahmini endpoint'i
    - Girdi: `CarFeatures` Pydantic modeli (32 alan)
    - Feature engineering:
      1. Motor hacmi parse etme (cc degerinden sayi cikartma, ortalama alma)
      2. Motor gucu parse etme (hp degerinden sayi cikartma)
      3. Cekis enum mapping: "Onden Cekis" → "OndenCekis", "Arkadan Itis" → "ArkadanItis", "4WD/AWD" → "DortCeker"
      4. Yakit tipi mapping: "LPG & Benzin" → "Benzin_LPG"
      5. Kasa tipi mapping: "Hatchback/5" → "Hatchback", "Station Wagon" → "StationWagon", "Panelvan" → "Van"
      6. Vites mapping: "Yari Otomatik" → "YariOtomatik"
      7. Panel boya mapping: "Boyanmis/Lokal Boyanmis" → "Boyali", "Belirtilmemis" → "Orijinal"
      8. Turetilmis ozellikler: hasar_skoru (boyali panel sayisi), degisim_skoru (degismis panel sayisi), marka_seri (birlestirme), agir_hasarli_binary
    - Cikti: `{ durum: "basarili", tahmin_sonucu: { fiyat_etiketi: "XXX.XXX,XX TL", birim: "TL" } }`
    - Turkce sayi formati (nokta binlik, virgul ondalik)

- **ML_Model_V4/train_model.py (Model Egitimi):**
  - Veri kaynagi: `arabam_merged.json` (web scraping verisi)
  - Veri isleme pipeline:
    1. Sadece ikinci el araclar filtreleme
    2. Sayisal alan donusumleri (fiyat, yil, km)
    3. Turkce tarih formati parse etme
    4. Eksik veri doldurma ve temizleme
    5. Nadir kategori gruplama (threshold: 15 model, 10 seri → "Diger")
    6. IQR yontemi ile outlier temizleme (1.5xIQR)
    7. Grup bazli outlier temizleme (marka/seri)
    8. Fiyat filtresi: > 100.000 TL
  - **Model Mimarisi:**
    - Sayisal sutunlar (7): yil, km, motor_hacmi, motor_gucu, hasar_skoru, degisim_skoru, agir_hasarli_binary → StandardScaler
    - Kategorik sutunlar (19): marka_seri, model, vites_tipi, yakit_tipi, kasa_tipi, cekis + 13 panel → OneHotEncoder
    - Regressor: `RandomForestRegressor` (n_estimators=100, random_state=42, n_jobs=-1)
  - Degerlendirme metrikleri: R2 Score, Mean Absolute Error (MAE)
  - Cikti: `araba_modeli.pkl` (scikit-learn pipeline, ~1.1GB)

- **ML_Model_V4/requirements.txt:**
  - numpy==2.4.3, pandas==3.0.1, scikit-learn==1.8.0
  - matplotlib==3.10.8, seaborn==0.13.2 (veri gorsellestirme)
  - fastapi==0.135.1, uvicorn==0.34.0 (API sunucusu)

**Ilgili Dosyalar:**
- `ML_Model_V4/api.py`
- `ML_Model_V4/train_model.py`
- `ML_Model_V4/requirements.txt`
- `ML_Model_V4/araba_modeli.pkl`

---

### Kullanilan Teknolojiler ve Kutuphaneler

| Teknoloji | Kullanim Alani |
|-----------|----------------|
| ASP.NET Core 10 | Web API framework |
| MongoDB.Driver | Veritabani erisimi |
| MediatR | CQRS pattern |
| HttpClient | FastAPI ile iletisim |
| FastAPI | ML model API sunucusu |
| Uvicorn | ASGI sunucu |
| scikit-learn | Makine ogrenmesi (RandomForest) |
| pandas / numpy | Veri isleme |
| Pydantic | Veri dogrulama (FastAPI) |

---

### API Endpoint Ozeti

**Backend (ASP.NET Core):**

| Metod | Endpoint | Yetki | Aciklama |
|-------|----------|-------|----------|
| GET | /cars | Hayir | Ilanlari listele ve filtrele |
| POST | /cars | Hayir* | Yeni ilan ekle |
| PUT | /cars/{id} | Hayir* | Ilan guncelle (sahiplik kontrolu) |
| DELETE | /cars/{id} | Hayir* | Ilan sil |
| POST | /api/upload/image | Hayir | Gorsel yukle |
| POST | /api/prediction/predict | Hayir | Fiyat tahmini al |

*Not: [Authorize] attribute'u kodda yorum satiri olarak birakilmis, ileride aktif edilecek.

**ML Servisi (FastAPI):**

| Metod | Endpoint | Aciklama |
|-------|----------|----------|
| GET | /health | Saglik kontrolu |
| POST | /predict | Arac fiyat tahmini |
