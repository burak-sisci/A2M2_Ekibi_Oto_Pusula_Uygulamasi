using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace A2M2.API.Models;

/// <summary>
/// Kullanıcı modeli — MongoDB koleksiyonu: users
/// </summary>
public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("name")]
    public string Name { get; set; } = null!;

    [BsonElement("email")]
    public string Email { get; set; } = null!;

    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } = null!;

    [BsonElement("phone")]
    public string Phone { get; set; } = null!;

    [BsonElement("gender")]
    public string? Gender { get; set; } // Erkek, Kadın, Belirtmek İstemiyorum

    [BsonElement("birthDate")]
    public DateTime? BirthDate { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
