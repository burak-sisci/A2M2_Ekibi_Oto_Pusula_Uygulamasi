using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using backend.API.Modules.Auth.Application;
using backend.API.Modules.Auth.Domain;
using StackExchange.Redis;

namespace backend.API.Modules.Auth.Infrastructure;

/// <summary>
/// Refresh token'lar Redis'te `refresh:{userId}:{tokenId}` key'inde, JSON body olarak tutulur.
/// Ham token istemciye dönülür; serverda yalnızca SHA-256 hash'i saklanır.
/// Lookup için ayrıca `refresh-lookup:{hash}` → `{userId}:{tokenId}` reverse index'i tutarız.
/// </summary>
public class RedisRefreshTokenStore : IRefreshTokenStore
{
    private readonly IDatabase _db;
    private readonly IConnectionMultiplexer _mux;
    private const string Prefix       = "refresh:";
    private const string LookupPrefix = "refresh-lookup:";

    public RedisRefreshTokenStore(IConnectionMultiplexer mux)
    {
        _mux = mux;
        _db  = mux.GetDatabase();
    }

    public async Task SaveAsync(RefreshToken token)
    {
        var ttl = token.ExpiresAt - DateTime.UtcNow;
        if (ttl <= TimeSpan.Zero) return;

        var key       = $"{Prefix}{token.UserId}:{token.TokenId}";
        var lookupKey = $"{LookupPrefix}{token.TokenHash}";
        var payload   = JsonSerializer.Serialize(token);

        var tx = _db.CreateTransaction();
        _ = tx.StringSetAsync(key, payload, ttl);
        _ = tx.StringSetAsync(lookupKey, $"{token.UserId}:{token.TokenId}", ttl);
        await tx.ExecuteAsync();
    }

    public async Task<RefreshToken?> ValidateAsync(string rawToken)
    {
        var hash      = ComputeHash(rawToken);
        var lookupKey = $"{LookupPrefix}{hash}";
        var pointer   = await _db.StringGetAsync(lookupKey);
        if (pointer.IsNullOrEmpty) return null;

        var parts = pointer.ToString().Split(':');
        if (parts.Length != 2) return null;

        var key = $"{Prefix}{parts[0]}:{parts[1]}";
        var raw = await _db.StringGetAsync(key);
        if (raw.IsNullOrEmpty) return null;

        var token = JsonSerializer.Deserialize<RefreshToken>(raw.ToString());
        if (token is null || token.TokenHash != hash) return null;
        if (token.ExpiresAt < DateTime.UtcNow) return null;

        return token;
    }

    public async Task RevokeAsync(string userId, string tokenId)
    {
        var key = $"{Prefix}{userId}:{tokenId}";
        var raw = await _db.StringGetAsync(key);
        if (raw.IsNullOrEmpty) return;

        var token = JsonSerializer.Deserialize<RefreshToken>(raw.ToString());
        if (token is not null)
            await _db.KeyDeleteAsync($"{LookupPrefix}{token.TokenHash}");
        await _db.KeyDeleteAsync(key);
    }

    public async Task RevokeAllForUserAsync(string userId)
    {
        // Production-safe değil (SCAN+single endpoint varsayar), Upstash single-node için OK.
        var endpoints = _mux.GetEndPoints();
        foreach (var ep in endpoints)
        {
            var server = _mux.GetServer(ep);
            await foreach (var key in server.KeysAsync(pattern: $"{Prefix}{userId}:*"))
            {
                var raw = await _db.StringGetAsync(key);
                if (!raw.IsNullOrEmpty)
                {
                    var token = JsonSerializer.Deserialize<RefreshToken>(raw.ToString());
                    if (token is not null)
                        await _db.KeyDeleteAsync($"{LookupPrefix}{token.TokenHash}");
                }
                await _db.KeyDeleteAsync(key);
            }
        }
    }

    public static string ComputeHash(string raw)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
        return Convert.ToHexString(bytes);
    }
}
