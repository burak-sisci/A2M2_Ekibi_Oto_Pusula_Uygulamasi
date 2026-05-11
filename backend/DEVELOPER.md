# OtoPusula Backend — Geliştirici Analiz & Uygulama Planı

> Tarih: 2026-05-06  
> Hedef: Mevcut backend kodunu YAML API sözleşmesine tam uyumlu hale getirmek,  
> her geliştiricinin en az bir kez **RabbitMQ** ve **Redis** kullanmasını sağlamak,  
> tüm bunları **SOLID · GRASP · Clean Architecture** kuralları çerçevesinde yapmak.

---

## 1. Mevcut Durum Analizi

### 1.1 Proje Mimarisi

```
backend.API/
├── Modules/                  ← Dikey dilimleme (Vertical Slice / Modular Monolith)
│   ├── Auth/                 ← Burak Şişci
│   │   ├── Application/      ← Use-case'ler, interface'ler
│   │   ├── Domain/           ← User entity
│   │   └── Infrastructure/   ← Mongo + Redis implantasyonları
│   ├── Cars/                 ← Anıl Elmaz
│   ├── Comments/             ← Mehmet Uludağ
│   ├── Lists/                ← Mehmet Öz
│   └── Prediction/           ← Anıl Elmaz (ML proxy)
├── Shared/                   ← Ortak altyapı (DB, Events, Pagination, JWT)
└── Presentation/             ← Controllers + Middlewares
```

**Mevcut teknoloji yığını:** .NET 10, MongoDB, Redis (StackExchange.Redis), MediatR, BCrypt, JWT Bearer, Swagger/Swashbuckle

---

## 2. Eksik API & Kod Analizi

### 2.1 Modül A — Kullanıcı & Kimlik (Burak Şişci)

| Durum | Endpoint | Sorun |
|---|---|---|
| ✅ | POST /auth/register | Çalışıyor |
| ✅ | POST /auth/login | Çalışıyor |
| ✅ | POST /auth/logout | Çalışıyor |
| ❌ | GET /users/{userId} | Hiç yok — controller yok |
| ⚠️ | PUT /users/{userId} | Mevcut: `PUT /auth/profile` — URL YAML ile uyumsuz, `name` alanı güncellenemiyor |
| ⚠️ | DELETE /users/{userId} | Mevcut: `DELETE /auth/{id}` — URL YAML ile uyumsuz, yetki kontrolü yok |

**Domain Eksikliği:** `User` entity'sinde `Name`, `Gender`, `BirthDate` alanları yok.

**RabbitMQ Kullanımı (Burak):**  
`POST /auth/register` başarıyla tamamlanınca `UserRegisteredEvent` mesajı yayınlanacak.  
Ön uç e-posta bildirimi veya onboarding servisleri bu event'i tüketecek.

**Redis Kullanımı (Burak):**  
Token blacklist zaten Redis üzerinden çalışıyor ✅ (`RedisTokenBlacklist`).

---

### 2.2 Modül B — Araç İlanları & Yapay Zeka (Anıl Elmaz)

| Durum | Endpoint | Sorun |
|---|---|---|
| ✅ | POST /cars | Çalışıyor |
| ✅ | GET /cars | Çalışıyor (filtreleme + pagination) |
| ✅ | PUT /cars/{id} | Çalışıyor |
| ✅ | DELETE /cars/{id} | Çalışıyor |
| ✅ | POST /cars/predict-price | Çalışıyor |
| ❌ | GET /cars/{carId} | **Hiç yok** — controller action'ı yok |

**RabbitMQ Kullanımı (Anıl):**  
`POST /cars` ile yeni ilan eklenince `CarCreatedEvent` yayınlanacak.  
Bu event; ileride arama indexleme, bildirim veya AI öneri servisleri tarafından tüketilebilir.

**Redis Kullanımı (Anıl):**  
`GET /cars` filtrelenmiş listesi Redis'te 5 dakikalık TTL ile cache'lenecek.  
Cache key: filtre parametrelerinin hash'i + pagination değerleri.  
Yeni ilan eklenince (`POST /cars`) ilgili cache anahtarları temizlenecek (cache invalidation).

---

### 2.3 Modül C — Özel Listeler & Favoriler (Mehmet Öz)

