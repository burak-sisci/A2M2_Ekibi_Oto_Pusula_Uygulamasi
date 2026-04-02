using backend.API.Modules.Auth.Application;
using backend.API.Modules.Auth.Domain;
using backend.API.Shared.Database;
using MongoDB.Driver;

namespace backend.API.Modules.Auth.Infrastructure;

public class MongoUserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _collection;

    public MongoUserRepository(MongoDbContext context)
    {
        _collection = context.GetCollection<User>("users");
        CreateIndexes();
    }

    private void CreateIndexes()
    {
        var emailIndex = new CreateIndexModel<User>(
            Builders<User>.IndexKeys.Ascending(u => u.Email),
            new CreateIndexOptions { Unique = true });

        var phoneIndex = new CreateIndexModel<User>(
            Builders<User>.IndexKeys.Ascending(u => u.Phone),
            new CreateIndexOptions { Unique = true });

        _collection.Indexes.CreateMany([emailIndex, phoneIndex]);
    }

    public async Task<User?> GetByEmailAsync(string email)
        => await _collection.Find(u => u.Email == email).FirstOrDefaultAsync();

    public async Task<User?> GetByIdAsync(string id)
        => await _collection.Find(u => u.Id == id).FirstOrDefaultAsync();

    public async Task<bool> ExistsByEmailAsync(string email)
        => await _collection.CountDocumentsAsync(u => u.Email == email) > 0;

    public async Task<bool> ExistsByPhoneAsync(string phone)
        => await _collection.CountDocumentsAsync(u => u.Phone == phone) > 0;

    public async Task CreateAsync(User user, IClientSessionHandle? session = null)
    {
        if (session is not null)
            await _collection.InsertOneAsync(session, user);
        else
            await _collection.InsertOneAsync(user);
    }

    public async Task<User?> GetByResetTokenAsync(string token)
        => await _collection.Find(u => u.ResetToken == token).FirstOrDefaultAsync();

    public async Task<bool> UpdateAsync(string id, User user)
    {
        var result = await _collection.ReplaceOneAsync(u => u.Id == id, user);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
{
    var result = await _collection.DeleteOneAsync(u => u.Id == id);
    return result.DeletedCount > 0;
}

}
