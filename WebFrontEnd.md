# Web Frontend Görev Dağılımı

**Web Frontend Adresi:** [a2m2ekibiotopusulauygulamasi](https://a2m2ekibiotopusulauygulamasi-production.up.railway.app/)

Bu dokümanda, web uygulamasının kullanıcı arayüzü (UI) ve kullanıcı deneyimi (UX) görevleri listelenmektedir. Her grup üyesi, kendisine atanan sayfaların tasarımı, implementasyonu ve kullanıcı etkileşimlerinden sorumludur.

---

## Grup Üyelerinin Web Frontend Görevleri

1. [Burak Şişci'ın Web Frontend Görevleri](Burak-Sisci/Burak-Sisci-Web-Frontend-Gorevleri.md)
2. [Mehmet Öz'üm Web Frontend Görevleri](Mehmet-Oz/Mehmet-Oz-Web-Frontend-Gorevleri.md)
3. [Anıl Elmaz'ın Web Frontend Görevleri](Anil-Elmaz/Anil-Elmaz-Web-Frontend-Gorevleri.md)
4. [Mehmet Uludağ'ın Web Frontend Görevleri](Mehmet-Uludag/Mehmet-Uludag-Web-Frontend-Gorevleri.md)

---

## Genel Web Frontend Prensipleri

### 1. Responsive Tasarim
- **Mobile-First Approach:** Tailwind CSS mobile-first breakpoint sistemi kullanildi
- **Breakpoints:**
  - Mobile: < 640px (varsayilan)
  - SM (Tablet): 640px
  - MD: 768px
  - LG (Desktop): 1024px
  - XL: 1280px
- **Flexible Layouts:** CSS Grid ve Flexbox (Tailwind utility class'lari ile)
- **Responsive Grid Ornekleri:**
  - Arac listesi: `grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4`
  - Form alanlari: `grid-cols-1 md:grid-cols-2`
  - Anasayfa ozellikler: `grid-cols-1 md:grid-cols-3`
- **Touch-Friendly:** Buton ve linklerde yeterli tiklama alani

### 2. Tasarim Sistemi
- **CSS Framework:** Tailwind CSS 3.4
- **Renk Paleti:**
  - Primary: Blue (50-950 skalasi, CSS variables ile)
  - Accent: Amber / Orange
  - Dark Mode: Slate tonlari
- **Tipografi:** Inter font ailesi (Google Fonts), sistem fontlari fallback
- **Spacing:** Tailwind'in varsayilan 4px grid sistemi (p-2, p-4, m-8 vb.)
- **Iconography:** React Icons kutuphanesi (Font Awesome, Remix Icon setleri)
- **Component Library:** Proje icin ozel gelistirilmis React component'leri

### 3. Performans Optimizasyonu
- **Code Splitting:** React Router ile route-based splitting
- **Lazy Loading:** Sayfa bazli component yukleme
- **Minification:** react-scripts build ile otomatik CSS ve JavaScript minification
- **Compression:** Nginx (production) uzerinden Gzip compression
- **Bundle Size:** Tree shaking (Webpack, react-scripts ile otomatik)

### 4. SEO (Search Engine Optimization)
- **Meta Tags:** Title ve description `public/index.html` icinde tanimli
- **Semantic HTML:** Header, main, footer, section, article gibi HTML5 elementleri
- **Alt Text:** Arac resimlerinde alt attribute kullanimi

### 5. Erisilebirlik (Accessibility)
- **Keyboard Navigation:** Form alanlari ve butonlar tab ile gezinilebilir
- **Focus Indicators:** Tailwind `focus:ring` ile gorunen focus durumlari
- **Color Contrast:** Dark ve light modda yeterli kontrast oranlari
- **ARIA Labels:** Form alanlari ve interaktif elementlerde label kullanimi

### 6. Browser Compatibility
- **Modern Browsers:** Chrome, Firefox, Safari, Edge (son 2 versiyon)
- **CSS Prefixes:** PostCSS Autoprefixer ile otomatik
- **Browserslist Konfigurasyonu:**
  - Production: `>0.2%`, `not dead`, `not op_mini all`
  - Development: Son Chrome, Firefox, Safari versiyonlari

---

## Teknoloji Stack

| Teknoloji | Versiyon | Kullanim Alani |
|-----------|----------|----------------|
| React | 19.2.4 | UI Framework |
| React Router | 7.13.2 | Client-Side Routing |
| Axios | 1.13.6 | HTTP Client (API istekleri) |
| Tailwind CSS | 3.4.19 | Utility-First CSS Framework |
| React Toastify | 11.0.5 | Toast Bildirimleri |
| React Icons | 5.6.0 | Icon Kutuphanesi |
| PostCSS | 8.5.8 | CSS Isleme |
| Autoprefixer | 10.4.27 | CSS Prefix Otomasyonu |

---

## Proje Yapisi

```
frontend/src/
├── api/                        # API entegrasyon katmani
│   ├── axiosInstance.js        # Axios konfigurasyonu + interceptor'lar
│   ├── authApi.js              # Kimlik dogrulama endpoint'leri
│   ├── carApi.js               # Arac CRUD + resim yukleme
│   ├── commentApi.js           # Yorum islemleri
│   ├── listApi.js              # Kullanici listeleri
│   └── predictionApi.js        # ML fiyat tahmini
├── components/                 # React Component'leri
│   ├── Auth/                   # Kimlik dogrulama sayfalari
│   │   ├── LoginForm.js        # Giris formu
│   │   ├── RegisterForm.js     # Kayit formu
│   │   ├── ForgotPassword.js   # Sifremi unuttum
│   │   ├── ResetPassword.js    # Sifre sifirlama
│   │   └── Profile.js          # Profil duzenleme
│   ├── Cars/                   # Arac ilan yonetimi
│   │   ├── CarList.js          # Sayfalamali ilan listesi
│   │   ├── CarCard.js          # Ilan onizleme karti
│   │   ├── CarDetail.js        # Ilan detay sayfasi
│   │   ├── CarForm.js          # Ilan olusturma/duzenleme formu
│   │   └── CarFilter.js        # Gelismis filtre kontrolleri
│   ├── Home/
│   │   └── HomePage.js         # Anasayfa (hero + ozellikler + populer ilanlar)
│   ├── Layout/
│   │   ├── Navbar.js           # Navigasyon (responsive, tema degistirme)
│   │   └── Footer.js           # Alt bilgi
│   ├── Comments/
│   │   └── CommentSection.js   # Yorum bolumu
│   ├── Lists/
│   │   └── UserLists.js        # Favori listeleri
│   ├── Prediction/
│   │   └── PricePredictor.js   # ML fiyat tahmin formu
│   └── ProtectedRoute.js      # Kimlik dogrulama guard'i
├── context/                    # React Context API
│   ├── AuthContext.js          # Kullanici oturum durumu
│   ├── ThemeContext.js         # Karanlik/aydinlik mod
│   └── jwtHelper.js           # JWT cozumleme yardimcisi
├── constants/
│   └── carBrands.js            # Araba marka listesi (70+ marka)
├── App.js                      # Ana routing componenti
└── index.js                    # Uygulama giris noktasi
```

---

## Routing (Sayfa Yonlendirme)

Client-side routing icin **React Router 7** kullanilmaktadir.

| Yol | Component | Korumali | Aciklama |
|-----|-----------|----------|----------|
| `/` | HomePage | Hayir | Anasayfa |
| `/cars` | CarList | Hayir | Arac ilan listesi |
| `/cars/:id` | CarDetail | Hayir | Ilan detay sayfasi |
| `/cars/new` | CarForm | Evet | Yeni ilan olusturma |
| `/cars/edit/:id` | CarForm | Evet | Ilan duzenleme |
| `/prediction` | PricePredictor | Hayir | Fiyat tahmini |
| `/login` | LoginForm | Hayir | Giris |
| `/register` | RegisterForm | Hayir | Kayit |
| `/forgot-password` | ForgotPassword | Hayir | Sifremi unuttum |
| `/reset-password` | ResetPassword | Hayir | Sifre sifirlama |
| `/profile` | Profile | Evet | Profil duzenleme |
| `/lists` | UserLists | Evet | Favori listeleri |

**Korumali Rotalar:** `ProtectedRoute` component'i ile sarilmistir. Kullanici giris yapmamissa `/login` sayfasina yonlendirilir.

---

## State Management (Durum Yonetimi)

### 7.1 Global State - React Context API

Proje genelinde iki Context kullanilmaktadir:

**AuthContext (Kimlik Dogrulama Durumu):**
- `user` - Aktif kullanici bilgisi (id, email)
- `token` - JWT token
- `loading` - Yukleme durumu
- `loginUser(token)` - Giris islemi
- `logoutUser()` - Cikis islemi
- Token localStorage'da saklanir
- Uygulama baslatildiginda token gecerliligi kontrol edilir
- .NET JWT formatindaki uzun claim URI'leri desteklenir

**ThemeContext (Tema Durumu):**
- `dark` - Karanlik mod aktif mi (boolean)
- `toggleTheme()` - Tema degistirme
- Tercih localStorage'da saklanir
- Sistem tercihine (`prefers-color-scheme`) fallback yapar
- HTML root elementine `dark` class'i ekler/kaldirir

### 7.2 Local State - React Hooks

- Component bazli state icin `useState` hook'u
- Yan etkiler icin `useEffect` hook'u
- Form state'i tek bir nesne olarak yonetilir (`{ field1, field2, ... }`)

---

## API Entegrasyonu

### 8.1 HTTP Client - Axios

Merkezi Axios instance'i (`axiosInstance.js`) kullanilmaktadir:

- **Base URL:** `REACT_APP_API_URL` ortam degiskeni veya `http://localhost:5078`
- **Request Interceptor:** Her istege `Authorization: Bearer <token>` header'i eklenir
- **Response Interceptor:** 401 hatasi alindiginda token temizlenir ve `/login` sayfasina yonlendirilir

### 8.2 API Modulleri

| Modul | Dosya | Endpoint'ler |
|-------|-------|-------------|
| Auth | `authApi.js` | register, login, logout, forgot-password, reset-password, profile |
| Cars | `carApi.js` | GET/POST/PUT/DELETE /cars, POST /api/upload/image |
| Comments | `commentApi.js` | GET/POST/PUT/DELETE /cars/{carId}/comments |
| Lists | `listApi.js` | GET/POST/DELETE /lists, PUT /lists/{id}/items |
| Prediction | `predictionApi.js` | POST /api/prediction/predict |

### 8.3 Hata Yonetimi

- `try/catch` bloklari ile API hatalari yakalanir
- `react-toastify` ile kullaniciya bildirim gosterilir:
  - `toast.error()` - Hata mesajlari
  - `toast.success()` - Basari mesajlari
  - `toast.warn()` - Uyari mesajlari (ornegin ML modeli aktif degilse)

---

## Dark Mode (Karanlik Mod)

Tailwind CSS `class` stratejisi ile uygulanmistir:

- **Tetikleme:** Navbar'daki gunes/ay ikonu ile
- **Uygulama:** `document.documentElement` uzerine `dark` class'i eklenir
- **Tailwind Kullanimi:** `dark:` prefix'i ile
  - Arka plan: `bg-white dark:bg-slate-900`
  - Metin: `text-slate-900 dark:text-white`
  - Kenarliklar: `border-slate-200 dark:border-slate-700`
- **Kalicilik:** localStorage'da saklanir, sayfa yenilendiginde korunur

---

## Component Detaylari

### Anasayfa (HomePage)
- Hero bolumu: Arka plan gorseli + gradient metin + CTA butonlari
- Fiyat tahmini ozellik tanitimi
- "Neden OtoPusula?" bolumu (3 kolonlu grid)
- Populer ilanlar (8 arac, responsive grid)
- Alt CTA bolumu

### Arac Listesi (CarList)
- Sayfalama: Sayfa basina 12 ilan
- Filtreleme: Marka, seri, fiyat araligi, konum vb.
- Yukleme ve bos durum gosterimleri
- Responsive grid yapisi

### Arac Detay (CarDetail)
- Resim galerisi (thumbnail navigasyonu ile)
- 10 ogelik ozellik gridi (ikonlarla)
- Detay tablosu
- Boya/degisen panel durumu (13 parca, renk kodlamasi ile)
- Favori butonu + liste secim modali
- Sahip icin duzenleme/silme butonlari
- Yorum bolumu

### Arac Formu (CarForm)
- 6 form bolumu: temel bilgiler, teknik ozellikler, durum, resimler, boya/degisen
- 20+ form alani
- 13 parca icin panel durumu takibi (Orijinal/Boyali/Degismis)
- Resim yukleme (dosya + URL)
- Olusturma ve duzenleme modlari

### Fiyat Tahmini (PricePredictor)
- 14+ form alani
- Acilir/kapanir boya-degisen panelleri (13 parca)
- ML modeli sonucu gosterimi
- Model aktif degilse uyari mesaji (503 hatasi)

### Navbar
- Desktop: Yatay menu (ikon + metin)
- Mobil: Hamburger menu, dikey acilir liste
- Tema degistirme butonu (gunes/ay ikonu)
- Giris durumuna gore kosullu menu ogeleri

---

## Build ve Deployment

### Build Araci
- **react-scripts 5.0.1** (Create React App)
- Webpack ile otomatik bundling, minification, tree shaking

### Build Komutlari

| Komut | Aciklama |
|-------|----------|
| `npm start` | Gelistirme sunucusu (port 3000) |
| `npm run build` | Production build olusturma |
| `npm test` | Testleri calistirma |

### Ortam Degiskenleri
- `.env` dosyasi ile yonetilir
- `REACT_APP_API_URL` - Backend API adresi
- React build sirasinda degiskenler koda gomulur (bake edilir)

### Deployment
- **Platform:** Railway
- **Docker:** Multi-stage build (Node.js build + Nginx serve)
- **Web Server:** Nginx (SPA routing destegi ile)
- **CI/CD:** GitHub push ile otomatik Railway deploy

### Nginx Konfigurasyonu
- SPA routing: Tum yollar `index.html`'e yonlendirilir (`try_files $uri $uri/ /index.html`)
- Port 80 uzerinden serve edilir

---

## Test

| Arac | Kullanim |
|------|----------|
| Jest | Test runner |
| React Testing Library | Component testleri |
| @testing-library/user-event | Kullanici etkilesim simulasyonu |
