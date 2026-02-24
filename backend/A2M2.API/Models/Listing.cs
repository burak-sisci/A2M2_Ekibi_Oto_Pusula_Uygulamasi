using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace A2M2.API.Models;

/// <summary>
/// Araç ilanı modeli — MongoDB koleksiyonu: listings (cars)
/// </summary>
public class Listing
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = null!;

    [BsonElement("brand")]
    public string Brand { get; set; } = null!;

    [BsonElement("model")]
    public string Model { get; set; } = null!;

    [BsonElement("year")]
    public int Year { get; set; }

    [BsonElement("km")]
    public int Km { get; set; }

    [BsonElement("fuelType")]
    public string FuelType { get; set; } = null!; // Benzin, Dizel, LPG, Elektrik, Hibrit

    [BsonElement("gearType")]
    public string GearType { get; set; } = null!; // Manuel, Otomatik, Yarı Otomatik

    [BsonElement("price")]
    public decimal Price { get; set; }

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("images")]
    public List<string> Images { get; set; } = new();

    [BsonElement("location")]
    public LocationInfo? Location { get; set; }

    [BsonElement("damageInfo")]
    public List<string> DamageInfo { get; set; } = new();

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Konum bilgisi alt nesnesi
/// </summary>
public class LocationInfo
{
    [BsonElement("city")]
    public string? City { get; set; }

    [BsonElement("district")]
    public string? District { get; set; }
}
