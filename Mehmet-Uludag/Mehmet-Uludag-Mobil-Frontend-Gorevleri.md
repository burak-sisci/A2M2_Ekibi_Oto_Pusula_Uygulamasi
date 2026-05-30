# Mehmet Uludağ'ın Mobil Frontend Görevleri

> **Modül D — Etkileşim ve İletişim (Yorum ve Paylaşım)** kapsamındaki 6 endpoint için Flutter mobil arayüz tasarımı ve implementasyonu.

---

## 1. Yorum Ekleme Ekranı
- **API Endpoint:** `POST /cars/{carId}/comments`
- **Görev:** Kullanıcının ilan altına yorum yapabileceği UI bileşeni
- **UI Bileşenleri:**
  - Yorum giriş alanı (`TextField`, multiline, max 500 karakter, `maxLines: 5`)
  - Karakter sayacı (örn. "342 / 500")
  - "Yorumla" butonu (`ElevatedButton`)
  - `CircularProgressIndicator` (gönderim sırasında)
  - Kullanıcı avatarı + ad bilgisi (üst tarafta, hangi kullanıcı yorumladığı gözüksün)
- **Form Validasyonu:**
  - Boş yorum gönderilemez
  - Max 500 karakter
  - Spam tespit (3+ aynı yorum üst üste backend 429 döner)
- **Kullanıcı Deneyimi:**
  - Yorum gönderildikten sonra input temizlenir, listede yeni yorum en üstte görünür (optimistic update)
  - Send butonu spinning loading animasyonu
  - Başarı → `SnackBar` "Yorumunuz eklendi"
  - Hata → input altında error mesajı (`SnackBar` ile)
  - Klavye gelince yorum input'u görünür kalır (`KeyboardAware`)
  - Bildirim akışı: yorum eklenince **ilan sahibinin telefonuna push notification** gider (backend → RabbitMQ → Notification Service → FCM)
- **Teknik Detaylar:**
  - Platform: **Flutter** Material 3
  - `CommentAddViewModel` (`Provider`)
  - HTTP: **Dio** + `CommentRepository.add(carId, content)`
  - Optimistic update: yorum hemen listeye eklenir, server response'tan gerçek id alındığında güncellenir
  - Backend tarafı: yorum kaydedilir → `yorum.olusturuldu` queue'sune event publish edilir → Notification Service push gönderir

## 2. İlan Yorumlarını Listeleme Ekranı
- **API Endpoint:** `GET /cars/{carId}/comments`
- **Görev:** Bir ilana yapılmış tüm yorumları kronolojik sırayla listeleme
- **UI Bileşenleri:**
  - Yorumlar başlığı + "(N yorum)" sayacı
  - Yorum kartları (`ListView`):
    - Kullanıcı avatarı (`CircleAvatar`)
    - Kullanıcı adı + yorum tarihi (`timeago` formatı: "3 saat önce", "2 gün önce")
    - Yorum metni
    - Beğeni sayısı + kalp ikonu (like/unlike toggle)
    - Sahibiyse "Düzenle" / "Sil" overflow menu (sağ üst `PopupMenuButton`)
  - "Yorum Yap" CTA (üstte sticky veya alt FAB)
  - Empty state: "İlk yorumu sen yap!"
  - Pull-to-refresh
- **Kullanıcı Deneyimi:**
  - Yorumlar kronolojik (en yeniden eskiye) sıralı
  - Loading skeleton kartları
  - Beğeni butonuna basınca anında ikon değişir (optimistic)
  - Long-press yorum → context menü (kopyala / şikayet et)
- **Teknik Detaylar:**
  - `CommentsViewModel` — `_comments: List<Comment>`, loading, error state
  - `CommentRepository.getByCarId(carId)`
  - Response: `List<Comment>` (id, userId, carId, content, likeCount, likedByUsers, createdAt)
  - `intl` paketi ile tarih formatı (Türkçe locale)
  - GoRouter `AppRoutes.carComments` (`:id` parameterli)

## 3. Paylaşım Linki Üretme
- **API Endpoint:** `GET /cars/{carId}/share`
- **Görev:** Aracın başka platformlarda paylaşılabilmesi için kısa link üretme
- **UI Bileşenleri:**
  - Araç detay ekranında "Paylaş" butonu (`Icons.share`, sağ üst `AppBar.actions`)
  - Native share sheet (Android intent / iOS Share Sheet):
    - WhatsApp, Telegram, SMS, Twitter, Email, vs.
  - Veya `BottomSheet`:
    - Üretilen link metin alanı (`SelectableText`)
    - "Kopyala" + "WhatsApp ile Paylaş" + "Daha Fazla" butonları
- **Kullanıcı Deneyimi:**
  - Paylaş butonuna basınca arka planda API'den kısa link çekilir (~200 ms)
  - Link kopyalandığında `SnackBar` "Link panoya kopyalandı"
  - Link içinde araç başlığı + fiyat preview (rich link)
- **Teknik Detaylar:**
  - `CommentRepository.getShareLink(carId)` (`/cars/{carId}/share` endpoint)
  - Response: `{ shareLink: "https://...", expiresAt: ... }`
  - Backend tarafı `RedisShareLinkService` — link 90 gün cache'lenir (aynı carId → aynı link)
  - `share_plus` paketi ile native share API
  - `Clipboard.setData(ClipboardData(text: link))` ile kopya
  - Deep link / Universal link backend tarafında resolve edilebilir

