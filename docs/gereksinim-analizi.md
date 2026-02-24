# Gereksinim Analizi

## Tüm Gereksinimler 

#### **Modül A: Kullanıcı ve Kimlik Yönetimi**

*(Üye olma ve profil süreçlerini yöneten temel modül)*

1. Üye Olma
API Metodu: POST /auth/register
Açıklama: Kullanıcıların sisteme kayıt olmasını sağlar. İsim, email, telefon numarası, cinsiyet*, doğum tarihi* ve şifre alınarak veritabanında yeni bir kullanıcı oluşturulur.
2. Kullanıcı Girişi (Login)
API Metodu: POST /auth/login
Açıklama: Kullanıcının hem email veya telefon numarası hemde şifresiyle sisteme girmesini sağlar. Başarılı girişte Token (yetki belgesi) döndürür.
3. Profil Görüntüleme
API Metodu: GET /users/{userId}
Açıklama: Kullanıcının profil bilgilerini (ad, email, katılım tarihi) getiren listeleme/okuma metodudur.
4. Profil Güncelleme
API Metodu: PUT /users/{userId}
Açıklama: Kullanıcının şifre, telefon veya isim gibi kişisel bilgilerini değiştirmesini/güncellemesini sağlar.
5. Hesap Silme
API Metodu: DELETE /users/{userId}
Açıklama: Kullanıcının kendi isteğiyle hesabını ve sistemdeki tüm kişisel verilerini kalıcı olarak silmesini sağlar.
6. Kullanıcı Çıkışı (Logout)
API Metodu: POST /auth/logout
Açıklama: Sisteme giriş yapmış olan kullanıcının oturumunu güvenli bir şekilde sonlandırmasını sağlar. Kullanıcının cihazındaki aktif erişim token'ı (yetki belgesi) silinerek/geçersiz kılınarak başkalarının hesaba erişmesi engellenir.

#### **Modül B: Araç İlanları ve Yapay Zeka**

*(Projenin bel kemiği; ilanlar, kazıma ve fiyat tahmin modülü)*

1. Yeni İlan Ekleme
API Metodu: POST /cars
Açıklama: Aracın teknik özellikleri (marka, km, boya vb.), konumu ve fotoğraflarıyla birlikte sisteme yeni bir ilan olarak eklenmesini sağlar.
2. Fiyat Tahmini Alma (ML)
API Metodu: POST /cars/predict-price
Açıklama: Kullanıcının gönderdiği araç özelliklerinin makine öğrenmesi modeline sokularak, tahmini piyasa değerinin hesaplanıp geri döndürülmesini sağlar.
3. İlanları Filtreleme
API Metodu: GET /cars
Açıklama: Sistemdeki araç ilanlarının fiyat, kilometre, marka ve konuma göre filtrelenerek listelenmesini sağlar.
4. İlan Detayı Görüntüleme
API Metodu: GET /cars/{carId}
Açıklama: Seçilen tek bir araca ait tüm resimlerin, donanım detaylarının ve açıklama metinlerinin getirilmesini sağlar.
5. İlan Güncelleme
API Metodu: PUT /cars/{carId}
Açıklama: İlan sahibinin araç fiyatını veya açıklamasını sonradan düzenlemesini sağlar.
6. İlan Silme
API Metodu: DELETE /cars/{carId}
Açıklama: Satışı gerçekleşen veya kaldırılmak istenen ilanın sistemden silinmesini sağlar.

#### **Modül C: Özel Listeler ve Favoriler**

*(Kullanıcıların kendi koleksiyonlarını yönettiği modül)*

1. Yeni Liste Oluşturma
API Metodu: POST /lists
Açıklama: Kullanıcıların "Alınacaklar", "Kıyaslanacaklar" gibi isimlerle yeni koleksiyonlar oluşturmasını sağlar (Favoriler listesi kayıt anında otomatik açılır).
2. Listeye İlan Ekleme
API Metodu: POST /lists/{listId}/cars
Açıklama: Beğenilen bir araç ilanının, kullanıcının seçtiği spesifik bir listeye (veya default Favoriler listesine) eklenmesini sağlar.
3. Kullanıcının Listelerini Görüntüleme
API Metodu: GET /users/{userId}/lists
Açıklama: Kullanıcının sahip olduğu tüm listelerin ve içlerindeki araç sayılarının getirilmesini sağlar.
4. Liste İçeriğini Görüntüleme
API Metodu: GET /lists/{listId}
Açıklama: Spesifik bir listenin içine girildiğinde, o listedeki tüm araç ilanlarının detaylarıyla birlikte ekrana getirilmesini sağlar.
5. Liste İsmini Güncelleme
API Metodu: PUT /lists/{listId}
Açıklama: Kullanıcının daha önce oluşturduğu bir listenin adını değiştirmesini sağlar (Favoriler hariç).
6. Listeden İlan Çıkarma
API Metodu: DELETE /lists/{listId}/cars/{carId}
Açıklama: Önceden listeye eklenmiş bir aracın sadece o listeden çıkarılmasını sağlar (İlanın kendisi silinmez).
7. Listeyi Tamamen Silme
API Metodu: DELETE /lists/{listId}
Açıklama: Kullanıcının oluşturduğu özel bir koleksiyonu kökten silmesini sağlar.

#### **Modül D: Etkileşim ve İletişim (Yorum ve Paylaşım)**

*(İlanlar üzerindeki kullanıcı değerlendirmelerini ve dışa aktarımı yöneten modül)*

1. Yorum Ekleme
API Metodu: POST /cars/{carId}/comments
Açıklama: Kullanıcıların ilan altına soru sormak veya görüş belirtmek için metin tabanlı yorum yapmasını sağlar.
2. İlan Yorumlarını Listeleme
API Metodu: GET /cars/{carId}/comments
Açıklama: Bir ilana yapılmış olan tüm yorumların tarih sırasına göre listelenmesini sağlar.
3. Paylaşım Linki Üretme
API Metodu: GET /cars/{carId}/share
Açıklama: İlanın başka platformlarda (WhatsApp, Telegram vb.) paylaşılabilmesi için benzersiz ve izlenebilir bir kısa link üretilmesini sağlar.
4. Yorum Güncelleme
API Metodu: PUT /comments/{commentId}
Açıklama: Kullanıcının daha önceden yaptığı bir yorumdaki yazım hatalarını veya içeriği sonradan düzenlemesini sağlar.
5. Yorum Silme
API Metodu: DELETE /comments/{commentId}
Açıklama: Kullanıcının kendi yaptığı bir yorumu sistemden kalıcı olarak silmesini sağlar.
6. Kullanıcının Kendi Yorumlarını Listeleme
API Metodu: GET /users/{userId}/comments
Açıklama: Bir kullanıcının profil sayfasına girdiğinde, platformdaki farklı ilanlara yaptığı tüm geçmiş yorumları tek bir liste halinde görmesini sağlar.

## Gereksinim Dağılımları

1. [Burak Şişci'nin Gereksinimleri](ekip/Burak-Sisci/Burak-Sisci-Gereksinimler.md)
2. [Anıl Elmaz'ın Gereksinimleri](ekip/Anil-Elmaz/Anil-Elmaz-Gereksinimler.md)
3. [Mehmet Öz'ün Gereksinimleri](ekip/Mehmet-Oz/Mehmet-Oz-Gereksinimler.md)
4. [Mehmet Uludağ'ın Gereksinimleri](/ekip/Mehmet-Uludag/Mehmet-Uludag-Gereksinimler.md)