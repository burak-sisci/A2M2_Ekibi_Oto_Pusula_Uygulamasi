using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.API.Modules.Comments.Domain;

public class Comment
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("kullaniciId")]
    public string? UserId { get; set; }

    [BsonElement("ilanId")]
    public string? CarId { get; set; }

    [BsonElement("icerik")]
    public string? Content { get; set; }

    [BsonElement("begeniSayisi")]
    public int LikeCount { get; set; } = 0;

    [BsonElement("begenenKullanicilar")]
    public List<string> LikedByUsers { get; set; } = [];

    [BsonElement("olusturulmaTarihi")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("guncellemeTarihi")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