## 4. Yorum Güncelleme
- **API Endpoint:** `PUT /comments/{commentId}`
- **Görev:** Kullanıcının kendi yorumunu düzenlemesi
- **UI Bileşenleri:**
  - Yorum kartının overflow menüsünde "Düzenle" seçeneği
  - `AlertDialog` veya tam ekran düzenleme:
    - `TextField` (mevcut yorum metniyle dolu)
    - Karakter sayacı
    - "Kaydet" + "İptal" butonları
  - Düzenlenmiş yorum kartında "(düzenlendi)" badge'i
- **Form Validasyonu:**
  - Min 1, max 500 karakter
  - Değişiklik yapılmamışsa "Kaydet" disabled
- **Kullanıcı Deneyimi:**
  - "Kaydet" basıldığında yorum kartı UI'da güncellenir (optimistic)
  - "(düzenlendi)" işareti `updatedAt > createdAt` olduğunda gösterilir
  - Backend yetki: `userId == comment.userId` değilse 403
- **Teknik Detaylar:**
  - `CommentRepository.update(commentId, content)`
  - `Dio.put('/comments/{commentId}', data: {content})`
  - Optimistic update + rollback
  - 403 → "Bu yorumu düzenleme yetkin yok" (mobil için zaten kendi yorumlarını düzenleyebilir)

## 5. Yorum Silme
- **API Endpoint:** `DELETE /comments/{commentId}`
- **Görev:** Kullanıcının kendi yorumunu silmesi
- **UI Bileşenleri:**
  - Yorum kartının overflow menüsünde "Sil" seçeneği (kırmızı, destructive)
  - Onay `AlertDialog` ("Bu yorumu silmek istediğinizden emin misiniz?")
  - Silme sırasında kart üzerinde loading overlay
- **Kullanıcı Deneyimi:**
  - Onay → API çağrısı → yorum kartı listeden animasyonla kaybolur (`AnimatedList.removeItem`)
  - "Geri al" SnackBar action (3 sn, undo destekli)
  - Yorum sayısı badge'i güncellenir
- **Teknik Detaylar:**
  - `CommentRepository.delete(commentId)`
  - `Dio.delete('/comments/{commentId}')`
  - Optimistic delete + rollback
  - Backend yetki: `userId == comment.userId` veya `userId == car.ownerId` (ilan sahibi de silebilir)
  - Like'lar (`likedByUsers`) de silinir backend tarafında
  - Local state: `_comments.removeWhere((c) => c.id == commentId)`

## 6. Kullanıcının Kendi Yorumlarını Listeleme
- **API Endpoint:** `GET /users/{userId}/comments`
- **Görev:** Kullanıcının profil sayfasında geçmiş yorumlarını gösterme
- **UI Bileşenleri:**
  - Profil sayfasında "Yorumlarım" sekmesi (`TabBar` veya ayrı ekran)
  - Yorum kartları (her birinde):
    - Yorum metni
    - Yapılan araç bilgisi (marka + model, link)
    - Yorum tarihi (timeago)
    - Beğeni sayısı
    - "Düzenle" / "Sil" butonları
  - Empty state: "Henüz hiç yorum yapmadın"
  - Pull-to-refresh
- **Kullanıcı Deneyimi:**
  - Karta tıklayınca aracın yorumlar sayfasına götürür (yorumun bulunduğu yere scroll)
  - Yorumların tarih sırasıyla listelenmesi (en yeni üstte)
  - Loading skeleton kartları
- **Teknik Detaylar:**
  - `UserCommentsViewModel`
  - `CommentRepository.getByUserId(userId)`
  - Response: `List<Comment>` + her birinin `car: { brand, model }` populated
  - Backend MongoDB aggregation: `comments.userId` + `$lookup` ile car detayları
  - `comment_userId` index (backend MongoDB) ile hızlı sorgu

---

## Ortak Teknik Notlar

- **Mimari:** MVVM, `lib/viewmodels/comment/`, `lib/view/pages/comment/`, `lib/view/widgets/comment/`
- **State Management:** Provider 6.1 + `ChangeNotifier`
- **Repository:** `CommentRepository`
- **HTTP:** Dio 5.7 + `AuthInterceptor` (Bearer + refresh)
- **Push Notification Akışı:** Yorum eklenince `yorum.olusturuldu` queue → Notification Service → FCM → ilan sahibinin telefonu
  - `CommentNotificationWorker` self-comment kontrolü yapar (kullanıcı kendi ilanına yorum yaparsa push gönderilmez)
- **Optimistic Update:** Tüm yorumlar (ekle/düzenle/sil/like) UI'da anında, error rollback
- **Like/Unlike:** Aynı yorum sayfası repository methodlarıyla (`likeComment`, `unlikeComment`) ele alınır
- **Tarih Formatı:** `intl` paketi, Türkçe locale (`tr_TR`), `timeago` benzeri "3 saat önce" formatı
- **Cache:** Yorumlar sık değiştiği için minimum cache; backend tarafı yorum CRUD'da `comments:car:{carId}` cache invalidate
