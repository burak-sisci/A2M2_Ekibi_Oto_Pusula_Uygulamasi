import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../../core/constants/app_constants.dart';
import '../../../core/constants/app_strings.dart';
import '../../../data/repositories/comment_repository.dart';
import '../../../viewmodels/auth/auth_session_view_model.dart';
import '../../../viewmodels/base_view_model.dart';
import '../../../viewmodels/comment/car_comments_view_model.dart';
import '../../widgets/comment/comment_input_bar.dart';
import '../../widgets/comment/comment_tile.dart';
import '../../widgets/common/confirm_dialog.dart';
import '../../widgets/common/empty_state.dart';
import '../../widgets/common/error_view.dart';
import '../../widgets/common/loading_indicator.dart';

class CommentsPage extends StatelessWidget {
  final String carId;

  const CommentsPage({super.key, required this.carId});

  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider(
      create: (ctx) => CarCommentsViewModel(
        commentRepository: ctx.read<CommentRepository>(),
        carId: carId,
      )..load(),
      child: const _CommentsView(),
    );
  }
}

class _CommentsView extends StatefulWidget {
  const _CommentsView();

  @override
  State<_CommentsView> createState() => _CommentsViewState();
}

class _CommentsViewState extends State<_CommentsView> {
  late final ScrollController _scroll;

  @override
  void initState() {
    super.initState();
    _scroll = ScrollController();
    _scroll.addListener(_onScroll);
  }

  @override
  void dispose() {
    _scroll.dispose();
    super.dispose();
  }

  void _onScroll() {
    final vm = context.read<CarCommentsViewModel>();
    if (_scroll.position.extentAfter < AppConstants.paginationScrollThreshold) {
      vm.loadMore();
    }
  }

  @override
  Widget build(BuildContext context) {
    final currentUserId = context.read<AuthSessionViewModel>().userId;
    return Consumer<CarCommentsViewModel>(
      builder: (ctx, vm, __) {
        // İkincil işlem hatalarını snackbar ile göster
        if (vm.operationError != null) {
          WidgetsBinding.instance.addPostFrameCallback((_) {
            if (!mounted) return;
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(content: Text(vm.operationError!)),
            );
            vm.clearOperationError();
          });
        }
        return Scaffold(
          appBar: AppBar(title: const Text(AppStrings.commentsTitle)),
          body: Column(
            children: [
              Expanded(child: _buildBody(ctx, vm, currentUserId)),
              CommentInputBar(
                onSend: vm.addComment,
                isLoading: vm.isLoading,
              ),
            ],
          ),
        );
      },
    );
  }

  Widget _buildBody(
    BuildContext context,
    CarCommentsViewModel vm,
    String? currentUserId,
  ) {
    switch (vm.state) {
      case ViewState.loading:
        return const LoadingIndicator();
      case ViewState.error:
        return ErrorView(message: vm.errorMessage!, onRetry: vm.load);
      case ViewState.success:
      case ViewState.idle:
        if (vm.comments.isEmpty) {
          return const EmptyState(
            message: AppStrings.emptyComments,
            icon: Icons.comment_outlined,
          );
        }
        return RefreshIndicator(
          onRefresh: vm.load,
          child: ListView.separated(
            controller: _scroll,
            physics: const AlwaysScrollableScrollPhysics(),
            padding: const EdgeInsets.symmetric(vertical: AppConstants.space8),
            itemCount: vm.comments.length + (vm.isFetchingMore ? 1 : 0),
            separatorBuilder: (_, __) => const Divider(height: 1),
            itemBuilder: (ctx, i) {
              if (i == vm.comments.length) {
                return const Center(
                  child: Padding(
                    padding: EdgeInsets.all(AppConstants.space16),
                    child: CircularProgressIndicator(),
                  ),
                );
              }
              final comment = vm.comments[i];
              return CommentTile(
                comment: comment,
                currentUserId: currentUserId,
                onLike: () => vm.likeComment(comment.id, currentUserId),
                onUnlike: () => vm.unlikeComment(comment.id, currentUserId),
                onDelete: () async {
                  final ok = await ConfirmDialog.show(
                    ctx,
                    title: 'Yorumu Sil',
                    message: AppStrings.deleteCommentConfirm,
                    confirmLabel: AppStrings.delete,
                    isDangerous: true,
                  );
                  if (ok) vm.deleteComment(comment.id);
                },
                onEdit: () =>
                    _showEditDialog(ctx, vm, comment.id, comment.content),
              );
            },
          ),
        );
    }
  }

  Future<void> _showEditDialog(
    BuildContext context,
    CarCommentsViewModel vm,
    String commentId,
    String currentText,
  ) async {
    final controller = TextEditingController(text: currentText);
    final ok = await showDialog<bool>(
      context: context,
      builder: (dialogCtx) => AlertDialog(
        title: const Text('Yorumu Düzenle'),
        content: TextField(
          controller: controller,
          maxLines: 4,
          autofocus: true,
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(dialogCtx).pop(false),
            child: const Text(AppStrings.cancel),
          ),
          TextButton(
            onPressed: () => Navigator.of(dialogCtx).pop(true),
            child: const Text(AppStrings.save),
          ),
        ],
      ),
    );
    if (ok == true && controller.text.trim().isNotEmpty) {
      await vm.updateComment(commentId, controller.text.trim());
    }
    controller.dispose();
  }
}
