# Anil Elmaz - Web Frontend Gorevleri

## Modul B: Arac Ilanlari ve Yapay Zeka (Frontend)

---

### 1. Arac API Servis Katmani

**Sorumluluk:** Arac ilanlari ile ilgili tum API cagrlarinin tanimlanmasi.

**Yapilan Isler:**

- **api/carApi.js:**
  - `getCars(params)` → GET `/cars` - Ilan listesi (sayfalama ve filtre parametreleri ile)
  - `getCarById(id)` → GET `/cars/{id}` - Tekil ilan detayi
  - `createCar(data)` → POST `/cars` - Yeni ilan olusturma
  - `updateCar(id, data)` → PUT `/cars/{id}` - Ilan guncelleme
  - `deleteCar(id)` → DELETE `/cars/{id}` - Ilan silme
  - `uploadImage(file)` → POST `/api/upload/image` - Gorsel yukleme (FormData)

- **api/predictionApi.js:**
  - `predictPrice(data)` → POST `/api/prediction/predict` - ML fiyat tahmini

**Ilgili Dosyalar:**
- `frontend/src/api/carApi.js`
- `frontend/src/api/predictionApi.js`

---

### 2. Ilan Listeleme Sayfasi (CarList)

**Sorumluluk:** Arac ilanlarinin sayfalanmis ve filtrelenmis sekilde goruntulenmesi.

**Yapilan Isler:**

- **components/Cars/CarList.js:**
  - Sayfa basina 12 arac gosterimi
  - `getCars()` API cagrisi ile veri cekme (limit, offset, filtre parametreleri)
  - CarFilter bileseni ile filtre UI entegrasyonu
  - Sayfalama kontrolleri: Onceki/Sonraki butonlari
  - Toplam sonuc sayisi gosterimi
  - Her arac icin CarCard bileseni render etme
  - Yukleme durumunda loading gosterimi

**Ilgili Dosya:**
- `frontend/src/components/Cars/CarList.js`

---

### 3. Arac Karti Bileseni (CarCard)

**Sorumluluk:** Tek bir arac ilaninin kart gorunumunde sunulmasi.

**Yapilan Isler:**

- **components/Cars/CarCard.js:**
  - Gorsel: Ilk resim veya placeholder gorsel
  - Baslik: Marka + Model + Yil
  - Fiyat: Turkce formatta (XXX.XXX TL)
  - Teknik bilgiler: Kilometre, Yakit tipi, Vites tipi
  - Rozetler:
    - "Agir Hasar" rozeti (kirmizi - agirHasarKaydi true ise)
    - "Takasa Uygun" rozeti (yesil - takasaUygun true ise)
  - Tiklanabilir kart: CarDetail sayfasina yonlendirme (`/cars/{id}`)

**Ilgili Dosya:**
- `frontend/src/components/Cars/CarCard.js`

---

### 4. Ilan Detay Sayfasi (CarDetail)

**Sorumluluk:** Secilen aracin tum detaylarinin goruntulenmesi.

**Yapilan Isler:**

- **components/Cars/CarDetail.js:**
  - URL parametresinden arac ID'si alma (`useParams`)
  - `getCarById(id)` ile veri cekme

  - **Gorsel Galeri:**
    - Ana gorsel alani (buyuk gorsel)
    - Kucuk resim (thumbnail) listesi
    - Thumbnail tiklandiginda ana gorseli degistirme

  - **Teknik Ozellikler Bolumu:**
    - Kilometre, Yakit tipi, Vites tipi, Motor gucu, Kasa tipi, Konum
    - Ikon ile gorsel zenginlestirme

  - **Detay Tablosu:**
    - Yil, Renk, Motor hacmi, Cekis turu, Arac durumu, Yakit tuketimi, Yakit deposu
    - Tablo formati ile duzenli gosterim

  - **Panel Boya/Degisim Gorsellestirmesi:**
    - 13 panel icin durum gosterimi (Orijinal/Boyali/Degismis)
    - Renk kodlamasi: Yesil (Orijinal), Sari (Boyali), Kirmizi (Degismis)

  - **Etklesim Butonlari:**
    - Favori butonu: Listelerden birine ekleme (modal ile liste secimi)
    - Duzenle butonu: Sadece ilan sahibine gorulur (`/cars/{id}/edit`)
    - Sil butonu: Sadece ilan sahibine gorulur, onay sonrasi silme

  - **Yorum Bolumu:**
    - CommentSection bileseni entegrasyonu (Mehmet Uludag modulu)

