using MongoDB.Driver;

namespace backend.API.Shared.Database;

/// <summary>
/// Replica set/cluster üzerinde transaction kullanır. Standalone (yerel geliştirme) MongoDB
/// transaction desteklemediği için NotSupportedException yakalanır ve action transaction'sız tekrarlanır.
/// Ders ödevi için yeterli — production'da replica set kurulması önerilir.
/// </summary>
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
        try
        {
            session.StartTransaction();
        }
        catch (NotSupportedException)
        {
            // Standalone MongoDB — transaction yok, doğrudan koş
            await action(session);
            return;
        }

        try
        {
            await action(session);
            await session.CommitTransactionAsync();
        }
        catch (NotSupportedException)
        {
            // Transaction içinde herhangi bir komut standalone'da NotSupported atabilir
            try { await session.AbortTransactionAsync(); } catch { }
            await action(session);
        }
        catch
        {
            try { await session.AbortTransactionAsync(); } catch { }
            throw;
        }
    }

    public async Task<T> ExecuteInTransactionAsync<T>(Func<IClientSessionHandle, Task<T>> action)
    {
        using var session = await _mongoClient.StartSessionAsync();
        try
        {
            session.StartTransaction();
        }
        catch (NotSupportedException)
        {
            return await action(session);
        }

        try
        {
            var result = await action(session);
            await session.CommitTransactionAsync();
            return result;
        }
        catch (NotSupportedException)
        {
            try { await session.AbortTransactionAsync(); } catch { }
            return await action(session);
        }
        catch
        {
            try { await session.AbortTransactionAsync(); } catch { }
            throw;
        }
    }
}
