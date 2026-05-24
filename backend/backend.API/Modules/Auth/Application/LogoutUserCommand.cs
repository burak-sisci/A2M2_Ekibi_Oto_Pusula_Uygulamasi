using backend.API.Shared.Security;

namespace backend.API.Modules.Auth.Application;

public class LogoutUserCommand
{
    private readonly ITokenBlacklist    _tokenBlacklist;
    private readonly IRefreshTokenStore _refreshStore;
    private readonly JwtTokenGenerator  _jwt;

    public LogoutUserCommand(
        ITokenBlacklist tokenBlacklist,
        IRefreshTokenStore refreshStore,
        JwtTokenGenerator jwt)
    {
        _tokenBlacklist = tokenBlacklist;
        _refreshStore   = refreshStore;
        _jwt            = jwt;
    }

    /// <summary>
    /// Access token JTI'sini blacklist'e ekler. Refresh token sağlanmışsa onu da revoke eder.
    /// </summary>
    public async Task ExecuteAsync(string accessToken, string? refreshToken = null)
    {
        // 1) Access token blacklist
        var jti = _jwt.GetJtiFromToken(accessToken);
        if (jti is not null)
        {
            var remaining = _jwt.GetRemainingExpiry(accessToken);
            if (remaining is not null)
                await _tokenBlacklist.AddAsync(jti, remaining.Value);
        }

        // 2) Refresh token revoke
        if (!string.IsNullOrEmpty(refreshToken))
        {
            var stored = await _refreshStore.ValidateAsync(refreshToken);
            if (stored is not null)
                await _refreshStore.RevokeAsync(stored.UserId, stored.TokenId);
        }
    }
}
