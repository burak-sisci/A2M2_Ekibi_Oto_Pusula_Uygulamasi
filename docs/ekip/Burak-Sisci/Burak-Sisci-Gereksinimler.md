#### **Modül A: Kullanıcı ve Kimlik Yönetimi**

*(Üye olma ve profil süreçlerini yöneten temel modül)*

1. Üye Olma
    * API Metodu: POST /auth/register
    * Açıklama: Kullanıcıların sisteme kayıt olmasını sağlar. İsim, email, telefon numarası, cinsiyet*, doğum tarihi* ve şifre alınarak veritabanında yeni bir kullanıcı oluşturulur.
2. Kullanıcı Girişi (Login)
    * API Metodu: POST /auth/login
    * Açıklama: Kullanıcının hem email veya telefon numarası hemde şifresiyle sisteme girmesini sağlar. Başarılı girişte Token (yetki belgesi) döndürür.
3. Profil Görüntüleme
    * API Metodu: GET /users/{userId}
    * Açıklama: Kullanıcının profil bilgilerini (ad, email, katılım tarihi) getiren listeleme/okuma metodudur.
4. Profil Güncelleme
    * API Metodu: PUT /users/{userId}
    * Açıklama: Kullanıcının şifre, telefon veya isim gibi kişisel bilgilerini değiştirmesini/güncellemesini sağlar.
5. Hesap Silme
    * API Metodu: DELETE /users/{userId}
    * Açıklama: Kullanıcının kendi isteğiyle hesabını ve sistemdeki tüm kişisel verilerini kalıcı olarak silmesini sağlar.
6. Kullanıcı Çıkışı (Logout)
    * API Metodu: POST /auth/logout
    * Açıklama: Sisteme giriş yapmış olan kullanıcının oturumunu güvenli bir şekilde sonlandırmasını sağlar. Kullanıcının cihazındaki aktif erişim token'ı (yetki belgesi) silinerek/geçersiz kılınarak başkalarının hesaba erişmesi engellenir.