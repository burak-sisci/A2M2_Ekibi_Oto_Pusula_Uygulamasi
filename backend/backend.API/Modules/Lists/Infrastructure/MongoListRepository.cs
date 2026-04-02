using backend.API.Modules.Lists.Application;
using backend.API.Modules.Lists.Domain;
using backend.API.Shared.Database;
using MongoDB.Driver;

namespace backend.API.Modules.Lists.Infrastructure;

public class MongoListRepository : IListRepository
{
    private readonly IMongoCollection<UserList> _collection;

    public MongoListRepository(MongoDbContext context)
    {
        _collection = context.GetCollection<UserList>("lists");
    }

    public async Task<List<UserList>> GetByUserIdAsync(string userId)
        => await _collection.Find(l => l.UserId == userId).ToListAsync();

    public async Task<UserList?> GetByIdAsync(string id)
        => await _collection.Find(l => l.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(UserList list, IClientSessionHandle? session = null)
    {
        if (session is not null)
            await _collection.InsertOneAsync(session, list);
        else
            await _collection.InsertOneAsync(list);
    }

    public async Task<bool> AddItemAsync(string listId, string carId)
    {
        var result = await _collection.UpdateOneAsync(
            l => l.Id == listId,
            Builders<UserList>.Update.AddToSet(l => l.Items, carId));
        return result.ModifiedCount > 0;
    }

    public async Task CreateDefaultListAsync(string userId, IClientSessionHandle? session = null)
    {
        var defaultList = new UserList
        {
            UserId = userId,
            ListName = "Favoriler"
        };
        await CreateAsync(defaultList, session);
    }
     
     public async Task<bool> DeleteAsync(string id, string userId)
    {
        var result = await _collection.DeleteOneAsync(l => l.Id == id && l.UserId == userId);
        return result.DeletedCount > 0;
    }
}