**Ilgili Dosya:**
- `frontend/src/components/Cars/CarDetail.js`

---

### 5. Ilan Olusturma/Duzenleme Formu (CarForm)

**Sorumluluk:** Yeni ilan olusturma ve mevcut ilan duzenleme formunun uygulanmasi.

**Yapilan Isler:**

- **components/Cars/CarForm.js:**
  - Iki modlu form: Olusturma (yeni) ve Duzenleme (mevcut)
  - URL'de `:id` parametresi varsa duzenleme modu, yoksa olusturma modu

  - **Form Alanlari (30+ alan):**
    - Temel: Marka (60+ secenekli dropdown), Seri, Model, Yil, Fiyat, Kilometre, Renk
    - Teknik: Yakit tipi, Vites tipi, Kasa tipi, Motor hacmi (cc), Motor gucu (HP), Cekis turu, Yakit tuketimi, Yakit deposu
    - Durum: Arac durumu (Sifir/Ikinci El), Satici tipi (Sahibinden/Galeriden), Konum
    - Boolean: Agir hasar kaydi, Takasa uygun
    - Panel durumu: 13 panel icin ayri ayri secim (Orijinal/Boyali/Degismis)

  - **Gorsel Yukleme Sistemi:**
    - Dosya secimi ile gorsel yukleme
    - `uploadImage(file)` API cagrisi (FormData)
    - Yuklenen gorsellerin onizlemesi
    - Gorsel silme (listeden cikarma)
    - URL ile gorsel ekleme destegi

  - **Duzenleme Modu:**
    - `getCarById(id)` ile mevcut veriyi cekme
    - Form alanlarini mevcut verilerle doldurma
    - `updateCar(id, data)` ile guncelleme

  - **Olusturma Modu:**
    - `createCar(data)` ile yeni ilan
    - Basarili olusturma sonrasi ilan detay sayfasina yonlendirme

**Ilgili Dosya:**
- `frontend/src/components/Cars/CarForm.js`

---

### 6. Filtre Bileseni (CarFilter)

**Sorumluluk:** Ilan arama ve filtreleme arayuzunun uygulanmasi.

**Yapilan Isler:**

- **components/Cars/CarFilter.js:**
  - Acilir/kapanir filtre paneli
  - Filtre alanlari:
    - Marka (dropdown, 60+ marka secenegi)
    - Seri (metin girisi)
    - Model (metin girisi)
    - Konum (metin girisi)
    - Minimum fiyat / Maksimum fiyat
  - Filtre uygulama: CarList bilesenine parametre olarak gecirme
  - Filtre sifirlama fonksiyonu

**Ilgili Dosya:**
- `frontend/src/components/Cars/CarFilter.js`

---

### 7. Fiyat Tahmini Sayfasi (PricePredictor)

**Sorumluluk:** Yapay zeka destekli arac fiyat tahmin arayuzunun olusturulmasi.

**Yapilan Isler:**

- **components/Prediction/PricePredictor.js:**
  - **Tahmin Formu (20+ alan):**
    - Ayni alanlar CarForm ile paralel: Marka, Seri, Model, Yil, Kilometre, Renk
    - Teknik: Yakit tipi, Vites tipi, Kasa tipi, Motor hacmi, Motor gucu, Cekis
    - Durum: Arac durumu, Satici tipi, Yakit tuketimi, Yakit deposu
    - Boolean: Agir hasar kaydi, Takasa uygun
    - Acilir/kapanir panel boya bolumu: 13 panel icin durum secimi

  - **Tahmin Istegi:**
    - `predictPrice(data)` API cagrisi
    - Arac verilerini backend'in beklentisine uygun formata donusturme
    - Yukleme durumunda spinner gosterimi

  - **Sonuc Gosterimi:**
    - Tahmini fiyat buyuk font ile gosterim
    - Dogruluk notu: "%92 R2 Score ile tahmin edilmistir"
    - Basarili/basarisiz durum yonetimi

