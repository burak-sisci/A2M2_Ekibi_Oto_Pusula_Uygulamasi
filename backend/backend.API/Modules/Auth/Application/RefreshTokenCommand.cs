using backend.API.Modules.Auth.Domain;
using backend.API.Modules.Auth.Infrastructure;
using backend.API.Shared.Security;

namespace backend.API.Modules.Auth.Application;

/// <summary>
/// Refresh token rotation: gelen refresh token doğrulanır → revoke edilir → yeni access + refresh dönülür.
/// </summary>
public class RefreshTokenCommand
{
    private readonly IRefreshTokenStore _store;
    private readonly IUserRepository    _userRepository;
    private readonly JwtTokenGenerator  _jwt;

    public RefreshTokenCommand(
        IRefreshTokenStore store,
        IUserRepository userRepository,
        JwtTokenGenerator jwt)
    {
        _store          = store;
        _userRepository = userRepository;
        _jwt            = jwt;
    }

    public async Task<RefreshTokenResult> ExecuteAsync(string rawRefreshToken)
    {
        var existing = await _store.ValidateAsync(rawRefreshToken);
        if (existing is null)
            throw new UnauthorizedAccessException("Refresh token geçersiz veya süresi dolmuş.");

        var user = await _userRepository.GetByIdAsync(existing.UserId);
        if (user is null)
            throw new UnauthorizedAccessException("Kullanıcı bulunamadı.");

        // Eski refresh'i iptal et (rotation)
        await _store.RevokeAsync(existing.UserId, existing.TokenId);

        // Yeni access + refresh üret
        var newAccess               = _jwt.GenerateAccessToken(user.Id, user.Email);
        var (newRefresh, newRefId)  = _jwt.GenerateRefreshToken();
        var refreshDays             = _jwt.GetRefreshTokenDays();

        await _store.SaveAsync(new RefreshToken
        {
            TokenId    = newRefId,
            UserId     = user.Id,
            TokenHash  = RedisRefreshTokenStore.ComputeHash(newRefresh),
            IssuedAt   = DateTime.UtcNow,
            ExpiresAt  = DateTime.UtcNow.AddDays(refreshDays),
            DeviceInfo = existing.DeviceInfo
        });

        return new RefreshTokenResult(newAccess, newRefresh, _jwt.GetAccessTokenSeconds());
    }
}
