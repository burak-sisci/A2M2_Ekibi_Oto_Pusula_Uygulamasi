import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';
import '../../../core/constants/app_constants.dart';
import '../../../core/constants/app_strings.dart';
import '../../../core/router/app_router.dart';
import '../../../core/utils/validators.dart';
import '../../../data/repositories/auth_repository.dart';
import '../../../viewmodels/auth/auth_session_view_model.dart';
import '../../../viewmodels/auth/register_view_model.dart';
import '../../widgets/common/app_dropdown.dart';
import '../../widgets/common/app_text_field.dart';
import '../../widgets/common/primary_button.dart';

class RegisterPage extends StatefulWidget {
  const RegisterPage({super.key});

  @override
  State<RegisterPage> createState() => _RegisterPageState();
}

class _RegisterPageState extends State<RegisterPage> {
  final _formKey = GlobalKey<FormState>();
  final _name = TextEditingController();
  final _email = TextEditingController();
  final _phone = TextEditingController();
  final _password = TextEditingController();
  bool _passwordVisible = false;
  String? _gender;
  late final RegisterViewModel _vm;

  @override
  void initState() {
    super.initState();
    _vm = RegisterViewModel(authRepository: context.read<AuthRepository>());
    _vm.addListener(_onVmChange);
  }

  @override
  void dispose() {
    _name.dispose();
    _email.dispose();
    _phone.dispose();
    _password.dispose();
    _vm.removeListener(_onVmChange);
    _vm.dispose();
    super.dispose();
  }

  void _onVmChange() {
    if (_vm.registerSuccess && _vm.token != null && _vm.userId != null) {
      context.read<AuthSessionViewModel>().setAuthenticated(_vm.token!, _vm.userId!);
    }
    if (_vm.hasError && mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(_vm.errorMessage!)),
      );
    }
  }

  void _submit() {
    if (!_formKey.currentState!.validate()) return;
    _vm.register(
      name: _name.text.trim(),
      email: _email.text.trim(),
      phone: _phone.text.trim(),
      password: _password.text,
      gender: _gender,
    );
  }

  @override
  Widget build(BuildContext context) {
    return ListenableBuilder(
      listenable: _vm,
      builder: (context, _) => Scaffold(
        appBar: AppBar(title: const Text(AppStrings.registerTitle)),
        body: SafeArea(
          child: SingleChildScrollView(
            padding: const EdgeInsets.all(AppConstants.space24),
            child: Form(
              key: _formKey,
              child: Column(
                children: [
                  AppTextField(
                    label: AppStrings.nameLabel,
                    controller: _name,
                    validator: (v) => Validators.required(v, 'Ad soyad'),
                    textInputAction: TextInputAction.next,
                  ),
                  const SizedBox(height: AppConstants.space16),
                  AppTextField(
                    label: AppStrings.emailLabel,
                    controller: _email,
                    validator: Validators.email,
                    keyboardType: TextInputType.emailAddress,
                    textInputAction: TextInputAction.next,
                  ),
                  const SizedBox(height: AppConstants.space16),
                  AppTextField(
                    label: AppStrings.phoneLabel,
                    controller: _phone,
                    validator: Validators.phone,
                    keyboardType: TextInputType.phone,
                    textInputAction: TextInputAction.next,
                  ),
                  const SizedBox(height: AppConstants.space16),
                  AppTextField(
                    label: AppStrings.passwordLabel,
                    controller: _password,
                    validator: Validators.password,
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
                  const SizedBox(height: AppConstants.space16),
                  AppDropdown<String>(
                    label: AppStrings.genderLabel,
                    value: _gender,
                    items: ['Erkek', 'Kadın', 'Belirtmek istemiyorum']
                        .map((e) => DropdownMenuItem(value: e, child: Text(e)))
                        .toList(),
                    onChanged: (v) => setState(() => _gender = v),
                  ),
                  const SizedBox(height: AppConstants.space32),
                  PrimaryButton(
                    label: AppStrings.registerButton,
                    isLoading: _vm.isLoading,
                    onPressed: _submit,
                  ),
                  const SizedBox(height: AppConstants.space16),
                  TextButton(
                    onPressed: () => context.go(AppRoutes.login),
                    child: const Text(AppStrings.hasAccount),
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
