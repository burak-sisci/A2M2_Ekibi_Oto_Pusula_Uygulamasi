#### **Modül A: Kullanıcı ve Kimlik Yönetimi**

*(Üye olma ve profil süreçlerini yöneten temel modül)*

1. Üye Olma
API Metodu: POST /auth/register
Açıklama: Kullanıcıların sisteme kayıt olmasını sağlar. İsim, email ve şifre alınarak veritabanında yeni bir kullanıcı oluşturulur.
2. Kullanıcı Girişi (Login)
API Metodu: POST /auth/login
Açıklama: Kullanıcının email ve şifresiyle doğrulama yapıp sisteme girmesini sağlar. Başarılı girişte Token (yetki belgesi) döndürür.
3. Profil Görüntüleme
API Metodu: GET /users/{userId}
Açıklama: Kullanıcının profil bilgilerini (ad, email, katılım tarihi) getiren listeleme/okuma metodudur.
4. Tüm Kullanıcıları Listeleme
API Metodu: GET /users
Açıklama: Sistemdeki tüm kayıtlı kullanıcıların liste halinde getirilmesini sağlar (Genellikle admin paneli için kullanılır).
5. Profil Güncelleme
API Metodu: PUT /users/{userId}
Açıklama: Kullanıcının şifre, telefon veya isim gibi kişisel bilgilerini değiştirmesini/güncellemesini sağlar.
6. Hesap Silme
API Metodu: DELETE /users/{userId}
Açıklama: Kullanıcının kendi isteğiyle hesabını ve sistemdeki tüm kişisel verilerini kalıcı olarak silmesini sağlar.

#### **Modül B: Araç İlanları ve Yapay Zeka**

*(Projenin bel kemiği; ilanlar, kazıma ve fiyat tahmin modülü)*

1. Yeni İlan Ekleme
API Metodu: POST /cars
Açıklama: Aracın teknik özellikleri (marka, km, boya vb.), konumu ve fotoğraflarıyla birlikte sisteme yeni bir ilan olarak eklenmesini sağlar.
2. Fiyat Tahmini Alma (ML)
API Metodu: POST /cars/predict-price
Açıklama: Kullanıcının gönderdiği araç özelliklerinin makine öğrenmesi modeline sokularak, tahmini piyasa değerinin hesaplanıp geri döndürülmesini sağlar.
3. Veri Kazıma (Scraping) Tetikleme
API Metodu: POST /cars/scrape
Açıklama: Sahibinden.com gibi kaynaklardan güncel ilan verilerinin çekilip ML modelini beslemek üzere veritabanına kaydedilmesi işlemini başlatır.
4. İlanları Filtreleme ve Listeleme
API Metodu: GET /cars
Açıklama: Sistemdeki araç ilanlarının fiyat, kilometre, marka ve konuma göre filtrelenerek listelenmesini sağlar.
5. İlan Detayı Görüntüleme
API Metodu: GET /cars/{carId}
Açıklama: Seçilen tek bir araca ait tüm resimlerin, donanım detaylarının ve açıklama metinlerinin getirilmesini sağlar.
6. İlan Güncelleme
API Metodu: PUT /cars/{carId}
Açıklama: İlan sahibinin araç fiyatını veya açıklamasını sonradan düzenlemesini sağlar.
7. İlan Silme
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

#### **Modül D: Sosyal Etkileşim (Yorum, Beğeni, Paylaşım)**

*(İlanların dinamikleştiği modül)*

1. Yorum Ekleme
API Metodu: POST /cars/{carId}/comments
Açıklama: Kullanıcıların ilan altına soru sormak veya görüş belirtmek için yorum yapmasını sağlar.
2. İlanı Beğenme (Like)
API Metodu: POST /cars/{carId}/likes
Açıklama: Kullanıcıların bir ilana beğeni atmasını sağlayarak ilanın popülerliğini artırır.
3. Yorumları Listeleme
API Metodu: GET /cars/{carId}/comments
Açıklama: Bir ilana yapılmış olan tüm yorumların tarih sırasına ve yorumu yapan kişiye göre listelenmesini sağlar.
4. Paylaşım Linki Üretme
API Metodu: GET /cars/{carId}/share
Açıklama: İlanın başka platformlarda (WhatsApp vb.) paylaşılabilmesi için benzersiz ve izlenebilir bir kısa link üretilmesini sağlar.
5. Yorum Güncelleme
API Metodu: PUT /comments/{commentId}
Açıklama: Kullanıcının daha önceden yaptığı bir yorumdaki yazım hatalarını veya içeriği düzenlemesini sağlar.
6. Yorum Silme
API Metodu: DELETE /comments/{commentId}
Açıklama: Kullanıcının kendi yorumunu veya ilan sahibinin kendi ilanındaki istenmeyen bir yorumu silmesini sağlar.
7. Beğeniyi Kaldırma (Unlike)
API Metodu: DELETE /cars/{carId}/likes
Açıklama: Kullanıcının daha önceden beğendiği bir ilandan beğenisini geri çekmesini sağlar.