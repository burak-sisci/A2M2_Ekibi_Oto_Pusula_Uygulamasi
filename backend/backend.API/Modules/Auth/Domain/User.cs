using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.API.Modules.Auth.Domain;

public enum Cinsiyet { Erkek, Kadin, BelirtmekIstemiyorum }

[BsonIgnoreExtraElements]
public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("ad")]
    public string Ad { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("telefon")]
    public string Phone { get; set; } = string.Empty;

    [BsonElement("sifreHash")]
    public string PasswordHash { get; set; } = string.Empty;

    [BsonElement("cinsiyet")]
    [BsonRepresentation(BsonType.String)]
    [BsonIgnoreIfNull]
    public Cinsiyet? Cinsiyet { get; set; }

    [BsonElement("dogumTarihi")]
    [BsonIgnoreIfNull]
    public DateTime? DogumTarihi { get; set; }

    [BsonElement("olusturulmaTarihi")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("guncellemeTarihi")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("resetToken")]
    [BsonIgnoreIfNull]
    public string? ResetToken { get; set; }

    [BsonElement("resetTokenExpires")]
    [BsonIgnoreIfNull]
    public DateTime? ResetTokenExpires { get; set; }
}
