import 'package:flutter/material.dart';
import '../../../core/constants/app_constants.dart';
import '../../../core/constants/app_strings.dart';
import '../../../core/theme/app_colors.dart';

class CommentInputBar extends StatefulWidget {
  final void Function(String text) onSend;
  final bool isLoading;

  const CommentInputBar({
    super.key,
    required this.onSend,
    this.isLoading = false,
  });

  @override
  State<CommentInputBar> createState() => _CommentInputBarState();
}

class _CommentInputBarState extends State<CommentInputBar> {
  final _controller = TextEditingController();

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  void _send() {
    final text = _controller.text.trim();
    if (text.isEmpty) return;
    widget.onSend(text);
    _controller.clear();
  }

  @override
  Widget build(BuildContext context) {
    return SafeArea(
      child: Container(
        padding: const EdgeInsets.symmetric(
          horizontal: AppConstants.space12,
          vertical: AppConstants.space8,
        ),
        decoration: const BoxDecoration(
          color: AppColors.surface,
          border: Border(top: BorderSide(color: AppColors.border)),
        ),
        child: Row(
          children: [
            Expanded(
              child: TextField(
                controller: _controller,
                minLines: 1,
                maxLines: 4,
                textInputAction: TextInputAction.newline,
                decoration: const InputDecoration(
                  hintText: AppStrings.commentPlaceholder,
                  border: InputBorder.none,
                  contentPadding: EdgeInsets.symmetric(
                    horizontal: AppConstants.space8,
                    vertical: AppConstants.space8,
                  ),
                ),
              ),
            ),
            Semantics(
              label: AppStrings.sendComment,
              child: IconButton(
                onPressed: widget.isLoading ? null : _send,
                icon: widget.isLoading
                    ? const SizedBox(
                        width: 20,
                        height: 20,
                        child: CircularProgressIndicator(strokeWidth: 2),
                      )
                    : const Icon(Icons.send, color: AppColors.primary),
                constraints: const BoxConstraints(
                  minWidth: AppConstants.minTapTarget,
                  minHeight: AppConstants.minTapTarget,
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
