# Mehmet Uludağ'ın Mobil Backend Görevleri

> **Modül D — Etkileşim ve İletişim (Yorum ve Paylaşım)** kapsamındaki 6 endpoint için Flutter ↔ ASP.NET Core 10 backend bağlantı katmanı.

---

## 1. Yorum Ekleme Servisi
- **API Endpoint:** `POST /cars/{carId}/comments`
- **Görev:** Mobil uygulamada yorum gönderme + RabbitMQ event tetikleme akışı
- **İşlevler:**
  - Yorum metni toplama, validasyon (1-500 karakter, boş engeli)
  - `Dio.post('/cars/{carId}/comments', data: {Icerik: text})` çağrısı
  - Başarılı response: yeni `Comment` döner (id, userId, carId, content, createdAt)
  - Backend tarafı yorum kaydedildikten sonra `yorum.olusturuldu` RabbitMQ kuyruğuna `CommentCreatedEvent` publish eder
  - Notification Service kuyruğu tüketir → ilan sahibinin FCM token'larını çeker → push gönderir
  - Mobil tarafta optimistic update: yorum hemen listede görünür
- **Teknik Detaylar:**
  - HTTP: **Dio 5.7**
  - Repository: `CommentRepository.add(carId, content)`
  - State: `CommentAddViewModel` (`Provider`/`ChangeNotifier`)
  - JWT Bearer otomatik (`AuthInterceptor`)
  - Backend MediatR: `AddCommentCommand` → DB save + RabbitMQ publish
  - **Push Notification Akışı:**
    - Backend → CloudAMQP `yorum.olusturuldu` queue
    - HF Notification Service kuyruğu tüketir
    - MongoDB'den `cars.ownerId` lookup → `devices` koleksiyonundan FCM token'lar
    - Firebase Cloud Messaging → ilan sahibinin telefonu 🔔
  - Self-comment kontrolü: `AuthorUserId == CarOwnerId` ise Notification Service push göndermez

## 2. İlan Yorumlarını Listeleme Servisi
- **API Endpoint:** `GET /cars/{carId}/comments`
- **Görev:** Bir aracın yorumlarını kronolojik sırayla çekme
- **İşlevler:**
  - `Dio.get('/cars/{carId}/comments')` çağrısı
  - Response: `List<Comment>` (id, userId, carId, content, likeCount, likedByUsers, createdAt, updatedAt)
  - Tarih sırasıyla sıralı (en yeni üstte) — backend `MongoCommentRepository` ile `Sort.Descending(createdAt)`
  - Mobil tarafta `_comments` listesi güncellenir
  - Pull-to-refresh ile force fetch
- **Teknik Detaylar:**
  - Repository: `CommentRepository.getByCarId(carId)`
  - Backend: `MongoCommentRepository.GetByCarIdAsync` + `comment_carId` index ile hızlı sorgu
  - `_comments` Provider state, UI yeniden render
  - Backend Redis cache (yorum sık güncellenir, kısa TTL)
  - 404 (car bulunamadı) → "İlan bulunamadı"

## 3. Paylaşım Linki Üretme Servisi
- **API Endpoint:** `GET /cars/{carId}/share`
- **Görev:** Araç ilanı için benzersiz kısa paylaşım linki çekme
- **İşlevler:**
  - `Dio.get('/cars/{carId}/share')` çağrısı
  - Response: `{ shareLink: "https://otopusula-backend.onrender.com/share/<hash>" }` benzeri kısa link
  - Aynı carId için aynı link döner (idempotent)
  - Mobil tarafta link clipboard'a kopyalanır veya native share sheet'e gönderilir
- **Teknik Detaylar:**
  - Repository: `CommentRepository.getShareLink(carId)`
  - Backend: `RedisShareLinkService` — `share:{carId}` key'inde link 90 gün cache
  - İlk istek: yeni link üretir + cache'e yazar
  - Sonraki istekler: cache'den hızlıca döner
  - Mobil tarafı: `share_plus` paketi ile native share API, `Clipboard.setData` ile copy

## 4. Yorum Güncelleme Servisi
- **API Endpoint:** `PUT /comments/{commentId}`
- **Görev:** Kullanıcının kendi yorumunu güncellemesi
- **İşlevler:**
  - Yeni yorum metni toplama, validasyon
  - `Dio.put('/comments/{commentId}', data: {content: newText})` çağrısı
  - Backend yetki kontrolü: `comment.userId == User.NameIdentifier` değilse 403
  - Başarılı update: `updatedAt > createdAt` → mobil tarafı "(düzenlendi)" badge gösterir
- **Teknik Detaylar:**
  - Repository: `CommentRepository.update(commentId, content)`
  - `UpdateCommentCommand` (backend MediatR)
  - Optimistic update + rollback on error
  - Like'lar etkilenmez (sadece `content` ve `updatedAt` değişir)
  - 403 → "Bu yorumu düzenleme yetkin yok"
  - 404 → "Yorum silinmiş veya bulunamadı"

