import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';
import '../../../core/constants/app_constants.dart';
import '../../../core/constants/app_strings.dart';
import '../../../core/router/app_router.dart';
import '../../../core/theme/app_colors.dart';
import '../../../core/utils/formatters.dart';
import '../../../data/repositories/car_repository.dart';
import '../../../data/repositories/comment_repository.dart';
import '../../../viewmodels/auth/auth_session_view_model.dart';
import '../../../viewmodels/base_view_model.dart';
import '../../../viewmodels/car/car_detail_view_model.dart';
import '../../widgets/car/car_image_carousel.dart';
import '../../widgets/car/car_spec_row.dart';
import '../../widgets/common/confirm_dialog.dart';
import '../../widgets/common/error_view.dart';
import '../../widgets/common/loading_indicator.dart';
import '../../widgets/list/add_to_list_sheet.dart';

class CarDetailPage extends StatelessWidget {
  final String carId;

  const CarDetailPage({super.key, required this.carId});

  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider(
      create: (ctx) => CarDetailViewModel(
        carRepository: ctx.read<CarRepository>(),
        commentRepository: ctx.read<CommentRepository>(),
        carId: carId,
      )..load(),
      child: const _CarDetailView(),
    );
  }
}

class _CarDetailView extends StatelessWidget {
  const _CarDetailView();

  @override
  Widget build(BuildContext context) {
    return Consumer<CarDetailViewModel>(
      builder: (ctx, vm, __) {
        if (vm.deleteSuccess) {
          WidgetsBinding.instance.addPostFrameCallback((_) {
            vm.resetDeleteSuccess();
            context.pop();
          });
        }
        return Scaffold(
          appBar: AppBar(
            title: const Text(AppStrings.carDetailTitle),
            actions: [
              if (vm.car != null) ...[
                Semantics(
                  label: 'Paylaş',
                  child: IconButton(
                    icon: const Icon(Icons.share),
                    onPressed: () => _shareLink(ctx, vm),
                  ),
                ),
                Semantics(
                  label: 'Listeye Ekle',
                  child: IconButton(
                    icon: const Icon(Icons.bookmark_add_outlined),
                    onPressed: () => AddToListSheet.show(ctx, carId: vm.carId),
                  ),
                ),
                if (vm.car!.userId ==
                    ctx.read<AuthSessionViewModel>().userId)
                  Semantics(
                    label: 'İlanı Sil',
                    child: IconButton(
                      icon: const Icon(Icons.delete_outline,
                          color: AppColors.danger),
                      onPressed: () => _deleteCar(ctx, vm),
                    ),
                  ),
              ],
            ],
          ),
          body: _buildBody(ctx, vm),
        );
      },
    );
  }

