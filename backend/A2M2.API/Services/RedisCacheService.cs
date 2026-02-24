using StackExchange.Redis;
using System.Text.Json;

namespace A2M2.API.Services;

/// <summary>
/// Redis cache servisi — Get/Set/Delete/DeletePattern
/// </summary>
public class RedisCacheService
{
    private readonly IConnectionMultiplexer? _redis;
    private readonly IDatabase? _db;

    public RedisCacheService(IConfiguration config)
    {
        try
        {
            var connectionString = config["Redis:ConnectionString"] ?? "localhost:6379";
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _db = _redis.GetDatabase();
            Console.WriteLine("Redis bağlantısı başarılı");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Redis bağlantı hatası: {ex.Message}");
        }
    }

    /// <summary>Cache'den oku</summary>
    public async Task<T?> GetAsync<T>(string key)
    {
        if (_db == null) return default;
        try
        {
            var value = await _db.StringGetAsync(key);
            if (value.IsNullOrEmpty) return default;
            return JsonSerializer.Deserialize<T>(value!);
        }
        catch { return default; }
    }

    /// <summary>Cache'e yaz</summary>
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        if (_db == null) return;
        try
        {
            var json = JsonSerializer.Serialize(value);
            await _db.StringSetAsync(key, json, expiry ?? TimeSpan.FromMinutes(5));
        }
        catch { /* log */ }
    }

    /// <summary>Cache anahtarını sil</summary>
    public async Task DeleteAsync(string key)
    {
        if (_db == null) return;
        try { await _db.KeyDeleteAsync(key); }
        catch { /* log */ }
    }

    /// <summary>Pattern'e uyan tüm cache anahtarlarını sil</summary>
    public async Task DeletePatternAsync(string pattern)
    {
        if (_redis == null) return;
        try
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var keys = server.Keys(pattern: pattern).ToArray();
            if (keys.Length > 0)
                await _db!.KeyDeleteAsync(keys);
        }
        catch { /* log */ }
    }
}
