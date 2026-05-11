using backend.API.Modules.Cars.Domain;
using backend.API.Shared.Paginition;

namespace backend.API.Modules.Cars.Application;

public interface ICarCacheService
{
    Task<PagedResult<Car>?> GetListCacheAsync(string anahtarKismi);
    Task SetListCacheAsync(string anahtarKismi, PagedResult<Car> sonuc);
    Task InvalidateListCacheAsync();
}
