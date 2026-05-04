import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';
import '../../../core/constants/app_constants.dart';
import '../../../core/constants/app_strings.dart';
import '../../../core/router/app_router.dart';
import '../../../data/repositories/car_repository.dart';
import '../../../viewmodels/base_view_model.dart';
import '../../../viewmodels/car/car_list_view_model.dart';
import '../../widgets/car/car_card.dart';
import '../../widgets/car/car_filter_sheet.dart';
import '../../widgets/common/empty_state.dart';
import '../../widgets/common/error_view.dart';
import '../../widgets/common/loading_indicator.dart';

class CarListPage extends StatelessWidget {
  const CarListPage({super.key});

  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider(
      create: (ctx) =>
          CarListViewModel(carRepository: ctx.read<CarRepository>())..load(),
      child: const _CarListView(),
    );
  }
}

class _CarListView extends StatefulWidget {
  const _CarListView();

  @override
  State<_CarListView> createState() => _CarListViewState();
}

class _CarListViewState extends State<_CarListView> {
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
    final vm = context.read<CarListViewModel>();
    // 200 px kala sonraki sayfa yükle (developer.md §11)
    if (_scroll.position.extentAfter < AppConstants.paginationScrollThreshold) {
      vm.loadMore();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Consumer<CarListViewModel>(
      builder: (_, vm, __) {
        return Scaffold(
          appBar: AppBar(
            title: const Text(AppStrings.carsTitle),
            actions: [
              Semantics(
                label: 'Filtrele',
                child: IconButton(
                  icon: const Icon(Icons.tune),
                  onPressed: () => CarFilterSheet.show(
                    context,
                    initialFilter: vm.filter,
                    onApply: vm.applyFilter,
                  ),
                ),
              ),
            ],
          ),
          body: _buildBody(vm),
        );
      },
    );
  }

  Widget _buildBody(CarListViewModel vm) {
    switch (vm.state) {
      case ViewState.loading:
        return const LoadingIndicator();
      case ViewState.error:
        return ErrorView(
          message: vm.errorMessage!,
          onRetry: () => vm.load(),
        );
      case ViewState.success:
      case ViewState.idle:
        if (vm.cars.isEmpty) {
          return EmptyState(
            message: AppStrings.emptyCarList,
            icon: Icons.directions_car_outlined,
          );
        }
        return RefreshIndicator(
          onRefresh: () => vm.load(vm.filter),
          child: ListView.separated(
            controller: _scroll,
            padding: const EdgeInsets.all(AppConstants.space16),
            itemCount: vm.cars.length + (vm.isFetchingMore ? 1 : 0),
            separatorBuilder: (_, __) => const SizedBox(height: AppConstants.space12),
            itemBuilder: (ctx, i) {
              if (i == vm.cars.length) {
                return const Center(
                  child: Padding(
                    padding: EdgeInsets.all(AppConstants.space16),
                    child: CircularProgressIndicator(),
                  ),
                );
              }
              final car = vm.cars[i];
              return CarCard(
                car: car,
                onTap: () => ctx.push(AppRoutes.carDetailPath(car.id)),
              );
            },
          ),
        );
    }
  }
}
