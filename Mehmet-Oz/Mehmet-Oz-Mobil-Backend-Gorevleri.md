# Mehmet Öz'ün Mobil Backend Görevleri

**Mobil Front-end ile Back-end Bağlanmış Test Videosu:** [Link buraya eklenecek](https://example.com)

> **Modül C — Özel Listeler ve Favoriler** kapsamındaki 7 endpoint için Flutter ↔ ASP.NET Core 10 backend bağlantı katmanı.

---

## 1. Yeni Liste Oluşturma Servisi
- **API Endpoint:** `POST /lists`
- **Görev:** Kullanıcının yeni özel liste oluşturması için servis entegrasyonu
- **İşlevler:**
  - Liste adı toplama, validasyon (min 2 max 50, boş olamaz, rezerve isim engeli)
  - `Dio.post('/lists', data: {name})` ile API isteği
  - Başarılı response: yeni `UserList` döner — local state'e eklenir
  - Hata yakalama: 409 (aynı isimde liste var), 400 (validation), 401 (refresh)
- **Teknik Detaylar:**
  - HTTP: **Dio 5.7** + `BaseOptions(baseUrl: AppConstants.baseUrl)`
  - Repository: `ListRepository.create(name)`
  - State: `ListsViewModel.createList(name)` (`Provider`/`ChangeNotifier`)
  - JWT Bearer otomatik (`AuthInterceptor`)
  - `userId` JWT claim'inden backend tarafında okunur — request body'de yer almaz
  - Backend Redis cache invalidation: `lists:user:{userId}` key silinir

## 2. Listeye İlan Ekleme Servisi
- **API Endpoint:** `POST /lists/{listId}/cars`
- **Görev:** Beğenilen aracı seçilen listeye ekleme
- **İşlevler:**
  - `AddToListSheet` üzerinden listId + carId toplama
  - `Dio.post('/lists/{listId}/cars', data: {carId})` çağrısı
  - Backend tarafında `$addToSet` ile MongoDB duplicate check
  - Optimistic update: kalp ikonu hemen dolu görünür
  - Aynı carId zaten listede ise backend 200 idempotent veya 409 — UI sessiz ignore
- **Teknik Detaylar:**
  - Repository: `ListRepository.addCarToList(listId, carId)`
  - `AddToListSheetViewModel` state — `selectedLists: Set<String>`
  - Authorization: 403 (başkasının listesine ekleme imkansız çünkü userId match yok)
  - Backend: `AddCarToListCommand` + Redis cache invalidation

## 3. Kullanıcının Listelerini Görüntüleme Servisi
- **API Endpoint:** `GET /users/{userId}/lists`
- **Görev:** Kullanıcının tüm listelerini ve içlerindeki araç sayılarını getirme
- **İşlevler:**
  - `Dio.get('/users/{userId}/lists')` çağrısı
  - Response: `List<UserList>` — id, ad, varsayilan, ilanSayisi, kayitTarihi
  - Mobil tarafta `_lists` listesi güncellenir, UI yeniden render olur
  - Pull-to-refresh ile force fetch
- **Teknik Detaylar:**
  - Repository: `ListRepository.getUserLists(userId)`
  - Backend: `MongoListRepository.GetByUserIdAsync` + aggregation ile her liste için `cars` count
  - Redis cache: `lists:user:{userId}` (5 dk TTL)
  - 401 → AuthInterceptor refresh akışı
  - 403 → başkasının listelerine bakma engeli (mobil için kendi listeleri zaten)

## 4. Liste İçeriğini Görüntüleme Servisi
- **API Endpoint:** `GET /lists/{listId}`
- **Görev:** Spesifik bir listenin içindeki araçları detaylı çekme
- **İşlevler:**
  - `Dio.get('/lists/{listId}')` çağrısı
  - Response: `UserList` + populated `cars: List<Car>` (MongoDB aggregation join ile)
  - Mobil tarafta `ListDetailViewModel._cars` güncellenir
  - Sonsuz scroll yok (genelde 10-50 araç, tek seferde getirilir)
- **Teknik Detaylar:**
  - Repository: `ListRepository.getById(listId)`
  - Backend: `MongoListRepository.GetByIdWithCarsAsync` — MongoDB `$lookup` ile car detayları
  - Yetki: backend `userId == currentUser` veya liste public (gelecek özellik)
  - `RedisListCacheService` cache

## 5. Liste İsmini Güncelleme Servisi
- **API Endpoint:** `PUT /lists/{listId}`
- **Görev:** Liste adını değiştirme
- **İşlevler:**
  - Yeni isim toplama, validasyon
  - `Dio.put('/lists/{listId}', data: {name: newName})` çağrısı
  - Başarılı response: güncellenmiş `UserList` döner
  - Backend yetki kontrolü:
    - `varsayilan == true` ise 403 (Favoriler değiştirilemez)
    - `userId != currentUser` ise 403
- **Teknik Detaylar:**
  - Repository: `ListRepository.updateName(listId, newName)`
  - `UpdateListNameCommand` (backend)
  - Optimistic update + rollback on error
  - Redis cache invalidation
  - 409 → "Bu isimde bir listen zaten var"

## 6. Listeden İlan Çıkarma Servisi
- **API Endpoint:** `DELETE /lists/{listId}/cars/{carId}`
- **Görev:** Listedeki bir aracın referansını çıkarma (ilan kendisi silinmez)
- **İşlevler:**
  - `Dio.delete('/lists/{listId}/cars/{carId}')` çağrısı
  - Backend: `RemoveCarFromListCommand` — MongoDB `$pull` ile carId çıkarılır
  - Mobil tarafta `_cars` listesinden filter ile silinir (animasyonla)
  - "Geri al" SnackBar action ile undo
- **Teknik Detaylar:**
  - Repository: `ListRepository.removeCarFromList(listId, carId)`
  - Optimistic update — UI'da kart hemen kaybolur
  - Hata olursa rollback (kartı geri ekle)
  - 404 → araç zaten listede değil (sessizce ignore)
  - Redis cache invalidation

## 7. Listeyi Tamamen Silme Servisi
- **API Endpoint:** `DELETE /lists/{listId}`
- **Görev:** Listeyi tamamen silme
- **İşlevler:**
  - Onay dialog sonrası `Dio.delete('/lists/{listId}')` çağrısı
  - Backend: `DeleteListCommand` — `userId` + `varsayilan != true` kontrolü
  - Başarılı silme sonrası listeler sayfasından kaldırma (Provider notifyListeners)
- **Teknik Detaylar:**
  - Repository: `ListRepository.delete(listId)`
  - 403 → "Favoriler listesi silinemez"
  - Local state: `_lists.removeWhere((l) => l.id == listId)`
  - GoRouter `pop()` → ana liste ekranına dönüş
  - Backend cascade: liste içindeki carId referansları temizlenir (sadece liste dokümanı silinir, cars koleksiyonu etkilenmez)
  - Redis cache invalidation: `lists:user:{userId}`

---

## Ortak Teknik Notlar

- **API Base URL:** `https://otopusula-backend.onrender.com`
- **HTTP Client:** Dio 5.7
  - `AuthInterceptor` — Bearer token + 401 refresh rotation
  - `ErrorInterceptor` — `DioException` → `ApiException` mapping
- **Repository:** `ListRepository`
- **State Management:** Provider 6.1 + `ChangeNotifier`
- **MongoDB:** `lists` koleksiyonu — `userId` üzerinde index (`list_userId`)
- **Redis Caching:** `RedisListCacheService` — `lists:user:{userId}` (5 dk TTL); CRUD operasyonlarında invalidate
- **Otomatik "Favoriler":** Kullanıcı kayıt olduğunda transaction içinde otomatik oluşturulur (`CreateDefaultListCommand`). Mobil tarafı bu liste'nin var olduğunu bildiği için yeni kullanıcı için ilk login sonrası listeler sayfasında "Favoriler" görünür.
- **MediatR Cascade:** Kullanıcı silindiğinde `UserDeletedEvent` ile tüm listeleri silinir