## 5. Yorum Silme Servisi
- **API Endpoint:** `DELETE /comments/{commentId}`
- **Görev:** Yorum silme — kendi yorumlarını veya ilan sahibi ise herhangi bir yorumu
- **İşlevler:**
  - Onay dialog'u sonrası `Dio.delete('/comments/{commentId}')` çağrısı
  - Backend yetki: `comment.userId == currentUser` veya `car.ownerId == currentUser`
  - Başarılı silme → mobil tarafta yorum listeden kaldırılır
  - Backend cascade: yorumla beraber `likedByUsers` array'i de silinir
- **Teknik Detaylar:**
  - Repository: `CommentRepository.delete(commentId)`
  - `DeleteCommentCommand` (backend)
  - Optimistic delete + rollback
  - Local state: `_comments.removeWhere((c) => c.id == commentId)`
  - Backend Redis cache: `comments:car:{carId}` invalidate
  - Mobil tarafı `AnimatedList.removeItem` ile animasyonlu silme

## 6. Yorum Beğenme / Beğeniyi Geri Alma Servisi
- **API Endpoint:**
  - `POST /comments/{commentId}/like`
  - `DELETE /comments/{commentId}/like`
- **Görev:** Yoruma beğeni ekleme veya geri alma + beğeni sayısı senkronizasyonu
- **İşlevler:**
  - Mevcut beğeni durumunu `Comment.likedByUsers` listesinden okuma
  - Eğer kullanıcı listede yoksa: `Dio.post('/comments/{commentId}/like')` ile beğeni ekle
  - Listede varsa: `Dio.delete('/comments/{commentId}/like')` ile beğeniyi kaldır
  - Backend response'tan güncel `begeniSayisi` ve `begendimMi` bilgilerini al → UI state'i güncelle
  - Backend MongoDB tarafında atomic update:
    - LikeCommentCommand: `$addToSet` ile `likedByUsers` array'ine kullanıcı eklenir + `$inc` ile likeCount artar
    - UnlikeCommentCommand: `$pull` ile kullanıcı çıkarılır + `$inc -1` ile sayaç azalır
  - Idempotent: aynı POST tekrar gelirse `$addToSet` sayesinde duplicate olmaz, sayaç aynı kalır
- **Teknik Detaylar:**
  - HTTP: **Dio 5.7**
  - Repository: `CommentRepository.likeComment(commentId)` + `unlikeComment(commentId)`
  - Backend: `LikeCommentCommand` + `UnlikeCommentCommand` (MediatR handler)
  - State: `CommentsViewModel.toggleLike(commentId)` — optimistic update + rollback on error
  - JWT Bearer otomatik (`AuthInterceptor`)
  - Yetki: backend authenticated user gerekli (`[Authorize]` filter)
  - Race condition koruması: backend MongoDB atomic operation kullanıyor (`$addToSet`/`$pull`)
  - Hata yönetimi: 401 (refresh akışı), 404 (yorum silinmiş), 5xx (retry mesajı)

---

## Ortak Teknik Notlar

- **API Base URL:** `https://otopusula-backend.onrender.com`
- **HTTP Client:** Dio 5.7
  - `AuthInterceptor` — Bearer token + 401 refresh rotation
  - `ErrorInterceptor` — DioException → ApiException mapping
- **Repository:** `CommentRepository`
- **State Management:** Provider 6.1 + `ChangeNotifier`
- **Push Notification Akışı (kritik):**
  ```
  Mobile POST /cars/{carId}/comments
      ↓
  Backend save → MediatR AddCommentCommand
      ↓
  CommentCreatedEvent publish → CloudAMQP yorum.olusturuldu queue
      ↓
  HF Notification Service consume
      ↓
  MongoDB: car.ownerId → devices.fcmToken[]
      ↓
  Firebase Cloud Messaging → ilan sahibinin telefonu 🔔
  ```
- **Self-Comment Check:** `CommentNotificationWorker` `AuthorUserId == CarOwnerId` ise push göndermez (yapan kişi kendi ilanına yorum yapmışsa)
- **Geçersiz FCM Token:** Notification Service `InvalidArgument` veya `Unregistered` hatalarında otomatik MongoDB'den temizler — Mobil tarafı bunu bilmeyebilir (transparent)
- **MongoDB İndeksler:**
  - `comment_carId` → araç yorumlarını hızlı listeleme
  - `comment_userId` → kullanıcının kendi yorumlarını hızlı listeleme
- **Redis Cache:** `comments:car:{carId}` (kısa TTL), yorum CRUD'da invalidate
- **MediatR Cascade:** Kullanıcı silindiğinde `UserDeletedEvent` ile tüm yorumları silinir