| Durum | Endpoint | Sorun |
|---|---|---|
| ✅ | POST /lists | Çalışıyor |
| ❌ | GET /users/{userId}/lists | Hiç yok |
| ❌ | GET /lists/{listId} | Hiç yok |
| ❌ | PUT /lists/{listId} | Hiç yok |
| ⚠️ | POST /lists/{listId}/cars | Mevcut: `PUT /lists/{id}/items` — HTTP metodu ve URL yanlış |
| ❌ | DELETE /lists/{listId}/cars/{carId} | Hiç yok |
| ✅ | DELETE /lists/{listId} | Çalışıyor |

**Domain Eksikliği:** `UserList` entity'sinde `IsDefault` ve `UpdatedAt` alanları yok.

**RabbitMQ Kullanımı (Mehmet Öz):**  
`POST /lists/{listId}/cars` ile listeye araç eklenince `CarFavoritedEvent` yayınlanacak.  
Event; ileride ilan sahibine bildirim göndermek için kullanılabilir.

**Redis Kullanımı (Mehmet Öz):**  
`GET /users/{userId}/lists` sonucu (araç sayılarıyla birlikte liste özeti) Redis'te  
3 dakikalık TTL ile cache'lenecek. Liste güncellemelerinde cache temizlenecek.

---

### 2.4 Modül D — Etkileşim & İletişim (Mehmet Uludağ)

