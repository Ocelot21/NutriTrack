import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'auth_providers.dart';

class RegisterPage extends ConsumerStatefulWidget {
  const RegisterPage({super.key});

  @override
  ConsumerState<RegisterPage> createState() => _RegisterPageState();
}

class _RegisterPageState extends ConsumerState<RegisterPage> {
  // Step 1
  final _usernameCtrl = TextEditingController();
  final _emailCtrl = TextEditingController();
  final _passwordCtrl = TextEditingController();
  final _confirmPasswordCtrl = TextEditingController();

  // Step 2
  final _firstNameCtrl = TextEditingController();
  final _lastNameCtrl = TextEditingController();
  final _countryCtrl = TextEditingController(text: 'BA');
  final _timeZoneCtrl = TextEditingController(text: 'Europe/Sarajevo');

  @override
  void dispose() {
    _usernameCtrl.dispose();
    _emailCtrl.dispose();
    _passwordCtrl.dispose();
    _confirmPasswordCtrl.dispose();
    _firstNameCtrl.dispose();
    _lastNameCtrl.dispose();
    _countryCtrl.dispose();
    _timeZoneCtrl.dispose();
    super.dispose();
  }

  void _onNextStep() {
    final controller = ref.read(registerControllerProvider.notifier);

    controller.setCredentials(
      username: _usernameCtrl.text.trim(),
      email: _emailCtrl.text.trim(),
      password: _passwordCtrl.text,
      confirmPassword: _confirmPasswordCtrl.text,
    );
  }

  void _onBack() {
    final controller = ref.read(registerControllerProvider.notifier);
    controller.goBackToCredentials();
  }

  Future<void> _onSubmit() async {
    final controller = ref.read(registerControllerProvider.notifier);

    controller.setProfileInfo(
      firstName: _firstNameCtrl.text.trim(),
      lastName: _lastNameCtrl.text.trim(),
      countryIso2: _countryCtrl.text.trim(),
      timeZoneId: _timeZoneCtrl.text.trim(),
    );

    final state = ref.read(registerControllerProvider);
    if (state.error != null) {
      return;
    }

    await controller.submit();

    final newState = ref.read(registerControllerProvider);
    if (newState.error == null && mounted) {
      context.go('/login');
    }
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final state = ref.watch(registerControllerProvider);

    final isSubmitting = state.isSubmitting;
    final error = state.error;
    final step = state.step;

    return Scaffold(
      appBar: AppBar(
        title: const Text('Create account'),
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Center(
          child: ConstrainedBox(
            constraints: const BoxConstraints(maxWidth: 420),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                _StepIndicator(currentStep: step),
                const SizedBox(height: 24),
                if (error != null) ...[
                  Text(
                    error,
                    style: theme.textTheme.bodyMedium?.copyWith(
                      color: theme.colorScheme.error,
                    ),
                  ),
                  const SizedBox(height: 12),
                ],
                if (step == RegisterStep.credentials)
                  _CredentialsStep(
                    usernameCtrl: _usernameCtrl,
                    emailCtrl: _emailCtrl,
                    passwordCtrl: _passwordCtrl,
                    confirmPasswordCtrl: _confirmPasswordCtrl,
                    isSubmitting: isSubmitting,
                    onNext: _onNextStep,
                  )
                else
                  _ProfileStep(
                    firstNameCtrl: _firstNameCtrl,
                    lastNameCtrl: _lastNameCtrl,
                    countryCtrl: _countryCtrl,
                    timeZoneCtrl: _timeZoneCtrl,
                    isSubmitting: isSubmitting,
                    onBack: _onBack,
                    onSubmit: _onSubmit,
                  ),
                const SizedBox(height: 16),
                TextButton(
                  onPressed: () => context.go('/login'),
                  child: const Text('Already have an account? Log in'),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

class _StepIndicator extends StatelessWidget {
  final RegisterStep currentStep;

  const _StepIndicator({required this.currentStep});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    const steps = 2;
    final int index =
    currentStep == RegisterStep.credentials ? 0 : 1;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'Step ${index + 1} of $steps',
          style: theme.textTheme.bodyMedium,
        ),
        const SizedBox(height: 8),
        LinearProgressIndicator(
          value: (index + 1) / steps,
        ),
      ],
    );
  }
}

class _CredentialsStep extends StatelessWidget {
  final TextEditingController usernameCtrl;
  final TextEditingController emailCtrl;
  final TextEditingController passwordCtrl;
  final TextEditingController confirmPasswordCtrl;
  final bool isSubmitting;
  final VoidCallback onNext;

  const _CredentialsStep({
    required this.usernameCtrl,
    required this.emailCtrl,
    required this.passwordCtrl,
    required this.confirmPasswordCtrl,
    required this.isSubmitting,
    required this.onNext,
  });

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Text(
          'Account details',
          style: theme.textTheme.headlineMedium,
        ),
        const SizedBox(height: 8),
        Text(
          'Choose your username and password.',
          style: theme.textTheme.bodyMedium,
        ),
        const SizedBox(height: 24),
        TextField(
          controller: usernameCtrl,
          decoration: const InputDecoration(
            labelText: 'Username',
          ),
        ),
        const SizedBox(height: 12),
        TextField(
          controller: emailCtrl,
          keyboardType: TextInputType.emailAddress,
          decoration: const InputDecoration(
            labelText: 'Email',
          ),
        ),
        const SizedBox(height: 12),
        TextField(
          controller: passwordCtrl,
          obscureText: true,
          decoration: const InputDecoration(
            labelText: 'Password',
          ),
        ),
        const SizedBox(height: 12),
        TextField(
          controller: confirmPasswordCtrl,
          obscureText: true,
          decoration: const InputDecoration(
            labelText: 'Confirm password',
          ),
        ),
        const SizedBox(height: 24),
        SizedBox(
          height: 48,
          child: FilledButton(
            onPressed: isSubmitting ? null : onNext,
            child: isSubmitting
                ? const SizedBox(
              height: 20,
              width: 20,
              child: CircularProgressIndicator(strokeWidth: 2),
            )
                : const Text('Next'),
          ),
        ),
      ],
    );
  }
}

