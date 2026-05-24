using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace backend.API.Shared.Security;

public class JwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Access token üretir. Süresi ACCESS_TOKEN_MINUTES env'inden, yoksa EXPIRYMINUTES'tan, yoksa 15 dk.
    /// Mobil akışta refresh ile birlikte kullanılır; web akışında tek başına kullanılabilir.
    /// </summary>
    public string GenerateAccessToken(string userId, string email)
    {
        string? secret  = _configuration["JWT_SECRET"];
        var issuer      = _configuration["AUDIENCE"];   // mevcut backend'in tersine yazımı korunuyor
        var audience    = _configuration["ISSUER"];
        var expiryMin   = int.Parse(
            _configuration["ACCESS_TOKEN_MINUTES"]
            ?? _configuration["EXPIRYMINUTES"]
            ?? "15");

        var key         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,   userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier,     userId)
        };

        var token = new JwtSecurityToken(
            issuer:             issuer,
            audience:           audience,
            claims:             claims,
            expires:            DateTime.UtcNow.AddMinutes(expiryMin),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Geriye dönük uyumluluk: eski `GenerateToken` çağrıları access token üretir.
    /// </summary>
    public string GenerateToken(string userId, string email) => GenerateAccessToken(userId, email);

    /// <summary>
    /// Opaque refresh token üretir (32 byte random → base64url). Veritabanına ham hâliyle yazılır.
    /// </summary>
    public (string Token, string TokenId) GenerateRefreshToken()
    {
        var raw    = RandomNumberGenerator.GetBytes(32);
        var token  = Convert.ToBase64String(raw).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        // tokenId — store key'i; token'ın kendisi key olabilir ama prefix temizliği için ayrı tutuyoruz.
        var tokenId = Guid.NewGuid().ToString("N");
        return (token, tokenId);
    }

    public int GetAccessTokenSeconds()
    {
        var expiryMin = int.Parse(
            _configuration["ACCESS_TOKEN_MINUTES"]
            ?? _configuration["EXPIRYMINUTES"]
            ?? "15");
        return expiryMin * 60;
    }

    public int GetRefreshTokenDays()
        => int.Parse(_configuration["REFRESH_TOKEN_DAYS"] ?? "30");

    public string? GetJtiFromToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            return jwt.Id;
        }
        catch
        {
            return null;
        }
    }

    public TimeSpan? GetRemainingExpiry(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var remaining = jwt.ValidTo - DateTime.UtcNow;
            return remaining > TimeSpan.Zero ? remaining : null;
        }
        catch
        {
            return null;
        }
    }
}
