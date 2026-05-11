import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';
import '../../../core/constants/app_constants.dart';
import '../../../core/constants/app_strings.dart';
import '../../../core/router/app_router.dart';
import '../../../core/theme/app_colors.dart';
import '../../../core/theme/app_text_styles.dart';
import '../../../core/utils/formatters.dart';
import '../../../data/repositories/auth_repository.dart';
import '../../../data/repositories/user_repository.dart';
import '../../../viewmodels/auth/auth_session_view_model.dart';
import '../../../viewmodels/base_view_model.dart';
import '../../../viewmodels/profile/profile_view_model.dart';
import '../../widgets/common/confirm_dialog.dart';
import '../../widgets/common/error_view.dart';
import '../../widgets/common/loading_indicator.dart';

class ProfilePage extends StatelessWidget {
  const ProfilePage({super.key});

  @override
  Widget build(BuildContext context) {
    final userId = context.read<AuthSessionViewModel>().userId ?? '';
    return ChangeNotifierProvider(
      create: (ctx) => ProfileViewModel(
        userRepository: ctx.read<UserRepository>(),
        authRepository: ctx.read<AuthRepository>(),
        userId: userId,
      )..load(),
      child: const _ProfileView(),
    );
  }
}

class _ProfileView extends StatelessWidget {
  const _ProfileView();

  @override
  Widget build(BuildContext context) {
    return Consumer<ProfileViewModel>(
      builder: (ctx, vm, __) {
        if (vm.deleteSuccess) {
          WidgetsBinding.instance.addPostFrameCallback((_) {
            vm.resetDeleteSuccess();
            context.read<AuthSessionViewModel>().logout();
          });
        }
        return Scaffold(
          appBar: AppBar(
            title: const Text(AppStrings.profileTitle),
            actions: [
              Semantics(
                label: 'Profili Düzenle',
                child: IconButton(
                  icon: const Icon(Icons.edit_outlined),
                  onPressed: () async {
                    await context.push(AppRoutes.editProfile);
                    if (context.mounted) {
                      context.read<ProfileViewModel>().load();
                    }
                  },
                ),
              ),
            ],
          ),
          body: _buildBody(ctx, vm),
        );
      },
    );
  }

  Widget _buildBody(BuildContext context, ProfileViewModel vm) {
    switch (vm.state) {
      case ViewState.loading:
        return const LoadingIndicator();
      case ViewState.error:
        return ErrorView(message: vm.errorMessage!, onRetry: vm.load);
      case ViewState.success:
      case ViewState.idle:
        if (vm.user == null) return const SizedBox.shrink();
        final user = vm.user!;
        return ListView(
          padding: const EdgeInsets.all(AppConstants.space16),
          children: [
            const SizedBox(height: AppConstants.space16),
            Center(
              child: CircleAvatar(
                radius: 40,
                backgroundColor: AppColors.primary,
                child: Text(
                  user.name.isNotEmpty ? user.name[0].toUpperCase() : '?',
                  style: AppTextStyles.h1.copyWith(color: Colors.white),
                ),
              ),
            ),
            const SizedBox(height: AppConstants.space16),
            Center(
              child: Text(user.name, style: AppTextStyles.h2),
            ),
            const SizedBox(height: AppConstants.space16),
            Center(
              child: Container(
                padding: const EdgeInsets.symmetric(
                    horizontal: AppConstants.space16, vertical: AppConstants.space8),
                decoration: BoxDecoration(
                  color: AppColors.primary.withOpacity(0.08),
                  borderRadius: BorderRadius.circular(AppConstants.radiusPill),
                ),
                child: Row(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Icon(Icons.directions_car_outlined,
                        size: 16, color: AppColors.primary),
                    const SizedBox(width: 6),
                    Text(
                      '${user.listingCount} İlan',
                      style: AppTextStyles.small
                          .copyWith(color: AppColors.primary, fontWeight: FontWeight.w600),
                    ),
                  ],
                ),
              ),
            ),
            const SizedBox(height: AppConstants.space16),
            _InfoTile(icon: Icons.email_outlined, label: 'E-posta', value: user.email),
            const Divider(),
            _InfoTile(
              icon: Icons.phone_outlined,
              label: 'Telefon',
              value: user.phone.isNotEmpty ? user.phone : '-',
            ),
            if (user.gender != null) ...[
              const Divider(),
              _InfoTile(
                icon: Icons.person_outlined,
                label: 'Cinsiyet',
                value: user.gender!,
              ),
            ],
            const Divider(),
            _InfoTile(
              icon: Icons.calendar_today_outlined,
              label: 'Üyelik Tarihi',
              value: Formatters.date(user.createdAt),
            ),
            const SizedBox(height: AppConstants.space32),
            OutlinedButton.icon(
              icon: const Icon(Icons.logout),
              label: const Text(AppStrings.logoutTitle),
              onPressed: () => _logout(context),
            ),
            const SizedBox(height: AppConstants.space12),
            OutlinedButton.icon(
              style: OutlinedButton.styleFrom(
                foregroundColor: AppColors.danger,
                side: const BorderSide(color: AppColors.danger),
              ),
              icon: const Icon(Icons.delete_forever_outlined),
              label: const Text(AppStrings.deleteAccountTitle),
              onPressed: () => _deleteAccount(context, vm),
            ),
          ],
        );
    }
  }

  Future<void> _logout(BuildContext context) async {
    final ok = await ConfirmDialog.show(
      context,
      title: AppStrings.logoutTitle,
      message: AppStrings.logoutConfirm,
      confirmLabel: AppStrings.yes,
    );
    if (ok && context.mounted) {
      await context.read<AuthSessionViewModel>().logout();
    }
  }

  Future<void> _deleteAccount(BuildContext context, ProfileViewModel vm) async {
    final ok = await ConfirmDialog.show(
      context,
      title: AppStrings.deleteAccountTitle,
      message: AppStrings.deleteAccountConfirm,
      confirmLabel: AppStrings.delete,
      isDangerous: true,
    );
    if (ok) vm.deleteAccount();
  }
}

class _InfoTile extends StatelessWidget {
  final IconData icon;
  final String label;
  final String value;

  const _InfoTile({required this.icon, required this.label, required this.value});

  @override
  Widget build(BuildContext context) {
    return ListTile(
      leading: Icon(icon, color: AppColors.primary),
      title: Text(label, style: AppTextStyles.small),
      subtitle: Text(value, style: AppTextStyles.body),
      contentPadding: EdgeInsets.zero,
    );
  }
}
