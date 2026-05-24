using MongoDB.Bson;
using MongoDB.Driver;
using NotificationService.Models;

namespace NotificationService.Services;

/// <summary>
/// Backend ile aynı MongoDB veritabanına bağlanıp ihtiyacımız olan minimal okumaları yapar.
/// Backend'in domain modellerini referans almıyoruz; sadece BsonDocument okuyup gerekli alanları çekiyoruz.
/// </summary>
public class MongoLookup
{
    private readonly IMongoDatabase _db;
    private readonly ILogger<MongoLookup> _logger;

    public MongoLookup(ILogger<MongoLookup> logger)
    {
        _logger = logger;
        var conn = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? "mongodb://localhost:27017";
        var dbName = Environment.GetEnvironmentVariable("DATABASE_NAME") ?? "projctdb";

        var settings = MongoClientSettings.FromConnectionString(conn);
        settings.ServerSelectionTimeout = TimeSpan.FromSeconds(10);
        var client = new MongoClient(settings);
        _db = client.GetDatabase(dbName);

        _logger.LogInformation("MongoDB bağlandı: {DbName}", dbName);
    }

    /// <summary>İlan sahibinin userId'sini döndürür. Bulunamazsa null.</summary>
    public async Task<CarSummary?> GetCarOwnerAsync(string carId, CancellationToken ct)
    {
        var col = _db.GetCollection<BsonDocument>("cars");
        ObjectId oid;
        var filter = ObjectId.TryParse(carId, out oid)
            ? Builders<BsonDocument>.Filter.Eq("_id", oid)
            : Builders<BsonDocument>.Filter.Eq("_id", carId);

        var doc = await col.Find(filter).FirstOrDefaultAsync(ct);
        if (doc is null) return null;

        return new CarSummary(
            OwnerId: doc.GetValue("ownerId", "").AsString,
            Brand:   doc.GetValue("brand",   "").ToString() ?? "",
            Model:   doc.GetValue("model",   "").ToString() ?? "");
    }

    public async Task<List<DeviceRecord>> GetUserDevicesAsync(string userId, CancellationToken ct)
    {
        var col = _db.GetCollection<BsonDocument>("devices");
        var filter = Builders<BsonDocument>.Filter.Eq("userId", userId);
        var docs = await col.Find(filter).ToListAsync(ct);

        return docs.Select(d => new DeviceRecord(
            UserId:   d.GetValue("userId", "").AsString,
            FcmToken: d.GetValue("fcmToken", "").AsString,
            Platform: d.GetValue("platform", "android").AsString)).ToList();
    }

    public async Task<string?> GetUserNameAsync(string userId, CancellationToken ct)
    {
        var col = _db.GetCollection<BsonDocument>("users");
        ObjectId oid;
        var filter = ObjectId.TryParse(userId, out oid)
            ? Builders<BsonDocument>.Filter.Eq("_id", oid)
            : Builders<BsonDocument>.Filter.Eq("_id", userId);

        var doc = await col.Find(filter).FirstOrDefaultAsync(ct);
        return doc?.GetValue("ad", "").AsString;
    }

    public async Task DeleteDeviceByTokenAsync(string fcmToken, CancellationToken ct)
    {
        var col = _db.GetCollection<BsonDocument>("devices");
        await col.DeleteManyAsync(Builders<BsonDocument>.Filter.Eq("fcmToken", fcmToken), ct);
    }
}

public record CarSummary(string OwnerId, string Brand, string Model);
