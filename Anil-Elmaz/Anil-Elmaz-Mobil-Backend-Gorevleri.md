# Anıl Elmaz'ın Mobil Backend Görevleri

> **Modül B — Araç İlanları ve Yapay Zeka** kapsamındaki 6 endpoint için Flutter ↔ ASP.NET Core 10 backend bağlantı katmanı.

---

## 1. Yeni İlan Ekleme Servisi
- **API Endpoint:** `POST /cars`
- **Görev:** Mobil uygulamada yeni araç ilanı oluşturma servis entegrasyonu
- **İşlevler:**
  - Form verilerini (`marka`, `seri`, `model`, `yil`, `km`, `fiyat`, `vitesTipi`, `yakitTipi`, `kasaTipi`, `cekis`, `konum`, `aciklama`, `boyaliDegisen` paneli, `resimler`) toplama
  - Form validasyonu (zorunlu alanlar, sayısal aralıklar)
  - Önce resimleri `/upload` endpoint'ine yükle → URL'leri al
  - Sonra `POST /cars` ile DTO + resim URL'leri ile ilan oluştur
  - Başarılı kayıt → araç detay ekranına yönlendirme
  - Hata: 400 (validation), 401 (refresh dene), 413 (resim çok büyük), 5xx
- **Teknik Detaylar:**
  - HTTP: **Dio** + `MultipartFile.fromBytes(jpgBytes, filename)` (resim upload için `multipart/form-data`)
  - Request: `CarCreateDto.toJson()`
  - Response: yeni oluşturulan `Car` (ile carId)
  - Repository: `CarRepository.create(dto)`
  - State: `CarCreateViewModel` (`ChangeNotifier`, loading + error + success state)
  - JWT Bearer otomatik (`AuthInterceptor`)
  - Auto user assignment: backend `ownerId` = current `User.NameIdentifier` claim'inden okur

## 2. Fiyat Tahmini (ML) Servisi
- **API Endpoint:** `POST /api/Prediction/predict`
- **Görev:** Kullanıcının araç özelliklerini ML modeline gönderip tahmin alma
- **İşlevler:**
  - `CarPredictDto` (marka, model, yıl, km, vites/yakıt/kasa/çekiş, hasar bilgisi, vs.) toplama
  - `Dio.post('/api/Prediction/predict', data: dto.toJson())`
  - Backend FastAPI ML servisine proxy yapar (`FASTAPI_BASE_URL=https://burak-sisci-otopusula-ml.hf.space`)
  - Response parse: `{ durum, tahmin_sonucu: { fiyat_etiketi, birim } }`
  - UI'da formatlı fiyat gösterme
- **Teknik Detaylar:**
  - Cold start tolerance: Dio timeout 60 sn (Render + HF cold start kombinasyonu)
  - Repository: `AiRepository.predict(CarPredictDto)`
  - `PricePredictViewModel`
  - Backend tarafı:
    - Mobil → Backend (`POST /api/Prediction/predict`)
    - Backend → HF Spaces ML (`POST {FASTAPI_BASE_URL}/predict`)
    - ML → sklearn `Pipeline.predict()` → fiyat (TL)
  - Error: 503 (ML uyuyor, retry mesajı), 400 (eksik alan)

## 3. İlan Filtreleme ve Listeleme Servisi
- **API Endpoint:** `GET /cars?brand=...&minPrice=...&maxPrice=...&limit=...&offset=...`
- **Görev:** Filtreli ilan listesini çekme + pagination yönetimi
- **İşlevler:**
  - Filtre değerlerini query parametrelerine çevirme (`CarsFilter`)
  - Pagination: `PaginationParameters(limit, offset)`
  - `Dio.get('/cars', queryParameters: {...})`
  - Response: `{ data, totalCount, limit, offset, totalPages }`
  - Sonsuz scroll: liste sonuna yaklaşınca otomatik sonraki sayfa
  - Filtre değişince `_offset=0` reset + temiz fetch
- **Teknik Detaylar:**
  - `CarListViewModel` — `_items`, `_offset`, `_hasMore`, `_filter`, `_loadingMore`
  - `CarRepository.getAll(filter, pagination)`
  - Backend tarafı Redis cache (`RedisCarCacheService`) ile aynı filtre hızlanır
  - Backend MongoDB compound index (`cars_search_compound`) ile hızlı filtre
  - Pull-to-refresh → `_items.clear()` + yeniden fetch

## 4. İlan Detay Servisi
- **API Endpoint:** `GET /cars/{carId}`
- **Görev:** Tek bir aracın tüm detaylarını çekme
- **İşlevler:**
  - `Dio.get('/cars/{carId}')` → `Car.fromJson(response.data)`
  - Detay sayfası açıldığında otomatik çağrılır
  - Cache: aynı carId tekrar açılırsa Provider state'inden hemen gösterir, sonra arka planda refresh
  - 404 olursa "İlan bulunamadı" mesajı ve liste sayfasına dönüş
