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

    public async Task CreateAsync(UserList liste, IClientSessionHandle? session = null)
    {
        if (session is not null)
            await _collection.InsertOneAsync(session, liste);
        else
            await _collection.InsertOneAsync(liste);
    }

    public async Task<bool> AddItemAsync(string listId, string carId)
    {
        var guncelleme = Builders<UserList>.Update
            .AddToSet(l => l.Items, carId)
            .Set(l => l.UpdatedAt, DateTime.UtcNow);

        var sonuc = await _collection.UpdateOneAsync(l => l.Id == listId, guncelleme);
        return sonuc.ModifiedCount > 0;
    }

    public async Task<bool> RemoveItemAsync(string listId, string carId)
    {
        var guncelleme = Builders<UserList>.Update
            .Pull(l => l.Items, carId)
            .Set(l => l.UpdatedAt, DateTime.UtcNow);

        var sonuc = await _collection.UpdateOneAsync(l => l.Id == listId, guncelleme);
        return sonuc.ModifiedCount > 0;
    }

    public async Task CreateDefaultListAsync(string userId, IClientSessionHandle? session = null)
    {
        var varsayilanListe = new UserList
        {
            UserId    = userId,
            ListName  = "Favoriler",
            IsDefault = true
        };
        await CreateAsync(varsayilanListe, session);
    }

    public async Task<bool> DeleteAsync(string id, string userId)
    {
        // Varsayılan liste silinemez
        var liste = await GetByIdAsync(id);
        if (liste is null || liste.IsDefault) return false;

        var sonuc = await _collection.DeleteOneAsync(l => l.Id == id && l.UserId == userId);
        return sonuc.DeletedCount > 0;
    }

    public async Task<bool> UpdateNameAsync(string id, string userId, string yeniAd)
    {
        var liste = await GetByIdAsync(id);
        if (liste is null || liste.IsDefault) return false;

        var guncelleme = Builders<UserList>.Update
            .Set(l => l.ListName, yeniAd)
            .Set(l => l.UpdatedAt, DateTime.UtcNow);

        var sonuc = await _collection.UpdateOneAsync(l => l.Id == id && l.UserId == userId, guncelleme);
        return sonuc.ModifiedCount > 0;
    }

    public async Task DeleteAllByUserIdAsync(string userId)
        => await _collection.DeleteManyAsync(l => l.UserId == userId);
}
