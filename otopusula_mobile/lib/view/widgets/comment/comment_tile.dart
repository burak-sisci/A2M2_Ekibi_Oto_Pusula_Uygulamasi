import 'package:flutter/material.dart';
import '../../../core/constants/app_constants.dart';
import '../../../core/theme/app_colors.dart';
import '../../../core/theme/app_text_styles.dart';
import '../../../core/utils/formatters.dart';
import '../../../data/models/comment.dart';

class CommentTile extends StatelessWidget {
  final Comment comment;
  final String? currentUserId;
  final VoidCallback? onDelete;
  final VoidCallback? onEdit;
  final VoidCallback? onLike;

  const CommentTile({
    super.key,
    required this.comment,
    this.currentUserId,
    this.onDelete,
    this.onEdit,
    this.onLike,
  });

  bool get _isOwner => currentUserId != null && currentUserId == comment.userId;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(
        horizontal: AppConstants.space16,
        vertical: AppConstants.space8,
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          CircleAvatar(
            radius: 18,
            backgroundColor: AppColors.surfaceMuted,
            child: const Icon(Icons.person, color: AppColors.textSecondary, size: 20),
          ),
          const SizedBox(width: AppConstants.space12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  children: [
                    Text(
                      comment.userId,
                      style: AppTextStyles.small
                          .copyWith(fontWeight: FontWeight.w600),
                    ),
                    const Spacer(),
                    Text(
                      Formatters.relativeDate(comment.createdAt),
                      style: AppTextStyles.small,
                    ),
                    if (_isOwner) ...[
                      const SizedBox(width: 4),
                      _OwnerMenu(onEdit: onEdit, onDelete: onDelete),
                    ],
                  ],
                ),
                const SizedBox(height: AppConstants.space4),
                Text(comment.content, style: AppTextStyles.body),
                // TODO: Like sayısı backend'den gelince buraya eklenir
                if (onLike != null)
                  Semantics(
                    label: 'Beğen',
                    child: IconButton(
                      onPressed: onLike,
                      icon: const Icon(Icons.thumb_up_outlined,
                          size: 18, color: AppColors.textSecondary),
                      constraints: const BoxConstraints(
                        minWidth: AppConstants.minTapTarget,
                        minHeight: AppConstants.minTapTarget,
                      ),
                    ),
                  ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _OwnerMenu extends StatelessWidget {
  final VoidCallback? onEdit;
  final VoidCallback? onDelete;

  const _OwnerMenu({this.onEdit, this.onDelete});

  @override
  Widget build(BuildContext context) {
    return PopupMenuButton<String>(
      onSelected: (v) {
        if (v == 'edit') onEdit?.call();
        if (v == 'delete') onDelete?.call();
      },
      itemBuilder: (_) => const [
        PopupMenuItem(value: 'edit', child: Text('Düzenle')),
        PopupMenuItem(value: 'delete', child: Text('Sil')),
      ],
      icon: const Icon(Icons.more_vert, size: 18, color: AppColors.textSecondary),
    );
  }
}
