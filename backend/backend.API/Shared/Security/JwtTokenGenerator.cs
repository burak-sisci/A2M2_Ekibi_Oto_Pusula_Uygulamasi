using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace backend.API.Shared.Security;

public class JwtTokenGenerator
{
    private readonly IConfiguration _configiration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configiration = configuration;
    }

    public string GenerateToken(string userId, string email)
    {
        string? secret = _configiration["JWT_SECRET"];
        var issuer = _configiration["AUDIENCE"];
        var audience = _configiration["ISSUER"];
        var expiryMinutes = int.Parse(_configiration["EXPIRYMINUTES"] ?? "60");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
                new Claim(JwtRegisteredClaimNames.Sub,userId),
                new Claim(JwtRegisteredClaimNames.Email,email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier,userId)
            };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

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
