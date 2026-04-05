**Front-end Test Videosu:** [Youtube Link](https://youtu.be/sJ1E3SAboy8)

# Mehmet Uludag - Web Frontend Gorevleri

## Modul D: Etkilesim ve Iletisim - Yorum ve Paylasim (Frontend)

---

### 1. Yorum API Servis Katmani

**Sorumluluk:** Yorum islemleri ile ilgili tum API cagrlarinin tanimlanmasi.

**Yapilan Isler:**

- **api/commentApi.js:**
  - `getComments(carId, params)` → GET `/cars/{carId}/comments` - Ilana ait yorumlari getirme (sayfalanmis)
  - `getComment(carId, id)` → GET `/cars/{carId}/comments/{id}` - Tekil yorum getirme
  - `addComment(carId, data)` → POST `/cars/{carId}/comments` - Yeni yorum ekleme
  - `updateComment(carId, id, data)` → PUT `/cars/{carId}/comments/{id}` - Yorum guncelleme
  - `deleteComment(carId, id)` → DELETE `/cars/{carId}/comments/{id}` - Yorum silme
  - Tum istekler axiosInstance uzerinden JWT token ile gonderilir

**Ilgili Dosya:**
- `frontend/src/api/commentApi.js`

---

### 2. Yorum Bolumu Bileseni (CommentSection)

**Sorumluluk:** Ilan detay sayfasindaki yorum alaninin tum islevselliginin uygulanmasi.

**Yapilan Isler:**

- **components/Comments/CommentSection.js:**

  - **Yorum Sayisi Gosterimi:**
    - Toplam yorum sayisi baslik olarak gosterilir
    - API'den donen `totalCount` degeri kullanilir

  - **Yorum Ekleme Formu:**
    - Textarea alani: Yorum metni girisi
    - "Yorum Yap" butonu
    - `addComment(carId, { content })` API cagrisi
    - Sadece oturum acmis kullanicilara gorulur (useAuth kontrolu)
    - Oturum kapali ise "Yorum yapmak icin giris yapin" mesaji
    - Basarili ekleme sonrasi:
      - Yorum listesini yenileme
      - Textarea'yi temizleme
      - Toast bildirimi

  - **Yorum Listesi:**
    - Yorumlarin tarih sirasina gore listelenmesi
    - Her yorum karti:
      - Yorum icerigi (metin)
      - Yorum tarihi (formatlanmis)
      - Kullanici bilgisi
    - `getComments(carId, { limit, offset })` API cagrisi

  - **Yorum Duzenleme:**
    - "Duzenle" butonu: Sadece yorum sahibine gorulur (userId eslesmesi)
    - Duzenleme modunda textarea'ya donusum
    - Mevcut yorum metni textarea'ya yuklenme
    - "Kaydet" ve "Iptal" butonlari
    - `updateComment(carId, commentId, { content })` API cagrisi
    - Basarili guncelleme sonrasi:
      - Duzenleme modundan cikilma
      - Yorum listesini yenileme
      - Toast bildirimi

  - **Yorum Silme:**
    - "Sil" butonu: Sadece yorum sahibine gorulur
    - Onay penceresi (confirm dialog)
    - `deleteComment(carId, commentId)` API cagrisi
    - Basarili silme sonrasi:
      - Yorum listesini yenileme
      - Toast bildirimi

  - **Sayfalama:**
    - Sayfa basina 10 yorum
    - "Onceki" ve "Sonraki" sayfalama butonlari
    - Sayfa degistiginde `getComments` yeniden cagirilir
    - Toplam sayfa sayisi hesaplama (totalCount / limit)

**Ilgili Dosya:**
- `frontend/src/components/Comments/CommentSection.js`

---

### 3. CarDetail Sayfasi ile Entegrasyon

**Sorumluluk:** Yorum bileseninin ilan detay sayfasina entegre edilmesi.

**Yapilan Isler:**

- **CarDetail.js icinde CommentSection kullanimi (Anil Elmaz ile ortak calisma):**
  - CarDetail sayfasinin alt bolumune CommentSection bileseni yerlestirme
  - `carId` prop'u ile ilan ID'sini CommentSection'a gecirme
  - Yorum bolumunun ilan detaylari altinda konumlandirilmasi
  - Yorum yukleme sirasinda loading gosterimi

**Ilgili Dosya:**
- `frontend/src/components/Cars/CarDetail.js` (yorum entegrasyon kismi)

---

### 4. CSS Stil Dosyasi

**Sorumluluk:** Yorum bilesenleri icin stil tanimlari.

**Yapilan Isler:**

- **components/Comments/Comments.css:**
  - Yorum bolumu genel yerlesimi
  - Yorum ekleme formu stilleri (textarea, buton)
  - Tekil yorum karti tasarimi:
    - Yorum metni tipografisi
    - Tarih gosterim formati
    - Kullanici bilgisi alani
  - Duzenleme modu stilleri (aktif textarea, kaydet/iptal butonlari)
  - Aksiyon butonlari (duzenle, sil) hover efektleri
  - Sayfalama kontrol stilleri (onceki/sonraki butonlari)
  - Bos durum mesaji stilleri ("Henuz yorum yapilmamis")
  - Dark mode uyumlulugu
  - Responsive tasarim (mobil uyum)

**Ilgili Dosya:**
- `frontend/src/components/Comments/Comments.css`

---

### 5. Auth Stil Dosyasi

**Sorumluluk:** Kimlik dogrulama sayfalarinin ortak stil tanimlari.

**Yapilan Isler:**

- **components/Auth/Auth.css:**
  - Login, Register, ForgotPassword, ResetPassword, Profile sayfalari icin ortak stiller
  - Form container yerlesimi (merkezleme, genislik)
  - Input alanlari stilleri (border, padding, focus durumu)
  - Buton stilleri (primary, danger, outline)
  - Link stilleri (sifremi unuttum, kayit ol)
  - Hata mesaji gosterim stilleri
  - Dark mode uyumlulugu
  - Responsive tasarim

**Ilgili Dosya:**
- `frontend/src/components/Auth/Auth.css`

---

### Kullanilan Teknolojiler

| Teknoloji | Kullanim Alani |
|-----------|----------------|
| React 18 | UI framework |
| Axios | HTTP istemci |
| React Toastify | Bildirim sistemi |
| Context API (useAuth) | Oturum durumu kontrolu |
| Tailwind CSS + Custom CSS | Stil ve tasarim |
| React Icons | Ikon kutuphanesi |

---

### Sayfa/Bilesen Ozeti

| Bilesen | Dosya Yolu | Aciklama |
|---------|-----------|----------|
| CommentSection | `components/Comments/CommentSection.js` | Yorum CRUD + sayfalama |
| CarDetail (yorum kismi) | `components/Cars/CarDetail.js` | Yorum bileseni entegrasyonu |
| Comments.css | `components/Comments/Comments.css` | Yorum stilleri |
| Auth.css | `components/Auth/Auth.css` | Auth sayfa stilleri |

---

### Bilesen Ozellikleri Tablosu

| Ozellik | Detay |
|---------|-------|
| Yorum Ekleme | Oturum acik kullanicilar icin textarea + buton |
| Yorum Listeleme | Tarih sirasina gore, sayfalanmis (10/sayfa) |
| Yorum Duzenleme | Sadece yorum sahibi, inline editing |
| Yorum Silme | Sadece yorum sahibi, onay dialog'u |
| Sayfalama | Onceki/Sonraki, toplam sayfa gosterimi |
| Yetkilendirme | useAuth ile oturum kontrolu, userId eslesmesi |
