import 'package:flutter/material.dart';
import '../../../core/constants/app_constants.dart';
import '../../../core/constants/app_strings.dart';
import '../../../data/repositories/car_repository.dart';
import '../common/app_dropdown.dart';
import '../common/app_text_field.dart';
import '../common/primary_button.dart';
import '../common/secondary_button.dart';

class CarFilterSheet extends StatefulWidget {
  final CarFilterParams initialFilter;
  final void Function(CarFilterParams) onApply;

  const CarFilterSheet({
    super.key,
    required this.initialFilter,
    required this.onApply,
  });

  static Future<void> show(
    BuildContext context, {
    required CarFilterParams initialFilter,
    required void Function(CarFilterParams) onApply,
  }) =>
      showModalBottomSheet(
        context: context,
        isScrollControlled: true,
        shape: const RoundedRectangleBorder(
          borderRadius: BorderRadius.vertical(
            top: Radius.circular(AppConstants.radiusSheet),
          ),
        ),
        builder: (_) => CarFilterSheet(
          initialFilter: initialFilter,
          onApply: onApply,
        ),
      );

  @override
  State<CarFilterSheet> createState() => _CarFilterSheetState();
}

class _CarFilterSheetState extends State<CarFilterSheet> {
  late final TextEditingController _brand;
  late final TextEditingController _model;
  late final TextEditingController _minPrice;
  late final TextEditingController _maxPrice;
  late final TextEditingController _minKm;
  late final TextEditingController _maxKm;
  late final TextEditingController _city;
  String? _fuelType;
  String? _gearType;

  @override
  void initState() {
    super.initState();
    final f = widget.initialFilter;
    _brand = TextEditingController(text: f.brand ?? '');
    _model = TextEditingController(text: f.model ?? '');
    _minPrice = TextEditingController(text: f.minPrice?.toString() ?? '');
    _maxPrice = TextEditingController(text: f.maxPrice?.toString() ?? '');
    _minKm = TextEditingController(text: f.minKm?.toString() ?? '');
    _maxKm = TextEditingController(text: f.maxKm?.toString() ?? '');
    _city = TextEditingController(text: f.city ?? '');
    _fuelType = f.fuelType;
    _gearType = f.gearType;
  }

  @override
  void dispose() {
    _brand.dispose();
    _model.dispose();
    _minPrice.dispose();
    _maxPrice.dispose();
    _minKm.dispose();
    _maxKm.dispose();
    _city.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return DraggableScrollableSheet(
      initialChildSize: 0.85,
      minChildSize: 0.5,
      maxChildSize: 0.95,
      expand: false,
      builder: (_, controller) => Padding(
        padding: EdgeInsets.fromLTRB(
          AppConstants.space16,
          AppConstants.space16,
          AppConstants.space16,
          MediaQuery.of(context).viewInsets.bottom + AppConstants.space16,
        ),
        child: ListView(
          controller: controller,
          children: [
            Text(AppStrings.filterTitle,
                style: Theme.of(context).textTheme.titleMedium),
            const SizedBox(height: AppConstants.space16),
            AppTextField(label: AppStrings.brandLabel, controller: _brand),
            const SizedBox(height: AppConstants.space12),
            AppTextField(label: AppStrings.modelLabel, controller: _model),
            const SizedBox(height: AppConstants.space12),
            Row(children: [
              Expanded(
                child: AppTextField(
                  label: 'Min Fiyat',
                  controller: _minPrice,
                  keyboardType: TextInputType.number,
                ),
              ),
              const SizedBox(width: AppConstants.space8),
              Expanded(
                child: AppTextField(
                  label: 'Max Fiyat',
                  controller: _maxPrice,
                  keyboardType: TextInputType.number,
                ),
              ),
            ]),
            const SizedBox(height: AppConstants.space12),
            Row(children: [
              Expanded(
                child: AppTextField(
                  label: 'Min KM',
                  controller: _minKm,
                  keyboardType: TextInputType.number,
                ),
              ),
              const SizedBox(width: AppConstants.space8),
              Expanded(
                child: AppTextField(
                  label: 'Max KM',
                  controller: _maxKm,
                  keyboardType: TextInputType.number,
                ),
              ),
            ]),
            const SizedBox(height: AppConstants.space12),
            AppDropdown<String>(
              label: AppStrings.fuelTypeLabel,
              value: _fuelType,
              items: ['Benzin', 'Dizel', 'Elektrik', 'Hibrit', 'LPG']
                  .map((e) => DropdownMenuItem(value: e, child: Text(e)))
                  .toList(),
              onChanged: (v) => setState(() => _fuelType = v),
            ),
            const SizedBox(height: AppConstants.space12),
            AppDropdown<String>(
              label: AppStrings.gearTypeLabel,
              value: _gearType,
              items: ['Manuel', 'Otomatik', 'Yarı Otomatik']
                  .map((e) => DropdownMenuItem(value: e, child: Text(e)))
                  .toList(),
              onChanged: (v) => setState(() => _gearType = v),
            ),
            const SizedBox(height: AppConstants.space12),
            AppTextField(label: AppStrings.cityLabel, controller: _city),
            const SizedBox(height: AppConstants.space24),
            PrimaryButton(label: AppStrings.filterTitle, onPressed: _apply),
            const SizedBox(height: AppConstants.space8),
            SecondaryButton(label: 'Temizle', onPressed: _clear),
          ],
        ),
      ),
    );
  }

  void _apply() {
    widget.onApply(CarFilterParams(
      brand: _brand.text.trim().isEmpty ? null : _brand.text.trim(),
      model: _model.text.trim().isEmpty ? null : _model.text.trim(),
      minPrice: int.tryParse(_minPrice.text.trim()),
      maxPrice: int.tryParse(_maxPrice.text.trim()),
      minKm: int.tryParse(_minKm.text.trim()),
      maxKm: int.tryParse(_maxKm.text.trim()),
      fuelType: _fuelType,
      gearType: _gearType,
      city: _city.text.trim().isEmpty ? null : _city.text.trim(),
    ));
    Navigator.of(context).pop();
  }

  void _clear() {
    setState(() {
      _brand.clear();
      _model.clear();
      _minPrice.clear();
      _maxPrice.clear();
      _minKm.clear();
      _maxKm.clear();
      _city.clear();
      _fuelType = null;
      _gearType = null;
    });
  }
}
