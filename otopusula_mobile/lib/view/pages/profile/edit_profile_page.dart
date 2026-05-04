import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';
import '../../../core/constants/app_constants.dart';
import '../../../core/constants/app_strings.dart';
import '../../../core/utils/validators.dart';
import '../../../data/repositories/auth_repository.dart';
import '../../../data/repositories/user_repository.dart';
import '../../../viewmodels/auth/auth_session_view_model.dart';
import '../../../viewmodels/profile/profile_view_model.dart';
import '../../widgets/common/app_text_field.dart';
import '../../widgets/common/primary_button.dart';

class EditProfilePage extends StatelessWidget {
  const EditProfilePage({super.key});

  @override
  Widget build(BuildContext context) {
    final userId = context.read<AuthSessionViewModel>().userId ?? '';
    return ChangeNotifierProvider(
      create: (ctx) => ProfileViewModel(
        userRepository: ctx.read<UserRepository>(),
        authRepository: ctx.read<AuthRepository>(),
        userId: userId,
      )..load(),
      child: const _EditProfileView(),
    );
  }
}

class _EditProfileView extends StatefulWidget {
  const _EditProfileView();

  @override
  State<_EditProfileView> createState() => _EditProfileViewState();
}

class _EditProfileViewState extends State<_EditProfileView> {
  final _formKey = GlobalKey<FormState>();
  final _phone = TextEditingController();
  bool _initialized = false;

  @override
  void dispose() {
    _phone.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Consumer<ProfileViewModel>(
      builder: (ctx, vm, __) {
        if (vm.user != null && !_initialized) {
          _phone.text = vm.user!.phone;
          _initialized = true;
        }
        if (vm.updateSuccess) {
          WidgetsBinding.instance.addPostFrameCallback((_) {
            ScaffoldMessenger.of(context).showSnackBar(
              const SnackBar(content: Text('Profil güncellendi.')),
            );
            context.pop();
          });
        }
        return Scaffold(
          appBar: AppBar(title: const Text(AppStrings.editProfileTitle)),
          body: SafeArea(
            child: Padding(
              padding: const EdgeInsets.all(AppConstants.space24),
              child: Form(
                key: _formKey,
                child: Column(
                  children: [
                    // Yalnızca telefon güncellenebilir (PUT /auth/profile — developer.md + AuthController)
                    AppTextField(
                      label: AppStrings.phoneLabel,
                      controller: _phone,
                      validator: Validators.phone,
                      keyboardType: TextInputType.phone,
                    ),
                    const SizedBox(height: AppConstants.space32),
                    PrimaryButton(
                      label: AppStrings.save,
                      isLoading: vm.isLoading,
                      onPressed: () {
                        if (!_formKey.currentState!.validate()) return;
                        vm.updatePhone(_phone.text.trim());
                      },
                    ),
                  ],
                ),
              ),
            ),
          ),
        );
      },
    );
  }
}