| Durum | Endpoint | Sorun |
|---|---|---|
| ✅ | POST /cars/{carId}/comments | Çalışıyor |
| ✅ | GET /cars/{carId}/comments | Çalışıyor |
| ⚠️ | PUT /comments/{commentId} | Mevcut: `PUT /cars/{carId}/comments/{id}` — URL farklı |
| ⚠️ | DELETE /comments/{commentId} | Mevcut: `DELETE /cars/{carId}/comments/{id}` — URL farklı |
| ❌ | GET /users/{userId}/comments | Hiç yok |
| ❌ | GET /cars/{carId}/share | Hiç yok |
| ❌ | POST /comments/{commentId}/like | Hiç yok (YAML'da tanımlı) |
| ❌ | DELETE /comments/{commentId}/like | Hiç yok (YAML'da tanımlı) |

**Domain Eksikliği:** `Comment` entity'sinde `LikeCount`, `LikedByUsers` (beğeni listesi) alanları yok.

**RabbitMQ Kullanımı (Mehmet Uludağ):**  
`POST /cars/{carId}/comments` ile yorum eklenince `CommentCreatedEvent` yayınlanacak.  
Event; ilan sahibine bildirim göndermek için kullanılabilir.

**Redis Kullanımı (Mehmet Uludağ):**  
`GET /cars/{carId}/share` ile üretilen paylaşım linkleri Redis'te saklanacak (90 gün TTL).  
Link kısaltma ve izlenebilirlik Redis üzerinden yönetilecek.

---

## 3. Yapılacaklar Listesi (Uygulama Sırası)

### Adım 1 — RabbitMQ Altyapısı (Shared)
- [ ] `RabbitMQ.Client` NuGet paketi eklenmesi
- [ ] `Shared/Messaging/IRabbitMqPublisher.cs` — interface
- [ ] `Shared/Messaging/RabbitMqPublisher.cs` — implementasyon (durable queue, JSON serialization)
- [ ] `Shared/Messaging/RabbitMqConsumerBase.cs` — base consumer class
- [ ] `Program.cs`'e RabbitMQ singleton DI kaydı
- [ ] `.env`'e `RABBITMQ_CONNECTION_STRING` eklenmesi

### Adım 2 — Domain Güncellemeleri
- [ ] `User` entity: `Name`, `Gender`, `BirthDate` alanları eklenmesi
- [ ] `UserList` entity: `IsDefault`, `UpdatedAt` alanları eklenmesi
- [ ] `Comment` entity: `LikeCount`, `LikedByUsers` (List<string>) alanları eklenmesi

### Adım 3 — Modül A (Burak Şişci)
- [ ] `UsersController.cs` — `/users/{userId}` route'ları için yeni controller
  - [ ] `GET /users/{userId}` — GetUserQuery
  - [ ] `PUT /users/{userId}` — UpdateUserCommand (name, phone güncelleme)
  - [ ] `DELETE /users/{userId}` — DeleteUserCommand (yetki kontrolü eklenmesi)
- [ ] `RegisterUserCommand` içinde `UserRegisteredEvent` yayınlanması
- [ ] `UserRegisteredEventConsumer.cs` — event consumer (loglama / bildirim)

### Adım 4 — Modül B (Anıl Elmaz)
- [ ] `CarsController` — `GET /cars/{carId}` action eklenmesi
- [ ] `GetCarByIdQuery.cs` oluşturulması
- [ ] `AddCarCommand` içinde `CarCreatedEvent` yayınlanması
- [ ] `CarCreatedEventConsumer.cs` — event consumer
- [ ] `ICarCacheService.cs` + `RedisCarCacheService.cs` — cache implementasyonu
- [ ] `GetCarsQuery` Redis cache entegrasyonu (read-through + invalidation)

### Adım 5 — Modül C (Mehmet Öz)
- [ ] `ListsController` route güncellemesi + eksik endpointler:
  - [ ] `GET /users/{userId}/lists` — GetUserListsQuery (UsersController'a da eklenecek)
  - [ ] `GET /lists/{listId}` — GetListDetailQuery
  - [ ] `PUT /lists/{listId}` — UpdateListNameCommand (IsDefault koruması ile)
  - [ ] `POST /lists/{listId}/cars` — AddCarToListCommand (PUT→POST düzeltme)
  - [ ] `DELETE /lists/{listId}/cars/{carId}` — RemoveCarFromListCommand
- [ ] `IListRepository`'e eksik metotlar eklenmesi
- [ ] `AddCarToListCommand` içinde `CarFavoritedEvent` yayınlanması
- [ ] `CarFavoritedEventConsumer.cs` — event consumer
- [ ] `IListCacheService.cs` + `RedisListCacheService.cs` — liste özeti cache

### Adım 6 — Modül D (Mehmet Uludağ)
- [ ] `IndependentCommentsController.cs` — `/comments/{commentId}` route'ları:
  - [ ] `PUT /comments/{commentId}` — UpdateCommentCommand
  - [ ] `DELETE /comments/{commentId}` — DeleteCommentCommand
  - [ ] `POST /comments/{commentId}/like` — LikeCommentCommand
  - [ ] `DELETE /comments/{commentId}/like` — UnlikeCommentCommand
- [ ] `UsersController`'a `GET /users/{userId}/comments` eklenmesi
- [ ] `CarsController`'a `GET /cars/{carId}/share` eklenmesi
- [ ] `ICommentRepository`'e `GetByUserIdAsync`, `LikeAsync`, `UnlikeAsync` eklenmesi
- [ ] `AddCommentCommand` içinde `CommentCreatedEvent` yayınlanması
- [ ] `CommentCreatedEventConsumer.cs` — event consumer
- [ ] `IShareLinkService.cs` + `RedisShareLinkService.cs` — share link Redis implementasyonu

### Adım 7 — Program.cs Kayıtları
- [ ] Tüm yeni servisler, komutlar ve consumer'ların DI'ya kaydı
- [ ] RabbitMQ ve yeni Redis servislerinin kaydı

---

## 4. Mimari Kararlar

### 4.1 SOLID Prensipleri

| Prensip | Uygulama |
|---|---|
| **S**ingle Responsibility | Her Command/Query sınıfı tek bir use-case'i yönetir |
| **O**pen/Closed | `IRabbitMqPublisher` interface'i genişletilebilir; event tipleri `record` ile kapalı |
| **L**iskov Substitution | `MongoXxxRepository` → `IXxxRepository` → Controller bağımlılığı |
| **I**nterface Segregation | `ICarCacheService`, `IShareLinkService` — ince, amaca özel interface'ler |
| **D**ependency Inversion | Controller'lar concrete sınıflara değil interface'lere bağımlı |

### 4.2 GRASP Prensipleri

| Prensip | Uygulama |
|---|---|
| **Information Expert** | Bir listenin IsDefault kontrolü `UserList` entity'si içinde |
| **Creator** | `Car`, `Comment`, `UserList` entity'lerini ilgili Command sınıfları oluşturur |
| **Controller** | HTTP katmanı sadece Presentation; iş mantığı Application'da |
| **Low Coupling** | RabbitMQ event'leri modüller arası doğrudan bağımlılığı ortadan kaldırır |
| **High Cohesion** | Her modül kendi domain, application, infrastructure'ını içerir |
| **Pure Fabrication** | `RabbitMqPublisher`, `RedisCarCacheService` — domain modeline ait değil ama gerekli |
| **Indirection** | `IRabbitMqPublisher` ve `IXxxCacheService` interface'leri dolaylı bağlantı |
| **Polymorphism** | Consumer'lar `RabbitMqConsumerBase<TEvent>` generic base'den türer |

### 4.3 Clean Architecture Katmanları

```
Presentation  →  Application  →  Domain
     ↓                ↓
Infrastructure  →  Shared (Events, DB, Messaging, Cache)
```

- **Domain**: Saf entity'ler, value object'ler — dış bağımlılık yok
- **Application**: Use-case'ler (Command/Query), interface kontratları — sadece domain'e bağımlı
- **Infrastructure**: MongoDB, Redis, RabbitMQ implementasyonları — interface'leri implante eder
- **Presentation**: Controller'lar — Application use-case'lerini orkestre eder

---

## 5. RabbitMQ Event Akışı

```
[POST /auth/register]  →  UserRegisteredEvent  →  UserRegisteredConsumer (loglama/bildirim)
[POST /cars]           →  CarCreatedEvent       →  CarCreatedConsumer     (index/öneri)
[POST /lists/{id}/cars]→  CarFavoritedEvent     →  CarFavoritedConsumer   (ilan sahibi bildirimi)
[POST /cars/{id}/comments] → CommentCreatedEvent → CommentCreatedConsumer (ilan sahibi bildirimi)
```

**Event yapısı (record pattern):**
```csharp
public record UserRegisteredEvent(string UserId, string Email, string Name, DateTime OccurredAt);
public record CarCreatedEvent(string CarId, string OwnerId, string Brand, string Model, DateTime OccurredAt);
public record CarFavoritedEvent(string CarId, string UserId, string ListId, DateTime OccurredAt);
public record CommentCreatedEvent(string CommentId, string CarId, string AuthorId, DateTime OccurredAt);
```

---

## 6. Redis Kullanım Özeti

| Modül | Kullanım | Key Pattern | TTL |
|---|---|---|---|
| Auth (Burak) | Token blacklist | `blacklist:{token}` | Token süresine eşit |
| Cars (Anıl) | Liste cache | `cars:list:{hash(filter+page)}` | 5 dakika |
| Lists (Mehmet Öz) | Kullanıcı listeleri özeti | `lists:user:{userId}` | 3 dakika |
| Comments (Mehmet Uludağ) | Paylaşım linkleri | `share:{carId}` | 90 gün |

---

## 7. Geliştirici Sorumluluk Matrisi

| Geliştirici | Modül | Eksik API'ler | RabbitMQ Rolü | Redis Rolü |
|---|---|---|---|---|
| **Burak Şişci** | Auth / Users | GET, PUT, DELETE /users/{userId} | Publisher (UserRegistered) | Token Blacklist ✅ |
| **Anıl Elmaz** | Cars / Prediction | GET /cars/{carId} | Publisher (CarCreated) | Cars List Cache |
| **Mehmet Öz** | Lists | 5 eksik endpoint | Publisher (CarFavorited) | User Lists Cache |
| **Mehmet Uludağ** | Comments | 6 eksik endpoint + route düzeltme | Publisher (CommentCreated) | Share Link Cache |

---

## 8. Ortam Değişkenleri (.env)

Mevcut `.env`'e aşağıdaki satırlar eklenecek:

```env
RABBITMQ_CONNECTION_STRING=amqp://guest:guest@localhost:5672/
```

---

## 9. Önemli Notlar

1. **Türkçe/İngilizce karışıklığı**: Mevcut kodda `Marka`, `Fiyat`, `Konum` gibi Türkçe alan isimleri var. YAML sözleşmesi `brand`, `price`, `location` kullanıyor. Yeni eklenen kodlarda YAML'a uygun İngilizce isimler kullanılacak; mevcut alanlar dokunulmadan bırakılacak (breaking change riski).

2. **Controller route çakışması**: `CommentsController` şu an `/cars/{carId}/comments` route'una bağlı. YAML'ın gerektirdiği `/comments/{commentId}` route'ları için **ayrı bir controller** oluşturulacak.

3. **MediatR / Service hybrid**: Proje bazı yerlerde MediatR (`IMediator.Send`), bazı yerlerde doğrudan servis injection kullanıyor. Yeni kod MediatR pattern'ini tercih edecek.

4. **`[Authorize]` eksikliği**: `CarsController`'da `//[Authorize]` yoruma alınmış. Yeni endpointlerde YAML sözleşmesine uygun şekilde kimlik doğrulama zorunlu tutulacak.
