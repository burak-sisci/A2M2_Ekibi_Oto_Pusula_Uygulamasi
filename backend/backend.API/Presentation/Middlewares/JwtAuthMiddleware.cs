using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using backend.API.Modules.Auth.Application;
using backend.API.Modules.Auth.Infrastructure;

namespace backend.API.Presentation.Middlewares;

public class JwtAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public JwtAuthMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context, ITokenBlacklist tokenBlacklist)
    {
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(context);
            return;
        }

        var token = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

        if (!string.IsNullOrEmpty(token))
        {
            var jti = ExtractJti(token);
            if (jti is not null && await tokenBlacklist.IsBlacklistedAsync(jti))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { message = "Token geçersiz kılınmış. Lütfen tekrar giriş yapın." });
                return;
            }
        }

        await _next(context);
    }

    private string? ExtractJti(string token)
    {
        try
        {
            var secret = _configuration["JWT_SECRET"]!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var handler = new JwtSecurityTokenHandler();
            handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false
            }, out _);
            var jwt = handler.ReadJwtToken(token);
            return jwt.Id;
        }
        catch
        {
            return null;
        }
    }
}
