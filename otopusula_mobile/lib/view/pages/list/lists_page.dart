import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';
import '../../../core/constants/app_constants.dart';
import '../../../core/constants/app_strings.dart';
import '../../../core/router/app_router.dart';
import '../../../data/repositories/list_repository.dart';
import '../../../data/repositories/user_repository.dart';
import '../../../viewmodels/auth/auth_session_view_model.dart';
import '../../../viewmodels/base_view_model.dart';
import '../../../viewmodels/list/user_lists_view_model.dart';
import '../../widgets/common/confirm_dialog.dart';
import '../../widgets/common/empty_state.dart';
import '../../widgets/common/error_view.dart';
import '../../widgets/common/loading_indicator.dart';
import '../../widgets/list/list_card.dart';

class ListsPage extends StatelessWidget {
  const ListsPage({super.key});

  @override
  Widget build(BuildContext context) {
    final userId = context.read<AuthSessionViewModel>().userId ?? '';
    return ChangeNotifierProvider(
      create: (ctx) => UserListsViewModel(
        userRepository: ctx.read<UserRepository>(),
        listRepository: ctx.read<ListRepository>(),
        userId: userId,
      )..load(),
      child: const _ListsView(),
    );
  }
}

class _ListsView extends StatelessWidget {
  const _ListsView();

  @override
  Widget build(BuildContext context) {
    return Consumer<UserListsViewModel>(
      builder: (ctx, vm, __) => Scaffold(
        appBar: AppBar(
          title: const Text(AppStrings.listsTitle),
          actions: [
            Semantics(
              label: 'Yeni Liste',
              child: IconButton(
                icon: const Icon(Icons.add),
                onPressed: () => _showCreateDialog(ctx, vm),
              ),
            ),
          ],
        ),
        body: _buildBody(ctx, vm),
      ),
    );
  }

  Widget _buildBody(BuildContext context, UserListsViewModel vm) {
    switch (vm.state) {
      case ViewState.loading:
        return const LoadingIndicator();
      case ViewState.error:
        return ErrorView(message: vm.errorMessage!, onRetry: vm.load);
      case ViewState.success:
      case ViewState.idle:
        if (vm.lists.isEmpty) {
          return EmptyState(
            message: 'Henüz listeniz yok.',
            icon: Icons.bookmark_border,
            action: TextButton(
              onPressed: () => _showCreateDialog(context, vm),
              child: const Text('Liste Oluştur'),
            ),
          );
        }
        return RefreshIndicator(
          onRefresh: vm.load,
          child: ListView.separated(
            padding: const EdgeInsets.all(AppConstants.space16),
            itemCount: vm.lists.length,
            separatorBuilder: (_, __) =>
                const SizedBox(height: AppConstants.space8),
            itemBuilder: (ctx, i) {
              final list = vm.lists[i];
              return ListCard(
                userList: list,
                onTap: () => ctx.push(AppRoutes.listDetailPath(list.id)),
                onDelete: list.isDefault
                    ? null
                    : () => _confirmDelete(ctx, vm, list.id),
              );
            },
          ),
        );
    }
  }

  Future<void> _showCreateDialog(
    BuildContext context,
    UserListsViewModel vm,
  ) async {
    final controller = TextEditingController();
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (_) => AlertDialog(
        title: const Text(AppStrings.listCreateTitle),
        content: TextField(
          controller: controller,
          autofocus: true,
          decoration:
              const InputDecoration(hintText: AppStrings.listNameLabel),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(false),
            child: const Text(AppStrings.cancel),
          ),
          TextButton(
            onPressed: () => Navigator.of(context).pop(true),
            child: const Text(AppStrings.save),
          ),
        ],
      ),
    );
    if (confirmed == true && controller.text.trim().isNotEmpty) {
      vm.createList(controller.text.trim());
    }
    controller.dispose();
  }

  Future<void> _confirmDelete(
    BuildContext context,
    UserListsViewModel vm,
    String listId,
  ) async {
    final ok = await ConfirmDialog.show(
      context,
      title: 'Listeyi Sil',
      message: AppStrings.deleteListConfirm,
      confirmLabel: AppStrings.delete,
      isDangerous: true,
    );
    if (ok) vm.deleteList(listId);
  }
}
