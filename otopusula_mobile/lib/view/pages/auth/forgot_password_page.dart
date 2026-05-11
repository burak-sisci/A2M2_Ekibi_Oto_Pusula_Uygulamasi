import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';
import '../../../core/constants/app_constants.dart';
import '../../../core/router/app_router.dart';
import '../../../data/repositories/auth_repository.dart';
import '../../widgets/common/app_text_field.dart';
import '../../widgets/common/primary_button.dart';

class ForgotPasswordPage extends StatefulWidget {
  const ForgotPasswordPage({super.key});

  @override
  State<ForgotPasswordPage> createState() => _ForgotPasswordPageState();
}

class _ForgotPasswordPageState extends State<ForgotPasswordPage> {
  final _formKey = GlobalKey<FormState>();
  final _email = TextEditingController();
  bool _isLoading = false;
  String? _errorMessage;

  @override
  void dispose() {
    _email.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;
    setState(() {
      _isLoading = true;
      _errorMessage = null;
    });
    try {
      final repo = context.read<AuthRepository>();
      final result = await repo.forgotPassword(_email.text.trim());
      final token = result['resetToken'] as String?;
      if (!mounted) return;
      if (token != null && token.isNotEmpty) {
        // Token'ı direkt ResetPassword sayfasına aktar
        context.push(
          '${AppRoutes.resetPassword}?token=${Uri.encodeComponent(token)}',
        );
      } else {
        final mesaj = result['mesaj'] as String? ?? 'İstek gönderildi.';
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text(mesaj)),
        );
      }
    } catch (e) {
      if (!mounted) return;
      setState(() =>
          _errorMessage = 'İstek gönderilemedi. Lütfen tekrar deneyin.');
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Şifremi Unuttum')),
      body: SafeArea(
        child: SingleChildScrollView(
          physics: const AlwaysScrollableScrollPhysics(),
          padding: const EdgeInsets.all(AppConstants.space24),
          child: Form(
            key: _formKey,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const SizedBox(height: AppConstants.space16),
                Text(
                  'E-posta adresinizi girin',
                  style: Theme.of(context).textTheme.titleMedium,
                ),
                const SizedBox(height: AppConstants.space8),
                const Text(
                  'Şifre sıfırlama kodunu alacaksınız.',
                  style: TextStyle(color: Colors.grey),
                ),
                const SizedBox(height: AppConstants.space24),
                AppTextField(
                  label: 'E-posta',
                  controller: _email,
                  keyboardType: TextInputType.emailAddress,
                  validator: (v) {
                    if (v == null || v.trim().isEmpty) {
                      return 'E-posta zorunludur.';
                    }
                    if (!v.contains('@')) return 'Geçerli bir e-posta girin.';
                    return null;
                  },
                ),
                if (_errorMessage != null) ...[
                  const SizedBox(height: AppConstants.space12),
                  Text(_errorMessage!,
                      style: const TextStyle(color: Colors.red)),
                ],
                const SizedBox(height: AppConstants.space24),
                PrimaryButton(
                  label: 'Sıfırlama Kodu Gönder',
                  isLoading: _isLoading,
                  onPressed: _submit,
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
