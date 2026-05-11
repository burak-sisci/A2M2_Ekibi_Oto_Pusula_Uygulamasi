using backend.API.Modules.Auth.Application;

namespace backend.API.Modules.Auth.Infrastructure;

/// <summary>Redis yoksa token kara liste desteklenmez; logout yine de çalışır.</summary>
public class NoOpTokenBlacklist : ITokenBlacklist
{
    public Task AddAsync(string jti, TimeSpan expiry) => Task.CompletedTask;
    public Task<bool> IsBlacklistedAsync(string jti) => Task.FromResult(false);
}
