# 🚗 Car Market API

Bu proje, ekiplerin birbirini engellemeden bağımsız çalışabilmesi için **Modüler Monolit (Modular Monolith)** mimarisine uygun olarak tasarlanmış bir Araç Satış Platformu API'sıdır. 

Proje temelinde Clean Architecture prensipleri (Domain, Application, Infrastructure, Presentation) benimsenmiştir. Veri erişim katmanında **MongoDB**, kimlik doğrulama süreçlerinde **JWT** ve token yönetimi için **Redis** entegrasyonları kullanılacaktır ancak herhangi bir kurulum ve paket doğrulanması yapılmamıştır.

---

## 🛠️ Kullanılan Teknolojiler ve Ortak Altyapı (Shared/Core)

Bu kısım projenin omurgasıdır ve tüm modüller tarafından ortak kullanılır:
* **Veritabanı Yönetimi:** `MongoDbContext` ve `MongoTransactionManager` ile MongoDB entegrasyonu ve Transaction (işlem) yönetimi.
* **Güvenlik (Security):** JWT Token üretimi (`JwtTokenGenerator`) ve Custom Yetkilendirme Middleware'i (`JwtAuthMiddleware`).
* **Hata Yönetimi:** Merkezi hata yakalama sistemi (`GlobalExceptionHandler`).
* **Sayfalama (Pagination):** Veri listelemeleri için standart `PagedResult` ve `PaginationParameters` yapıları.

---

## 📦 Modüller ve Görev Dağılımı (İş Paketleri)

Projeyi geliştirecek ekip üyeleri için modüller ve atanabilecek örnek görevler aşağıda listelenmiştir:

### 1. Auth Modülü 🔐
**Sorumluluk Alanı:** Kimlik doğrulama ve kullanıcı yönetimi.
* **Mevcut Durum:** `BCrypt` ile şifreleme, MongoDB `UserRepository` işlemleri, Redis tabanlı Token Blacklist (Çıkış yapma/Logout mekanizması) eklendi.
* **Geliştirici Görevleri (To-Do):** - [ ] Şifremi unuttum / Şifre sıfırlama akışlarının eklenmesi.
  - [ ] Rol tabanlı yetkilendirme (Role-based authorization - Admin/User) yapısının genişletilmesi.

### 2. Cars Modülü 🚙
**Sorumluluk Alanı:** Araç ilanlarının oluşturulması, güncellenmesi, silinmesi ve listelenmesi.
* **Mevcut Durum:** `Car` domain nesnesi oluşturuldu, `MongoCarRepository` ve sayfalama destekli araç arama (`GetCarsQuery`) işlemleri tamamlandı.
* **Geliştirici Görevleri (To-Do):**
  - [ ] Marka, model, kilometre ve fiyat aralığı gibi gelişmiş filtreleme seçeneklerinin API'a eklenmesi.
  - [ ] Araç fotoğraflarının yüklenmesi (File Upload - S3 veya Local) entegrasyonu.

### 3. Comments Modülü 💬
**Sorumluluk Alanı:** Kullanıcıların araç ilanlarına yorum ve değerlendirme yapabilmesi.
* **Mevcut Durum:** `Comment` entity'si, `MongoCommentRepository` ve bir araca ait yorumları getiren `GetCarCommentsQuery` altyapısı kuruldu.
* **Geliştirici Görevleri (To-Do):**
  - [ ] Yorum silme
  - [ ] Yorum güncelleme
  - [ ] Yorum ekleme,listeme ve getirme

### 4. Lists Modülü ⭐
**Sorumluluk Alanı:** Kullanıcıların favori araçlarını kaydettikleri koleksiyonların yönetimi.
* **Mevcut Durum:** `UserList` entity'si, `MongoListRepository` ve yeni kullanıcı kayıt olduğunda otomatik varsayılan liste (Favorilerim) oluşturan `CreateDefaultListCommand` yapısı eklendi.
* **Geliştirici Görevleri (To-Do):**
  - [ ] Listelere araç ekleme/çıkarma (Add/Remove to List) endpoint'lerinin yazılması.
  - [ ] Kullanıcıların birden fazla özel liste (örn: "Almayı Düşündüklerim", "Karşılaştıracaklarım") oluşturabilme yeteneğinin eklenmesi.

---

## 🏗️ Mimari ve Geliştirme Notları

* Her modül kendi içerisinde **Domain** (Varlıklar), **Application** (İş Kuralları ve Use-Case'ler), **Infrastructure** (Veritabanı/Dış Servisler) ve **Presentation** (Controller'lar) klasörlerine ayrılmıştır.
* **ÖNEMLİ:** Geliştirme yaparken modüller arası sıkı bağımlılıklardan (Tight Coupling) kaçının. Bir modül, diğer modülün veritabanına *doğrudan* erişmemelidir. İletişim, Application katmanındaki servisler veya MediatR benzeri event/message yapıları üzerinden sağlanmalıdır.