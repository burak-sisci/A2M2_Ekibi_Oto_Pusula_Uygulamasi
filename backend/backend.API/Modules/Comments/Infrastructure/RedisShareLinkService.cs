using backend.API.Modules.Comments.Application;
using StackExchange.Redis;

namespace backend.API.Modules.Comments.Infrastructure;

public class RedisShareLinkService : IShareLinkService
{
    private readonly IDatabase _db;
    private const string OnEk = "share:";
    private static readonly TimeSpan Ttl = TimeSpan.FromDays(90);

    public RedisShareLinkService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public async Task<ShareLinkSonuc> GetOrCreateAsync(string carId, string baseUrl)
    {
        var anahtar = $"{OnEk}{carId}";
        var mevcutKod = await _db.StringGetAsync(anahtar);

        string kod;
        if (mevcutKod.IsNullOrEmpty)
        {
            // Rastgele kısa kod üret
            kod = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(6))
                .Replace("+", "-").Replace("/", "_").TrimEnd('=');

            await _db.StringSetAsync(anahtar, kod, Ttl);
        }
        else
        {
            kod = mevcutKod!;
        }

        var kisaUrl     = $"{baseUrl}/s/{kod}";
        var orijinalUrl = $"{baseUrl}/cars/{carId}";
        var gecerlilik  = DateTime.UtcNow.Add(Ttl);

        return new ShareLinkSonuc(kisaUrl, orijinalUrl, gecerlilik);
    }
}
