using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using backend.API.Modules.Cars.Domain;
using backend.API.Shared.Events;
using backend.API.Shared.Messaging;
using backend.API.Shared.Paginition;

namespace backend.API.Modules.Cars.Application;

public class GetCarsQuery
{
    private readonly ICarRepository _carRepository;
    private readonly ICarCacheService _cache;

    public GetCarsQuery(ICarRepository carRepository, ICarCacheService cache)
    {
        _carRepository = carRepository;
        _cache         = cache;
    }

    public async Task<PagedResult<Car>> ExecuteAsync(CarsFilter filtre, PaginationParameters sayfalama)
    {
        var cacheAnahtari = OlusturCacheAnahtari(filtre, sayfalama);

        var onbellek = await _cache.GetListCacheAsync(cacheAnahtari);
        if (onbellek is not null) return onbellek;

        var sonuc = await _carRepository.GetAllAsync(filtre, sayfalama);
        await _cache.SetListCacheAsync(cacheAnahtari, sonuc);

        return sonuc;
    }

    private static string OlusturCacheAnahtari(CarsFilter filtre, PaginationParameters sayfalama)
    {
        var ham = JsonSerializer.Serialize(filtre) + sayfalama.Limit + sayfalama.Offset;
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(ham));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}

public class GetCarByIdQuery
{
    private readonly ICarRepository _carRepository;

    public GetCarByIdQuery(ICarRepository carRepository)
    {
        _carRepository = carRepository;
    }

    public async Task<Car?> ExecuteAsync(string id)
        => await _carRepository.GetByIdAsync(id);
}

public class AddCarCommand
{
    private readonly ICarRepository _carRepository;
    private readonly ICarCacheService _cache;
    private readonly IRabbitMqPublisher _publisher;

    public AddCarCommand(ICarRepository carRepository, ICarCacheService cache, IRabbitMqPublisher publisher)
    {
        _carRepository = carRepository;
        _cache         = cache;
        _publisher     = publisher;
    }

    public async Task<Car> ExecuteAsync(AddCarRequest istek, string ilanSahibi)
    {
        var ilan = new Car
        {
            Marka                = istek.Marka,
            Seri                 = istek.Seri,
            Model                = istek.Model,
            Yil                  = istek.Yil,
            Fiyat                = istek.Fiyat,
            Kilometre            = istek.Kilometre,
            VitesTipi            = istek.VitesTipi,
            YakitTipi            = istek.YakitTipi,
            KasaTipi             = istek.KasaTipi,
            Renk                 = istek.Renk,
            MotorHacmi           = istek.MotorHacmi,
            MotorGucu            = istek.MotorGucu,
            Cekis                = istek.Cekis,
            AracDurumu           = istek.AracDurumu,
            OrtalamaYakitTuketim = istek.OrtalamaYakitTuketim,
            YakitDeposu          = istek.YakitDeposu,
            AgirHasarKaydi       = istek.AgirHasarKaydi,
            TakasaUygun          = istek.TakasaUygun,
            Kimden               = istek.Kimden,
            Resimler             = istek.Resimler,
            Konum                = istek.Konum,
            Aciklama             = istek.Aciklama,
            BoyaliDegisen        = istek.BoyaliDegisen,
            IlanSahibi           = ilanSahibi
        };

        await _carRepository.CreateAsync(ilan);

        // Redis: Yeni ilan eklenince liste cache'i temizle
        await _cache.InvalidateListCacheAsync();

        // RabbitMQ: Anıl Elmaz — yeni ilan event'i
        await _publisher.PublishAsync(
            new CarCreatedEvent(ilan.Id, ilanSahibi, ilan.Marka, ilan.Model, DateTime.UtcNow),
            RabbitMqQueues.IlanOlusturuldu);

        return ilan;
    }
}

public class UpdateCarCommand
{
    private readonly ICarRepository _carRepository;
    private readonly ICarCacheService _cache;

    public UpdateCarCommand(ICarRepository carRepository, ICarCacheService cache)
    {
        _carRepository = carRepository;
        _cache         = cache;
    }