**Ilgili Dosya:**
- `frontend/src/components/Prediction/PricePredictor.js`

---

### 8. Ana Sayfa (HomePage)

**Sorumluluk:** Uygulamanin giris sayfasinin tasarlanmasi ve uygulanmasi.

**Yapilan Isler:**

- **components/Home/HomePage.js:**
  - **Hero Bolumu:**
    - Dikkat cekici baslik ve alt baslik
    - CTA butonlari: "Fiyat Tahmini Yap" ve "Ilanlari Gor"
    - Animasyonlu arka plan efektleri (blob animasyonlari, gradient overlay)

  - **Yapay Zeka Bolumu:**
    - AI fiyat tahmin ozelliginin tanitimi
    - Ozellik ve avantajlarin listelenmesi

  - **Neden Oto Pusula Bolumu:**
    - 3 sutunlu ozellik gridi
    - Yapay Zeka Destegi, Guvenlik, Hiz ozellikleri

  - **Populer Ilanlar Bolumu:**
    - `getCars({ limit: 8 })` ile son 8 ilan cekme
    - CarCard bilesenleri ile grid gosterim
    - "Tum Ilanlari Gor" linki

  - **CTA Bolumu:**
    - "Aracinizi Satmak mi Istiyorsunuz?" mesaji
    - "Ilan Ver" ve "Fiyat Ogren" butonlari

  - **Responsive tasarim:** Tailwind CSS ile mobil uyumlu grid yapilar

**Ilgili Dosya:**
- `frontend/src/components/Home/HomePage.js`

---

### 9. CSS Stil Dosyalari

**Sorumluluk:** Arac modulu ve tahmin sayfasi icin stil tanimlari.

**Yapilan Isler:**

- **components/Cars/Cars.css:**
  - CarList grid yerlesimi
  - CarCard kart tasarimi (golge, hover efektleri)
  - CarDetail sayfa duzeni (galeri, tablo, panel gorsellestirmesi)
  - CarForm form stilleri
  - CarFilter panel stilleri
  - Responsive breakpoint'ler

- **components/Prediction/Prediction.css:**
  - Tahmin formu duzeni
  - Sonuc gosterim alani
  - Panel secim UI stilleri
  - Yukleme animasyonu

**Ilgili Dosyalar:**
- `frontend/src/components/Cars/Cars.css`
- `frontend/src/components/Prediction/Prediction.css`

---

### Kullanilan Teknolojiler

| Teknoloji | Kullanim Alani |
|-----------|----------------|
| React 18 | UI framework |
| React Router v6 | Sayfa yonlendirme |
| Axios | HTTP istemci |
| React Toastify | Bildirim sistemi |
| Tailwind CSS | Stil ve tasarim |
| React Icons | Ikon kutuphanesi |
| FormData API | Dosya yukleme |

---

### Sayfa/Bilesen Ozeti

| Bilesen | Dosya Yolu | Aciklama |
|---------|-----------|----------|
| CarList | `components/Cars/CarList.js` | Ilan listesi + sayfalama |
| CarCard | `components/Cars/CarCard.js` | Tekil ilan karti |
| CarDetail | `components/Cars/CarDetail.js` | Ilan detay sayfasi |
| CarForm | `components/Cars/CarForm.js` | Ilan olusturma/duzenleme |
| CarFilter | `components/Cars/CarFilter.js` | Filtre paneli |
| PricePredictor | `components/Prediction/PricePredictor.js` | AI fiyat tahmini |
| HomePage | `components/Home/HomePage.js` | Ana sayfa |