class _ProfileStep extends StatelessWidget {
  final TextEditingController firstNameCtrl;
  final TextEditingController lastNameCtrl;
  final TextEditingController countryCtrl;
  final TextEditingController timeZoneCtrl;
  final bool isSubmitting;
  final VoidCallback onBack;
  final VoidCallback onSubmit;

  const _ProfileStep({
    required this.firstNameCtrl,
    required this.lastNameCtrl,
    required this.countryCtrl,
    required this.timeZoneCtrl,
    required this.isSubmitting,
    required this.onBack,
    required this.onSubmit,
  });

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Text(
          'Profile & region',
          style: theme.textTheme.headlineMedium,
        ),
        const SizedBox(height: 8),
        Text(
          'Tell us a bit more about yourself.',
          style: theme.textTheme.bodyMedium,
        ),
        const SizedBox(height: 24),
        TextField(
          controller: firstNameCtrl,
          decoration: const InputDecoration(
            labelText: 'First name',
          ),
        ),
        const SizedBox(height: 12),
        TextField(
          controller: lastNameCtrl,
          decoration: const InputDecoration(
            labelText: 'Last name',
          ),
        ),
        const SizedBox(height: 12),
        TextField(
          controller: countryCtrl,
          decoration: const InputDecoration(
            labelText: 'Country ISO2',
            helperText: 'e.g. BA, US, DE',
          ),
        ),
        const SizedBox(height: 12),
        TextField(
          controller: timeZoneCtrl,
          decoration: const InputDecoration(
            labelText: 'Time zone ID',
            helperText: 'e.g. Europe/Sarajevo',
          ),
        ),
        const SizedBox(height: 24),
        Row(
          children: [
            Expanded(
              child: OutlinedButton(
                onPressed: isSubmitting ? null : onBack,
                child: const Text('Back'),
              ),
            ),
            const SizedBox(width: 12),
            Expanded(
              child: FilledButton(
                onPressed: isSubmitting ? null : onSubmit,
                child: isSubmitting
                    ? const SizedBox(
                  height: 20,
                  width: 20,
                  child: CircularProgressIndicator(strokeWidth: 2),
                )
                    : const Text('Register'),
              ),
            ),
          ],
        ),
      ],
    );
  }
}
