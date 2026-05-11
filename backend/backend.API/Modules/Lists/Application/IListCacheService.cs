using backend.API.Modules.Lists.Domain;

namespace backend.API.Modules.Lists.Application;

public interface IListCacheService
{
    Task<List<UserList>?> GetUserListsCacheAsync(string userId);
    Task SetUserListsCacheAsync(string userId, List<UserList> listeler);
    Task InvalidateUserListsCacheAsync(string userId);
}
