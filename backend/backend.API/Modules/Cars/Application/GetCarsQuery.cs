using backend.API.Modules.Cars.Domain;
using backend.API.Shared.Paginition;

namespace backend.API.Modules.Cars.Application;

public class GetCarsQuery
{
    private readonly ICarRepository _carRepository;

    public GetCarsQuery(ICarRepository carRepository)
    {
        _carRepository = carRepository;
    }

    public async Task<PagedResult<Car>> ExecuteAsync(CarsFilter filter, PaginationParameters pagination)
        => await _carRepository.GetAllAsync(filter, pagination);
}

public class AddCarCommand
{
    private readonly ICarRepository _carRepository;

    public AddCarCommand(ICarRepository carRepository)
    {
        _carRepository = carRepository;
    }

    public async Task<Car> ExecuteAsync(AddCarRequest request, string ilanSahibi)
    {
        var car = new Car
        {
            Marka            = request.Marka,
            Seri             = request.Seri,
            Model            = request.Model,
            Yil              = request.Yil,
            Fiyat            = request.Fiyat,
            Kilometre        = request.Kilometre,
            VitesTipi        = request.VitesTipi,
            YakitTipi        = request.YakitTipi,
            KasaTipi         = request.KasaTipi,
            Renk             = request.Renk,
            MotorHacmi       = request.MotorHacmi,
            MotorGucu        = request.MotorGucu,
            Cekis            = request.Cekis,
            AracDurumu       = request.AracDurumu,
            OrtalamaYakitTuketim = request.OrtalamaYakitTuketim,
            YakitDeposu      = request.YakitDeposu,
            AgirHasarKaydi   = request.AgirHasarKaydi,
            TakasaUygun      = request.TakasaUygun,
            Kimden           = request.Kimden,
            Resimler         = request.Resimler,
            Konum            = request.Konum,
            BoyaliDegisen    = request.BoyaliDegisen,
            IlanSahibi       = ilanSahibi
        };

        await _carRepository.CreateAsync(car);
        return car;
    }
}

public class UpdateCarCommand
{
    private readonly ICarRepository _carRepository;

    public UpdateCarCommand(ICarRepository carRepository)
    {
        _carRepository = carRepository;
    }

    public async Task<bool> ExecuteAsync(string id, UpdateCarRequest request)
    {
        var existing = await _carRepository.GetByIdAsync(id);
        if (existing is null) return false;

        existing.Marka                = request.Marka;
        existing.Seri                 = request.Seri;
        existing.Model                = request.Model;
        existing.Yil                  = request.Yil;
        existing.Fiyat                = request.Fiyat;
        existing.Kilometre            = request.Kilometre;
        existing.VitesTipi            = request.VitesTipi;
        existing.YakitTipi            = request.YakitTipi;
        existing.KasaTipi             = request.KasaTipi;
        existing.Renk                 = request.Renk;
        existing.MotorHacmi           = request.MotorHacmi;
        existing.MotorGucu            = request.MotorGucu;
        existing.Cekis                = request.Cekis;
        existing.AracDurumu           = request.AracDurumu;
        existing.OrtalamaYakitTuketim = request.OrtalamaYakitTuketim;
        existing.YakitDeposu          = request.YakitDeposu;
        existing.AgirHasarKaydi       = request.AgirHasarKaydi;
        existing.TakasaUygun          = request.TakasaUygun;
        existing.Kimden               = request.Kimden;
        existing.Resimler             = request.Resimler;
        existing.Konum                = request.Konum;
        existing.BoyaliDegisen        = request.BoyaliDegisen;

        return await _carRepository.UpdateAsync(id, existing);
    }
}

public record AddCarRequest(
    string Marka,
    string Seri,
    string Model,
    int Yil,
    decimal Fiyat,
    int Kilometre,
    VitesTipi VitesTipi,
    YakitTipi YakitTipi,
    KasaTipi KasaTipi,
    string Renk,
    double MotorHacmi,
    int MotorGucu,
    CekisTuru Cekis,
    AracDurumu AracDurumu,
    double OrtalamaYakitTuketim,
    int YakitDeposu,
    bool AgirHasarKaydi,
    bool TakasaUygun,
    Kimden Kimden,
    List<string> Resimler,
    string Konum,
    BoyaliveDegisen BoyaliDegisen
);

public record UpdateCarRequest(
    string Marka,
    string Seri,
    string Model,
    int Yil,
    decimal Fiyat,
    int Kilometre,
    VitesTipi VitesTipi,
    YakitTipi YakitTipi,
    KasaTipi KasaTipi,
    string Renk,
    double MotorHacmi,
    int MotorGucu,
    CekisTuru Cekis,
    AracDurumu AracDurumu,
    double OrtalamaYakitTuketim,
    int YakitDeposu,
    bool AgirHasarKaydi,
    bool TakasaUygun,
    Kimden Kimden,
    List<string> Resimler,
    string Konum,
    BoyaliveDegisen BoyaliDegisen
);