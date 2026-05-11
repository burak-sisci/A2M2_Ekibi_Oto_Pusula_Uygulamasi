import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../../core/constants/app_constants.dart';
import '../../../core/constants/app_strings.dart';
import '../../../core/theme/app_colors.dart';
import '../../../core/theme/app_text_styles.dart';
import '../../../core/utils/validators.dart';
import '../../../data/repositories/ai_repository.dart';
import '../../../viewmodels/base_view_model.dart';
import '../../../viewmodels/car/price_predict_view_model.dart';
import '../../widgets/common/app_dropdown.dart';
import '../../widgets/common/app_text_field.dart';
import '../../widgets/common/error_view.dart';
import '../../widgets/common/primary_button.dart';

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

const _panelStatusItems = [
  _DropItem('Orijinal', 'Orijinal'),
  _DropItem('Boyalı', 'Boyali'),
  _DropItem('Değişmiş', 'Değişmiş'),
];

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

class PricePredictPage extends StatelessWidget {
  const PricePredictPage({super.key});

  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider(
      create: (ctx) =>
          PricePredictViewModel(aiRepository: ctx.read<AiRepository>()),
      child: const _PricePredictView(),
    );
  }
}

class _PricePredictView extends StatefulWidget {
  const _PricePredictView();

  @override
  State<_PricePredictView> createState() => _PricePredictViewState();
}

class _PricePredictViewState extends State<_PricePredictView> {
  final _formKey = GlobalKey<FormState>();
  final _brand = TextEditingController();
  final _seri = TextEditingController();
  final _model = TextEditingController();
  final _year = TextEditingController();
  final _km = TextEditingController();
  final _renk = TextEditingController();
  final _motorHacmi = TextEditingController();
  final _motorGucu = TextEditingController();
  final _ortYakit = TextEditingController();
  final _yakitDeposu = TextEditingController();

  String? _fuelType;
  String? _gearType;
  String? _kasaTipi;
  String? _aracDurumu;
  String? _cekis;

  final Map<String, String> _panelStatus = {};

  @override
  void dispose() {
    _brand.dispose();
    _seri.dispose();
    _model.dispose();
    _year.dispose();
    _km.dispose();
    _renk.dispose();
    _motorHacmi.dispose();
    _motorGucu.dispose();
    _ortYakit.dispose();
    _yakitDeposu.dispose();
    super.dispose();
  }

  void _predict(PricePredictViewModel vm) {
    if (!_formKey.currentState!.validate()) return;
    if (_fuelType == null || _gearType == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Yakıt ve vites türü seçiniz.')),
      );
      return;
    }
    vm.predict(
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
      aracDurumu: _aracDurumu ?? 'İkinciEl',
      cekis: _cekis ?? 'ÖndenÇekiş',
      ortalamaYakitTuketim: double.tryParse(_ortYakit.text.trim()) ?? 0.0,
      yakitDeposu: int.tryParse(_yakitDeposu.text.trim()) ?? 0,
      boyaliDegisen: Map.from(_panelStatus),
    );
  }

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

  Widget _sectionHeader(String title) => Padding(
        padding: const EdgeInsets.only(
            top: AppConstants.space24, bottom: AppConstants.space8),
        child: Text(title,
            style: const TextStyle(fontSize: 15, fontWeight: FontWeight.w600)),
      );

  @override
  Widget build(BuildContext context) {
    return Consumer<PricePredictViewModel>(
      builder: (ctx, vm, __) => Scaffold(
        appBar: AppBar(title: const Text(AppStrings.pricePredictTitle)),
        body: SafeArea(
          child: SingleChildScrollView(
            physics: const AlwaysScrollableScrollPhysics(),
            padding: const EdgeInsets.all(AppConstants.space16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Form(
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
                        validator: (v) => Validators.positiveInt(v, 'KM'),
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
                      _drop('Araç Durumu', _aracDurumuItems, _aracDurumu,
                          (v) => _aracDurumu = v),
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

                      // ── Boyalı / Değişen ────────────────────────────
                      _sectionHeader('Boyalı / Değişen (Opsiyonel)'),
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
                    ],
                  ),
                ),
                const SizedBox(height: AppConstants.space24),
                PrimaryButton(
                  label: 'Fiyat Tahmin Et',
                  isLoading: vm.isLoading,
                  onPressed: () => _predict(vm),
                ),
                const SizedBox(height: AppConstants.space24),
                if (vm.state == ViewState.error)
                  ErrorView(message: vm.errorMessage!),
                if (vm.result != null) ...[
                  _ResultCard(vm: vm),
                ],
                const SizedBox(height: AppConstants.space32),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

class _ResultCard extends StatelessWidget {
  final PricePredictViewModel vm;

  const _ResultCard({required this.vm});

  @override
  Widget build(BuildContext context) {
    final r = vm.result!;
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(AppConstants.space16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              AppStrings.estimatedPrice,
              style: AppTextStyles.small,
            ),
            const SizedBox(height: AppConstants.space4),
            Text(
              '${r.priceLabel} ${r.unit}',
              style: AppTextStyles.h2.copyWith(color: AppColors.primary),
            ),
            const SizedBox(height: AppConstants.space8),
            Text(
              'Durum: ${r.status}',
              style: AppTextStyles.small,
            ),
          ],
        ),
      ),
    );
  }
}