  Widget _buildBody(BuildContext context, CarDetailViewModel vm) {
    switch (vm.state) {
      case ViewState.loading:
        return const LoadingIndicator();
      case ViewState.error:
        return ErrorView(message: vm.errorMessage!, onRetry: vm.load);
      case ViewState.success:
      case ViewState.idle:
        if (vm.car == null) return const SizedBox.shrink();
        final car = vm.car!;
        return SingleChildScrollView(
          physics: const AlwaysScrollableScrollPhysics(),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              CarImageCarousel(imageUrls: car.images),
              Padding(
                padding: const EdgeInsets.all(AppConstants.space16),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      [
                        car.brand,
                        if (car.series.isNotEmpty) car.series,
                        car.model,
                        car.year.toString(),
                      ].join(' '),
                      style: Theme.of(context).textTheme.headlineSmall,
                    ),
                    const SizedBox(height: AppConstants.space8),
                    Text(
                      Formatters.price(car.price),
                      style: Theme.of(context)
                          .textTheme
                          .headlineSmall
                          ?.copyWith(color: AppColors.primary),
                    ),
                    const Divider(height: AppConstants.space32),
                    CarSpecRow(
                      icon: Icons.speed,
                      label: AppStrings.kmLabel,
                      value: Formatters.km(car.km),
                    ),
                    const SizedBox(height: AppConstants.space12),
                    CarSpecRow(
                      icon: Icons.local_gas_station,
                      label: AppStrings.fuelTypeLabel,
                      value: car.fuelType,
                    ),
                    const SizedBox(height: AppConstants.space12),
                    CarSpecRow(
                      icon: Icons.settings,
                      label: AppStrings.gearTypeLabel,
                      value: car.gearType,
                    ),
                    if (car.bodyType.isNotEmpty) ...[
                      const SizedBox(height: AppConstants.space12),
                      CarSpecRow(
                        icon: Icons.directions_car_outlined,
                        label: 'Kasa Tipi',
                        value: car.bodyType,
                      ),
                    ],
                    if (car.color.isNotEmpty) ...[
                      const SizedBox(height: AppConstants.space12),
                      CarSpecRow(
                        icon: Icons.palette_outlined,
                        label: 'Renk',
                        value: car.color,
                      ),
                    ],
                    const SizedBox(height: AppConstants.space12),
                    CarSpecRow(
                      icon: Icons.location_on_outlined,
                      label: AppStrings.cityLabel,
                      value: '${car.location.city} / ${car.location.district}',
                    ),
                    if (car.sellerType.isNotEmpty) ...[
                      const SizedBox(height: AppConstants.space12),
                      CarSpecRow(
                        icon: Icons.storefront_outlined,
                        label: 'Kimden',
                        value: car.sellerType,
                      ),
                    ],
                    const SizedBox(height: AppConstants.space12),
                    Row(
                      children: [
                        if (car.heavilyDamaged)
                          const _BadgeChip(
                            label: 'Ağır Hasar Kaydı Var',
                            color: AppColors.danger,
                            icon: Icons.warning_amber_rounded,
                          ),
                        if (car.tradeEligible) ...[
                          if (car.heavilyDamaged)
                            const SizedBox(width: AppConstants.space8),
                          const _BadgeChip(
                            label: 'Takasa Uygun',
                            color: AppColors.primary,
                            icon: Icons.swap_horiz,
                          ),
                        ],
                      ],
                    ),
                    const Divider(height: AppConstants.space32),
                    Text(
                      AppStrings.descriptionLabel,
                      style: Theme.of(context).textTheme.titleMedium,
                    ),
                    const SizedBox(height: AppConstants.space8),
                    Text(car.description),
                    if (car.damageInfo.isNotEmpty) ...[
                      const Divider(height: AppConstants.space32),
                      Text(
                        AppStrings.damageInfoLabel,
                        style: Theme.of(context).textTheme.titleMedium,
                      ),
                      const SizedBox(height: AppConstants.space8),
                      ...car.damageInfo.map(
                        (d) => Padding(
                          padding: const EdgeInsets.only(
                              bottom: AppConstants.space4),
                          child: Row(children: [
                            const Icon(Icons.warning_amber_outlined,
                                size: 16, color: AppColors.warning),
                            const SizedBox(width: 6),
                            Text(d),
                          ]),
                        ),
                      ),
                    ],
                    const Divider(height: AppConstants.space32),
                    OutlinedButton.icon(
                      onPressed: () => context
                          .push(AppRoutes.carCommentsPath(car.id)),
                      icon: const Icon(Icons.comment_outlined),
                      label: const Text(AppStrings.commentsTitle),
                    ),
                    const SizedBox(height: AppConstants.space32),
                  ],
                ),
              ),
            ],
          ),
        );
    }
  }

  Future<void> _deleteCar(BuildContext context, CarDetailViewModel vm) async {
    final confirmed = await ConfirmDialog.show(
      context,
      title: 'İlanı Sil',
      message: AppStrings.deleteCarConfirm,
      confirmLabel: AppStrings.delete,
      isDangerous: true,
    );
    if (confirmed) vm.deleteCar();
  }

  Future<void> _shareLink(BuildContext context, CarDetailViewModel vm) async {

    await vm.fetchShareLink();
    if (!context.mounted) return;
    if (vm.shareLink != null) {
      await Clipboard.setData(ClipboardData(text: vm.shareLink!.shortUrl));
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('${AppStrings.shareLinkCopied}\n${vm.shareLink!.shortUrl}')),
      );
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(vm.shareLinkError ?? 'Paylaşım linki alınamadı.')),
      );
    }
  }
}

class _BadgeChip extends StatelessWidget {
  final String label;
  final Color color;
  final IconData icon;

  const _BadgeChip({
    required this.label,
    required this.color,
    required this.icon,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
      decoration: BoxDecoration(
        color: color.withOpacity(0.1),
        border: Border.all(color: color.withOpacity(0.4)),
        borderRadius: BorderRadius.circular(AppConstants.radiusPill),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(icon, size: 14, color: color),
          const SizedBox(width: 4),
          Text(
            label,
            style: TextStyle(fontSize: 12, color: color, fontWeight: FontWeight.w500),
          ),
        ],
      ),
    );
  }
}
