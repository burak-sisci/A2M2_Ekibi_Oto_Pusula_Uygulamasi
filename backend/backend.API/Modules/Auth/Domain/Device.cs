using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.API.Modules.Auth.Domain;

/// <summary>
/// Mobil cihazın FCM token kaydı. Bir kullanıcı birden çok cihaza sahip olabilir.
/// Notification Service yorum bildirimi gönderirken bu koleksiyondan token'ları çeker.
/// </summary>
[BsonIgnoreExtraElements]
public class Device
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("userId")]
    public string UserId { get; set; } = default!;

    [BsonElement("fcmToken")]
    public string FcmToken { get; set; } = default!;

    [BsonElement("platform")]
    public string Platform { get; set; } = "android";

    [BsonElement("lastSeenAt")]
    public DateTime LastSeenAt { get; set; } = DateTime.UtcNow;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
