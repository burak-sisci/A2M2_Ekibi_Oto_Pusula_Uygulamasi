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

class CarCreatePage extends StatelessWidget {
  const CarCreatePage({super.key});

  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider(
      create: (ctx) =>
          CarCreateViewModel(carRepository: ctx.read<CarRepository>()),
      child: const _CarCreateView(),
    );
  }
}

class _CarCreateView extends StatefulWidget {
  const _CarCreateView();

  @override
  State<_CarCreateView> createState() => _CarCreateViewState();
}

class _CarCreateViewState extends State<_CarCreateView> {
  final _formKey = GlobalKey<FormState>();
  final _brand = TextEditingController();
  final _model = TextEditingController();
  final _year = TextEditingController();
  final _km = TextEditingController();
  final _price = TextEditingController();
  final _city = TextEditingController();
  final _district = TextEditingController();
  final _description = TextEditingController();
  String? _fuelType;
  String? _gearType;

  @override
  void dispose() {
    _brand.dispose();
    _model.dispose();
    _year.dispose();
    _km.dispose();
    _price.dispose();
    _city.dispose();
    _district.dispose();
    _description.dispose();
    super.dispose();
  }

  void _submit(CarCreateViewModel vm) {
    if (!_formKey.currentState!.validate()) return;
    vm.createCar(
      brand: _brand.text.trim(),
      model: _model.text.trim(),
      year: int.parse(_year.text.trim()),
      km: int.parse(_km.text.trim()),
      fuelType: _fuelType!,
      gearType: _gearType!,
      price: int.parse(_price.text.trim()),
      city: _city.text.trim(),
      district: _district.text.trim(),
      description: _description.text.trim(),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Consumer<CarCreateViewModel>(
      builder: (ctx, vm, __) {
        if (vm.createdCar != null) {
          WidgetsBinding.instance.addPostFrameCallback((_) {
            ScaffoldMessenger.of(context).showSnackBar(
              const SnackBar(content: Text('İlan başarıyla oluşturuldu.')),
            );
            context.pop();
          });
        }
        if (vm.hasError) {
          WidgetsBinding.instance.addPostFrameCallback((_) {
            ScaffoldMessenger.of(context)
                .showSnackBar(SnackBar(content: Text(vm.errorMessage!)));
          });
        }
        return Scaffold(
          appBar: AppBar(title: const Text(AppStrings.carCreateTitle)),
          body: SafeArea(
            child: SingleChildScrollView(
              padding: const EdgeInsets.all(AppConstants.space16),
              child: Form(
                key: _formKey,
                child: Column(
                  children: [
                    // TODO: Fotoğraf seçimi için image_picker paketi gereklidir.
                    // Paket onaylı listede değil (developer.md §1 Bağımlılık ilkesi).
                    // Geliştirici onayladığında buraya ImagePickerWidget eklenecek.
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
                      validator: (v) {
                        if (v == null || v.isEmpty) return 'KM zorunludur.';
                        if (int.tryParse(v) == null || int.parse(v) < 0) {
                          return 'Geçerli bir KM girin.';
                        }
                        return null;
                      },
                    ),
                    const SizedBox(height: AppConstants.space12),
                    AppDropdown<String>(
                      label: AppStrings.fuelTypeLabel,
                      value: _fuelType,
                      items: ['Benzin', 'Dizel', 'Elektrik', 'Hibrit', 'LPG']
                          .map((e) => DropdownMenuItem(value: e, child: Text(e)))
                          .toList(),
                      onChanged: (v) => setState(() => _fuelType = v),
                      validator: (v) =>
                          v == null ? 'Yakıt türü seçin.' : null,
                    ),
                    const SizedBox(height: AppConstants.space12),
                    AppDropdown<String>(
                      label: AppStrings.gearTypeLabel,
                      value: _gearType,
                      items: ['Manuel', 'Otomatik', 'Yarı Otomatik']
                          .map((e) => DropdownMenuItem(value: e, child: Text(e)))
                          .toList(),
                      onChanged: (v) => setState(() => _gearType = v),
                      validator: (v) =>
                          v == null ? 'Vites türü seçin.' : null,
                    ),
                    const SizedBox(height: AppConstants.space12),
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
                      validator: (v) => Validators.minLength(v, 10, 'Açıklama'),
                    ),
                    const SizedBox(height: AppConstants.space32),
                    PrimaryButton(
                      label: 'İlanı Yayınla',
                      isLoading: vm.isLoading,
                      onPressed: () => _submit(vm),
                    ),
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
