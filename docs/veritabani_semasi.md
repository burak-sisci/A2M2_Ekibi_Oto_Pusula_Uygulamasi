#### 1. Users Koleksiyonu (Modül A: Kullanıcı Yönetimi)

Sisteme kayıt olan kullanıcıların temel ve kişisel bilgilerinin tutulduğu şemadır.

* `_id`: ObjectId (Otomatik atanır)
* `name`: String (Zorunlu)
* `email`: String (Zorunlu, Benzersiz)
* `password`: String (Zorunlu, Hashlenmiş olarak tutulur)
* `phone`: String (Zorunlu, Benzersiz)
* `gender`: String (Opsiyonel - Örn: "Erkek", "Kadın", "Belirtmek İstemiyorum")
* `birthDate`: Date (Opsiyonel)
* `createdAt` & `updatedAt`: Timestamp (Kayıt ve güncellenme tarihleri)

#### 2. Cars (Listings) Koleksiyonu (Modül B: İlanlar)

Araç ilanlarının tüm detaylarını barındıran, projenin en geniş şemasıdır.

* `_id`: ObjectId
* `userId`: ObjectId (Referans: Users -> İlanı kimin verdiğini belirtir)
* `brand`: String (Zorunlu - Örn: Toyota)
* `model`: String (Zorunlu - Örn: Corolla)
* `year`: Number (Zorunlu - Örn: 2020)
* `km`: Number (Zorunlu - Örn: 45000)
* `fuelType`: String (Zorunlu - Benzin, Dizel, LPG, Elektrik, Hibrit)
* `gearType`: String (Zorunlu - Manuel, Otomatik, Yarı Otomatik)
* `price`: Number (Zorunlu)
* `description`: String (İlan detay metni)
* `images`: Array of Strings (Fotoğraf URL'lerini tutan liste)
* `location`: Object (Örn: `{ city: "İstanbul", district: "Kadıköy" }`)
* `damageInfo`: Array (Örn: `["Kaput Değişen", "Sağ Çamurluk Boyalı"]`)
* `createdAt` & `updatedAt`: Timestamp

#### 3. Lists Koleksiyonu (Modül C: Listeler ve Favoriler)

Kullanıcıların beğendikleri araçları grupladığı (Favoriler vb.) koleksiyondur. *(Not: Her kullanıcı kayıt olduğunda sistem otomatik olarak `isDefault: true` olan bir "Favoriler" listesi oluşturmalıdır).*

* `_id`: ObjectId
* `userId`: ObjectId (Referans: Users -> Listenin sahibini belirtir)
* `name`: String (Zorunlu - Örn: "Favoriler", "Almayı Düşündüklerim")
* `isDefault`: Boolean (Varsayılan liste mi? Favoriler için `true`, diğerleri için `false`)
* `cars`: Array of ObjectIds (Referans: Cars -> Listeye eklenen araçların ID'lerini tutan dizi)
* `createdAt` & `updatedAt`: Timestamp

#### 4. Comments Koleksiyonu (Modül D: Sosyal Etkileşim)

İlanlara yapılan yorumların, ilandan ve kullanıcıdan bağımsız ama onlara referanslı olarak tutulduğu şemadır.

* `_id`: ObjectId
* `userId`: ObjectId (Referans: Users -> Yorumu kimin yaptığını belirtir)
* `carId`: ObjectId (Referans: Cars -> Yorumun hangi ilana yapıldığını belirtir)
* `text`: String (Zorunlu - Yorumun içeriği)
* `createdAt` & `updatedAt`: Timestamp (Yorumun yapılma ve son düzenlenme tarihi)