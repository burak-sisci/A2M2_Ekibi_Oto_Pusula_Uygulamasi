**API Test Videosu:** [Youtube Link](https://youtu.be/sJ1E3SAboy8)

# Mehmet Uludag - REST API Gorevleri

## Modul D: Etkilesim ve Iletisim - Yorum ve Paylasim (Backend)

---

### 1. Yorum Domain Modeli

**Sorumluluk:** Yorum varlik sinifinin tanimlanmasi.

**Yapilan Isler:**

- **Domain/Comment.cs:**
  - `Id` (string?) - MongoDB ObjectId, otomatik uretilir
  - `UserId` (string?) - Yorum yazan kullanicinin ID'si
  - `CarId` (string?) - Yorumun yapildigi arac ilaninin ID'si
  - `Content` (string?) - Yorum metni
  - `CreatedAt` (DateTime) - Yorum tarihi

**Ilgili Dosya:**
- `backend/backend.API/Modules/Comments/Domain/Comment.cs`

---

### 2. Yorum Repository Katmani

**Sorumluluk:** MongoDB ile yorum veri erisim islemleri.

**Yapilan Isler:**

- **ICommentRepository.cs (Interface):**
  - `GetByCarIdAsync(string carId, PaginationParameters pagination)` - Ilana ait sayfalanmis yorumlari getirme
  - `GetByCommentIdAsync(string id)` - Tekil yorum sorgulama
  - `CreateAsync(Comment comment)` - Yeni yorum olusturma
  - `UpdateAsync(Comment comment)` - Yorum guncelleme
  - `DeleteAsync(string id)` - Yorum silme
  - `DeleteAllByUserIdAsync(string userId)` - Kullanicinin tum yorumlarini silme (cascade delete)
  - `DeleteAllByCarIdAsync(string carId)` - Ilana ait tum yorumlari silme (cascade delete)

- **MongoCommentRepository.cs (Implementation):**
  - MongoDB koleksiyonu: `"comments"`
  - CarId alani uzerinde index olusturma (sorgu performansi icin)
  - `GetByCarIdAsync`: CarId'ye gore filtreleme + sayfalama (Skip/Take)
  - `GetByCommentIdAsync`: ObjectId ile tekil sorgulama
  - `CreateAsync`: Yeni belge ekleme
  - `UpdateAsync`: Mevcut belgeyi guncelleme (ReplaceOne)
  - `DeleteAsync`: ID ile silme
  - `DeleteAllByUserIdAsync`: UserId eslesenleri toplu silme (DeleteMany)
  - `DeleteAllByCarIdAsync`: CarId eslesenleri toplu silme (DeleteMany)

**Ilgili Dosyalar:**
- `backend/backend.API/Modules/Comments/Application/ICommentRepository.cs`
- `backend/backend.API/Modules/Comments/Infrastructure/MongoCommentRepository.cs`

---

### 3. Ilan Yorumlarini Listeleme (GET /cars/{carId}/comments)

**Sorumluluk:** Bir ilana yapilmis tum yorumlarin sayfalanmis sekilde getirilmesi.

**Yapilan Isler:**

- **GetCarCommentsQuery.cs:**
  - `ExecuteAsync(string carId, PaginationParameters pagination)` - Sayfalanmis yorum listesi
  - Donus tipi: `PagedResult<Comment>` (items + totalCount)

- **CommentControllers.cs - GET /cars/{carId}/comments:**
  - Yetki gerektirmez (herkes okuyabilir)
  - Query parametreleri: `limit` (varsayilan 10), `offset` (varsayilan 0)
  - Status 200 OK ile yorum listesi

**Ilgili Dosyalar:**
- `backend/backend.API/Modules/Comments/Application/GetCarCommentsQuery.cs`
- `backend/backend.API/Presentation/Controllers/CommentControllers.cs`

---

### 4. Tekil Yorum Goruntuleme (GET /cars/{carId}/comments/{id})

**Sorumluluk:** Belirli bir yorumun detaylarinin getirilmesi.

**Yapilan Isler:**

- **GetCommentQuery.cs:**
  - `ExecuteAsync(string id)` - Tekil yorum sorgulama
  - Bulunamazsa hata firlatma

- **CommentControllers.cs - GET /cars/{carId}/comments/{id}:**
  - `[Authorize]` attribute ile korunmus endpoint
  - Status 200 OK ile yorum detayi

**Ilgili Dosyalar:**
- `backend/backend.API/Modules/Comments/Application/GetCarCommentsQuery.cs` (ayni dosyada tanimli)
- `backend/backend.API/Presentation/Controllers/CommentControllers.cs`

---

### 5. Yorum Ekleme (POST /cars/{carId}/comments)

**Sorumluluk:** Bir ilana yeni yorum eklenmesi.

**Yapilan Isler:**

- **AddCommentCommand.cs:**
  - `ExecuteAsync(AddCommentRequest request, string userId)` - Yeni yorum olusturma
  - Girdi: `AddCommentRequest(CarId, Content)`
  - UserId JWT claims'den alinir
  - CreatedAt otomatik olarak ayarlanir
  - Repository uzerinden veritabanina kaydetme

- **CommentControllers.cs - POST /cars/{carId}/comments:**
  - `[Authorize]` attribute ile korunmus endpoint
  - Girdi: `AddCommentBodyRequest(Content)` - CarId URL'den alinir
  - JWT claims'den UserId cikartma
  - Status 201 Created donusu

**Ilgili Dosyalar:**
- `backend/backend.API/Modules/Comments/Application/GetCarCommentsQuery.cs` (AddCommentCommand burada tanimli)
- `backend/backend.API/Presentation/Controllers/CommentControllers.cs`

---

### 6. Yorum Guncelleme (PUT /cars/{carId}/comments/{id})

**Sorumluluk:** Kullanicinin kendi yorumunu duzenlemesi.

**Yapilan Isler:**

- **UpdateCommentCommand.cs:**
  - `ExecuteAsync(UpdateCommentRequest request, string userId)` - Yorum guncelleme
  - Girdi: `UpdateCommentRequest(Content, Id)`
  - **Yetkilendirme kontrolu:** Yorumu sadece yazan kisi duzenleyebilir (UserId eslesmesi)
  - Yetki yoksa `UnauthorizedAccessException` firlatma
  - Guncellenmis yorumu repository uzerinden kaydetme

- **CommentControllers.cs - PUT /cars/{carId}/comments/{id}:**
  - `[Authorize]` attribute ile korunmus endpoint
  - Girdi: `UpdateCommentBodyRequest(Content)` - Comment ID URL'den alinir
  - JWT claims'den UserId cikartma
  - Sahiplik kontrolu: Sadece yorum sahibi duzenleyebilir
  - Status 200 OK veya 401 Unauthorized

**Ilgili Dosyalar:**
- `backend/backend.API/Modules/Comments/Application/GetCarCommentsQuery.cs` (UpdateCommentCommand burada tanimli)
- `backend/backend.API/Presentation/Controllers/CommentControllers.cs`

---

### 7. Yorum Silme (DELETE /cars/{carId}/comments/{id})

**Sorumluluk:** Kullanicinin kendi yorumunu silmesi.

**Yapilan Isler:**

- **DeleteCommentCommand.cs:**
  - `ExecuteAsync(DeleteCommentRequest request)` - Yorum silme
  - Girdi: `DeleteCommentRequest(Id, UserId)`
  - **Yetkilendirme kontrolu:** Yorumu sadece yazan kisi silebilir
  - Yetki yoksa `UnauthorizedAccessException` firlatma
  - Repository uzerinden silme islemi

- **CommentControllers.cs - DELETE /cars/{carId}/comments/{id}:**
  - `[Authorize]` attribute ile korunmus endpoint
  - JWT claims'den UserId cikartma
  - Sahiplik kontrolu
  - Status 200 OK veya 401 Unauthorized

**Ilgili Dosyalar:**
- `backend/backend.API/Modules/Comments/Application/GetCarCommentsQuery.cs` (DeleteCommentCommand burada tanimli)
- `backend/backend.API/Presentation/Controllers/CommentControllers.cs`

---

### 8. Cascade Delete Event Handler'lari

**Sorumluluk:** Kullanici veya ilan silindiginde iliskili yorumlarin otomatik temizlenmesi.

**Yapilan Isler:**

- **UserDeletedEventHandler.cs (Comments Modulu):**
  - `UserDeletedEvent` MediatR notification dinleyicisi
  - Kullanici silindiginde `DeleteAllByUserIdAsync(userId)` cagrisi
  - Kullanicinin platformdaki tum yorumlari otomatik silinir

- **CarDeletedEventHandler.cs (Comments Modulu):**
  - `CarDeletedEvent` MediatR notification dinleyicisi
  - Ilan silindiginde `DeleteAllByCarIdAsync(carId)` cagrisi
  - Ilana ait tum yorumlar otomatik silinir

**Ilgili Dosyalar:**
- `backend/backend.API/Modules/Comments/Application/EventHandler/UserDeletedEventHandler.cs`
- `backend/backend.API/Modules/Comments/Application/EventHandler/CarDeletedEventHandler.cs`

---

### 9. Sayfalama Altyapisi (PagedResult)

**Sorumluluk:** API yanitlarinda sayfalama desteginin saglanmasi.

**Yapilan Isler:**

- **PagedResult<T> sinifi:**
  - `Items` (List<T>) - Mevcut sayfadaki veriler
  - `TotalCount` (int) - Toplam kayit sayisi
  - Yorum listeleme ve diger modullerde ortak kullanilir
  - Frontend'de sayfalama kontrolleri icin totalCount bilgisi doner

- **PaginationParameters sinifi:**
  - `Limit` (int) - Sayfa basina kayit sayisi
  - `Offset` (int) - Atlanan kayit sayisi

**Ilgili Dosyalar:**
- `backend/backend.API/Shared/` (PagedResult ve PaginationParameters)

---

### Kullanilan Teknolojiler ve Kutuphaneler

| Teknoloji | Kullanim Alani |
|-----------|----------------|
| ASP.NET Core 10 | Web API framework |
| MongoDB.Driver | Veritabani erisimi (index, DeleteMany) |
| JWT Bearer Authentication | Endpoint korumasi |
| MediatR | Event-driven cascade delete |

---

### API Endpoint Ozeti

| Metod | Endpoint | Yetki | Aciklama |
|-------|----------|-------|----------|
| GET | /cars/{carId}/comments | Hayir | Ilan yorumlarini listele (sayfalanmis) |
| GET | /cars/{carId}/comments/{id} | Evet | Tekil yorum goruntule |
| POST | /cars/{carId}/comments | Evet | Yorum ekle |
| PUT | /cars/{carId}/comments/{id} | Evet | Yorum guncelle (sadece yazar) |
| DELETE | /cars/{carId}/comments/{id} | Evet | Yorum sil (sadece yazar) |

---

### Moduller Arasi Entegrasyonlar

| Entegrasyon | Aciklama |
|-------------|----------|
| UserDeletedEvent (Auth Modulu) | Kullanici silindiginde tum yorumlari cascade sil |
| CarDeletedEvent (Cars Modulu) | Ilan silindiginde tum yorumlari cascade sil |
| CarDetail (Frontend) | Ilan detay sayfasinda yorum bolumu gosterimi |
| CommentSection (Frontend) | Yorum ekleme, duzenleme, silme UI bileseni |
