import 'package:flutter/material.dart';
import '../../../core/constants/app_constants.dart';
import '../../../core/theme/app_colors.dart';
import '../../../core/theme/app_text_styles.dart';
import '../../../data/models/list_model.dart';

class ListCard extends StatelessWidget {
  final UserList userList;
  final VoidCallback? onTap;
  final VoidCallback? onDelete;

  const ListCard({
    super.key,
    required this.userList,
    this.onTap,
    this.onDelete,
  });

  @override
  Widget build(BuildContext context) {
    return Card(
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(AppConstants.radiusCard),
        child: Padding(
          padding: const EdgeInsets.all(AppConstants.space16),
          child: Row(
            children: [
              const Icon(Icons.list_alt_outlined, color: AppColors.primary, size: 28),
              const SizedBox(width: AppConstants.space12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(userList.name, style: AppTextStyles.h3),
                    const SizedBox(height: AppConstants.space4),
                    Text(
                      '${userList.totalCars} ilan',
                      style: AppTextStyles.small,
                    ),
                  ],
                ),
              ),
              if (userList.isDefault)
                Container(
                  padding: const EdgeInsets.symmetric(
                    horizontal: AppConstants.space8,
                    vertical: AppConstants.space4,
                  ),
                  decoration: BoxDecoration(
                    color: AppColors.primary.withValues(alpha: 0.1),
                    borderRadius: BorderRadius.circular(AppConstants.radiusPill),
                  ),
                  child: Text(
                    'Varsayılan',
                    style: AppTextStyles.small.copyWith(color: AppColors.primary),
                  ),
                ),
              if (!userList.isDefault && onDelete != null)
                Semantics(
                  label: 'Listeyi sil',
                  child: IconButton(
                    onPressed: onDelete,
                    icon: const Icon(Icons.delete_outline, color: AppColors.danger),
                    constraints: const BoxConstraints(
                      minWidth: AppConstants.minTapTarget,
                      minHeight: AppConstants.minTapTarget,
                    ),
                  ),
                ),
            ],
          ),
        ),
      ),
    );
  }
}
