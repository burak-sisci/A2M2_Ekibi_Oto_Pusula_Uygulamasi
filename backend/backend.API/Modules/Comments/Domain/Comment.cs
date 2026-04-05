using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.API.Modules.Comments.Domain;

public class Comment
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id {get;set;}=ObjectId.GenerateNewId().ToString();

    [BsonElement("userId")]
    public string? UserId {get;set;}

    [BsonElement("carId")]
    public string? CarId {get;set;}

    [BsonElement("content")]
    public string? Content { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }=DateTime.UtcNow;
}