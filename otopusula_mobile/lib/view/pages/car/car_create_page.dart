import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';
import '../../../core/constants/app_constants.dart';
import '../../../core/constants/app_strings.dart';
import '../../../core/utils/validators.dart';
import '../../../data/repositories/car_repository.dart';
import '../../../viewmodels/car/car_create_view_model.dart';
import '../../widgets/common/app_dropdown.dart';
import '../../widgets/common/app_text_field.dart';
import '../../widgets/common/primary_button.dart';

// ─── Yardımcı: Gösterim adı ↔ API enum değeri ────────────────────────────────
class _DropItem {
  final String label;
  final String value;
  const _DropItem(this.label, this.value);
}

const _yakitItems = [
  _DropItem('Benzin', 'Benzin'),
  _DropItem('Dizel', 'Dizel'),
  _DropItem('Elektrik', 'Elektrik'),
  _DropItem('Hibrit', 'Hibrit'),
  _DropItem('LPG', 'LPG'),
  _DropItem('Benzin + LPG', 'Benzin_LPG'),
];

const _vitesItems = [
  _DropItem('Manuel', 'Düz'),
  _DropItem('Otomatik', 'Otomatik'),
  _DropItem('Yarı Otomatik', 'YariOtomatik'),
];

const _kasaItems = [
  _DropItem('Sedan', 'Sedan'),
  _DropItem('Hatchback', 'Hatchback'),
  _DropItem('SUV', 'SUV'),
  _DropItem('Crossover', 'Crossover'),
  _DropItem('Coupe', 'Coupe'),
  _DropItem('Cabrio', 'Cabrio'),
  _DropItem('Kombi', 'StationWagon'),
  _DropItem('Minivan', 'Minivan'),
  _DropItem('Pick-up', 'Pickup'),
  _DropItem('Panelvan', 'Van'),
];

const _cekisItems = [
  _DropItem('Önden Çekiş', 'ÖndenÇekiş'),
  _DropItem('Arkadan İtiş', 'Arkadanİtiş'),
  _DropItem('Dört Çeker', 'DörtÇeker'),
  _DropItem('4×4', 'DörtcarpiDört'),
];

const _aracDurumuItems = [
  _DropItem('Sıfır', 'Sifir'),
  _DropItem('İkinci El', 'İkinciEl'),
];

const _kimdenItems = [
  _DropItem('Sahibinden', 'Sahibinden'),
  _DropItem('Galeriden', 'Galeriden'),
];

const _panelStatusItems = [
  _DropItem('Orijinal', 'Orijinal'),
  _DropItem('Boyalı', 'Boyali'),
  _DropItem('Değişmiş', 'Değişmiş'),
];

// 13 panel alanı: (JSON key, Türkçe gösterim)
const _panels = [
  ('motorKaputu', 'Motor Kaputu'),
  ('tavan', 'Tavan'),
  ('önTampon', 'Ön Tampon'),
  ('arkaTampon', 'Arka Tampon'),
  ('arkaKaput', 'Arka Kaput'),
  ('sağÖnÇamurluk', 'Sağ Ön Çamurluk'),
  ('sağÖnKapi', 'Sağ Ön Kapı'),
  ('sağArkaKapi', 'Sağ Arka Kapı'),
  ('sağArkaÇamurluk', 'Sağ Arka Çamurluk'),
  ('solÖnÇamurluk', 'Sol Ön Çamurluk'),
  ('solÖnKapi', 'Sol Ön Kapı'),
  ('solArkaKapi', 'Sol Arka Kapı'),
  ('solArkaÇamurluk', 'Sol Arka Çamurluk'),
];

// ─── Page ─────────────────────────────────────────────────────────────────────

class CarCreatePage extends StatefulWidget {
  const CarCreatePage({super.key});

  @override
  State<CarCreatePage> createState() => _CarCreatePageState();
}

class _CarCreatePageState extends State<CarCreatePage> {
  CarCreateViewModel? _vm;

  final _formKey = GlobalKey<FormState>();
  final _brand = TextEditingController();
  final _seri = TextEditingController();
  final _model = TextEditingController();
  final _year = TextEditingController();
  final _km = TextEditingController();
  final _price = TextEditingController();
  final _city = TextEditingController();
  final _district = TextEditingController();
  final _description = TextEditingController();
  final _renk = TextEditingController();
  final _motorHacmi = TextEditingController();
  final _motorGucu = TextEditingController();
  final _ortYakit = TextEditingController();
  final _yakitDeposu = TextEditingController();

