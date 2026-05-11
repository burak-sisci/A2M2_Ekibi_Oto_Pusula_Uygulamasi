using System.Text.Json;
using backend.API.Modules.Cars.Application;
using backend.API.Modules.Cars.Domain;
using backend.API.Shared.Paginition;
using StackExchange.Redis;

namespace backend.API.Modules.Cars.Infrastructure;

public class RedisCarCacheService : ICarCacheService
{
    private readonly IDatabase _db;
    private readonly IServer _server;
    private const string OnEk = "cars:list:";
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(5);

    public RedisCarCacheService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
        _server = redis.GetServer(redis.GetEndPoints().First());
    }

    public async Task<PagedResult<Car>?> GetListCacheAsync(string anahtarKismi)
    {
        var deger = await _db.StringGetAsync($"{OnEk}{anahtarKismi}");
        if (deger.IsNullOrEmpty) return null;

        return JsonSerializer.Deserialize<PagedResult<Car>>((string)deger!);
    }

    public async Task SetListCacheAsync(string anahtarKismi, PagedResult<Car> sonuc)
    {
        var json = JsonSerializer.Serialize(sonuc);
        await _db.StringSetAsync($"{OnEk}{anahtarKismi}", json, Ttl);
    }

    public async Task InvalidateListCacheAsync()
    {
        var anahtarlar = _server.Keys(pattern: $"{OnEk}*").ToArray();
        if (anahtarlar.Length > 0)
            await _db.KeyDeleteAsync(anahtarlar);
    }
}
