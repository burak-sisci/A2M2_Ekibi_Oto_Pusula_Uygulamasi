using backend.API.Modules.Auth.Application;
using System.Collections.Concurrent;

namespace backend.API.Modules.Auth.Infrastructure;

/// <summary>
/// Redis kullanılamadığında fallback olarak çalışan in-memory token blacklist.
/// Not: Uygulama yeniden başlatıldığında blacklist sıfırlanır.
/// </summary>
public class InMemoryTokenBlacklist : ITokenBlacklist
{
    private readonly ConcurrentDictionary<string, DateTimeOffset> _blacklist = new();
    private readonly ILogger<InMemoryTokenBlacklist> _logger;

    public InMemoryTokenBlacklist(ILogger<InMemoryTokenBlacklist> logger)
    {
        _logger = logger;
        _logger.LogWarning("Redis bağlantısı bulunamadı! In-memory token blacklist kullanılıyor. Logout özelliği uygulama yeniden başlatıldığında sıfırlanacak.");
    }

    public Task AddAsync(string jti, TimeSpan expiry)
    {
        var expiresAt = DateTimeOffset.UtcNow.Add(expiry);
        _blacklist[jti] = expiresAt;
        
        // Süresi dolmuş tokenleri temizle (basit garbage collection)
        CleanupExpired();
        
        return Task.CompletedTask;
    }

    public Task<bool> IsBlacklistedAsync(string jti)
    {
        if (_blacklist.TryGetValue(jti, out var expiresAt))
        {
            if (DateTimeOffset.UtcNow < expiresAt)
                return Task.FromResult(true);
            
            // Süresi dolmuş, temizle
            _blacklist.TryRemove(jti, out _);
        }
        return Task.FromResult(false);
    }

    private void CleanupExpired()
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var kvp in _blacklist)
        {
            if (now >= kvp.Value)
                _blacklist.TryRemove(kvp.Key, out _);
        }
    }
}
