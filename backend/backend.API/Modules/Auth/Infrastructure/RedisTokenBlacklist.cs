using backend.API.Modules.Auth.Application;
using StackExchange.Redis;

namespace backend.API.Modules.Auth.Infrastructure;

public class RedisTokenBlacklist : ITokenBlacklist
{
    private readonly IDatabase _redis;
    private const string Prefix = "blacklist:";

    public RedisTokenBlacklist(IConnectionMultiplexer redis)
    {
        _redis = redis.GetDatabase();
    }

    public async Task AddAsync(string jti, TimeSpan expiry)
        => await _redis.StringSetAsync($"{Prefix}{jti}", "1", expiry);

    public async Task<bool> IsBlacklistedAsync(string jti)
        => await _redis.KeyExistsAsync($"{Prefix}{jti}");
}
