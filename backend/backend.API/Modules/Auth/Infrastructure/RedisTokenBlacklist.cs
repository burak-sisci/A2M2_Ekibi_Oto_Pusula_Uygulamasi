using backend.API.Modules.Auth.Application;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace backend.API.Modules.Auth.Infrastructure;

public class RedisTokenBlacklist : ITokenBlacklist
{
    private readonly IDatabase _redis;
    private readonly ILogger<RedisTokenBlacklist> _logger;
    private const string Prefix = "blacklist:";

    public RedisTokenBlacklist(IConnectionMultiplexer redis, ILogger<RedisTokenBlacklist> logger)
    {
        _redis = redis.GetDatabase();
        _logger = logger;
    }

    public async Task AddAsync(string jti, TimeSpan expiry)
    {
        try
        {
            await _redis.StringSetAsync($"{Prefix}{jti}", "1", expiry);
        }
        catch (RedisConnectionException ex)
        {
            _logger.LogWarning(ex, "Redis bağlantısı kurulamadı. Token blacklist'e eklenemedi: {Jti}", jti);
        }
        catch (RedisTimeoutException ex)
        {
            _logger.LogWarning(ex, "Redis zaman aşımı. Token blacklist'e eklenemedi: {Jti}", jti);
        }
    }

    public async Task<bool> IsBlacklistedAsync(string jti)
    {
        try
        {
            return await _redis.KeyExistsAsync($"{Prefix}{jti}");
        }
        catch (RedisConnectionException ex)
        {
            _logger.LogWarning(ex, "Redis bağlantısı kurulamadı. Blacklist kontrolü atlanıyor: {Jti}", jti);
            return false;
        }
        catch (RedisTimeoutException ex)
        {
            _logger.LogWarning(ex, "Redis zaman aşımı. Blacklist kontrolü atlanıyor: {Jti}", jti);
            return false;
        }
    }
}
