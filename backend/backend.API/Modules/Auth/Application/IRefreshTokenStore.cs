using backend.API.Modules.Auth.Domain;

namespace backend.API.Modules.Auth.Application;

public interface IRefreshTokenStore
{
    /// <summary>Refresh token'ı sakla. TTL = ExpiresAt - UtcNow.</summary>
    Task SaveAsync(RefreshToken token);

    /// <summary>Token'ı doğrula. Bulunamazsa veya hash uyuşmazsa null.</summary>
    Task<RefreshToken?> ValidateAsync(string rawToken);

    /// <summary>Tek bir tokenId'yi iptal et.</summary>
    Task RevokeAsync(string userId, string tokenId);

    /// <summary>Kullanıcının tüm refresh token'larını iptal et (logout-all).</summary>
    Task RevokeAllForUserAsync(string userId);
}
