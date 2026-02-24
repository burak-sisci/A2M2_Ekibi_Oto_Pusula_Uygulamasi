using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace A2M2.API.Models;

/// <summary>
/// Kullanıcı listesi modeli — MongoDB koleksiyonu: lists
/// Favoriler listesi kullanıcı kaydında otomatik oluşturulur (isDefault: true)
/// </summary>
public class UserList
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = null!;

    [BsonElement("name")]
    public string Name { get; set; } = null!;

    [BsonElement("isDefault")]
    public bool IsDefault { get; set; } = false;

    [BsonElement("cars")]
    public List<string> Cars { get; set; } = new(); // Array of ObjectId strings

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
