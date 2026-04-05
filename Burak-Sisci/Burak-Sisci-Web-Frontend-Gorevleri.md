# Burak Sisci - Web Frontend Gorevleri

## Modul A: Kullanici ve Kimlik Yonetimi (Frontend)

---

### 1. Axios API Altyapisi ve Interceptor'lar

**Sorumluluk:** Tum frontend-backend iletisiminin merkezi yapilandirmasi.

**Yapilan Isler:**

- **api/axiosInstance.js:**
  - Axios instance olusturma: `baseURL = process.env.REACT_APP_API_URL || 'http://localhost:5078'`
  - **Request Interceptor:** Her HTTP istegine localStorage'daki JWT token'i `Authorization: Bearer {token}` header'i olarak otomatik ekleme
  - **Response Interceptor:** 401 Unauthorized yaniti alindiginda:
    1. localStorage'dan token ve user bilgisini silme
    2. Kullaniciyi `/login` sayfasina yonlendirme (zaten login sayfasindaysa yonlendirme yapilmaz)
  - Tum diger modullerin API cagrilari bu instance uzerinden yapilir

**Ilgili Dosya:**
- `frontend/src/api/axiosInstance.js`

---

### 2. Auth API Servis Katmani

**Sorumluluk:** Kimlik dogrulama ile ilgili tum API cagrlarinin tanimlanmasi.

**Yapilan Isler:**

- **api/authApi.js:**
  - `register(data)` → POST `/auth/register` - Kullanici kaydi
  - `login(data)` → POST `/auth/login` - Kullanici girisi
  - `logout()` → POST `/auth/logout` - Oturum sonlandirma
  - `forgotPassword(data)` → POST `/auth/forgot-password` - Sifre sifirlama talebi
  - `resetPassword(data)` → POST `/auth/reset-password` - Yeni sifre belirleme
  - `updateProfile(data)` → PUT `/auth/profile` - Profil guncelleme
  - `deleteAccount(id)` → DELETE `/auth/{id}` - Hesap silme

**Ilgili Dosya:**
- `frontend/src/api/authApi.js`

---

### 3. Auth Context (Global State Yonetimi)

**Sorumluluk:** Kullanici oturum durumunun tum uygulama genelinde yonetilmesi.

**Yapilan Isler:**

- **context/AuthContext.js:**
  - `useAuth()` custom hook'u ile tum bilesenlerde erisim
  - State degiskenleri: `user` (id, email), `token`, `loading`
  - `loginUser(token)` metodu:
    1. JWT token'i decode etme (jwtHelper ile)
    2. User bilgisini (id, email) cikartma
    3. localStorage'a token ve user kaydetme
    4. State guncelleme
  - `logoutUser()` metodu:
    1. localStorage temizleme (token, user)
    2. State sifirlama
  - Sayfa yenilendiginde localStorage'dan oturum geri yukleme
  - `loading` state'i ile oturum kontrolu tamamlanana kadar bekleme

- **context/jwtHelper.js:**
  - `jwtDecode(token)` fonksiyonu
  - Base64 decode ile JWT payload cikartma
  - Claims (userId, email) elde etme

**Ilgili Dosyalar:**
- `frontend/src/context/AuthContext.js`
- `frontend/src/context/jwtHelper.js`

---

### 4. Kullanici Kayit Sayfasi (RegisterForm)

**Sorumluluk:** Yeni kullanici kayit formunun olusturulmasi.

**Yapilan Isler:**

- **components/Auth/RegisterForm.js:**
  - Form alanlari: Email, Telefon, Sifre (minimum 6 karakter)
  - Form validasyonu (bos alan kontrolu, sifre uzunlugu)
  - `register()` API cagrisi
  - Basarili kayit sonrasi toast bildirimi ve `/login` sayfasina yonlendirme
  - Hata durumunda toast ile kullaniciya bilgilendirme
  - Login sayfasina link ("Zaten hesabiniz var mi?")

**Ilgili Dosya:**
- `frontend/src/components/Auth/RegisterForm.js`

---

### 5. Kullanici Giris Sayfasi (LoginForm)

**Sorumluluk:** Kullanici giris formunun olusturulmasi.

**Yapilan Isler:**

- **components/Auth/LoginForm.js:**
  - Form alanlari: Email, Sifre
  - `login()` API cagrisi
  - Basarili giriste:
    1. Token'i AuthContext'e kaydetme (`loginUser(token)`)
    2. Ana sayfaya yonlendirme
  - Hata durumunda toast bildirimi
  - "Sifremi Unuttum" linki (`/forgot-password`)
  - "Kayit Ol" linki (`/register`)

**Ilgili Dosya:**
- `frontend/src/components/Auth/LoginForm.js`

---

### 6. Sifremi Unuttum Sayfasi (ForgotPassword)

**Sorumluluk:** Sifre sifirlama talebinin gonderilmesi.

**Yapilan Isler:**

- **components/Auth/ForgotPassword.js:**
  - Form alani: Email
  - `forgotPassword()` API cagrisi
  - Basarili istek sonrasi bilgilendirme mesaji
  - Login sayfasina geri donus linki

**Ilgili Dosya:**
- `frontend/src/components/Auth/ForgotPassword.js`

---

### 7. Sifre Sifirlama Sayfasi (ResetPassword)

**Sorumluluk:** Yeni sifre belirleme formunun olusturulmasi.

**Yapilan Isler:**

- **components/Auth/ResetPassword.js:**
  - URL parametrelerinden token ve email bilgisi alma
  - Form alani: Yeni Sifre
  - `resetPassword()` API cagrisi (token + yeni sifre)
  - Basarili sifirlama sonrasi login sayfasina yonlendirme
  - Hata durumunda bilgilendirme

