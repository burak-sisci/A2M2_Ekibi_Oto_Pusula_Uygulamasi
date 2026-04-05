using MongoDB.Driver;

namespace backend.API.Shared.Database;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IMongoClient mongoClient, IConfiguration configuration)
    {
        var databaseName = Environment.GetEnvironmentVariable("DATABASE_NAME")
            ?? configuration["MongoDB:DatabaseName"]
            ?? "denemedb";
        _database = mongoClient.GetDatabase(databaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string name) => _database.GetCollection<T>(name);

    public IMongoClient Client => _database.Client;
}