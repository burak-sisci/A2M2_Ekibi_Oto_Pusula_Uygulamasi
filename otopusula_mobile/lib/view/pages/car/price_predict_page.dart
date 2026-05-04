import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../../core/constants/app_constants.dart';
import '../../../core/constants/app_strings.dart';
import '../../../core/theme/app_colors.dart';
import '../../../core/theme/app_text_styles.dart';
import '../../../core/utils/formatters.dart';
import '../../../core/utils/validators.dart';
import '../../../data/repositories/ai_repository.dart';
import '../../../viewmodels/base_view_model.dart';
import '../../../viewmodels/car/price_predict_view_model.dart';
import '../../widgets/common/app_dropdown.dart';
import '../../widgets/common/app_text_field.dart';
import '../../widgets/common/error_view.dart';
import '../../widgets/common/primary_button.dart';

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
  final _model = TextEditingController();
  final _year = TextEditingController();
  final _km = TextEditingController();
  String? _fuelType;
  String? _gearType;

  @override
  void dispose() {
    _brand.dispose();
    _model.dispose();
    _year.dispose();
    _km.dispose();
    super.dispose();
  }

  void _predict(PricePredictViewModel vm) {
    if (!_formKey.currentState!.validate()) return;
    vm.predict(
      brand: _brand.text.trim(),
      model: _model.text.trim(),
      year: int.parse(_year.text.trim()),
      km: int.parse(_km.text.trim()),
      fuelType: _fuelType!,
      gearType: _gearType!,
    );
  }

  @override
  Widget build(BuildContext context) {
    return Consumer<PricePredictViewModel>(
      builder: (ctx, vm, __) => Scaffold(
        appBar: AppBar(title: const Text(AppStrings.pricePredictTitle)),
        body: SafeArea(
          child: SingleChildScrollView(
            padding: const EdgeInsets.all(AppConstants.space16),
            child: Column(
              children: [
                Form(
                  key: _formKey,
                  child: Column(
                    children: [
                      AppTextField(
                        label: AppStrings.brandLabel,
                        controller: _brand,
                        validator: (v) => Validators.required(v, 'Marka'),
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
                      AppDropdown<String>(
                        label: AppStrings.fuelTypeLabel,
                        value: _fuelType,
                        items: ['Benzin', 'Dizel', 'Elektrik', 'Hibrit', 'LPG']
                            .map((e) => DropdownMenuItem(value: e, child: Text(e)))
                            .toList(),
                        onChanged: (v) => setState(() => _fuelType = v),
                        validator: (v) => v == null ? 'Yakıt türü seçin.' : null,
                      ),
                      const SizedBox(height: AppConstants.space12),
                      AppDropdown<String>(
                        label: AppStrings.gearTypeLabel,
                        value: _gearType,
                        items: ['Manuel', 'Otomatik', 'Yarı Otomatik']
                            .map((e) => DropdownMenuItem(value: e, child: Text(e)))
                            .toList(),
                        onChanged: (v) => setState(() => _gearType = v),
                        validator: (v) => v == null ? 'Vites türü seçin.' : null,
                      ),
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
              Formatters.price(r.estimatedPrice),
              style: AppTextStyles.h2.copyWith(color: AppColors.primary),
            ),
            const Divider(height: AppConstants.space24),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(AppStrings.priceRange, style: AppTextStyles.small),
                    const SizedBox(height: AppConstants.space4),
                    Text(
                      '${Formatters.price(r.priceRange.min)} – ${Formatters.price(r.priceRange.max)}',
                      style: AppTextStyles.body,
                    ),
                  ],
                ),
                Column(
                  crossAxisAlignment: CrossAxisAlignment.end,
                  children: [
                    Text(AppStrings.confidence, style: AppTextStyles.small),
                    const SizedBox(height: AppConstants.space4),
                    Text(
                      Formatters.percent(r.confidence),
                      style: AppTextStyles.body.copyWith(
                        color: r.confidence >= 0.8
                            ? AppColors.success
                            : AppColors.warning,
                      ),
                    ),
                  ],
                ),
              ],
            ),
            const SizedBox(height: AppConstants.space8),
            Text(
              'Tahmin tarihi: ${Formatters.date(r.generatedAt)}',
              style: AppTextStyles.small,
            ),
          ],
        ),
      ),
    );
  }
}