**Ilgili Dosya:**
- `frontend/src/components/Auth/ResetPassword.js`

---

### 8. Profil Sayfasi (Profile)

**Sorumluluk:** Kullanici hesap yonetim sayfasinin olusturulmasi.

**Yapilan Isler:**

- **components/Auth/Profile.js:**
  - Kullanici email bilgisini gosterme
  - Telefon numarasi guncelleme formu
    - `updateProfile()` API cagrisi
    - Basarili guncelleme sonrasi toast bildirimi
  - Hesap silme fonksiyonu:
    - Onay penceresi (confirm dialog)
    - `deleteAccount(userId)` API cagrisi
    - Basarili silme sonrasi oturumu kapatma ve login'e yonlendirme
  - Cikis yapma butonu:
    - `logout()` API cagrisi
    - AuthContext uzerinden `logoutUser()` calistirma
    - Ana sayfaya yonlendirme

**Ilgili Dosya:**
- `frontend/src/components/Auth/Profile.js`

---

### 9. Korunmus Rota Bileseni (ProtectedRoute)

**Sorumluluk:** Yetkilendirme gerektiren sayfalarin korunmasi.

**Yapilan Isler:**

- **components/ProtectedRoute.js:**
  - `useAuth()` hook'u ile oturum durumu kontrolu
  - `loading` durumunda yukleme animasyonu gosterme
  - Oturum acik degilse `/login` sayfasina otomatik yonlendirme
  - Oturum aciksa cocuk bileseni render etme
  - Korunan rotalar: `/cars/new`, `/cars/:id/edit`, `/profile`, `/lists`

**Ilgili Dosya:**
- `frontend/src/components/ProtectedRoute.js`

---

### 10. Navbar - Kimlik Dogrulama Entegrasyonu

**Sorumluluk:** Navigasyon cubugundaki oturum durumuna bagli ogelerin yonetimi.

**Yapilan Isler:**

- **components/Layout/Navbar.js (Auth ile ilgili kisimlar):**
  - `useAuth()` hook'u ile oturum durumu okuma
  - Oturum acikken gorunen ogeler: Profilim, Listelerim, Cikis Yap
  - Oturum kapali iken gorunen ogeler: Giris Yap
  - Hizli cikis butonu:
    - `logout()` API cagrisi
    - `logoutUser()` ile state temizleme
    - Ana sayfaya yonlendirme
    - Hata durumunda bile state temizleme (guvenli cikis)
  - Mobil menu'de de ayni kosullu gorunum

**Ilgili Dosya:**
- `frontend/src/components/Layout/Navbar.js`

---

### 11. Tema Yonetimi (ThemeContext)

**Sorumluluk:** Karanlik/aydinlik tema gecis altyapisinin kurulmasi.

**Yapilan Isler:**

- **context/ThemeContext.js:**
  - `useTheme()` custom hook'u
  - State: `dark` (boolean)
  - `toggleTheme()` metodu: tema degistirme
  - localStorage'da tema tercihi saklama
  - Document root'a `"dark"` class'i ekleme/cikarma
  - Sayfa yenilendiginde tercih geri yukleme

- **Navbar.js tema butonu:**
  - Gunes/Ay ikonu ile tema gecis butonu
  - `toggleTheme()` cagirarak tema degistirme

**Ilgili Dosyalar:**
- `frontend/src/context/ThemeContext.js`
- `frontend/src/components/Layout/Navbar.js`

---

### 12. Uygulama Routing Yapisi

**Sorumluluk:** React Router ile sayfa yonlendirmelerinin tanimlanmasi.

**Yapilan Isler:**

- **App.js:**
  - React Router v6 yapilandirmasi
  - Public rotalar: `/`, `/cars`, `/login`, `/register`, `/forgot-password`, `/reset-password`, `/cars/:id`, `/prediction`
  - Korunmus rotalar (ProtectedRoute ile sarili): `/cars/new`, `/cars/:id/edit`, `/profile`, `/lists`
  - AuthProvider ve ThemeProvider ile uygulama sarilmasi
  - ToastContainer entegrasyonu (React Toastify)

**Ilgili Dosya:**
- `frontend/src/App.js`

---

### Kullanilan Teknolojiler

| Teknoloji | Kullanim Alani |
|-----------|----------------|
| React 18 | UI framework |
| React Router v6 | Sayfa yonlendirme |
| Axios | HTTP istemci |
| Context API | Global state yonetimi |
| React Toastify | Bildirim sistemi |
| localStorage | Oturum ve tema persistance |
| Tailwind CSS | Stil ve tasarim |

---

### Sayfa/Bilesen Ozeti

| Bilesen | Dosya Yolu | Aciklama |
|---------|-----------|----------|
| LoginForm | `components/Auth/LoginForm.js` | Giris formu |
| RegisterForm | `components/Auth/RegisterForm.js` | Kayit formu |
| ForgotPassword | `components/Auth/ForgotPassword.js` | Sifre sifirlama talebi |
| ResetPassword | `components/Auth/ResetPassword.js` | Yeni sifre belirleme |
| Profile | `components/Auth/Profile.js` | Profil yonetimi |
| ProtectedRoute | `components/ProtectedRoute.js` | Rota koruma |
| Navbar | `components/Layout/Navbar.js` | Auth durumlu navigasyon |
| AuthContext | `context/AuthContext.js` | Oturum state yonetimi |
| ThemeContext | `context/ThemeContext.js` | Tema state yonetimi |
