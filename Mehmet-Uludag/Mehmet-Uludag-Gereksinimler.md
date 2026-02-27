#### **Modül D: Etkileşim ve İletişim (Yorum ve Paylaşım)**

*(İlanlar üzerindeki kullanıcı değerlendirmelerini ve dışa aktarımı yöneten modül)*

1. Yorum Ekleme
    * API Metodu: POST /cars/{carId}/comments
    * Açıklama: Kullanıcıların ilan altına soru sormak veya görüş belirtmek için metin tabanlı yorum yapmasını sağlar.
2. İlan Yorumlarını Listeleme
    * API Metodu: GET /cars/{carId}/comments
    * Açıklama: Bir ilana yapılmış olan tüm yorumların tarih sırasına göre listelenmesini sağlar.
3. Paylaşım Linki Üretme
    * API Metodu: GET /cars/{carId}/share
    * Açıklama: İlanın başka platformlarda (WhatsApp, Telegram vb.) paylaşılabilmesi için benzersiz ve izlenebilir bir kısa link üretilmesini sağlar.
4. Yorum Güncelleme
    * API Metodu: PUT /comments/{commentId}
    * Açıklama: Kullanıcının daha önceden yaptığı bir yorumdaki yazım hatalarını veya içeriği sonradan düzenlemesini sağlar.
5. Yorum Silme
    * API Metodu: DELETE /comments/{commentId}
    * Açıklama: Kullanıcının kendi yaptığı bir yorumu sistemden kalıcı olarak silmesini sağlar.
6. Kullanıcının Kendi Yorumlarını Listeleme
    * API Metodu: GET /users/{userId}/comments
    * Açıklama: Bir kullanıcının profil sayfasına girdiğinde, platformdaki farklı ilanlara yaptığı tüm geçmiş yorumları tek bir liste halinde görmesini sağlar.