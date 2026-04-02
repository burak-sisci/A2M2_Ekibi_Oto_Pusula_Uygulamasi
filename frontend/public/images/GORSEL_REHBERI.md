# Oto Pusula - Gorsel Rehberi

Bu klasore asagidaki gorselleri ekleyin. Tum gorseller yuksek cozunurluklu (min 1920px genislik) olmalidir.

## Gerekli Gorseller

### 1. hero-bg.jpg
- **Konum:** Anasayfa hero arka plani
- **Boyut:** 1920x1080 veya daha buyuk
- **Icerik:** Yuksek kaliteli bir otomobil fotografi (tercihen koyu/dramatik tonlarda)
- **Dosya:** `HomePage.js` icindeki hero section `backgroundImage` satirini aktif edin

### 2. hero-car.png
- **Konum:** Anasayfa hero sag taraf (sadece buyuk ekranlarda gorunur)
- **Boyut:** 1000x800 (transparan PNG)
- **Icerik:** Seffaf arkaplani olan bir araba gorseli
- **Dosya:** `HomePage.js` icindeki hero image placeholder alanina `<img>` ekleyin

### 3. prediction-mockup.png
- **Konum:** Anasayfa fiyat tahmini bolumu (sag taraf)
- **Boyut:** 800x600
- **Icerik:** Fiyat tahmini sayfasinin ekran goruntusu veya mockup
- **Dosya:** `HomePage.js` icindeki prediction section image placeholder

### 4. cta-bg.jpg
- **Konum:** Anasayfa alt CTA bolumu arka plani
- **Boyut:** 1920x600
- **Icerik:** Genis acili otomobil veya yol fotografi
- **Dosya:** `HomePage.js` icindeki CTA section `backgroundImage` satirini aktif edin

## Gorsel Ekleme Talimatları

`HomePage.js` dosyasindaki ilgili `/* TODO */` yorumlarini bulun ve:

1. `backgroundImage` satirlarindaki yorumu kaldiriniz
2. `img-placeholder` div'lerini `<img>` etiketi ile degistiriniz

Ornek:
```jsx
// Oncesi:
style={{ background: 'linear-gradient(...)' }}

// Sonrasi:
style={{ backgroundImage: "url('/images/hero-bg.jpg')" }}
```
