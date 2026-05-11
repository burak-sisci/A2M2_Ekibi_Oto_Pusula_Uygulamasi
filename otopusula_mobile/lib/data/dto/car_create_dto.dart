class CarCreateDto {
  final String brand;
  final String seri;
  final String model;
  final int year;
  final int km;
  final String fuelType; // API enum: Benzin, Dizel, Elektrik, Hibrit, LPG, Benzin_LPG
  final String gearType; // API enum: Düz, Otomatik, YariOtomatik
  final String kasaTipi; // API enum: Sedan, Hatchback, SUV, Crossover, Coupe, Cabrio, StationWagon, Minivan, Pickup, Van
  final String renk;
  final double motorHacmi;
  final int motorGucu;
  final String cekis; // API enum: ÖndenÇekiş, Arkadanİtiş, DörtÇeker, DörtcarpiDört
  final String aracDurumu; // API enum: Sifir, İkinciEl
  final double ortalamaYakitTuketim;
  final int yakitDeposu;
  final bool agirHasarKaydi;
  final bool takasaUygun;
  final String kimden; // API enum: Sahibinden, Galeriden
  final int price;
  final String city;
  final String district;
  final String description;
  final Map<String, String> boyaliDegisen; // panel → PanelStatus enum string
  final List<String> imagePaths;

  const CarCreateDto({
    required this.brand,
    this.seri = '',
    required this.model,
    required this.year,
    required this.km,
    required this.fuelType,
    required this.gearType,
    this.kasaTipi = 'Sedan',
    this.renk = '',
    this.motorHacmi = 0.0,
    this.motorGucu = 0,
    this.cekis = 'ÖndenÇekiş',
    this.aracDurumu = 'İkinciEl',
    this.ortalamaYakitTuketim = 0.0,
    this.yakitDeposu = 0,
    this.agirHasarKaydi = false,
    this.takasaUygun = false,
    this.kimden = 'Sahibinden',
    required this.price,
    required this.city,
    required this.district,
    required this.description,
    this.boyaliDegisen = const {},
    this.imagePaths = const [],
  });

  Map<String, dynamic> toJson(List<String> uploadedUrls) {
    // Backend AddCarRequest — camelCase property names + JsonStringEnumConverter
    return {
      'marka': brand,
      'seri': seri,
      'model': model,
      'yil': year,
      'kilometre': km,
      'yakitTipi': fuelType,
      'vitesTipi': gearType,
      'kasaTipi': kasaTipi,
      'renk': renk,
      'motorHacmi': motorHacmi,
      'motorGucu': motorGucu,
      'cekis': cekis,
      'aracDurumu': aracDurumu,
      'ortalamaYakitTuketim': ortalamaYakitTuketim,
      'yakitDeposu': yakitDeposu,
      'agirHasarKaydi': agirHasarKaydi,
      'takasaUygun': takasaUygun,
      'kimden': kimden,
      'fiyat': price,
      'konum': '$city, $district',
      'aciklama': description,
      'resimler': uploadedUrls,
      'boyaliDegisen': _buildBoyaliDegisen(),
    };
  }

  Map<String, dynamic> _buildBoyaliDegisen() {
    // 13 panel alanı — JSON key = C# property camelCase adı (PropertyNamingPolicy.CamelCase)
    const panelFields = [
      'motorKaputu',       // MotorKaputu
      'tavan',             // Tavan
      'önTampon',          // ÖnTampon
      'arkaTampon',        // ArkaTampon
      'arkaKaput',         // ArkaKaput
      'sağÖnÇamurluk',    // SağÖnÇamurluk
      'sağÖnKapi',        // SağÖnKapi
      'sağArkaKapi',      // SağArkaKapi
      'sağArkaÇamurluk', // SağArkaÇamurluk
      'solÖnÇamurluk',   // SolÖnÇamurluk
      'solÖnKapi',       // SolÖnKapi
      'solArkaKapi',     // SolArkaKapi
      'solArkaÇamurluk', // SolArkaÇamurluk
    ];
    return {for (final f in panelFields) f: boyaliDegisen[f] ?? 'Orijinal'};
  }
}
