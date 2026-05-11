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
            physics: const AlwaysScrollableScrollPhysics(),
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
    final name = await showDialog<String>(
      context: context,
      builder: (dialogCtx) => const _CreateListDialog(),
    );
    if (name != null && name.isNotEmpty) {
      vm.createList(name);
    }
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

class _CreateListDialog extends StatefulWidget {
  const _CreateListDialog();

  @override
  State<_CreateListDialog> createState() => _CreateListDialogState();
}

class _CreateListDialogState extends State<_CreateListDialog> {
  late final TextEditingController _controller;

  @override
  void initState() {
    super.initState();
    _controller = TextEditingController();
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: const Text(AppStrings.listCreateTitle),
      content: TextField(
        controller: _controller,
        autofocus: true,
        decoration:
            const InputDecoration(hintText: AppStrings.listNameLabel),
        onSubmitted: (_) =>
            Navigator.of(context).pop(_controller.text.trim()),
      ),
      actions: [
        TextButton(
          onPressed: () => Navigator.of(context).pop(),
          child: const Text(AppStrings.cancel),
        ),
        TextButton(
          onPressed: () =>
              Navigator.of(context).pop(_controller.text.trim()),
          child: const Text(AppStrings.save),
        ),
      ],
    );
  }
}