  String? _fuelType;
  String? _gearType;
  String? _kasaTipi;
  String? _cekis;
  String? _aracDurumu;
  String? _kimden;
  bool _agirHasarKaydi = false;
  bool _takasaUygun = false;
  final Map<String, String> _panelStatus = {};

  // Tracks last shown error to avoid duplicate snackbars on rebuild
  String? _lastShownError;

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    _vm ??= CarCreateViewModel(carRepository: context.read<CarRepository>());
  }

  @override
  void dispose() {
    _vm?.dispose();
    for (final ctrl in [
      _brand, _seri, _model, _year, _km, _price, _city, _district,
      _description, _renk, _motorHacmi, _motorGucu, _ortYakit,
      _yakitDeposu,
    ]) {
      ctrl.dispose();
    }
    super.dispose();
  }

  void _submit() {
    final vm = _vm;
    if (vm == null) return;
    if (!_formKey.currentState!.validate()) return;
    if (_fuelType == null || _gearType == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Yakıt ve vites türü seçiniz.')),
      );
      return;
    }
    vm.createCar(
      brand: _brand.text.trim(),
      seri: _seri.text.trim(),
      model: _model.text.trim(),
      year: int.parse(_year.text.trim()),
      km: int.parse(_km.text.trim()),
      fuelType: _fuelType!,
      gearType: _gearType!,
      kasaTipi: _kasaTipi ?? 'Sedan',
      renk: _renk.text.trim(),
      motorHacmi: double.tryParse(_motorHacmi.text.trim()) ?? 0.0,
      motorGucu: int.tryParse(_motorGucu.text.trim()) ?? 0,
      cekis: _cekis ?? 'ÖndenÇekiş',
      aracDurumu: _aracDurumu ?? 'İkinciEl',
      ortalamaYakitTuketim: double.tryParse(_ortYakit.text.trim()) ?? 0.0,
      yakitDeposu: int.tryParse(_yakitDeposu.text.trim()) ?? 0,
      agirHasarKaydi: _agirHasarKaydi,
      takasaUygun: _takasaUygun,
      kimden: _kimden ?? 'Sahibinden',
      price: int.parse(_price.text.trim()),
      city: _city.text.trim(),
      district: _district.text.trim(),
      description: _description.text.trim(),
      boyaliDegisen: Map.from(_panelStatus),
    );
  }

  Widget _sectionHeader(String title) => Padding(
        padding: const EdgeInsets.only(
            top: AppConstants.space24, bottom: AppConstants.space8),
        child: Text(title,
            style: const TextStyle(fontSize: 15, fontWeight: FontWeight.w600)),
      );

  Widget _drop(String label, List<_DropItem> items, String? current,
      void Function(String?) onChanged) {
    return AppDropdown<String>(
      label: label,
      value: current,
      items: items
          .map((e) => DropdownMenuItem(value: e.value, child: Text(e.label)))
          .toList(),
      onChanged: (v) => setState(() => onChanged(v)),
    );
  }

  @override
  Widget build(BuildContext context) {
    final vm = _vm;
    if (vm == null) {
      return const Scaffold(body: Center(child: CircularProgressIndicator()));
    }

    return ListenableBuilder(
      listenable: vm,
      builder: (ctx, _) {
        // Navigate away on successful creation
        if (vm.createdCar != null) {
          WidgetsBinding.instance.addPostFrameCallback((_) {
            if (!mounted) return;
            ScaffoldMessenger.of(context).showSnackBar(
              const SnackBar(content: Text('İlan başarıyla oluşturuldu.')),
            );
            context.pop();
          });
        }

        // Show error snackbar once per error message
        if (vm.hasError &&
            vm.errorMessage != null &&
            vm.errorMessage != _lastShownError) {
          _lastShownError = vm.errorMessage;
          WidgetsBinding.instance.addPostFrameCallback((_) {
            if (!mounted) return;
            ScaffoldMessenger.of(context)
                .showSnackBar(SnackBar(content: Text(vm.errorMessage!)));
          });
        }

        return Scaffold(
          appBar: AppBar(title: const Text(AppStrings.carCreateTitle)),
          body: SafeArea(
            child: SingleChildScrollView(
              physics: const AlwaysScrollableScrollPhysics(),
              padding: const EdgeInsets.all(AppConstants.space16),
              child: Form(
                key: _formKey,
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    // ── Temel Bilgiler ──────────────────────────────
                    _sectionHeader('Araç Bilgileri'),
                    AppTextField(
                      label: AppStrings.brandLabel,
                      controller: _brand,
                      validator: (v) => Validators.required(v, 'Marka'),
                    ),
                    const SizedBox(height: AppConstants.space12),
                    AppTextField(
                      label: 'Seri',
                      controller: _seri,
                    ),
                    const SizedBox(height: AppConstants.space12),
                    AppTextField(
                      label: AppStrings.modelLabel,
                      controller: _model,
                      validator: (v) => Validators.required(v, 'Model'),
                    ),
                    const SizedBox(height: AppConstants.space12),
                    AppTextField(
                      label: AppStrings.yearLabel,
                      controller: _year,
                      keyboardType: TextInputType.number,
                      validator: (v) => Validators.positiveInt(v, 'Yıl'),
                    ),
                    const SizedBox(height: AppConstants.space12),
                    AppTextField(
                      label: AppStrings.kmLabel,
                      controller: _km,
                      keyboardType: TextInputType.number,
                      validator: (v) {
                        if (v == null || v.isEmpty) return 'KM zorunludur.';
                        if (int.tryParse(v) == null || int.parse(v) < 0) {
                          return 'Geçerli bir KM girin.';
                        }
                        return null;
                      },
                    ),
                    const SizedBox(height: AppConstants.space12),
                    AppTextField(
                      label: 'Renk',
                      controller: _renk,
                    ),

                    // ── Teknik Özellikler ───────────────────────────
                    _sectionHeader('Teknik Özellikler'),
                    _drop(AppStrings.fuelTypeLabel, _yakitItems, _fuelType,
                        (v) => _fuelType = v),
                    const SizedBox(height: AppConstants.space12),
                    _drop(AppStrings.gearTypeLabel, _vitesItems, _gearType,
                        (v) => _gearType = v),
                    const SizedBox(height: AppConstants.space12),
                    _drop('Kasa Tipi', _kasaItems, _kasaTipi,
                        (v) => _kasaTipi = v),
                    const SizedBox(height: AppConstants.space12),
                    _drop('Çekiş', _cekisItems, _cekis, (v) => _cekis = v),
                    const SizedBox(height: AppConstants.space12),
                    AppTextField(
                      label: 'Motor Hacmi (cc)',
                      controller: _motorHacmi,
                      keyboardType: const TextInputType.numberWithOptions(
                          decimal: true),
                    ),
                    const SizedBox(height: AppConstants.space12),
                    AppTextField(
                      label: 'Motor Gücü (HP)',
                      controller: _motorGucu,
                      keyboardType: TextInputType.number,
                    ),
                    const SizedBox(height: AppConstants.space12),
                    AppTextField(
                      label: 'Ort. Yakıt Tüketimi (L/100km)',
                      controller: _ortYakit,
                      keyboardType: const TextInputType.numberWithOptions(
                          decimal: true),
                    ),
                    const SizedBox(height: AppConstants.space12),
                    AppTextField(
                      label: 'Yakıt Deposu (L)',
                      controller: _yakitDeposu,
                      keyboardType: TextInputType.number,
                    ),

                    // ── Araç Durumu & Satıcı ────────────────────────
                    _sectionHeader('Durum & Satıcı'),
                    _drop('Araç Durumu', _aracDurumuItems, _aracDurumu,
                        (v) => _aracDurumu = v),
                    const SizedBox(height: AppConstants.space12),
                    _drop('Kimden', _kimdenItems, _kimden, (v) => _kimden = v),
                    const SizedBox(height: AppConstants.space12),
                    SwitchListTile(
                      contentPadding: EdgeInsets.zero,
                      title: const Text('Ağır Hasar Kaydı Var'),
                      value: _agirHasarKaydi,
                      onChanged: (v) => setState(() => _agirHasarKaydi = v),
                    ),
                    SwitchListTile(
                      contentPadding: EdgeInsets.zero,
                      title: const Text('Takasa Uygun'),
                      value: _takasaUygun,
                      onChanged: (v) => setState(() => _takasaUygun = v),
                    ),

                    // ── Boyalı / Değişen ────────────────────────────
                    _sectionHeader('Boyalı / Değişen'),
                    ...List.generate(_panels.length, (i) {
                      final (field, panelLabel) = _panels[i];
                      return Padding(
                        padding: const EdgeInsets.only(
                            bottom: AppConstants.space8),
                        child: Row(
                          children: [
                            Expanded(
                              flex: 3,
                              child: Text(panelLabel,
                                  style: const TextStyle(fontSize: 14)),
                            ),
                            const SizedBox(width: AppConstants.space8),
                            Expanded(
                              flex: 4,
                              child: DropdownButtonFormField<String>(
                                value: _panelStatus[field] ?? 'Orijinal',
                                isExpanded: true,
                                decoration: const InputDecoration(
                                  isDense: true,
                                  contentPadding: EdgeInsets.symmetric(
                                      horizontal: 12, vertical: 10),
                                  border: OutlineInputBorder(),
                                ),
                                items: _panelStatusItems
                                    .map((e) => DropdownMenuItem(
                                        value: e.value,
                                        child: Text(e.label)))
                                    .toList(),
                                onChanged: (v) => setState(
                                    () => _panelStatus[field] = v!),
                              ),
                            ),
                          ],
                        ),
                      );
                    }),

                    // ── Konum & Fiyat ───────────────────────────────
                    _sectionHeader('Konum & Fiyat'),
                    AppTextField(
                      label: AppStrings.priceLabel,
                      controller: _price,
                      keyboardType: TextInputType.number,
                      validator: (v) => Validators.positiveInt(v, 'Fiyat'),
                    ),
                    const SizedBox(height: AppConstants.space12),
                    AppTextField(
                      label: AppStrings.cityLabel,
                      controller: _city,
                      validator: (v) => Validators.required(v, 'İl'),
                    ),
                    const SizedBox(height: AppConstants.space12),
                    AppTextField(
                      label: AppStrings.districtLabel,
                      controller: _district,
                      validator: (v) => Validators.required(v, 'İlçe'),
                    ),
                    const SizedBox(height: AppConstants.space12),
                    AppTextField(
                      label: AppStrings.descriptionLabel,
                      controller: _description,
                      maxLines: 4,
                      validator: (v) =>
                          Validators.minLength(v, 10, 'Açıklama'),
                    ),

                    // ── Fotoğraflar ──────────────────────────────────
                    _sectionHeader(
                      'Fotoğraflar (${vm.imageUrls.length}/${AppConstants.maxCarImages})',
                    ),
                    if (vm.imageUrls.isNotEmpty) ...[
                      Wrap(
                        spacing: AppConstants.space8,
                        runSpacing: AppConstants.space8,
                        children: vm.imageUrls.asMap().entries.map((e) {
                          return Stack(
                            children: [
                              ClipRRect(
                                borderRadius: BorderRadius.circular(
                                    AppConstants.radiusCard),
                                child: Image.network(
                                  e.value,
                                  width: 90,
                                  height: 90,
                                  fit: BoxFit.cover,
                                  errorBuilder: (_, __, ___) => Container(
                                    width: 90,
                                    height: 90,
                                    color: Colors.grey.shade200,
                                    child: const Icon(Icons.broken_image,
                                        size: 32, color: Colors.grey),
                                  ),
                                ),
                              ),
                              Positioned(
                                top: 2,
                                right: 2,
                                child: GestureDetector(
                                  onTap: () => vm.removeImageUrl(e.key),
                                  child: Container(
                                    decoration: const BoxDecoration(
                                      color: Colors.black54,
                                      shape: BoxShape.circle,
                                    ),
                                    padding: const EdgeInsets.all(2),
                                    child: const Icon(Icons.close,
                                        size: 14, color: Colors.white),
                                  ),
                                ),
                              ),
                            ],
                          );
                        }).toList(),
                      ),
                      const SizedBox(height: AppConstants.space8),
                    ],
                    if (vm.canAddMoreImages)
                      vm.isUploading
                          ? const Padding(
                              padding:
                                  EdgeInsets.symmetric(vertical: AppConstants.space8),
                              child: Row(
                                children: [
                                  SizedBox(
                                    width: 20,
                                    height: 20,
                                    child: CircularProgressIndicator(strokeWidth: 2),
                                  ),
                                  SizedBox(width: AppConstants.space8),
                                  Text('Yükleniyor...'),
                                ],
                              ),
                            )
                          : OutlinedButton.icon(
                              onPressed: vm.pickAndUploadImage,
                              icon: const Icon(Icons.add_photo_alternate_outlined),
                              label: const Text('Galeriden Fotoğraf Ekle'),
                            ),

                    const SizedBox(height: AppConstants.space32),
                    PrimaryButton(
                      label: 'İlanı Yayınla',
                      isLoading: vm.isLoading,
                      onPressed: _submit,
                    ),
                    const SizedBox(height: AppConstants.space32),
                  ],
                ),
              ),
            ),
          ),
        );
      },
    );
  }
}
