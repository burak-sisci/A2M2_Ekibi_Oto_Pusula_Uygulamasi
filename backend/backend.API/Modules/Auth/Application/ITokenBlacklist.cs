namespace backend.API.Modules.Auth.Application;

public interface ITokenBlacklist
{
    Task AddAsync(string jti, TimeSpan expiry);
    Task<bool> IsBlacklistedAsync(string jti);
}
