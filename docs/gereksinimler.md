# Detaylı Gereksinim Listesi


#### **Üye 1: Veri Bilimi ve Yapay Zeka (AI & Data Scraper)**

1. **Veri Kazıma (Scraping):** Sahibinden.com (veya alternatif kaynaklardan) marka, model, yıl, km, fiyat, hasar durumu verilerinin otomatik çekilmesi.
2. **Veri Temizleme (Preprocessing):** Çekilen verilerdeki hatalı, boş veya tutarsız (örn: 2025 model Tofaş) verilerin temizlenerek analize hazır hale getirilmesi.
3. **Model Eğitimi (Training):** Temizlenen veri seti ile Regresyon algoritmaları (Random Forest/XGBoost) kullanılarak fiyat tahmin modelinin oluşturulması.
4. **Tahmin API'sı:** Eğitilen modelin, dışarıdan gelen araç özelliklerine (JSON formatında) anlık fiyat tahmini üretecek bir servise dönüştürülmesi.
5. **Değer Kaybı Analizi:** Aracın değişen ve boyalı parça sayısına göre (Tramer) ortalama değer kaybının hesaplanması modülü.
6. **Sürekli Öğrenme (Retraining):** Sistemin güncel kalması için belirli aralıklarla yeni ilan verileriyle modelin tekrar eğitilmesi senaryosu.


#### **Üye 2: Backend ve Veritabanı (Server Side)**

1. **Kimlik Doğrulama (Auth):** Kullanıcı kayıt (Register), Giriş (Login) ve Şifremi Unuttum süreçlerinin JWT (Json Web Token) ile güvenli yönetimi.
2. **Veritabanı Tasarımı:** Araçlar, Kullanıcılar, İlanlar ve Favoriler tablolarının ilişkisel veritabanında (PostgreSQL/MySQL) oluşturulması.
3. **İlan Yönetimi API:** Yeni ilan ekleme (Create), düzenleme (Update), silme (Delete) ve listeleme (Read) servislerinin (CRUD) yazılması.
4. **Görsel Depolama:** Kullanıcıların yüklediği araç fotoğraflarının sunucuda veya bulutta (AWS S3/Firebase vb.) optimize edilerek saklanması.
5. **Arama ve Filtreleme Motoru:** Marka, model, fiyat aralığı, il, ilçe gibi kriterlere göre gelişmiş sorgu servislerinin hazırlanması.
6. **İletişim Altyapısı:** Alıcı ve satıcı arasında mesajlaşma veya "Satıcıyı Ara" butonunun tetiklediği iletişim bilgisini getiren servis.


#### **Üye 3: Frontend Web (Web Arayüzü)**

1. **Responsive Dashboard:** Ana sayfanın hem masaüstü hem tablet uyumlu olması; "Vitrin İlanları" ve "Son Eklenenler" alanlarının tasarımı.
2. **İlan Detay Sayfası:** Seçilen aracın teknik özelliklerinin, fotoğraflarının (slider/galeri şeklinde) ve fiyat tahmin grafiğinin gösterilmesi.
3. **İlan Verme Sihirbazı:** Kullanıcının adım adım (Araba Seç -> Özellik Gir -> Fotoğraf Yükle -> Fiyatı Gör) ilan verebileceği kullanıcı dostu form yapısı.
4. **Favoriler Yönetimi:** Kullanıcının beğendiği ilanları "Kalp" ikonu ile favorilerine ekleyip, profil sayfasından yönetebilmesi.
5. **Sosyal Paylaşım:** İlan detay sayfasında "Linki Kopyala" veya "WhatsApp'ta Paylaş" butonlarının entegrasyonu.
6. **Kullanıcı Paneli:** Kullanıcının kendi verdiği ilanları görüp durumunu (Yayında/Satıldı) değiştirebileceği profil sayfası.


#### **Üye 4: Mobil Uygulama (Mobile App)**

1. **Kamera Entegrasyonu:** İlan verirken araç fotoğraflarının doğrudan telefon kamerasından çekilip uygulamaya yüklenebilmesi.
2. **Konum Servisleri:** "Yakınımdaki İlanlar" özelliği için telefonun GPS verisini kullanarak harita üzerinde araçları gösterme.
3. **Ekspertiz Seçimi (UI):** Aracın boyalı/değişen parçalarının (Kaput, Sağ Çamurluk vb.) interaktif bir araç şeması üzerinde dokunarak seçilmesi.
4. **Bildirim Sistemi (Push Notification):** Favoriye alınan bir aracın fiyatı düştüğünde veya satıldığında kullanıcıya mobil bildirim gönderilmesi.
5. **Hızlı Arama (Voice/Search):** Mobil arayüzde kullanıcı deneyimini artıracak sesli arama veya hızlı filtreleme butonları.
6. **Satıcı ile Hızlı Etkileşim:** İlan detayında tek tıkla "WhatsApp ile Mesaj At" veya "Hemen Ara" butonlarının mobil aksiyonları.