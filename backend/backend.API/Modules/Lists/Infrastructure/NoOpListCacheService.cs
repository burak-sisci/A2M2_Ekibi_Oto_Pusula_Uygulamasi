using backend.API.Modules.Lists.Application;
using backend.API.Modules.Lists.Domain;

namespace backend.API.Modules.Lists.Infrastructure;

/// <summary>Redis mevcut değilken kullanılan no-op cache — tüm operasyonlar sessizce atlanır.</summary>
public class NoOpListCacheService : IListCacheService
{
    public Task<List<UserList>?> GetUserListsCacheAsync(string userId)
        => Task.FromResult<List<UserList>?>(null);

    public Task SetUserListsCacheAsync(string userId, List<UserList> listeler)
        => Task.CompletedTask;

    public Task InvalidateUserListsCacheAsync(string userId)
        => Task.CompletedTask;
}
