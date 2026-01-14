import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:nutritrack_mobile/features/user/presentation/user_providers.dart';
import 'package:qr_flutter/qr_flutter.dart';

import '../data/two_factor_models.dart';
import '../data/user_models.dart';

class TwoFactorSettingsPage extends ConsumerStatefulWidget {
  const TwoFactorSettingsPage({super.key});

  @override
  ConsumerState<TwoFactorSettingsPage> createState() => _TwoFactorSettingsPageState();
}

class _TwoFactorSettingsPageState extends ConsumerState<TwoFactorSettingsPage> {
  final _code = TextEditingController();

  @override
  void dispose() {
    _code.dispose();
    super.dispose();
  }

  String _normalizeCode(String raw) => raw.replaceAll(RegExp(r'[^0-9]'), '');

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final state = ref.watch(userControllerProvider);
    final controller = ref.read(userControllerProvider.notifier);

    final me = state.me;
    final isBusy = state.isTwoFactorBusy;
    final setup = state.totpSetup;
    final error = state.error;

    if (me == null) {
      return Scaffold(
        appBar: AppBar(title: const Text('Two-factor authentication')),
        body: const Center(child: CircularProgressIndicator()),
      );
    }

    return Scaffold(
      appBar: AppBar(title: const Text('Two-factor authentication')),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Center(
          child: ConstrainedBox(
            constraints: const BoxConstraints(maxWidth: 520),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                Text(
                  'Protect your account',
                  style: theme.textTheme.headlineSmall,
                ),
                const SizedBox(height: 8),
                Text(
                  me.isTwoFactorEnabled
                      ? 'Two-factor authentication is enabled on your account.'
                      : 'Enable an authenticator app (Google/Microsoft/Authy) for extra security.',
                  style: theme.textTheme.bodyMedium,
                ),
                const SizedBox(height: 16),

                if (error != null) ...[
                  Text(
                    error,
                    style: theme.textTheme.bodyMedium?.copyWith(
                      color: theme.colorScheme.error,
                    ),
                  ),
                  const SizedBox(height: 12),
                ],

                if (me.isTwoFactorEnabled) ...[
                  Card(
                    child: Padding(
                      padding: const EdgeInsets.all(16),
                      child: Row(
                        children: [
                          Icon(Icons.verified_user, color: theme.colorScheme.primary),
                          const SizedBox(width: 12),
                          const Expanded(
                            child: Text('Enabled'),
                          ),
                        ],
                      ),
                    ),
                  ),
                  const SizedBox(height: 12),

                  OutlinedButton.icon(
                    onPressed: isBusy
                        ? null
                        : () async {
                      final code = await showDialog<String>(
                        context: context,
                        builder: (ctx) => _DisableTotpDialog(),
                      );

                      if (code == null) return;

                      final ok = await controller.disableTotp(_normalizeCode(code));
                      if (!context.mounted) return;

                      if (ok) {
                        ScaffoldMessenger.of(context).showSnackBar(
                          const SnackBar(content: Text('Two-factor disabled.')),
                        );
                      }
                    },
                    icon: const Icon(Icons.lock_open),
                    label: const Text('Disable two-factor'),
                  ),
                ] else ...[
                  if (setup == null) ...[
                    SizedBox(
                      height: 48,
                      child: ElevatedButton.icon(
                        onPressed: isBusy ? null : controller.beginTotpSetup,
                        icon: const Icon(Icons.shield),
                        label: isBusy
                            ? const Text('Starting...')
                            : const Text('Enable authenticator app'),
                      ),
                    ),
                  ] else ...[
                    _SetupCard(setup: setup),
                    const SizedBox(height: 16),

                    TextField(
                      controller: _code,
                      keyboardType: TextInputType.number,
                      decoration: const InputDecoration(
                        labelText: 'Enter 6-digit code',
                        hintText: '123456',
                      ),
                    ),
                    const SizedBox(height: 12),

                    SizedBox(
                      height: 48,
                      child: ElevatedButton(
                        onPressed: isBusy
                            ? null
                            : () async {
                          final code = _normalizeCode(_code.text);
                          if (code.length != 6) {
                            ScaffoldMessenger.of(context).showSnackBar(
                              const SnackBar(content: Text('Please enter the 6-digit code.')),
                            );
                            return;
                          }

                          final ok = await controller.confirmTotp(code);
                          if (ok && mounted) {
                            ScaffoldMessenger.of(context).showSnackBar(
                              const SnackBar(content: Text('Two-factor enabled.')),
                            );
                          }
                        },
                        child: isBusy
                            ? const SizedBox(
                          height: 20,
                          width: 20,
                          child: CircularProgressIndicator(strokeWidth: 2),
                        )
                            : const Text('Confirm & enable'),
                      ),
                    ),
                    const SizedBox(height: 8),

                    TextButton(
                      onPressed: isBusy ? null : controller.clearTotpSetup,
                      child: const Text('Cancel setup'),
                    ),
                  ],
                ],
              ],
            ),
          ),
        ),
      ),
    );
  }
}

class _SetupCard extends StatelessWidget {
  final TotpSetupData setup;

  const _SetupCard({required this.setup});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Text(
              'Step 1: Add to authenticator',
              style: theme.textTheme.titleMedium,
            ),
            const SizedBox(height: 8),
            const Text('Scan this QR code using your authenticator app.'),
            const SizedBox(height: 12),
            Center(
              child: QrImageView(
                data: setup.otpauthUri,
                size: 220,
              ),
            ),
            const SizedBox(height: 12),
            Text(
              'Or enter this setup key manually:',
              style: theme.textTheme.bodyMedium,
            ),
            const SizedBox(height: 8),
            SelectableText(
              setup.manualKeyBase32,
              style: theme.textTheme.titleMedium?.copyWith(
                fontFamily: 'monospace',
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _DisableTotpDialog extends StatefulWidget {
  @override
  State<_DisableTotpDialog> createState() => _DisableTotpDialogState();
}

class _DisableTotpDialogState extends State<_DisableTotpDialog> {
  final _codeCtrl = TextEditingController();

  @override
  void dispose() {
    _codeCtrl.dispose();
    super.dispose();
  }

  String _normalize(String raw) => raw.replaceAll(RegExp(r'[^0-9]'), '');

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: const Text('Disable two-factor'),
      content: TextField(
        controller: _codeCtrl,
        keyboardType: TextInputType.number,
        decoration: const InputDecoration(
          labelText: 'Authenticator code',
          hintText: '123456',
        ),
      ),
      actions: [
        TextButton(
          onPressed: () => Navigator.of(context).pop(),
          child: const Text('Cancel'),
        ),
        FilledButton(
          onPressed: () {
            final code = _normalize(_codeCtrl.text);
            if (code.length != 6) return;
            Navigator.of(context).pop(code);
          },
          child: const Text('Disable'),
        ),
      ],
    );
  }
}

