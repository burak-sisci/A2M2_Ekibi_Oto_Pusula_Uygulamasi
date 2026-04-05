using backend.API.Shared.Security;

namespace backend.API.Modules.Auth.Application;

public class LogoutUserCommand
{
    private readonly ITokenBlacklist _tokenBlacklist;
    private readonly JwtTokenGenerator _jwtTokenGenerator;

    public LogoutUserCommand(ITokenBlacklist tokenBlacklist, JwtTokenGenerator jwtTokenGenerator)
    {
        _tokenBlacklist = tokenBlacklist;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task ExecuteAsync(string token)
    {
        var jti = _jwtTokenGenerator.GetJtiFromToken(token);
        if (jti is null) return;

        var remaining = _jwtTokenGenerator.GetRemainingExpiry(token);
        if (remaining is not null)
            await _tokenBlacklist.AddAsync(jti, remaining.Value);
    }
}