    public async Task<bool> ExecuteAsync(string id, UpdateCarRequest istek)
    {
        var mevcut = await _carRepository.GetByIdAsync(id);
        if (mevcut is null) return false;

        mevcut.Marka                = istek.Marka;
        mevcut.Seri                 = istek.Seri;
        mevcut.Model                = istek.Model;
        mevcut.Yil                  = istek.Yil;
        mevcut.Fiyat                = istek.Fiyat;
        mevcut.Kilometre            = istek.Kilometre;
        mevcut.VitesTipi            = istek.VitesTipi;
        mevcut.YakitTipi            = istek.YakitTipi;
        mevcut.KasaTipi             = istek.KasaTipi;
        mevcut.Renk                 = istek.Renk;
        mevcut.MotorHacmi           = istek.MotorHacmi;
        mevcut.MotorGucu            = istek.MotorGucu;
        mevcut.Cekis                = istek.Cekis;
        mevcut.AracDurumu           = istek.AracDurumu;
        mevcut.OrtalamaYakitTuketim = istek.OrtalamaYakitTuketim;
        mevcut.YakitDeposu          = istek.YakitDeposu;
        mevcut.AgirHasarKaydi       = istek.AgirHasarKaydi;
        mevcut.TakasaUygun          = istek.TakasaUygun;
        mevcut.Kimden               = istek.Kimden;
        mevcut.Resimler             = istek.Resimler;
        mevcut.Konum                = istek.Konum;
        mevcut.Aciklama             = istek.Aciklama;
        mevcut.BoyaliDegisen        = istek.BoyaliDegisen;

        var basarili = await _carRepository.UpdateAsync(id, mevcut);
        if (basarili) await _cache.InvalidateListCacheAsync();

        return basarili;
    }
}

public class AddCarRequest
{
    public string Marka { get; set; } = string.Empty;
    public string Seri { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Yil { get; set; }
    public decimal Fiyat { get; set; }
    public int Kilometre { get; set; }
    public VitesTipi VitesTipi { get; set; }
    public YakitTipi YakitTipi { get; set; }
    public KasaTipi KasaTipi { get; set; }
    public string Renk { get; set; } = string.Empty;
    public double MotorHacmi { get; set; }
    public int MotorGucu { get; set; }
    public CekisTuru Cekis { get; set; }
    public AracDurumu AracDurumu { get; set; }
    public double OrtalamaYakitTuketim { get; set; }
    public int YakitDeposu { get; set; }
    public bool AgirHasarKaydi { get; set; }
    public bool TakasaUygun { get; set; }
    public Kimden Kimden { get; set; }
    public List<string> Resimler { get; set; } = new();
    public string Konum { get; set; } = string.Empty;
    public string Aciklama { get; set; } = string.Empty;
    public BoyaliveDegisen BoyaliDegisen { get; set; } = new();
}

public class UpdateCarRequest
{
    public string Marka { get; set; } = string.Empty;
    public string Seri { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Yil { get; set; }
    public decimal Fiyat { get; set; }
    public int Kilometre { get; set; }
    public VitesTipi VitesTipi { get; set; }
    public YakitTipi YakitTipi { get; set; }
    public KasaTipi KasaTipi { get; set; }
    public string Renk { get; set; } = string.Empty;
    public double MotorHacmi { get; set; }
    public int MotorGucu { get; set; }
    public CekisTuru Cekis { get; set; }
    public AracDurumu AracDurumu { get; set; }
    public double OrtalamaYakitTuketim { get; set; }
    public int YakitDeposu { get; set; }
    public bool AgirHasarKaydi { get; set; }
    public bool TakasaUygun { get; set; }
    public Kimden Kimden { get; set; }
    public List<string> Resimler { get; set; } = new();
    public string Konum { get; set; } = string.Empty;
    public string Aciklama { get; set; } = string.Empty;
    public BoyaliveDegisen BoyaliDegisen { get; set; } = new();
}
