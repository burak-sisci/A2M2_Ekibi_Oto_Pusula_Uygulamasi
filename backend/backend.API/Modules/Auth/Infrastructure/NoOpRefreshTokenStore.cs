using backend.API.Modules.Auth.Application;
using backend.API.Modules.Auth.Domain;

namespace backend.API.Modules.Auth.Infrastructure;

/// <summary>
/// Redis erişimi yokken kullanılır. Refresh akışı pratik olarak devre dışı kalır
/// (her zaman null doğrulama), ama uygulamayı çökertmek yerine sessiz geçer.
/// </summary>
public class NoOpRefreshTokenStore : IRefreshTokenStore
{
    public Task SaveAsync(RefreshToken token) => Task.CompletedTask;
    public Task<RefreshToken?> ValidateAsync(string rawToken) => Task.FromResult<RefreshToken?>(null);
    public Task RevokeAsync(string userId, string tokenId) => Task.CompletedTask;
    public Task RevokeAllForUserAsync(string userId) => Task.CompletedTask;
}
