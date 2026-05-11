// POST /api/Prediction/predict — Backend Car nesnesinin camelCase hâlini bekler
class PricePredictDto {
  final String brand;
  final String seri;
  final String model;
  final int year;
  final int km;
  final String fuelType;
  final String gearType;
  final String kasaTipi;
  final String renk;
  final double motorHacmi;
  final int motorGucu;
  final String aracDurumu;
  final String cekis;
  final double ortalamaYakitTuketim;
  final int yakitDeposu;
  final Map<String, String> boyaliDegisen;

  const PricePredictDto({
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
    this.aracDurumu = 'İkinciEl',
    this.cekis = 'ÖndenÇekiş',
    this.ortalamaYakitTuketim = 0.0,
    this.yakitDeposu = 0,
    this.boyaliDegisen = const {},
  });

  Map<String, dynamic> toJson() => {
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
        'aracDurumu': aracDurumu,
        'cekis': cekis,
        'ortalamaYakitTuketim': ortalamaYakitTuketim,
        'yakitDeposu': yakitDeposu,
        'boyaliDegisen': boyaliDegisen,
        'fiyat': 0,
        'agirHasarKaydi': false,
        'takasaUygun': false,
        'kimden': 'Sahibinden',
        'resimler': <String>[],
        'konum': '',
        'ilanSahibi': '',
      };
}
