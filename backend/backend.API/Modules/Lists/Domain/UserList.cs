using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.API.Modules.Lists.Domain;

public class UserList
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("userId")]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("listName")]
    public string ListName { get; set; } = string.Empty;

    [BsonElement("items")]
    public List<string> Items { get; set; } = [];

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
