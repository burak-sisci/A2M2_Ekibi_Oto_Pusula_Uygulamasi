namespace backend.API.Modules.Prediction.Domain;

public class PricePredictionResponse
{
    public string Durum { get; set; } = string.Empty;
    public TahminSonucu TahminSonucu { get; set; } = new();
}

public class TahminSonucu
{
    public string FiyatEtiketi { get; set; } = string.Empty;
    public string Birim { get; set; } = string.Empty;
}