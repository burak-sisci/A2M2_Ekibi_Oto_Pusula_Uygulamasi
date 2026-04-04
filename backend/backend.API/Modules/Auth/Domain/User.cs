using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.API.Modules.Auth.Domain;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("phone")]
    public string Phone { get; set; } = string.Empty;

    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } = string.Empty;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [BsonElement("resetToken")]
    [BsonIgnoreIfNull]
    public string? ResetToken { get; set; }

    [BsonElement("resetTokenExpires")]
    [BsonIgnoreIfNull]
    public DateTime? ResetTokenExpires { get; set; }
}
