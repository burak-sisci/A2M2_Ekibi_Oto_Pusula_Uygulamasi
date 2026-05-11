using backend.API.Modules.Cars.Application;
using backend.API.Modules.Cars.Domain;
using backend.API.Shared.Paginition;

namespace backend.API.Modules.Cars.Infrastructure;

/// <summary>Redis yoksa araç listesi önbelleğe alınmaz.</summary>
public class NoOpCarCacheService : ICarCacheService
{
    public Task<PagedResult<Car>?> GetListCacheAsync(string anahtarKismi)
        => Task.FromResult<PagedResult<Car>?>(null);

    public Task SetListCacheAsync(string anahtarKismi, PagedResult<Car> sonuc)
        => Task.CompletedTask;

    public Task InvalidateListCacheAsync()
        => Task.CompletedTask;
}
