using System.Text.Json;
using backend.API.Modules.Lists.Application;
using backend.API.Modules.Lists.Domain;
using StackExchange.Redis;

namespace backend.API.Modules.Lists.Infrastructure;

public class RedisListCacheService : IListCacheService
{
    private readonly IDatabase _db;
    private const string OnEk = "lists:user:";
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(3);

    public RedisListCacheService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public async Task<List<UserList>?> GetUserListsCacheAsync(string userId)
    {
        var deger = await _db.StringGetAsync($"{OnEk}{userId}");
        if (deger.IsNullOrEmpty) return null;

        return JsonSerializer.Deserialize<List<UserList>>((string)deger!);
    }

    public async Task SetUserListsCacheAsync(string userId, List<UserList> listeler)
    {
        var json = JsonSerializer.Serialize(listeler);
        await _db.StringSetAsync($"{OnEk}{userId}", json, Ttl);
    }

    public async Task InvalidateUserListsCacheAsync(string userId)
        => await _db.KeyDeleteAsync($"{OnEk}{userId}");
}
