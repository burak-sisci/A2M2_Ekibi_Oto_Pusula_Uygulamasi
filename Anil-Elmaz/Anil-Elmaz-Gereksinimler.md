#### **Modül B: Araç İlanları ve Yapay Zeka**

*(Projenin bel kemiği; ilanlar, kazıma ve fiyat tahmin modülü)*

1. Yeni İlan Ekleme
    * API Metodu: POST /cars
    * Açıklama: Aracın teknik özellikleri (marka, km, boya vb.), konumu ve fotoğraflarıyla birlikte sisteme yeni bir ilan olarak eklenmesini sağlar.
2. Fiyat Tahmini Alma (ML)
    * API Metodu: POST /cars/predict-price
    * Açıklama: Kullanıcının gönderdiği araç özelliklerinin makine öğrenmesi modeline sokularak, tahmini piyasa değerinin hesaplanıp geri döndürülmesini sağlar.
3. İlanları Filtreleme
    * API Metodu: GET /cars
    * Açıklama: Sistemdeki araç ilanlarının fiyat, kilometre, marka ve konuma göre filtrelenerek listelenmesini sağlar.
4. İlan Detayı Görüntüleme
    * API Metodu: GET /cars/{carId}
    * Açıklama: Seçilen tek bir araca ait tüm resimlerin, donanım detaylarının ve açıklama metinlerinin getirilmesini sağlar.
5. Araç Görseli Yükleme
    * API Metodu: POST /api/upload/image
    * Açıklama: Kullanıcının ilan oluştururken telefon galerisinden seçtiği araç fotoğraflarının sunucuya yüklenmesini sağlar. Yüklenen her görsel için benzersiz bir URL üretilir; bu URL'ler ilan oluşturma isteğinin `resimler` alanına eklenerek araçla ilişkilendirilir. Aynı ilana birden fazla görsel (en fazla 8) yüklenebilir.
6. İlan Silme
    * API Metodu: DELETE /cars/{carId}
    * Açıklama: Satışı gerçekleşen veya kaldırılmak istenen ilanın sistemden silinmesini sağlar.