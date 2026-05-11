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
                child: CarCard(
                  car: car,
                  onTap: () => context.push(AppRoutes.carDetailPath(car.id)),
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
    final controller = TextEditingController(text: vm.userList?.name ?? '');
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (dialogCtx) => AlertDialog(
        title: const Text('Listeyi Yeniden Adlandır'),
        content: TextField(
          controller: controller,
          autofocus: true,
          decoration: const InputDecoration(hintText: 'Yeni liste adı'),
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
    final newName = controller.text.trim();
    controller.dispose();
    if (confirmed == true && newName.isNotEmpty) {
      vm.rename(newName);
    }
  }
}
