# Mehmet Öz'ün Mobil Frontend Görevleri

**Mobile Front-end Demo Videosu:** [Link buraya eklenecek](https://example.com)

> **Modül C — Özel Listeler ve Favoriler** kapsamındaki 7 endpoint için Flutter mobil arayüz tasarımı ve implementasyonu.

---

## 1. Yeni Liste Oluşturma Ekranı
- **API Endpoint:** `POST /lists`
- **Görev:** "Alınacaklar", "Kıyaslanacaklar" gibi yeni koleksiyon oluşturma
- **UI Bileşenleri:**
  - "Yeni Liste" butonu (FAB veya `AppBar.actions`)
  - `AlertDialog` veya `BottomSheet` modal:
    - Liste adı `TextField` (max 50 karakter)
    - "Oluştur" + "İptal" butonları
  - `CircularProgressIndicator` (oluşturma sırasında)
- **Form Validasyonu:**
  - Liste adı boş olamaz
  - Liste adı min 2, max 50 karakter
  - "Favoriler" gibi rezerve isimler kullanılamaz (backend kontrolü)
- **Kullanıcı Deneyimi:**
  - Modal otomatik açıldığında klavye gelir, input focus'lu
  - Başarılı oluşturma → modal kapanır, `SnackBar` "Liste oluşturuldu" + ListelerSayfası refresh
  - Hata → input altında hata mesajı (örn. 409 "Bu isimde liste zaten var")
  - "Favoriler" otomatik oluşur kayıt sırasında (backend tarafı, transaction içinde)
- **Teknik Detaylar:**
  - Platform: **Flutter** Material 3
  - `ListsViewModel.createList(name)` (`Provider` + `ChangeNotifier`)
  - HTTP: **Dio** + `ListRepository.create(name)`
  - Navigation: GoRouter — modal kapanır, mevcut listeler sayfası refresh

## 2. Listeye İlan Ekleme (Bottom Sheet)
- **API Endpoint:** `POST /lists/{listId}/cars`
- **Görev:** Beğenilen araç ilanını seçilen listeye ekleme
- **UI Bileşenleri:**
  - Car detail veya car card üzerinde "Favoriye/Listeye Ekle" ikon butonu (kalp ikonu)
  - Butona basınca açılan `BottomSheet` (`add_to_list_sheet.dart`):
    - Mevcut listelerin checkbox'lı listesi
    - Her liste için "Favoriler ★" badge'i varsayılan olanda
    - "Yeni Liste Oluştur" butonu (alt kısımda)
    - "Tamam" butonu
- **Kullanıcı Deneyimi:**
  - Sheet açıldığında o anki listeleri çekme (`GET /users/{userId}/lists`)
  - Aracın halihazırda eklendiği listeler ✅ checked görünür
  - Tıklayıp aktif edince anında ekleme (`POST /lists/{listId}/cars`)
  - Tıklayıp deaktif edince çıkarma (`DELETE /lists/{listId}/cars/{carId}`)
  - Sheet kapatıldığında car kartında "favorilenmiş" ikonu güncellenir
- **Teknik Detaylar:**
  - `AddToListSheetViewModel`
  - `ListRepository.addCarToList(listId, carId)` + `removeCarFromList`
  - Optimistic update: kullanıcı tıklayınca UI hemen değişir
  - Hata olursa rollback + `SnackBar` "Eklenemedi"
  - Aynı carId aynı listede iki kez varsa backend 409 döner — UI bunu sessizce ignore eder

## 3. Kullanıcının Listelerini Görüntüleme Ekranı
- **API Endpoint:** `GET /users/{userId}/lists`
- **Görev:** Kullanıcının tüm listelerini ana ekranda göstermek
- **UI Bileşenleri:**
  - "Listelerim" başlığı (AppBar)
  - Liste kartları (`ListView`):
    - Her kart: liste adı, içindeki araç sayısı, "Favoriler" badge'i (varsayılan)
    - Trailing: > ikon (detaya gitmek için)
    - Long-press menü: "Yeniden Adlandır", "Sil"
  - "Yeni Liste Oluştur" FAB
  - Pull-to-refresh (`RefreshIndicator`)
  - Empty state: "Henüz listen yok, ilk listeyi oluştur" + CTA
- **Kullanıcı Deneyimi:**
  - İlk yükleme: shimmer placeholder kartları
  - Listeye tıklama → liste detay ekranı
  - Long-press → context menü (favoriler için sınırlı: sadece "Görüntüle")
- **Teknik Detaylar:**
  - `ListsViewModel` — `_lists`, loading, error state
  - `ListRepository.getUserLists(userId)`
  - Response: `List<UserList>` (id, ad, varsayilan, ilanSayisi, kayitTarihi)
  - Backend Redis cache: liste sayıları (`lists:user:{userId}`)

## 4. Liste İçeriği Görüntüleme Ekranı
- **API Endpoint:** `GET /lists/{listId}`
- **Görev:** Spesifik bir listenin içindeki araç ilanlarını detaylı gösterme
- **UI Bileşenleri:**
  - `AppBar` ile liste adı (başlığa düzenleme için tap)
  - Araç sayısı bilgisi ("12 ilan")
  - Araç kartları listesi (CarList gibi):
    - Resim, marka, model, yıl, fiyat
    - "Listeden Çıkar" ikon butonu (sağ üst)
  - "Listeyi Sil" buton (kırmızı, alt FAB veya menü)
  - Empty state: "Bu listede henüz araç yok, ilan ekle"
  - Pull-to-refresh
- **Kullanıcı Deneyimi:**
  - Araç kartına tıklayınca araç detay sayfasına gider
  - "Listeden Çıkar" → onay dialog → API silme
  - Liste adına AppBar'da tap → yeniden adlandırma dialog'u
- **Teknik Detaylar:**
  - `ListDetailViewModel` — `_list`, `_cars`, loading, error
  - `ListRepository.getById(listId)` → `UserList` + populated `cars`
  - Backend: aggregation ile liste içindeki carId'lere göre `cars` koleksiyonundan join
  - GoRouter `AppRoutes.listDetail` (`:id` parameterli)

## 5. Liste İsmini Güncelleme
- **API Endpoint:** `PUT /lists/{listId}`
- **Görev:** Liste adını değiştirme (Favoriler hariç)
- **UI Bileşenleri:**
  - Liste detay ekranında AppBar başlığına tap veya menü → "Yeniden Adlandır"
  - `AlertDialog`:
    - Eski isim ile dolu `TextField`
    - "Kaydet" + "İptal" butonları
- **Form Validasyonu:**
  - Min 2, max 50 karakter
  - Boş olamaz
  - Mevcut isimle aynı olursa "Değişiklik yok" mesajı (request gönderme)
- **Kullanıcı Deneyimi:**
  - Favoriler listesi için "Yeniden Adlandır" disabled veya görünmez
  - Başarılı update sonrası AppBar başlığı + listeler sayfası güncellenir
  - Hata: 403 Forbidden (Favoriler ise), 409 (aynı isimde liste var)
- **Teknik Detaylar:**
  - `ListDetailViewModel.updateName(newName)`
  - `ListRepository.updateName(listId, newName)` → `Dio.put('/lists/{listId}', data: {name})`
  - Optimistic update: AppBar başlığı hemen değişir, hata olursa rollback

## 6. Listeden İlan Çıkarma
- **API Endpoint:** `DELETE /lists/{listId}/cars/{carId}`
- **Görev:** Listedeki bir aracı silmeden çıkarma (ilan kendi koleksiyonunda kalır)
- **UI Bileşenleri:**
  - Liste detay ekranındaki her araç kartında "Çıkar" ikon butonu (X simgesi)
  - Veya araç kartına long-press → context menü → "Listeden Çıkar"
  - Onay dialog'u ("Bu aracı listeden çıkarmak istediğinize emin misiniz?")
- **Kullanıcı Deneyimi:**
  - Onay → API çağrısı → kart anında listeden kalkar (animasyon, `AnimatedList`)
  - "Geri al" SnackBar action (5 sn, basılırsa eski hale döner)
  - Liste boşalırsa empty state'e geçer
- **Teknik Detaylar:**
  - `ListDetailViewModel.removeCar(carId)`
  - `ListRepository.removeCarFromList(listId, carId)`
  - Optimistic update + rollback on error
  - Backend MongoDB: `lists` koleksiyonundan `$pull` ile carId çıkarılır

## 7. Listeyi Tamamen Silme
- **API Endpoint:** `DELETE /lists/{listId}`
- **Görev:** Kullanıcının özel listesini tamamen silme (Favoriler hariç)
- **UI Bileşenleri:**
  - Liste detay ekranı menüsünde "Listeyi Sil" (kırmızı, destructive)
  - veya Listeler ekranında long-press menü
  - Onay `AlertDialog` ("Bu listeyi silmek geri alınamaz. İçindeki araçlar kalır ama liste kaybolur.")
  - Çift onay (opsiyonel: "Silmek için liste adını yazın")
- **Kullanıcı Deneyimi:**
  - Favoriler listesi için silme butonu **görünmez** (frontend kontrol + backend 403)
  - Başarılı silme → Listeler sayfasına dönüş, ilgili kart kaybolur (animasyon)
  - `SnackBar` "Liste silindi"
- **Teknik Detaylar:**
  - `ListsViewModel.deleteList(listId)`
  - `ListRepository.delete(listId)`
  - GoRouter `pop()` → ana listeler ekranına dönüş
  - Backend yetki: `userId == currentUser` + `varsayilan != true`

---

## Ortak Teknik Notlar

- **Mimari:** MVVM, `lib/viewmodels/list/`, `lib/view/pages/list/`, `lib/view/widgets/list/`
- **State Management:** Provider 6.1 + `ChangeNotifier`
- **Repository:** `ListRepository`
- **HTTP:** Dio 5.7 + `AuthInterceptor` (Bearer + refresh)
- **Navigation:** GoRouter (`AppRoutes.lists`, `AppRoutes.listDetail`)
- **Optimistic Update Patern:** Tüm değişiklikler UI'da anında, error rollback
- **Favoriler Özel Durumu:** Backend `varsayilan: true` flag'i ile işaretli; rename + delete UI tarafında bloklu
- **Otomatik Favoriler:** Kullanıcı kayıt olduğunda backend transaction içinde "Favoriler" listesi yaratır (`CreateDefaultListCommand`)
