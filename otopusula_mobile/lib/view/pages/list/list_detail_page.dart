import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';
import '../../../core/constants/app_constants.dart';
import '../../../core/constants/app_strings.dart';
import '../../../core/router/app_router.dart';
import '../../../data/repositories/list_repository.dart';
import '../../../viewmodels/base_view_model.dart';
import '../../../viewmodels/list/list_detail_view_model.dart';
import '../../widgets/car/car_card.dart';
import '../../widgets/common/confirm_dialog.dart';
import '../../widgets/common/empty_state.dart';
import '../../widgets/common/error_view.dart';
import '../../widgets/common/loading_indicator.dart';

class ListDetailPage extends StatelessWidget {
  final String listId;

  const ListDetailPage({super.key, required this.listId});

  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider(
      create: (ctx) => ListDetailViewModel(
        listRepository: ctx.read<ListRepository>(),
        listId: listId,
      )..load(),
      child: const _ListDetailView(),
    );
  }
}

class _ListDetailView extends StatelessWidget {
  const _ListDetailView();

  @override
  Widget build(BuildContext context) {
    return Consumer<ListDetailViewModel>(
      builder: (ctx, vm, __) => Scaffold(
        appBar: AppBar(
          title: Text(vm.userList?.name ?? ''),
          actions: [
            if (vm.userList != null && !vm.userList!.isDefault)
              Semantics(
                label: 'Listeyi Yeniden Adlandır',
                child: IconButton(
                  icon: const Icon(Icons.edit_outlined),
                  onPressed: () => _showRenameDialog(ctx, vm),
                ),
              ),
          ],
        ),
        body: _buildBody(ctx, vm),
      ),
    );
  }

  Widget _buildBody(BuildContext context, ListDetailViewModel vm) {
    switch (vm.state) {
      case ViewState.loading:
        return const LoadingIndicator();
      case ViewState.error:
        return ErrorView(message: vm.errorMessage!, onRetry: vm.load);
      case ViewState.success:
      case ViewState.idle:
        if (vm.userList == null) return const SizedBox.shrink();
        final cars = vm.userList!.cars;
        if (cars.isEmpty) {
          return const EmptyState(
            message: AppStrings.emptyList,
            icon: Icons.bookmark_border,
          );
        }
        return RefreshIndicator(
          onRefresh: vm.load,
          child: ListView.separated(
            physics: const AlwaysScrollableScrollPhysics(),
            padding: const EdgeInsets.all(AppConstants.space16),
            itemCount: cars.length,
            separatorBuilder: (_, __) =>
                const SizedBox(height: AppConstants.space12),
            itemBuilder: (ctx, i) {
              final car = cars[i];
              return Dismissible(
                key: Key(car.id),
                direction: DismissDirection.endToStart,
                background: Container(
                  alignment: Alignment.centerRight,
                  padding: const EdgeInsets.only(right: AppConstants.space16),
                  color: Theme.of(context).colorScheme.error,
                  child: const Icon(Icons.delete, color: Colors.white),
                ),
                confirmDismiss: (_) => ConfirmDialog.show(
                  context,
                  title: AppStrings.removeFromList,
                  message: 'İlanı listeden çıkarmak istiyor musunuz?',
                  confirmLabel: 'Çıkar',
                  isDangerous: true,
                ),
                onDismissed: (_) => vm.removeCarFromList(car.id),
                child: Stack(
                  children: [
                    CarCard(
                      car: car,
                      onTap: () =>
                          context.push(AppRoutes.carDetailPath(car.id)),
                    ),
                    Positioned(
                      top: AppConstants.space8,
                      right: AppConstants.space8,
                      child: Material(
                        color: Colors.black54,
                        shape: const CircleBorder(),
                        child: Semantics(
                          label: 'İlanı listeden çıkar',
                          button: true,
                          child: IconButton(
                            icon: const Icon(
                              Icons.delete_outline,
                              color: Colors.white,
                              size: 20,
                            ),
                            tooltip: AppStrings.removeFromList,
                            onPressed: () async {
                              final ok = await ConfirmDialog.show(
                                context,
                                title: AppStrings.removeFromList,
                                message:
                                    'İlanı listeden çıkarmak istiyor musunuz?',
                                confirmLabel: 'Çıkar',
                                isDangerous: true,
                              );
                              if (ok) vm.removeCarFromList(car.id);
                            },
                          ),
                        ),
                      ),
                    ),
                  ],
                ),
              );
            },
          ),
        );
    }
  }

  Future<void> _showRenameDialog(
    BuildContext context,
    ListDetailViewModel vm,
  ) async {
    final newName = await showDialog<String>(
      context: context,
      builder: (dialogCtx) => _RenameListDialog(
        initialName: vm.userList?.name ?? '',
      ),
    );
    if (newName != null && newName.isNotEmpty) {
      vm.rename(newName);
    }
  }
}

class _RenameListDialog extends StatefulWidget {
  final String initialName;

  const _RenameListDialog({required this.initialName});

  @override
  State<_RenameListDialog> createState() => _RenameListDialogState();
}

class _RenameListDialogState extends State<_RenameListDialog> {
  late final TextEditingController _controller;

  @override
  void initState() {
    super.initState();
    _controller = TextEditingController(text: widget.initialName);
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: const Text('Listeyi Yeniden Adlandır'),
      content: TextField(
        controller: _controller,
        autofocus: true,
        decoration: const InputDecoration(hintText: 'Yeni liste adı'),
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
