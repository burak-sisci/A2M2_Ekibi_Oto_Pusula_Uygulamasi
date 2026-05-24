using backend.API.Modules.Auth.Application;
using backend.API.Modules.Auth.Domain;
using backend.API.Shared.Database;
using MongoDB.Driver;

namespace backend.API.Modules.Auth.Infrastructure;

public class MongoDeviceRepository : IDeviceRepository
{
    private readonly IMongoCollection<Device> _collection;

    public MongoDeviceRepository(MongoDbContext context)
    {
        _collection = context.GetCollection<Device>("devices");
    }

    public async Task UpsertAsync(Device device)
    {
        var filter = Builders<Device>.Filter.And(
            Builders<Device>.Filter.Eq(d => d.UserId, device.UserId),
            Builders<Device>.Filter.Eq(d => d.FcmToken, device.FcmToken));

        var update = Builders<Device>.Update
            .Set(d => d.Platform, device.Platform)
            .Set(d => d.LastSeenAt, DateTime.UtcNow)
            .SetOnInsert(d => d.Id, device.Id)
            .SetOnInsert(d => d.UserId, device.UserId)
            .SetOnInsert(d => d.FcmToken, device.FcmToken)
            .SetOnInsert(d => d.CreatedAt, DateTime.UtcNow);

        await _collection.UpdateOneAsync(
            filter,
            update,
            new UpdateOptions { IsUpsert = true });
    }

    public async Task DeleteByTokenAsync(string fcmToken)
        => await _collection.DeleteManyAsync(d => d.FcmToken == fcmToken);

    public async Task<List<Device>> GetByUserIdAsync(string userId)
        => await _collection.Find(d => d.UserId == userId).ToListAsync();
}
