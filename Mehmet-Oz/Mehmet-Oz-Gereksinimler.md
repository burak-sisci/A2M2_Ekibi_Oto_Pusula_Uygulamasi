#### **Modül C: Özel Listeler ve Favoriler**

*(Kullanıcıların kendi koleksiyonlarını yönettiği modül)*

1. Yeni Liste Oluşturma
    * API Metodu: POST /lists
    * Açıklama: Kullanıcıların "Alınacaklar", "Kıyaslanacaklar" gibi isimlerle yeni koleksiyonlar oluşturmasını sağlar (Favoriler listesi kayıt anında otomatik açılır).
2. Listeye İlan Ekleme
    * API Metodu: POST /lists/{listId}/cars
    * Açıklama: Beğenilen bir araç ilanının, kullanıcının seçtiği spesifik bir listeye (veya default Favoriler listesine) eklenmesini sağlar.
3. Kullanıcının Listelerini Görüntüleme
    * API Metodu: GET /users/{userId}/lists
    * Açıklama: Kullanıcının sahip olduğu tüm listelerin ve içlerindeki araç sayılarının getirilmesini sağlar.
4. Liste İçeriğini Görüntüleme
    * API Metodu: GET /lists/{listId}
    * Açıklama: Spesifik bir listenin içine girildiğinde, o listedeki tüm araç ilanlarının detaylarıyla birlikte ekrana getirilmesini sağlar.
5. Liste İsmini Güncelleme
    * API Metodu: PUT /lists/{listId}
    * Açıklama: Kullanıcının daha önce oluşturduğu bir listenin adını değiştirmesini sağlar (Favoriler hariç).
6. Listeden İlan Çıkarma
    * API Metodu: DELETE /lists/{listId}/cars/{carId}
    * Açıklama: Önceden listeye eklenmiş bir aracın sadece o listeden çıkarılmasını sağlar (İlanın kendisi silinmez).
7. Listeyi Tamamen Silme
    * API Metodu: DELETE /lists/{listId}
    * Açıklama: Kullanıcının oluşturduğu özel bir koleksiyonu kökten silmesini sağlar.