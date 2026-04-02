using MongoDB.Driver;

namespace backend.API.Shared.Database;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IMongoClient mongoClient, IConfiguration configuration)
    {
        var databaseName = configuration["DATABASE_NAME"];
        _database = mongoClient.GetDatabase(databaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string name) => _database.GetCollection<T>(name);

    public IMongoClient Client => _database.Client;
}