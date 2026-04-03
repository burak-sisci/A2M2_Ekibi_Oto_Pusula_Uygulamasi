using backend.API.Modules.Auth.Domain;

namespace backend.API.Modules.Auth.Application;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(string id);
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByPhoneAsync(string phone);
    Task CreateAsync(User user, MongoDB.Driver.IClientSessionHandle? session = null);
    Task<User?> GetByResetTokenAsync(string token);
    Task<bool> UpdateAsync(string id, User user);
    Task<bool> DeleteAsync(string id);
}
