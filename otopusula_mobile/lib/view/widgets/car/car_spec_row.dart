import 'package:flutter/material.dart';
import '../../../core/theme/app_colors.dart';
import '../../../core/theme/app_text_styles.dart';

class CarSpecRow extends StatelessWidget {
  final IconData icon;
  final String label;
  final String value;

  const CarSpecRow({
    super.key,
    required this.icon,
    required this.label,
    required this.value,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Icon(icon, size: 18, color: AppColors.textSecondary),
        const SizedBox(width: 8),
        Text(label, style: AppTextStyles.small),
        const Spacer(),
        Text(value, style: AppTextStyles.body),
      ],
    );
  }
}