- **Teknik Detaylar:**
  - `CarDetailViewModel.loadCar(carId)`
  - `CarRepository.getById(carId)`
  - `cached_network_image` ile resim caching (URL üzerinden)
  - Yetki kontrolü: `car.ownerId == currentUserId` ise "Düzenle/Sil" butonları görünür

## 5. Araç Görseli Yükleme Servisi
- **API Endpoint:** `POST /api/upload/image`
- **Görev:** İlan oluşturma akışında telefonun galerisinden seçilen araç fotoğraflarını sunucuya yükleme servisi
- **İşlevler:**
  - `image_picker` ile telefondan galeri/kamera seçimi (`pickMultiImage` veya `pickImage`)
  - Seçilen her dosya için `Dio.post('/api/upload/image', data: FormData)` çağrısı
  - `FormData.fromMap({'file': await MultipartFile.fromFile(imagePath)})` — `multipart/form-data` body
  - Backend cevabı: yüklenen dosyanın public URL'i (örn. `/uploads/<guid>.jpg`)
  - URL'ler ViewModel state'inde `_uploadedUrls: List<String>` listesinde tutulur
  - İlan submit (`POST /cars`) sırasında bu URL listesi `resimler` alanı olarak ilana eklenir
  - Paralel upload: 4 dosyaya kadar eşzamanlı (`Future.wait`)
  - Hata durumunda tek dosya retry (diğerleri etkilenmez)
- **Teknik Detaylar:**
  - HTTP: **Dio 5.7** + `FormData` (multipart/form-data otomatik content-type)
  - Repository: `CarRepository.uploadImage(imagePath) → Future<String>` (URL döner)
  - State: `CarCreateViewModel._pickedImages`, `_uploadedUrls`, `_uploading: bool`
  - **`image_picker` ^1.1.2** paketi — Android `MediaStore` + iOS `PHPicker` native API'leri
  - JWT Bearer otomatik (`AuthInterceptor`)
  - Backend: `UploadController.cs` → `[HttpPost("image")]` — `IFormFile` parse + GUID isimli dosya `wwwroot/uploads/` klasörüne yazılır
  - Static file serving: backend `app.UseStaticFiles()` ile `/uploads/*.jpg` URL'leri public servis edilir
  - Dosya boyutu kontrolü: backend `[RequestSizeLimit(5_242_880)]` (5 MB)
  - Desteklenen MIME types: `image/jpeg`, `image/png`, `image/heic`
  - 401 → AuthInterceptor refresh akışı
  - 413 (Payload Too Large) → "Dosya çok büyük, küçük resim seç"
  - Yükleme sonrası ilan oluşturma akışı:
    1. Tüm resimler `/api/upload/image`'a yüklenir, URL listesi toplanır
    2. `POST /cars` çağrısında `dto.toJson(uploadedUrls)` ile birlikte gönderilir
    3. Backend `Car.resimler` alanına URL'ler kaydedilir

## 6. İlan Silme Servisi
- **API Endpoint:** `DELETE /cars/{carId}`
- **Görev:** İlanın sistemden silinmesi
- **İşlevler:**
  - Onay dialog'u sonrası `Dio.delete('/cars/{carId}')` çağrısı
  - Başarılı → liste sayfasından kaldırma (Provider notifyListeners)
  - Hata: 401 (refresh), 403 (yetki yok), 404 (zaten silinmiş)
  - Cascade: backend ilana ait yorumları, favori liste referanslarını da temizler (MediatR event)
- **Teknik Detaylar:**
  - `CarRepository.delete(carId)`
  - Local state cleanup: `CarListViewModel._items` filtre ile silinen kaldırılır
  - `GoRouter.go(AppRoutes.cars)` ile detay sayfasından geri
  - Backend Redis cache invalidation
  - Backend MongoDB: car dokümanı silinir + yorumlar `comments` koleksiyonundan + listelerdeki carId referansları

---

## Ortak Teknik Notlar

- **API Base URL:** `https://otopusula-backend.onrender.com` (canlı)
- **HTTP Client:** Dio 5.7
  - `AuthInterceptor` — Bearer token + 401 refresh rotation
  - `ErrorInterceptor` — DioException → ApiException mapping
- **State Management:** Provider 6.1 + `ChangeNotifier`
- **Repository:** `CarRepository`, `AiRepository`
- **Resim Yönetimi:**
  - Upload: `MultipartFile.fromBytes` → backend `/upload`
  - Görüntüleme: `cached_network_image` (memory + disk cache)
  - Sıkıştırma: `image_picker`'ın `imageQuality` parametresi (~85)
- **AI/ML Akışı:** Mobil → Render Backend → HF Spaces ML → response → mobil
- **Cold Start:** Render free tier 15 dk idle sonra uyur — ilk istek 30 sn, retry mantığı önemli
- **Pagination:** Limit/offset based, `_hasMore` flag ile sonsuz scroll
- **Cache Stratejisi:** Backend Redis (kısa süreli), Mobile in-memory ViewModel state, Image cache disk
