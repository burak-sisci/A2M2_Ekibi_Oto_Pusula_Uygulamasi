import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import '../../../core/constants/app_constants.dart';
import '../../../core/theme/app_colors.dart';
import '../../../core/theme/app_text_styles.dart';
import '../../../core/utils/formatters.dart';
import '../../../data/models/car.dart';

class CarCard extends StatelessWidget {
  final Car car;
  final VoidCallback? onTap;

  const CarCard({super.key, required this.car, this.onTap});

  @override
  Widget build(BuildContext context) {
    return Card(
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(AppConstants.radiusCard),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            ClipRRect(
              borderRadius: const BorderRadius.vertical(
                top: Radius.circular(AppConstants.radiusCard),
              ),
              child: car.images.isNotEmpty
                  ? CachedNetworkImage(
                      imageUrl: car.images.first,
                      height: 160,
                      width: double.infinity,
                      fit: BoxFit.cover,
                      placeholder: (_, __) => _imagePlaceholder(),
                      errorWidget: (_, __, ___) => _imagePlaceholder(),
                    )
                  : _imagePlaceholder(),
            ),
            Padding(
              padding: const EdgeInsets.all(AppConstants.space12),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    '${car.brand} ${car.model} ${car.year}',
                    style: AppTextStyles.h3,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                  const SizedBox(height: AppConstants.space4),
                  Text(
                    Formatters.price(car.price),
                    style: AppTextStyles.h3.copyWith(color: AppColors.primary),
                  ),
                  const SizedBox(height: AppConstants.space8),
                  Row(
                    children: [
                      _chip(Icons.speed, Formatters.km(car.km)),
                      const SizedBox(width: AppConstants.space8),
                      _chip(Icons.local_gas_station, car.fuelType),
                      const SizedBox(width: AppConstants.space8),
                      _chip(Icons.settings, car.gearType),
                    ],
                  ),
                  const SizedBox(height: AppConstants.space4),
                  Row(
                    children: [
                      const Icon(Icons.location_on_outlined,
                          size: 14, color: AppColors.textSecondary),
                      const SizedBox(width: 2),
                      Text(
                        '${car.location.city} / ${car.location.district}',
                        style: AppTextStyles.small,
                      ),
                      const Spacer(),
                      Text(
                        Formatters.relativeDate(car.createdAt),
                        style: AppTextStyles.small,
                      ),
                    ],
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _imagePlaceholder() => Container(
        height: 160,
        color: AppColors.surfaceMuted,
        child: const Center(
          child: Icon(Icons.directions_car, color: AppColors.border, size: 48),
        ),
      );

  Widget _chip(IconData icon, String text) => Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(icon, size: 13, color: AppColors.textSecondary),
          const SizedBox(width: 2),
          Text(text, style: AppTextStyles.small),
        ],
      );
}
