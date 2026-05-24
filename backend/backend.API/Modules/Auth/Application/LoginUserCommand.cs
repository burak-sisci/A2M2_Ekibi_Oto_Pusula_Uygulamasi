using backend.API.Modules.Auth.Domain;
using backend.API.Modules.Auth.Infrastructure;
using backend.API.Shared.Security;

namespace backend.API.Modules.Auth.Application;

public class LoginUserCommand
{
    private readonly IUserRepository      _userRepository;
    private readonly IPasswordHasher      _passwordHasher;
    private readonly JwtTokenGenerator    _jwt;
    private readonly IRefreshTokenStore   _refreshStore;

    public LoginUserCommand(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        JwtTokenGenerator jwt,
        IRefreshTokenStore refreshStore)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwt            = jwt;
        _refreshStore   = refreshStore;
    }

    public async Task<LoginResult> ExecuteAsync(LoginRequest request)
    {
        // Email veya telefon ile giriş desteklenir
        var user = request.Identifier.Contains('@')
            ? await _userRepository.GetByEmailAsync(request.Identifier)
            : await _userRepository.GetByPhoneAsync(request.Identifier);

        if (user is null || !_passwordHasher.Verify(request.Sifre, user.PasswordHash))
            throw new UnauthorizedAccessException("Kimlik bilgileri hatalı.");

        var accessToken = _jwt.GenerateAccessToken(user.Id, user.Email);
        var (refreshRaw, refreshId) = _jwt.GenerateRefreshToken();
        var refreshDays = _jwt.GetRefreshTokenDays();

        await _refreshStore.SaveAsync(new RefreshToken
        {
            TokenId    = refreshId,
            UserId     = user.Id,
            TokenHash  = RedisRefreshTokenStore.ComputeHash(refreshRaw),
            IssuedAt   = DateTime.UtcNow,
            ExpiresAt  = DateTime.UtcNow.AddDays(refreshDays),
            DeviceInfo = request.DeviceInfo ?? ""
        });

        return new LoginResult(
            KullaniciId:     user.Id,
            Email:           user.Email,
            Ad:              user.Ad,
            Token:           accessToken,   // geriye uyumluluk — web FE kullanıyor
            AccessToken:     accessToken,
            RefreshToken:    refreshRaw,
            AccessExpiresIn: _jwt.GetAccessTokenSeconds());
    }
}

public record LoginRequest(string Identifier, string Sifre, string? DeviceInfo = null);

public record LoginResult(
    string KullaniciId,
    string Email,
    string Ad,
    string Token,
    string AccessToken,
    string RefreshToken,
    int    AccessExpiresIn);

public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Token, string YeniSifre);
public record RefreshTokenRequest(string RefreshToken);
public record RefreshTokenResult(string AccessToken, string RefreshToken, int AccessExpiresIn);
