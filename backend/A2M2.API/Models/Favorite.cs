using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace A2M2.API.Models;

/// <summary>
/// Favori modeli — MongoDB koleksiyonu: favorites
/// Unique index: userId + listingId
/// </summary>
public class Favorite
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = null!;

    [BsonElement("listingId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ListingId { get; set; } = null!;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
