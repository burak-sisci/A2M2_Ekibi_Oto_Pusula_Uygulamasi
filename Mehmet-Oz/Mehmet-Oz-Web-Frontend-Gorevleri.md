**Front-end Test Videosu:** [Youtube Link](https://youtu.be/t2m7DfLJ6tI)

# Mehmet Oz - Web Frontend Gorevleri

## Modul C: Ozel Listeler ve Favoriler (Frontend)

---

### 1. Liste API Servis Katmani

**Sorumluluk:** Kullanici listeleri ile ilgili tum API cagrlarinin tanimlanmasi.

**Yapilan Isler:**

- **api/listApi.js:**
  - `getLists()` → GET `/lists` - Kullanicinin tum listelerini getirme
  - `createList(data)` → POST `/lists` - Yeni liste olusturma
  - `addItemToList(id, data)` → PUT `/lists/{id}/items` - Listeye arac ekleme
  - `deleteList(id)` → DELETE `/lists/{id}` - Liste silme
  - Tum istekler axiosInstance uzerinden JWT token ile gonderilir

**Ilgili Dosya:**
- `frontend/src/api/listApi.js`

---

### 2. Kullanici Listeleri Sayfasi (UserLists)

**Sorumluluk:** Kullanicinin tum listelerinin goruntulenmesi ve yonetilmesi.

**Yapilan Isler:**

- **components/Lists/UserLists.js:**

  - **Yeni Liste Olusturma Formu:**
    - Metin girisi: Liste adi
    - "Olustur" butonu
    - `createList({ listName })` API cagrisi
    - Basarili olusturma sonrasi liste yenileme
    - Toast bildirimi

  - **Liste Kartlari Gorunumu:**
    - Her liste icin kart: Liste adi + arac sayisi
    - Genisleyebilir (expandable) kart yapisi
    - Tiklandiginda listenin icindeki araclari gosterme/gizleme

  - **Liste Icerigi Goruntuleme:**
    - Acilan listenin icindeki arac ID'leri
    - Her arac icin: Arac adi, fiyat (Turkce formatta), detay linki (`/cars/{id}`)
    - Bos liste durumunda bilgilendirme mesaji

  - **Liste Silme:**
    - Her kart uzerinde silme butonu
    - `deleteList(id)` API cagrisi
    - Silme sonrasi liste yenileme
    - Toast bildirimi

  - **Yukleme ve Hata Durumlari:**
    - Veriler yuklenirken loading gosterimi
    - API hata durumlarinda toast ile bilgilendirme
    - Bos liste durumunda "Henuz listeniz yok" mesaji

**Ilgili Dosya:**
- `frontend/src/components/Lists/UserLists.js`

---

### 3. Favoriye Ekleme Entegrasyonu (CarDetail icinde)

**Sorumluluk:** Ilan detay sayfasindan listeye arac ekleme arayuzunun uygulanmasi.

**Yapilan Isler:**

- **CarDetail.js icindeki liste entegrasyonu (Anil Elmaz ile ortak calisma):**
  - Favori butonu: Kalp ikonu ile tiklanabilir buton
  - Modal penceresi:
    - Kullanicinin mevcut listelerini gosterme (`getLists()` cagrisi)
    - Liste secimi dropdown/radio button
    - "Ekle" butonu ile secilen listeye arac ekleme
    - `addItemToList(listId, { carId })` API cagrisi
  - Oturum acik degilse favori butonu gizleme
  - Basarili ekleme sonrasi toast bildirimi

**Ilgili Dosya:**
- `frontend/src/components/Cars/CarDetail.js` (liste ile ilgili kisimlar)

---

### 4. Navbar - Listelerim Linki

**Sorumluluk:** Navigasyon cubugunda "Listelerim" sayfasina erisim saglanmasi.

**Yapilan Isler:**

- **components/Layout/Navbar.js (Liste ile ilgili kisimlar):**
  - "Listelerim" menu ogesi: Sadece oturum acikken gorulur
  - `/lists` rotasina yonlendirme
  - Hem masaustu hem mobil menude mevcut

**Ilgili Dosya:**
- `frontend/src/components/Layout/Navbar.js`

---

### 5. Footer Bileseni

**Sorumluluk:** Uygulama alt bilgi alaninin tasarlanmasi ve uygulanmasi.

**Yapilan Isler:**

- **components/Layout/Footer.js:**
  - **4 Sutunlu Yerlesim:**
    - **Marka Bolumu:** "Oto Pusula" logosu ve kisaca platform tanitimi
    - **Sosyal Medya Linkleri:** Facebook, Twitter, Instagram ikonlari
    - **Hizli Linkler:** Ana Sayfa, Ilanlar, Fiyat Tahmini, Ilan Ver
    - **Yardim Bolumu:** SSS, Kullanim Sartlari, Gizlilik Politikasi
    - **Iletisim Bolumu:** Adres, Telefon, E-posta
  - **Alt Kisim:**
    - Copyright metni
    - Platform aciklama yazisi
  - Responsive tasarim (mobil icin tek sutun, masaustu icin 4 sutun)

**Ilgili Dosya:**
- `frontend/src/components/Layout/Footer.js`

---

### 6. CSS Stil Dosyasi

**Sorumluluk:** Listeler modulu icin stil tanimlari.

**Yapilan Isler:**

- **components/Lists/Lists.css:**
  - Liste olusturma formu stilleri
  - Liste karti tasarimi (genisleyebilir kartlar)
  - Kart ici arac listesi duzeni
  - Silme butonu stilleri
  - Bos durum mesaji stilleri
  - Modal stilleri (favori ekleme)
  - Responsive breakpoint'ler
  - Dark mode uyumlulugu

**Ilgili Dosya:**
- `frontend/src/components/Lists/Lists.css`

---

### Kullanilan Teknolojiler

| Teknoloji | Kullanim Alani |
|-----------|----------------|
| React 18 | UI framework |
| React Router v6 | Sayfa yonlendirme |
| Axios | HTTP istemci |
| React Toastify | Bildirim sistemi |
| Tailwind CSS + Custom CSS | Stil ve tasarim |
| React Icons | Ikon kutuphanesi |

---

### Sayfa/Bilesen Ozeti

| Bilesen | Dosya Yolu | Aciklama |
|---------|-----------|----------|
| UserLists | `components/Lists/UserLists.js` | Liste yonetim sayfasi |
| Footer | `components/Layout/Footer.js` | Alt bilgi bileseni |
| CarDetail (liste kismi) | `components/Cars/CarDetail.js` | Favori ekleme modali |
| Navbar (liste linki) | `components/Layout/Navbar.js` | Listelerim navigasyonu |
