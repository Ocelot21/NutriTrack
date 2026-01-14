import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'auth_providers.dart';
import '../../metadata/presentation/metadata_providers.dart';
import 'package:nutritrack_shared/metadata/data/metadata_models.dart';

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
  final _countryDisplayCtrl = TextEditingController();
  final _timeZoneCtrl = TextEditingController();

  String? _selectedCountryCode;

  @override
  void initState() {
    super.initState();
    _selectedCountryCode = 'BA';
    _countryDisplayCtrl.text = 'Bosnia and Herzegovina';
    _timeZoneCtrl.text = 'Europe/Sarajevo';
  }

  @override
  void dispose() {
    _usernameCtrl.dispose();
    _emailCtrl.dispose();
    _passwordCtrl.dispose();
    _confirmPasswordCtrl.dispose();
    _firstNameCtrl.dispose();
    _lastNameCtrl.dispose();
    _countryDisplayCtrl.dispose();
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
      countryIso2: (_selectedCountryCode ?? '').trim(),
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

  // ===== pickers =====

  Future<void> _pickCountry(
      AsyncValue<List<CountrySummary>> countriesAsync,
      ) async {
    final countries = countriesAsync.value;

    if (countries == null || countries.isEmpty) {
      if (countriesAsync.isLoading) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Loading countries, please wait...')),
        );
      } else if (countriesAsync.hasError) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Failed to load countries.')),
        );
      }
      return;
    }

    final selected = await showModalBottomSheet<CountrySummary>(
      context: context,
      isScrollControlled: true,
      builder: (context) {
        String query = '';
        final media = MediaQuery.of(context);

        return SafeArea(
          child: Padding(
            padding: EdgeInsets.only(
              left: 16,
              right: 16,
              top: 16,
              bottom: media.viewInsets.bottom + 16,
            ),
            child: SizedBox(
              height: media.size.height * 0.75,
              child: StatefulBuilder(
                builder: (context, setState) {
                  final q = query.toLowerCase();
                  final filtered = q.isEmpty
                      ? countries
                      : countries
                      .where((c) =>
                  c.name.toLowerCase().contains(q) ||
                      c.code.toLowerCase().contains(q))
                      .toList();

                  return Column(
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: [
                      Text(
                        'Select country',
                        style: Theme.of(context).textTheme.titleMedium,
                      ),
                      const SizedBox(height: 8),
                      TextField(
                        decoration: const InputDecoration(
                          prefixIcon: Icon(Icons.search),
                          hintText: 'Search country',
                        ),
                        onChanged: (value) {
                          setState(() => query = value);
                        },
                      ),
                      const SizedBox(height: 8),
                      Expanded(
                        child: ListView.builder(
                          itemCount: filtered.length,
                          itemBuilder: (context, index) {
                            final c = filtered[index];
                            final isSelected =
                                (_selectedCountryCode ?? '')
                                    .toUpperCase() ==
                                    c.code.toUpperCase();

                            return ListTile(
                              title: Text(c.name),
                              subtitle: Text(c.code),
                              trailing: isSelected
                                  ? const Icon(Icons.check)
                                  : null,
                              onTap: () {
                                Navigator.of(context).pop(c);
                              },
                            );
                          },
                        ),
                      ),
                    ],
                  );
                },
              ),
            ),
          ),
        );
      },
    );

    if (selected != null) {
      _selectedCountryCode = selected.code;
      _countryDisplayCtrl.text = selected.name;
    }
  }

  Future<void> _pickTimeZone(
      AsyncValue<List<String>> timeZonesAsync,
      ) async {
    final timeZones = timeZonesAsync.value;

    if (timeZones == null || timeZones.isEmpty) {
      if (timeZonesAsync.isLoading) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Loading time zones, please wait...')),
        );
      } else if (timeZonesAsync.hasError) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Failed to load time zones.')),
        );
      }
      return;
    }

    final selected = await showModalBottomSheet<String>(
      context: context,
      isScrollControlled: true,
      builder: (context) {
        String query = '';
        final media = MediaQuery.of(context);

        return SafeArea(
          child: Padding(
            padding: EdgeInsets.only(
              left: 16,
              right: 16,
              top: 16,
              bottom: media.viewInsets.bottom + 16,
            ),
            child: SizedBox(
              height: media.size.height * 0.75,
              child: StatefulBuilder(
                builder: (context, setState) {
                  final q = query.toLowerCase();
                  final filtered = q.isEmpty
                      ? timeZones
                      : timeZones
                      .where((tz) => tz.toLowerCase().contains(q))
                      .toList();

                  return Column(
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: [
                      Text(
                        'Select time zone',
                        style: Theme.of(context).textTheme.titleMedium,
                      ),
                      const SizedBox(height: 8),
                      TextField(
                        decoration: const InputDecoration(
                          prefixIcon: Icon(Icons.search),
                          hintText: 'Search time zone',
                        ),
                        onChanged: (value) {
                          setState(() => query = value);
                        },
                      ),
                      const SizedBox(height: 8),
                      Expanded(
                        child: ListView.builder(
                          itemCount: filtered.length,
                          itemBuilder: (context, index) {
                            final tz = filtered[index];
                            final isSelected =
                                tz.toLowerCase() ==
                                    _timeZoneCtrl.text.trim().toLowerCase();

                            return ListTile(
                              title: Text(tz),
                              trailing: isSelected
                                  ? const Icon(Icons.check)
                                  : null,
                              onTap: () {
                                Navigator.of(context).pop(tz);
                              },
                            );
                          },
                        ),
                      ),
                    ],
                  );
                },
              ),
            ),
          ),
        );
      },
    );

    if (selected != null) {
      _timeZoneCtrl.text = selected;
    }
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final state = ref.watch(registerControllerProvider);

    final isSubmitting = state.isSubmitting;
    final error = state.error;
    final step = state.step;

    // ovdje *pratimo* metadata providere (prefetch)
    final countriesAsync = ref.watch(countriesProvider);
    final timeZonesAsync = ref.watch(timeZonesProvider);

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
                // logo
                Center(
                  child: Image.asset(
                    'assets/logo/logo_horizontal_1200_400.png',
                    package: 'nutritrack_shared',
                    height: 120,
                    fit: BoxFit.contain,
                  ),
                ),
                const SizedBox(height: 24),

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
                    countryDisplayCtrl: _countryDisplayCtrl,
                    timeZoneCtrl: _timeZoneCtrl,
                    isSubmitting: isSubmitting,
                    onBack: _onBack,
                    onSubmit: _onSubmit,
                    onPickCountry: () => _pickCountry(countriesAsync),
                    onPickTimeZone: () => _pickTimeZone(timeZonesAsync),
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

