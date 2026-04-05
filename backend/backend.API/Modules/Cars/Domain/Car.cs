using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.API.Modules.Cars.Domain;

public enum PanelStatus
{
     Orijinal,
     Boyali,
     Değişmiş
}

public enum VitesTipi
{
    Düz,
    Otomatik,
    YariOtomatik
}

public enum YakitTipi
{
     Benzin,
     Dizel,
     Elektrik,
     Hibrit,
     LPG,
    Benzin_LPG,
}

public enum KasaTipi
{
    Sedan,
    Hatchback,
    SUV,
    Crossover,
    Coupe,
    Cabrio,
    StationWagon, // Kombi
    Minivan,
    Pickup,
    Van           // Panelvan
}

public enum CekisTuru
{
     ÖndenÇekiş,
    Arkadanİtiş,
    DörtÇeker,
    DörtcarpiDört
}

public enum AracDurumu
{
    Sifir,
     İkinciEl
}

public enum Kimden
{
     Sahibinden,
     Galeriden
}

public class BoyaliveDegisen
{
    [BsonElement("rightRearFender")]
    [BsonRepresentation(BsonType.String)]
    public PanelStatus SağArkaÇamurluk { get; set; } = PanelStatus.Orijinal;  // Sağ Arka Çamurluk

    [BsonElement("rearHood")]
    [BsonRepresentation(BsonType.String)]
    public PanelStatus ArkaKaput { get; set; } = PanelStatus.  Orijinal;         // Arka Kaput

    [BsonElement("leftRearFender")]
    [BsonRepresentation(BsonType.String)]
    public PanelStatus SolArkaÇamurluk { get; set; } = PanelStatus.  Orijinal;   // Sol Arka Çamurluk

    [BsonElement("rightRearDoor")]
    [BsonRepresentation(BsonType.String)]
    public PanelStatus SağArkaKapi { get; set; } = PanelStatus.  Orijinal;    // Sağ Arka Kapı

    [BsonElement("rightFrontDoor")]
    [BsonRepresentation(BsonType.String)]
    public PanelStatus SağÖnKapi { get; set; } = PanelStatus.  Orijinal;   // Sağ Ön Kapı

    [BsonElement("roof")]
    [BsonRepresentation(BsonType.String)]
    public PanelStatus Tavan { get; set; } = PanelStatus.  Orijinal;             // Tavan

    [BsonElement("leftRearDoor")]
    [BsonRepresentation(BsonType.String)]
    public PanelStatus SolArkaKapi { get; set; } = PanelStatus.  Orijinal;     // Sol Arka Kapı

    [BsonElement("leftFrontDoor")]
    [BsonRepresentation(BsonType.String)]
    public PanelStatus SolÖnKapi { get; set; } = PanelStatus.  Orijinal;    // Sol Ön Kapı

    [BsonElement("rightFrontFender")]
    [BsonRepresentation(BsonType.String)]
    public PanelStatus SağÖnÇamurluk { get; set; } = PanelStatus.  Orijinal; // Sağ Ön Çamurluk

    [BsonElement("engineHood")]
    [BsonRepresentation(BsonType.String)]
    public PanelStatus MotorKaputu { get; set; } = PanelStatus.  Orijinal;       // Motor Kaputu

    [BsonElement("leftFrontFender")]
    [BsonRepresentation(BsonType.String)]
    public PanelStatus SolÖnÇamurluk { get; set; } = PanelStatus.  Orijinal;  // Sol Ön Çamurluk

    [BsonElement("frontBumper")]
    [BsonRepresentation(BsonType.String)]
    public PanelStatus ÖnTampon { get; set; } = PanelStatus.  Orijinal;      // Ön Tampon

    [BsonElement("rearBumper")]
    [BsonRepresentation(BsonType.String)]
    public PanelStatus  ArkaTampon { get; set; } = PanelStatus.  Orijinal;       // Arka Tampon
}

public class Car
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("brand")]
    public string Marka { get; set; } = string.Empty;                         // Marka

    [BsonElement("series")]
    public string Seri { get; set; } = string.Empty;                        // Seri

    [BsonElement("model")]
    public string Model { get; set; } = string.Empty;                         // Model

    [BsonElement("year")]
    public int Yil { get; set; }                                              // Yıl

    [BsonElement("price")]
    public decimal Fiyat { get; set; }                                         // Fiyat

    [BsonElement("kilometer")]
    public int Kilometre { get; set; }                                         // Kilometre

    [BsonElement("transmissionType")]
    [BsonRepresentation(BsonType.String)]
    public VitesTipi VitesTipi { get; set; }                     // Vites Tipi

    [BsonElement("fuelType")]
    [BsonRepresentation(BsonType.String)]
    public YakitTipi YakitTipi { get; set; }                                     // Yakıt Tipi

    [BsonElement("bodyType")]
    [BsonRepresentation(BsonType.String)]
    public KasaTipi KasaTipi { get; set; }                                     // Kasa Tipi

    [BsonElement("color")]
    public string Renk { get; set; } = string.Empty;                         // Renk

    [BsonElement("engineVolume")]
    public double MotorHacmi { get; set; }                                      // Motor Hacmi (cc)

    [BsonElement("enginePower")]
    public int MotorGucu { get; set; }                                       // Motor Gücü (HP)

    [BsonElement("driveType")]
    [BsonRepresentation(BsonType.String)]
    public CekisTuru Cekis { get; set; }                                   // Çekiş

    [BsonElement("vehicleCondition")]
    [BsonRepresentation(BsonType.String)]
    public AracDurumu AracDurumu { get; set; }                     // Araç Durumu

    [BsonElement("avgFuelConsumption")]
    public double OrtalamaYakitTuketim { get; set; }                             // Ort. Yakıt Tüketimi (L/100km)

    [BsonElement("fuelTankCapacity")]
    public int YakitDeposu { get; set; }                                  // Yakıt Deposu (L)

    [BsonElement("isHeavilyDamaged")]
    public bool AgirHasarKaydi { get; set; } = false;                       // Ağır Hasarlı

    [BsonElement("isEligibleForTrade")]
    public bool TakasaUygun { get; set; } = false;                     // Takasa Uygun

    [BsonElement("sellerType")]
    [BsonRepresentation(BsonType.String)]
    public Kimden Kimden { get; set; }                                 // Kimden

    [BsonElement("images")]
    public List<string> Resimler { get; set; } = new();                         // Görseller

    [BsonElement("location")]
    public string Konum { get; set; } = string.Empty;                      // Konum

    [BsonElement("ownerId")]
    public string IlanSahibi { get; set; } = string.Empty;                       // İlan Sahibi

    [BsonElement("paintAndChanged")]
    public BoyaliveDegisen BoyaliDegisen { get; set; } = new();             // Boya & Değişen

    [BsonElement("createdAt")]
    public DateTime IlanTarihi { get; set; } = DateTime.UtcNow;               // İlan Tarihi
}