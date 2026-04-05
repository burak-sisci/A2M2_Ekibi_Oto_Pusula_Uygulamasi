**API Test Videosu:** [Youtube Link](https://youtu.be/t2m7DfLJ6tI)


# Mehmet Oz - REST API Gorevleri

## Modul C: Ozel Listeler ve Favoriler (Backend)

---

### 1. Liste Domain Modeli

**Sorumluluk:** Kullanici listesi varlik sinifinin tanimlanmasi.

**Yapilan Isler:**

- **Domain/UserList.cs:**
  - `Id` (string) - MongoDB ObjectId, otomatik uretilir
  - `UserId` (string) - Listeye sahip kullanicinin ID'si
  - `ListName` (string) - Liste adi (ornek: "Favoriler", "Alinacaklar", "Kiyaslanacaklar")
  - `Items` (List<string>) - Listede bulunan arac ID'lerinin koleksiyonu
  - `CreatedAt` (DateTime) - Liste olusturulma tarihi

**Ilgili Dosya:**
- `backend/backend.API/Modules/Lists/Domain/UserList.cs`

---

### 2. Liste Repository Katmani

**Sorumluluk:** MongoDB ile liste veri erisim islemleri.

**Yapilan Isler:**

- **IListRepository.cs (Interface):**
  - `GetByUserIdAsync(string userId)` - Kullanicinin tum listelerini getirme
  - `GetByIdAsync(string id)` - Tekil liste sorgulama
  - `CreateAsync(UserList list, IClientSessionHandle? session)` - Yeni liste olusturma (transaction destekli)
  - `AddItemAsync(string listId, string carId)` - Listeye arac ekleme
  - `CreateDefaultListAsync(string userId, IClientSessionHandle? session)` - Varsayilan "Favoriler" listesi olusturma
  - `DeleteAsync(string id, string userId)` - Liste silme (sahiplik kontrolu ile)

- **MongoListRepository.cs (Implementation):**
  - MongoDB koleksiyonu: `"lists"`
  - `GetByUserIdAsync`: UserId'ye gore filtreleme
  - `GetByIdAsync`: ObjectId ile tekil sorgulama
  - `CreateAsync`: Yeni belge ekleme (opsiyonel session ile transaction destegi)
  - `AddItemAsync`: MongoDB `$push` operatoru ile Items dizisine eleman ekleme
  - `CreateDefaultListAsync`: "Favoriler" adli varsayilan liste olusturma
  - `DeleteAsync`: ID ve UserId eslesme kontrolu ile silme

**Ilgili Dosyalar:**
- `backend/backend.API/Modules/Lists/Application/IListRepository.cs`
- `backend/backend.API/Modules/Lists/Infrastructure/MongoListRepository.cs`

---

### 3. Varsayilan Liste Olusturma (CreateDefaultListCommand)

**Sorumluluk:** Yeni kullanici kaydedildiginde otomatik olarak "Favoriler" listesi olusturulmasi.

**Yapilan Isler:**

- **CreateDefaultListCommand.cs:**
  - `ExecuteAsync(string userId)` - Varsayilan liste olusturma
  - Kullanici kayit isleminde (RegisterUserCommand) transaction icinde cagirilir
  - Her yeni kullanicinin otomatik olarak bir "Favoriler" listesi olur
  - Transaction destegi: Kullanici olusturma ile atomik calisir (biri basarisiz olursa ikisi de geri alinir)

**Ilgili Dosya:**
- `backend/backend.API/Modules/Lists/Application/CreateDefaultListCommand.cs`

---

### 4. Listeye Eleman Ekleme (AddItemToListCommand)

**Sorumluluk:** Mevcut bir listeye arac ilani eklenmesi.

**Yapilan Isler:**

- **AddItemToListCommand.cs:**
  - `ExecuteAsync(string listId, string carId)` - Listeye arac ekleme
  - MongoDB `$push` operasyonu ile Items dizisine yeni carId ekleme
  - Basari/basarisizlik boolean donusu

**Ilgili Dosya:**
- `backend/backend.API/Modules/Lists/Application/CreateDefaultListCommand.cs` (ayni dosyada tanimli)

---

### 5. Listeleri Goruntuleme (GET /lists)

**Sorumluluk:** Oturum acmis kullanicinin tum listelerinin getirilmesi.

**Yapilan Isler:**

- **ListsController.cs - GET /lists:**
  - `[Authorize]` attribute ile korunmus endpoint
  - JWT claims'den UserId cikartma (`ClaimTypes.NameIdentifier`)
  - `GetByUserIdAsync(userId)` ile kullanicinin tum listelerini sorgulama
  - Donus: Liste adi, icerikteki arac sayisi, olusturulma tarihi
  - Status 200 OK

**Ilgili Dosya:**
- `backend/backend.API/Presentation/Controllers/ListsController.cs`

---

### 6. Yeni Liste Olusturma (POST /lists)

**Sorumluluk:** Kullanicinin yeni ozel liste olusturmasi.

**Yapilan Isler:**

- **ListsController.cs - POST /lists:**
  - `[Authorize]` attribute ile korunmus endpoint
  - Girdi: `CreateListRequest` (liste adi)
  - JWT claims'den UserId cikartma
  - Yeni UserList nesnesi olusturma (bos Items listesi ile)
  - `CreateAsync(list)` ile veritabanina kaydetme
  - Status 201 Created donusu

**Ilgili Dosya:**
- `backend/backend.API/Presentation/Controllers/ListsController.cs`

---

### 7. Listeye Arac Ekleme (PUT /lists/{id}/items)

**Sorumluluk:** Belirli bir listeye arac ilani eklenmesi.

**Yapilan Isler:**

- **ListsController.cs - PUT /lists/{id}/items:**
  - `[Authorize]` attribute ile korunmus endpoint
  - Girdi: `AddItemRequest` (eklenecek arac ID'si)
  - `AddItemAsync(listId, carId)` cagrisi
  - MongoDB $push operasyonu ile Items dizisine ekleme
  - Status 200 OK veya 404 Not Found

**Ilgili Dosya:**
- `backend/backend.API/Presentation/Controllers/ListsController.cs`

---

### 8. Liste Silme (DELETE /lists/{id})

**Sorumluluk:** Kullanicinin olusturdugu bir listeyi silmesi.

**Yapilan Isler:**

- **ListsController.cs - DELETE /lists/{id}:**
  - `[Authorize]` attribute ile korunmus endpoint
  - JWT claims'den UserId cikartma
  - Sahiplik kontrolu: Sadece liste sahibi silebilir
  - `DeleteAsync(id, userId)` cagrisi
  - Status 200 OK veya 404 Not Found

**Ilgili Dosya:**
- `backend/backend.API/Presentation/Controllers/ListsController.cs`

---

### Kullanilan Teknolojiler ve Kutuphaneler

| Teknoloji | Kullanim Alani |
|-----------|----------------|
| ASP.NET Core 10 | Web API framework |
| MongoDB.Driver | Veritabani erisimi ($push operatoru dahil) |
| JWT Bearer Authentication | Endpoint korumasi |
| MediatR | Command pattern (varsayilan liste) |

---

### API Endpoint Ozeti

| Metod | Endpoint | Yetki | Aciklama |
|-------|----------|-------|----------|
| GET | /lists | Evet | Kullanicinin tum listelerini getir |
| POST | /lists | Evet | Yeni liste olustur |
| PUT | /lists/{id}/items | Evet | Listeye arac ekle |
| DELETE | /lists/{id} | Evet | Liste sil |

*Tum endpoint'ler [Authorize] attribute ile korunmaktadir - JWT token gerektirir.*

---

### Modulu Arasi Entegrasyonlar

| Entegrasyon | Aciklama |
|-------------|----------|
| RegisterUserCommand (Auth Modulu) | Kullanici kaydinda CreateDefaultListCommand cagirilir (transaction icinde) |
| CarDetail (Frontend) | Kullanici bir ilani favori listesine eklerken PUT /lists/{id}/items endpoint'i kullanilir |
| MongoTransactionManager (Shared) | Varsayilan liste olusturma, kullanici kaydi ile atomik calisir |
