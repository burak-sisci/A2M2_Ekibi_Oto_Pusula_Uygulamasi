using MongoDB.Driver;

namespace backend.API.Shared.Database;

public class MongoTransactionManager
{

    private readonly IMongoClient _mongoClient;

    public MongoTransactionManager(IMongoClient mongoClient)
    {
        _mongoClient = mongoClient;
    }

    public async Task ExecuteInTransactionAsync(Func<IClientSessionHandle, Task> action)
    {
        using var session = await _mongoClient.StartSessionAsync();
        session.StartTransaction();

        try
        {
            await action(session);
            await session.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }

    public async Task<T> ExecuteInTransactionAsync<T>(Func<IClientSessionHandle, Task<T>> action)
    {
        using var session = await _mongoClient.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var result = await action(session);
            await session.CommitTransactionAsync();
            return result;
        }

        catch
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }
}