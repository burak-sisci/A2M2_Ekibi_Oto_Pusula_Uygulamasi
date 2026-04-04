using backend.API.Modules.Lists.Domain;
using MongoDB.Driver;

namespace backend.API.Modules.Lists.Application;

public interface IListRepository
{
    Task<List<UserList>> GetByUserIdAsync(string userId);
    Task<UserList?> GetByIdAsync(string id);
    Task CreateAsync(UserList list, IClientSessionHandle? session = null);
    Task<bool> AddItemAsync(string listId, string carId);
    Task CreateDefaultListAsync(string userId, IClientSessionHandle? session = null);
    Task<bool> DeleteAsync(string id, string userId);
}