// ===== UI helper widgeti =====

class _StepIndicator extends StatelessWidget {
  final RegisterStep currentStep;

  const _StepIndicator({required this.currentStep});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    const steps = 2;
    final index = currentStep == RegisterStep.credentials ? 0 : 1;

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
  final TextEditingController countryDisplayCtrl;
  final TextEditingController timeZoneCtrl;
  final bool isSubmitting;
  final VoidCallback onBack;
  final VoidCallback onSubmit;
  final VoidCallback onPickCountry;
  final VoidCallback onPickTimeZone;

  const _ProfileStep({
    required this.firstNameCtrl,
    required this.lastNameCtrl,
    required this.countryDisplayCtrl,
    required this.timeZoneCtrl,
    required this.isSubmitting,
    required this.onBack,
    required this.onSubmit,
    required this.onPickCountry,
    required this.onPickTimeZone,
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
          controller: countryDisplayCtrl,
          readOnly: true,
          decoration: const InputDecoration(
            labelText: 'Country',
            hintText: 'Select your country',
            helperText: 'Tap to choose your country',
            suffixIcon: Icon(Icons.arrow_drop_down),
          ),
          onTap: onPickCountry,
        ),
        const SizedBox(height: 12),
        TextField(
          controller: timeZoneCtrl,
          readOnly: true,
          decoration: const InputDecoration(
            labelText: 'Time zone',
            hintText: 'Select your time zone',
            helperText: 'Tap to choose your time zone',
            suffixIcon: Icon(Icons.arrow_drop_down),
          ),
          onTap: onPickTimeZone,
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