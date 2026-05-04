// Uygulama geneli Türkçe string sabitleri.
// Çoklu dil gerektiğinde flutter_localizations + ARB'ye geçilecek (developer.md §10).
class AppStrings {
  AppStrings._();

  // Genel
  static const appName = 'OtoPusula';
  static const loading = 'Yükleniyor…';
  static const retry = 'Tekrar Dene';
  static const cancel = 'İptal';
  static const confirm = 'Onayla';
  static const delete = 'Sil';
  static const edit = 'Düzenle';
  static const save = 'Kaydet';
  static const close = 'Kapat';
  static const yes = 'Evet';
  static const no = 'Hayır';
  static const unknown = 'Bilinmiyor';

  // Hata mesajları (kullanıcıya Türkçe — developer.md §13, kural 6)
  static const errorGeneric = 'Bir hata oluştu. Lütfen tekrar deneyin.';
  static const errorNetwork = 'İnternet bağlantısı kurulamadı.';
  static const errorUnauthorized = 'Oturum süreniz doldu. Lütfen tekrar giriş yapın.';
  static const errorNotFound = 'Aradığınız içerik bulunamadı.';
  static const errorServer = 'Sunucu hatası. Lütfen daha sonra tekrar deneyin.';
  static const errorForbidden = 'Bu işlem için yetkiniz bulunmuyor.';
  static const errorConflict = 'Bu bilgi zaten kayıtlı.';

  // Auth
  static const loginTitle = 'Giriş Yap';
  static const registerTitle = 'Üye Ol';
  static const logoutTitle = 'Çıkış Yap';
  static const logoutConfirm = 'Çıkış yapmak istediğinize emin misiniz?';
  static const emailLabel = 'E-posta';
  static const phoneLabel = 'Telefon';
  static const passwordLabel = 'Şifre';
  static const nameLabel = 'Ad Soyad';
  static const genderLabel = 'Cinsiyet';
  static const birthDateLabel = 'Doğum Tarihi';
  static const identifierLabel = 'E-posta veya Telefon';
  static const loginButton = 'Giriş Yap';
  static const registerButton = 'Üye Ol';
  static const noAccount = 'Hesabınız yok mu? Üye olun';
  static const hasAccount = 'Zaten hesabınız var mı? Giriş yapın';

  // İlanlar
  static const carsTitle = 'İlanlar';
  static const carCreateTitle = 'Yeni İlan';
  static const carDetailTitle = 'İlan Detayı';
  static const brandLabel = 'Marka';
  static const modelLabel = 'Model';
  static const yearLabel = 'Yıl';
  static const kmLabel = 'Kilometre';
  static const fuelTypeLabel = 'Yakıt Türü';
  static const gearTypeLabel = 'Vites Türü';
  static const priceLabel = 'Fiyat (₺)';
  static const cityLabel = 'İl';
  static const districtLabel = 'İlçe';
  static const descriptionLabel = 'Açıklama';
  static const damageInfoLabel = 'Hasar Bilgisi';
  static const imagesLabel = 'Fotoğraflar';
  static const filterTitle = 'Filtrele';
  static const emptyCarList = 'Henüz ilan bulunamadı.';
  static const deleteCarConfirm = 'Bu ilanı silmek istediğinize emin misiniz?';

  // Fiyat Tahmini
  static const pricePredictTitle = 'Fiyat Tahmini';
  static const estimatedPrice = 'Tahmini Fiyat';
  static const priceRange = 'Fiyat Aralığı';
  static const confidence = 'Güven Skoru';

  // Listeler
  static const listsTitle = 'Listelerim';
  static const listCreateTitle = 'Yeni Liste';
  static const listNameLabel = 'Liste Adı';
  static const emptyList = 'Bu liste boş.';
  static const addToList = 'Listeye Ekle';
  static const removeFromList = 'Listeden Çıkar';
  static const deleteListConfirm = 'Bu listeyi silmek istediğinize emin misiniz?';
  static const defaultListName = 'Favoriler';

  // Yorumlar
  static const commentsTitle = 'Yorumlar';
  static const commentPlaceholder = 'Yorumunuzu yazın…';
  static const sendComment = 'Gönder';
  static const emptyComments = 'Henüz yorum yok. İlk yorumu siz yapın!';
  static const deleteCommentConfirm = 'Bu yorumu silmek istediğinize emin misiniz?';

  // Profil
  static const profileTitle = 'Profil';
  static const editProfileTitle = 'Profili Düzenle';
  static const deleteAccountTitle = 'Hesabı Sil';
  static const deleteAccountConfirm = 'Hesabınızı silmek istediğinize emin misiniz? Bu işlem geri alınamaz.';

  // Paylaşım
  static const shareLink = 'Paylaşım Linki';
  static const shareLinkCopied = 'Link kopyalandı!';
}
