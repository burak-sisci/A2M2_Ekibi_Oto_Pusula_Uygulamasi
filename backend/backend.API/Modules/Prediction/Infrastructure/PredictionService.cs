using System.Text;
using System.Text.Json;
using backend.API.Modules.Cars.Domain;
using backend.API.Modules.Prediction.Application;
using backend.API.Modules.Prediction.Domain;

namespace backend.API.Modules.Prediction.Infrastructure;

public class PredictionService : IPredictionService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PredictionService> _logger;

    private static readonly JsonSerializerOptions DeserializeOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public PredictionService(HttpClient httpClient, ILogger<PredictionService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PricePredictionResponse> PredictAsync(Car car)
    {
        var b = car.BoyaliDegisen;

        var payload = new
        {
            ilan_tarihi        = car.IlanTarihi.ToString("yyyy-MM-dd"),
            marka              = car.Marka,
            seri               = car.Seri,
            model              = car.Model,
            yil                = car.Yil,
            km                 = (float)car.Kilometre,
            vites_tipi         = car.VitesTipi.ToString(),
            yakit_tipi         = car.YakitTipi.ToString(),
            kasa_tipi          = car.KasaTipi.ToString(),
            renk               = car.Renk,
            motor_hacmi        = $"{car.MotorHacmi} cc",
            motor_gucu         = $"{car.MotorGucu} hp",
            cekis              = car.Cekis.ToString(),
            arac_durumu        = car.AracDurumu.ToString(),
            ort_yakit_tuketimi = car.OrtalamaYakitTuketim.ToString("F1"),
            yakit_deposu       = car.YakitDeposu.ToString(),
            agir_hasarli       = car.AgirHasarKaydi ? "Evet" : "Hayır",
            boya_degisen       = string.Empty,   // FastAPI okumasa da alan zorunlu
            takasa_uygun       = car.TakasaUygun ? "Evet" : "Hayır",
            kimden             = car.Kimden.ToString(),

            // BoyaliveDegisen → panel string'leri
            sag_arka_camurluk  = b.SağArkaÇamurluk.ToString(),
            arka_kaput         = b.ArkaKaput.ToString(),
            sol_arka_camurluk  = b.SolArkaÇamurluk.ToString(),
            sag_arka_kapi      = b.SağArkaKapi.ToString(),
            sag_on_kapi        = b.SağÖnKapi.ToString(),
            tavan              = b.Tavan.ToString(),
            sol_arka_kapi      = b.SolArkaKapi.ToString(),
            sol_on_kapi        = b.SolÖnKapi.ToString(),
            sag_on_camurluk    = b.SağÖnÇamurluk.ToString(),
            motor_kaputu       = b.MotorKaputu.ToString(),
            sol_on_camurluk    = b.SolÖnÇamurluk.ToString(),
            on_tampon          = b.ÖnTampon.ToString(),
            arka_tampon        = b.ArkaTampon.ToString()
        };

        var json    = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _logger.LogInformation("FastAPI isteği → {Json}", json);

        var response = await _httpClient.PostAsync("/predict", content);

        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync();
            _logger.LogError("FastAPI hata: {Error}", err);
            throw new Exception($"FastAPI hatası: {err}");
        }

        var body   = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PricePredictionResponse>(body, DeserializeOptions);

        return result ?? throw new Exception("FastAPI'den boş yanıt döndü.");
    }
}