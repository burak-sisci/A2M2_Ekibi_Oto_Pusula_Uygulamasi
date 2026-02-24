using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace A2M2.API.Models;

/// <summary>
/// Yorum modeli — MongoDB koleksiyonu: comments
/// </summary>
public class Comment
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = null!;

    [BsonElement("carId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string CarId { get; set; } = null!;

    [BsonElement("text")]
    public string Text { get; set; } = null!;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
