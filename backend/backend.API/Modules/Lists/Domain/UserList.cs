using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.API.Modules.Lists.Domain;

public class UserList
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("kullaniciId")]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("listeAdi")]
    public string ListName { get; set; } = string.Empty;

    [BsonElement("varsayilan")]
    public bool IsDefault { get; set; } = false;

    [BsonElement("ilanlar")]
    public List<string> Items { get; set; } = [];

    [BsonElement("olusturulmaTarihi")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("guncellemeTarihi")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
