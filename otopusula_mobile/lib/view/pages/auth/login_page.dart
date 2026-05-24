import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';
import '../../../core/constants/app_constants.dart';
import '../../../core/constants/app_strings.dart';
import '../../../core/router/app_router.dart';
import '../../../core/utils/validators.dart';
import '../../../data/repositories/auth_repository.dart';
import '../../../viewmodels/auth/auth_session_view_model.dart';
import '../../../viewmodels/auth/login_view_model.dart';
import '../../widgets/common/app_text_field.dart';
import '../../widgets/common/primary_button.dart';

class LoginPage extends StatefulWidget {
  const LoginPage({super.key});

  @override
  State<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> {
  final _formKey = GlobalKey<FormState>();
  final _identifier = TextEditingController();
  final _password = TextEditingController();
  bool _passwordVisible = false;
  late final LoginViewModel _vm;

  @override
  void initState() {
    super.initState();
    _vm = LoginViewModel(authRepository: context.read<AuthRepository>());
    _vm.addListener(_onVmChange);
  }

  @override
  void dispose() {
    _identifier.dispose();
    _password.dispose();
    _vm.removeListener(_onVmChange);
    _vm.dispose();
    super.dispose();
  }

  void _onVmChange() {
    if (_vm.loginSuccess && _vm.token != null && _vm.userId != null) {
      context.read<AuthSessionViewModel>().setAuthenticated(
            _vm.token!,
            _vm.userId!,
            refreshToken: _vm.refreshToken,
          );
      // GoRouter redirect, auth durumu değişince otomatik /home'a yönlendirir
    }
    if (_vm.hasError && mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(_vm.errorMessage!)),
      );
    }
  }

  void _submit() {
    if (!_formKey.currentState!.validate()) return;
    _vm.login(_identifier.text.trim(), _password.text);
  }

  @override
  Widget build(BuildContext context) {
    return ListenableBuilder(
      listenable: _vm,
      builder: (context, _) => Scaffold(
        body: SafeArea(
          child: SingleChildScrollView(
            padding: const EdgeInsets.all(AppConstants.space24),
            child: Form(
              key: _formKey,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const SizedBox(height: AppConstants.space32),
                  Text(AppStrings.appName,
                      style: Theme.of(context).textTheme.headlineMedium),
                  const SizedBox(height: AppConstants.space8),
                  Text(AppStrings.loginTitle,
                      style: Theme.of(context).textTheme.titleMedium),
                  const SizedBox(height: AppConstants.space32),
                  AppTextField(
                    label: AppStrings.identifierLabel,
                    controller: _identifier,
                    validator: Validators.identifier,
                    keyboardType: TextInputType.emailAddress,
                    textInputAction: TextInputAction.next,
                  ),
                  const SizedBox(height: AppConstants.space16),
                  AppTextField(
                    label: AppStrings.passwordLabel,
                    controller: _password,
                    validator: (v) => Validators.required(v, 'Şifre'),
                    obscureText: !_passwordVisible,
                    textInputAction: TextInputAction.done,
                    suffixIcon: IconButton(
                      icon: Icon(
                        _passwordVisible ? Icons.visibility_off : Icons.visibility,
                      ),
                      onPressed: () =>
                          setState(() => _passwordVisible = !_passwordVisible),
                    ),
                  ),
                  const SizedBox(height: AppConstants.space32),
                  PrimaryButton(
                    label: AppStrings.loginButton,
                    isLoading: _vm.isLoading,
                    onPressed: _submit,
                  ),
                  const SizedBox(height: AppConstants.space16),
                  Center(
                    child: TextButton(
                      onPressed: () => context.push(AppRoutes.forgotPassword),
                      child: const Text('Şifremi Unuttum'),
                    ),
                  ),
                  Center(
                    child: TextButton(
                      onPressed: () => context.go(AppRoutes.register),
                      child: const Text(AppStrings.noAccount),
                    ),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}
