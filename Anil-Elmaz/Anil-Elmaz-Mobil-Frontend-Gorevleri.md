# Anıl Elmaz'ın Mobil Frontend Görevleri

**Mobile Front-end Demo Videosu:** [Link buraya eklenecek](https://example.com)

> **Modül B — Araç İlanları ve Yapay Zeka** kapsamındaki 6 endpoint için Flutter mobil arayüz tasarımı ve implementasyonu.

---

## 1. Yeni İlan Ekleme Ekranı
- **API Endpoint:** `POST /cars`
- **Görev:** Aracın teknik özellikleri, konumu ve fotoğraflarıyla yeni ilan oluşturma ekranı
- **UI Bileşenleri:**
  - Marka (`DropdownButton<String>`)
  - Seri / Model (`TextFormField`)
  - Yıl seçici (`DropdownButton<int>` veya `NumberPicker`)
  - Kilometre (`TextFormField`, keyboardType: number)
  - Fiyat (`TextFormField`, currency formatter)
  - Vites tipi (`DropdownButton`: Düz / Otomatik / YariOtomatik)
  - Yakıt tipi (`DropdownButton`: Benzin / Dizel / Elektrik / Hibrit / LPG / Benzin_LPG)
  - Kasa tipi (`DropdownButton`: Sedan / Hatchback / SUV / Coupe / Cabrio / Pickup / ...)
  - Renk (`TextFormField`)
  - Motor hacmi & gücü (`TextFormField`)
  - Çekiş (`DropdownButton`: ÖndenÇekiş / Arkadanİtiş / DörtÇeker)
  - Konum (`TextFormField`)
  - Açıklama (`TextFormField`, multiline, max 1000 karakter)
  - **Boya & Değişen detayları** (13 parça paneli için `ExpansionTile`, her parça için `DropdownButton`: Orijinal / Boyali / Değişmiş)
  - **Resim yükleme alanı** (`image_picker` ile 1-8 resim, grid layout)
  - "İlanı Yayınla" butonu (`ElevatedButton`)
- **Form Validasyonu:**
  - Tüm zorunlu alanlar dolu mu kontrolü (`Form` + `GlobalKey<FormState>`)
  - Yıl 1980-2026 aralığı
  - Kilometre 0-1.000.000 aralığı
  - Fiyat pozitif sayı
  - Resim sayısı 1-8 arası (`AppConstants.maxCarImages`)
  - Her resim max 5 MB (`AppConstants.maxImageSizeBytes`)
- **Kullanıcı Deneyimi:**
  - Multi-step form yapısı (`Stepper`) veya tek sayfada `SingleChildScrollView`
  - Image picker: galeri/kamera seçimi `ImageSource`
  - Yüklenen resimler küçük thumbnail önizleme + silme butonu
  - Yayınla butonuna basıldığında loading + disable
  - Başarılı → `SnackBar` "İlanınız yayınlandı" + araç detay ekranına yönlendirme
  - Hata → alan bazlı validation veya generic error mesajı
- **Teknik Detaylar:**
  - **Flutter** + Material 3
  - State: `CarCreateViewModel` (Provider)
  - HTTP: `Dio.post('/cars', data: dto.toJson())`
  - Resim upload: `MultipartFile.fromBytes` ile `multipart/form-data`
  - Repository: `CarRepository.create(CarCreateDto)`
  - Navigation: GoRouter → `AppRoutes.carDetail` (yeni carId ile)

## 2. Fiyat Tahmini (ML) Ekranı
- **API Endpoint:** `POST /api/Prediction/predict` (FastAPI → `/predict` proxy)
- **Görev:** Kullanıcının araç özelliklerini girip AI ile fiyat tahmini alması
- **UI Bileşenleri:**
  - "Fiyat Tahmin Et" başlığı + AI ikonlu hero
  - Marka, Seri, Model, Yıl, Km input alanları
  - Vites, Yakıt, Kasa, Çekiş dropdown'ları
  - Motor hacmi, gücü
  - Hasar/Boya durumu (13 parça paneli, ExpansionTile)
  - "Tahmin Et" butonu
  - Sonuç kartı (`Card`): "Tahmini Fiyat: **1.717.705 TL**" + animasyon
  - "Yeniden Hesapla" butonu
- **Form Validasyonu:**
  - Tüm gerekli alanlar dolu
  - Sayısal alanlar pozitif
- **Kullanıcı Deneyimi:**
  - Tahmin sırasında `CircularProgressIndicator` + "AI hesaplıyor..." metni (cold start için 30 sn açıklaması)
  - Sonuç güzel formatlanmış (1.717.705,47 TL)
  - "Bu fiyata ilan oluştur" CTA → CarCreate ekranına bu değerlerle yönlendirme
  - Hata → "Tahmin alınamadı, tekrar deneyin"
- **Teknik Detaylar:**
  - `PricePredictViewModel`
  - `AiRepository.predict(CarPredictDto)` → `Dio.post('/api/Prediction/predict')`
  - Backend, ML servisine (`FASTAPI_BASE_URL=https://burak-sisci-otopusula-ml.hf.space`) proxy yapar
  - Response: `{ durum, tahmin_sonucu: { fiyat_etiketi, birim } }`
  - Cold start (Render + HF) için timeout 60 sn

## 3. İlanları Filtreleme / Listeleme Ekranı
- **API Endpoint:** `GET /cars` (query: brand, model, minPrice, maxPrice, limit, offset)
- **Görev:** Tüm ilanları filtreli liste halinde gösterme
- **UI Bileşenleri:**
  - `AppBar` + arama input (`SearchBar`)
  - Filtre paneli (`BottomSheet`): Marka, Model, Fiyat aralığı (`RangeSlider`), Konum, Yıl
  - "Filtrele" + "Temizle" butonları
  - Sıralama dropdown'ı (Fiyat artan/azalan, Yıl, Tarih)
  - Araç kartları listesi (`ListView.builder`):
    - Resim (`cached_network_image`)
    - Marka + Model + Yıl
    - Fiyat (`NumberFormat.currency`)
    - Kilometre, konum
    - "Favoriye Ekle" ikon butonu
  - "Daha fazla yükle" / sonsuz scroll (`paginationScrollThreshold`)
  - Empty state ("Aramanıza uygun ilan yok")
  - Pull-to-refresh (`RefreshIndicator`)
- **Kullanıcı Deneyimi:**
  - İlk yüklemede loading skeleton
  - Sonsuz scroll: liste sonuna 200px kala otomatik sonraki sayfa
  - Filtre uygulandığında badge ile kaç filtre aktif gösterilir
  - Tıklayınca araç detay sayfasına geçiş
- **Teknik Detaylar:**
  - `CarListViewModel` — `_items`, `_offset`, `_hasMore`, `_filter` state'leri
  - `CarRepository.getAll(filter, pagination)` → `Dio.get('/cars', queryParameters: ...)`
  - Response: `{ data, totalCount, limit, offset, totalPages }`
  - Filtreler local state'te tutulur, "Uygula" basılınca request gider
  - Redis cache backend'de — aynı filtre tekrar istenince hızlı döner

## 4. İlan Detay Ekranı
- **API Endpoint:** `GET /cars/{carId}`
- **Görev:** Seçilen aracın tüm detaylarını gösterme
- **UI Bileşenleri:**
  - `PageView` ile resim galerisi (swipe + indicator)
  - Tam fiyat (büyük, vurgulu)
  - Marka + Seri + Model + Yıl başlığı
  - Hızlı bilgi grid'i: Km, Vites, Yakıt, Kasa, Konum
  - "Yorumları Gör" bölümü (yorum sayısı + ilk 3 yorum preview)
  - "Bu araca yorum yap" butonu → Yorum sayfası
  - "Favoriye Ekle" / "Listeye Ekle" butonu (`BottomSheet`)
  - "Paylaş" butonu (`/cars/{carId}/share` → kısa link)
  - "AI Fiyat Tahmini al" butonu (bu aracın değerini ML ile yeniden hesapla)
  - Boya & Değişen detay paneli (`ExpansionTile`)
  - İlan sahibinin profili (avatar + ad + "Profili Gör")
  - Sahibiyse "Düzenle" / "Sil" butonları görünür
- **Kullanıcı Deneyimi:**
  - Hero animasyon (liste → detay geçişinde resim büyür)
  - `SliverAppBar` ile scroll'da başlık küçülür
  - Resim tıklayınca tam ekran galeri (`PhotoView`)
- **Teknik Detaylar:**
  - `CarDetailViewModel` — `Car? car`, loading, error state
  - `CarRepository.getById(carId)`
  - `cached_network_image` ile resim caching
  - `Share.share(link)` ile native share sheet (`share_plus` veya GoRouter deep link)

## 5. İlan Güncelleme Ekranı
- **API Endpoint:** `PUT /cars/{carId}`
- **Görev:** İlan sahibinin araç fiyatını veya açıklamasını düzenlemesi
- **UI Bileşenleri:**
  - "İlanı Düzenle" başlığı
  - CarCreate ekranıyla aynı form layout, **mevcut değerlerle dolu**
  - Sadece izin verilen alanlar düzenlenebilir (fiyat, açıklama, konum, resimler)
  - "Kaydet" / "İptal" butonları
- **Form Validasyonu:**
  - Fiyat pozitif
  - Açıklama max 1000 karakter
- **Kullanıcı Deneyimi:**
  - Değişiklik yapılmadan "Kaydet" disabled (`isDirty` kontrolü)
  - Optimistic update + rollback
  - Unsaved changes uyarısı (`WillPopScope`)
- **Teknik Detaylar:**
  - `CarUpdateViewModel`
  - `CarRepository.update(carId, CarUpdateDto)` → `Dio.put('/cars/{carId}')`
  - Backend yetki kontrolü: sadece `ownerId == currentUserId` güncelleyebilir
  - 403 Forbidden → "Bu ilanı düzenleme yetkin yok"

## 6. İlan Silme
- **API Endpoint:** `DELETE /cars/{carId}`
- **Görev:** İlanın sistemden silinmesi
- **UI Bileşenleri:**
  - "İlanı Sil" butonu (sahip görünür, kırmızı, destructive)
  - Onay `AlertDialog` ("Bu ilanı silmek istediğinize emin misiniz?")
  - Silme sırasında loading
- **Kullanıcı Deneyimi:**
  - Onay → backend silme → `SnackBar` "İlan silindi" → liste sayfasına dönüş
  - Liste sayfasında ilgili araç kartı kaybolur (Provider notifyListeners ile)
  - Hata → "İlan silinemedi, tekrar deneyin"
- **Teknik Detaylar:**
  - `CarRepository.delete(carId)` → `Dio.delete('/cars/{carId}')`
  - Yetki kontrolü backend tarafında (`ownerId == currentUserId`)
  - Backend MediatR event ile cascade: ilana ait yorumlar, favori liste referansları temizlenir
  - GoRouter `go(AppRoutes.cars)` ile car list'e dönüş + scroll position korunur

---

## Ortak Teknik Notlar

- **Mimari:** MVVM, `lib/viewmodels/car/`, `lib/view/pages/car/`
- **Resim:** `cached_network_image` (network), `image_picker` (galeri/kamera), `MultipartFile` upload
- **State Management:** Provider 6.1 + `ChangeNotifier`
- **Repository:** `CarRepository`, `AiRepository`
- **AI/ML:** Backend `https://otopusula-backend.onrender.com/api/Prediction/predict` → HF Spaces ML (`https://burak-sisci-otopusula-ml.hf.space`)
- **Currency:** `intl` paketi (`NumberFormat.currency(locale: 'tr_TR', symbol: 'TL')`)
- **Pagination:** Limit/offset, sonsuz scroll, `_hasMore` flag
