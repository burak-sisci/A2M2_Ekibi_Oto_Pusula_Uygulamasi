using backend.API.Modules.Cars.Domain;
using backend.API.Shared.Paginition;

namespace backend.API.Modules.Cars.Application;

public interface ICarRepository
{
    Task<PagedResult<Car>> GetAllAsync(CarsFilter filter, PaginationParameters pagination);
    Task<Car?> GetByIdAsync(string id);
    Task CreateAsync(Car car);
    Task<bool> UpdateAsync(string id, Car car);
    Task<bool> DeleteAsync(string id);
    Task DeleteAllByOwnerIdAsync(string ownerId);
}

public record CarsFilter(
    string? Marka = null,
    string? Seri = null,
    string? Model = null,
    string? Konum = null,
    string? Renk = null,

    decimal? MinFiyat = null,
    decimal? MaxFiyat = null,

    int? MinKilometre = null,
    int? MaxKilometre = null,

    int? MinYil = null,
    int? MaxYil = null,

    int? MinMotorGucu = null,
    int? MaxMotorGucu = null,

    VitesTipi? VitesTipi = null,
    YakitTipi? YakitTipi = null,
    KasaTipi? KasaTipi = null,
    DriveType? Cekis = null,
    AracDurumu? AracDurumu = null,
    Kimden? Kimden = null,

    bool? AgirHasarKaydi = null,
    bool? TakasaUygun = null
);