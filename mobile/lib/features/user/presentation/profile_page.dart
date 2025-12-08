import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:image_picker/image_picker.dart';
import 'package:go_router/go_router.dart';

import '../data/user_models.dart';
import 'user_providers.dart';

class ProfilePage extends ConsumerStatefulWidget {
  const ProfilePage({super.key});

  @override
  ConsumerState<ProfilePage> createState() => _ProfilePageState();
}

class _ProfilePageState extends ConsumerState<ProfilePage> {
  bool _editingUsername = false;
  bool _editingFirstName = false;
  bool _editingLastName = false;
  bool _editingTimeZone = false;
  bool _editingCountry = false;

  final _usernameCtrl = TextEditingController();
  final _firstNameCtrl = TextEditingController();
  final _lastNameCtrl = TextEditingController();
  final _timeZoneCtrl = TextEditingController();
  final _countryCtrl = TextEditingController();

  final _currentPwdCtrl = TextEditingController();
  final _newPwdCtrl = TextEditingController();
  final _confirmPwdCtrl = TextEditingController();

  bool _changingPassword = false;
  bool _obscureCurrent = true;
  bool _obscureNew = true;
  bool _obscureConfirm = true;

  @override
  void dispose() {
    _usernameCtrl.dispose();
    _firstNameCtrl.dispose();
    _lastNameCtrl.dispose();
    _timeZoneCtrl.dispose();
    _countryCtrl.dispose();
    _currentPwdCtrl.dispose();
    _newPwdCtrl.dispose();
    _confirmPwdCtrl.dispose();
    super.dispose();
  }

  void _initTextControllers(Me me) {
    _usernameCtrl.text = me.username;
    _firstNameCtrl.text = me.firstName;
    _lastNameCtrl.text = me.lastName;
    _timeZoneCtrl.text = me.timeZoneId;
    _countryCtrl.text = me.country ?? '';
  }

  Future<void> _editField({
    required String fieldName,
    required TextEditingController controller,
    required VoidCallback stopEditing,
  }) async {
    final controllerState = ref.read(userControllerProvider.notifier);
    final ok = await controllerState.updateField(
      fieldName,
      controller.text.trim(),
    );

    if (!mounted) return;

    if (ok) {
      stopEditing();
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Profile updated')),
      );
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Failed to update profile')),
      );
    }
  }

  Future<void> _pickAvatar() async {
    final picker = ImagePicker();
    final picked = await picker.pickImage(source: ImageSource.gallery);
    if (picked == null) return;

    final file = await MultipartFile.fromFile(
      picked.path,
      filename: picked.name,
    );

    final controllerState = ref.read(userControllerProvider.notifier);
    final ok = await controllerState.uploadAvatar(file);

    if (!mounted) return;

    if (ok) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Avatar updated')),
      );
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Failed to update avatar')),
      );
    }
  }

  Future<void> _changePassword() async {
    final current = _currentPwdCtrl.text;
    final newPwd = _newPwdCtrl.text;
    final confirm = _confirmPwdCtrl.text;

    if (newPwd != confirm) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('New passwords do not match')),
      );
      return;
    }

    setState(() => _changingPassword = true);

    final controllerState = ref.read(userControllerProvider.notifier);
    final ok = await controllerState.changePassword(
      currentPassword: current,
      newPassword: newPwd,
      confirmPassword: confirm,
    );

    if (!mounted) return;

    setState(() => _changingPassword = false);

    if (ok) {
      _currentPwdCtrl.clear();
      _newPwdCtrl.clear();
      _confirmPwdCtrl.clear();

      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Password changed')),
      );
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Failed to change password')),
      );
    }
  }

  String _formatDate(DateTime? date) {
    if (date == null) return '-';
    return '${date.day.toString().padLeft(2, '0')}.'
        '${date.month.toString().padLeft(2, '0')}.'
        '${date.year}';
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(userControllerProvider);
    final theme = Theme.of(context);

    final me = state.me;

    if (me != null) {
      _initTextControllers(me);
    }

    return Scaffold(
      appBar: AppBar(
        title: const Text('Profile'),
      ),
      body: SafeArea(
        child: Center(
          child: ConstrainedBox(
            constraints: const BoxConstraints(maxWidth: 520),
            child: state.isLoading && me == null
                ? const Center(child: CircularProgressIndicator())
                : RefreshIndicator(
              onRefresh:
              ref.read(userControllerProvider.notifier).loadMe,
              child: ListView(
                padding: const EdgeInsets.all(16),
                children: [
                  if (state.error != null) ...[
                    Text(
                      state.error!,
                      style: theme.textTheme.bodyMedium?.copyWith(
                        color: theme.colorScheme.error,
                      ),
                    ),
                    const SizedBox(height: 12),
                  ],
                  if (me != null) ...[
                    _AvatarSection(
                      me: me,
                      onChangeAvatar: _pickAvatar,
                    ),
                    const SizedBox(height: 24),
                    _AccountSection(
                      me: me,
                      usernameCtrl: _usernameCtrl,
                      firstNameCtrl: _firstNameCtrl,
                      lastNameCtrl: _lastNameCtrl,
                      timeZoneCtrl: _timeZoneCtrl,
                      countryCtrl: _countryCtrl,
                      editingUsername: _editingUsername,
                      editingFirstName: _editingFirstName,
                      editingLastName: _editingLastName,
                      editingTimeZone: _editingTimeZone,
                      editingCountry: _editingCountry,
                      onEditUsername: () =>
                          setState(() => _editingUsername = true),
                      onCancelUsername: () =>
                          setState(() => _editingUsername = false),
                      onEditFirstName: () =>
                          setState(() => _editingFirstName = true),
                      onCancelFirstName: () =>
                          setState(() => _editingFirstName = false),
                      onEditLastName: () =>
                          setState(() => _editingLastName = true),
                      onCancelLastName: () =>
                          setState(() => _editingLastName = false),
                      onEditTimeZone: () =>
                          setState(() => _editingTimeZone = true),
                      onCancelTimeZone: () =>
                          setState(() => _editingTimeZone = false),
                      onEditCountry: () =>
                          setState(() => _editingCountry = true),
                      onCancelCountry: () =>
                          setState(() => _editingCountry = false),
                      onSaveUsername: () => _editField(
                        fieldName: 'username',
                        controller: _usernameCtrl,
                        stopEditing: () =>
                            setState(() => _editingUsername = false),
                      ),
                      onSaveFirstName: () => _editField(
                        fieldName: 'firstName',
                        controller: _firstNameCtrl,
                        stopEditing: () =>
                            setState(() => _editingFirstName = false),
                      ),
                      onSaveLastName: () => _editField(
                        fieldName: 'lastName',
                        controller: _lastNameCtrl,
                        stopEditing: () =>
                            setState(() => _editingLastName = false),
                      ),
                      onSaveTimeZone: () => _editField(
                        fieldName: 'timeZoneId',
                        controller: _timeZoneCtrl,
                        stopEditing: () =>
                            setState(() => _editingTimeZone = false),
                      ),
                      onSaveCountry: () => _editField(
                        fieldName: 'country',
                        controller: _countryCtrl,
                        stopEditing: () =>
                            setState(() => _editingCountry = false),
                      ),
                    ),
                    const SizedBox(height: 24),
                    _HealthProfileSection(
                      me: me,
                      formatDate: _formatDate,
                      onEditHealthProfile: () {
                        context.push('/health-profile');
                      },
                    ),
                    const SizedBox(height: 24),
                    _PasswordSection(
                      currentCtrl: _currentPwdCtrl,
                      newCtrl: _newPwdCtrl,
                      confirmCtrl: _confirmPwdCtrl,
                      obscureCurrent: _obscureCurrent,
                      obscureNew: _obscureNew,
                      obscureConfirm: _obscureConfirm,
                      onToggleCurrent: () => setState(
                              () => _obscureCurrent = !_obscureCurrent),
                      onToggleNew: () =>
                          setState(() => _obscureNew = !_obscureNew),
                      onToggleConfirm: () => setState(
                              () => _obscureConfirm = !_obscureConfirm),
                      onChangePassword: _changingPassword
                          ? null
                          : _changePassword,
                      changingPassword: _changingPassword,
                    ),
                  ] else ...[
                    Text(
                      'Unable to load profile.',
                      style: theme.textTheme.bodyMedium,
                    ),
                  ],
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}

class _AvatarSection extends StatelessWidget {
  final Me me;
  final VoidCallback onChangeAvatar;

  const _AvatarSection({
    required this.me,
    required this.onChangeAvatar,
  });

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final initials = (me.firstName.isNotEmpty ? me.firstName[0] : me.username[0])
        .toUpperCase();

    ImageProvider? image;
    if (me.avatarUrl != null && me.avatarUrl!.isNotEmpty) {
      image = NetworkImage(me.avatarUrl!);
    }

    return Column(
      children: [
        CircleAvatar(
          radius: 40,
          backgroundImage: me.avatarUrl != null && me.avatarUrl!.isNotEmpty
              ? NetworkImage(me.avatarUrl!)
              : null,
          child: (me.avatarUrl == null || me.avatarUrl!.isEmpty)
              ? const Icon(Icons.person, size: 40)
              : null,
        ),
        const SizedBox(height: 8),
        Text(
          '${me.firstName} ${me.lastName}',
          style: theme.textTheme.titleMedium,
        ),
        const SizedBox(height: 4),
        Text(
          me.email,
          style: theme.textTheme.bodyMedium,
        ),
        const SizedBox(height: 12),
        TextButton.icon(
          onPressed: onChangeAvatar,
          icon: const Icon(Icons.photo_camera),
          label: const Text('Change avatar'),
        ),
      ],
    );
  }
}

class _AccountSection extends StatelessWidget {
  final Me me;

  final TextEditingController usernameCtrl;
  final TextEditingController firstNameCtrl;
  final TextEditingController lastNameCtrl;
  final TextEditingController timeZoneCtrl;
  final TextEditingController countryCtrl;

  final bool editingUsername;
  final bool editingFirstName;
  final bool editingLastName;
  final bool editingTimeZone;
  final bool editingCountry;

  final VoidCallback onEditUsername;
  final VoidCallback onCancelUsername;
  final VoidCallback onSaveUsername;

  final VoidCallback onEditFirstName;
  final VoidCallback onCancelFirstName;
  final VoidCallback onSaveFirstName;

  final VoidCallback onEditLastName;
  final VoidCallback onCancelLastName;
  final VoidCallback onSaveLastName;

  final VoidCallback onEditTimeZone;
  final VoidCallback onCancelTimeZone;
  final VoidCallback onSaveTimeZone;

  final VoidCallback onEditCountry;
  final VoidCallback onCancelCountry;
  final VoidCallback onSaveCountry;

  const _AccountSection({
    required this.me,
    required this.usernameCtrl,
    required this.firstNameCtrl,
    required this.lastNameCtrl,
    required this.timeZoneCtrl,
    required this.countryCtrl,
    required this.editingUsername,
    required this.editingFirstName,
    required this.editingLastName,
    required this.editingTimeZone,
    required this.editingCountry,
    required this.onEditUsername,
    required this.onCancelUsername,
    required this.onSaveUsername,
    required this.onEditFirstName,
    required this.onCancelFirstName,
    required this.onSaveFirstName,
    required this.onEditLastName,
    required this.onCancelLastName,
    required this.onSaveLastName,
    required this.onEditTimeZone,
    required this.onCancelTimeZone,
    required this.onSaveTimeZone,
    required this.onEditCountry,
    required this.onCancelCountry,
    required this.onSaveCountry,
  });

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    Widget editableRow({
      required String label,
      required String value,
      required bool isEditing,
      required TextEditingController controller,
      required VoidCallback onEdit,
      required VoidCallback onCancel,
      required VoidCallback onSave,
    }) {
      return Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(label, style: theme.textTheme.bodySmall),
          const SizedBox(height: 4),
          if (!isEditing)
            Row(
              children: [
                Expanded(
                  child: Text(
                    value,
                    style: theme.textTheme.bodyMedium,
                  ),
                ),
                IconButton(
                  icon: const Icon(Icons.edit),
                  onPressed: onEdit,
                ),
              ],
            )
          else
            Row(
              children: [
                Expanded(
                  child: TextField(
                    controller: controller,
                    decoration: const InputDecoration(
                      isDense: true,
                    ),
                  ),
                ),
                IconButton(
                  icon: const Icon(Icons.close),
                  onPressed: onCancel,
                ),
                IconButton(
                  icon: const Icon(Icons.check),
                  onPressed: onSave,
                ),
              ],
            ),
          const SizedBox(height: 8),
        ],
      );
    }

    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('Account',
                style: theme.textTheme.titleMedium
                    ?.copyWith(fontWeight: FontWeight.bold)),
            const SizedBox(height: 12),
            editableRow(
              label: 'Username',
              value: me.username,
              isEditing: editingUsername,
              controller: usernameCtrl,
              onEdit: onEditUsername,
              onCancel: onCancelUsername,
              onSave: onSaveUsername,
            ),
            editableRow(
              label: 'First name',
              value: me.firstName,
              isEditing: editingFirstName,
              controller: firstNameCtrl,
              onEdit: onEditFirstName,
              onCancel: onCancelFirstName,
              onSave: onSaveFirstName,
            ),
            editableRow(
              label: 'Last name',
              value: me.lastName,
              isEditing: editingLastName,
              controller: lastNameCtrl,
              onEdit: onEditLastName,
              onCancel: onCancelLastName,
              onSave: onSaveLastName,
            ),
            const SizedBox(height: 4),
            Text('Email', style: theme.textTheme.bodySmall),
            const SizedBox(height: 4),
            Text(me.email, style: theme.textTheme.bodyMedium),
            const SizedBox(height: 8),
            if (me.role != null) ...[
              Text('Role', style: theme.textTheme.bodySmall),
              const SizedBox(height: 4),
              Text(me.role!, style: theme.textTheme.bodyMedium),
              const SizedBox(height: 8),
            ],
            editableRow(
              label: 'Time zone',
              value: me.timeZoneId,
              isEditing: editingTimeZone,
              controller: timeZoneCtrl,
              onEdit: onEditTimeZone,
              onCancel: onCancelTimeZone,
              onSave: onSaveTimeZone,
            ),
            editableRow(
              label: 'Country (ISO2)',
              value: me.country ?? '',
              isEditing: editingCountry,
              controller: countryCtrl,
              onEdit: onEditCountry,
              onCancel: onCancelCountry,
              onSave: onSaveCountry,
            ),
            const SizedBox(height: 4),
            Text(
              me.isEmailVerified ? 'Email verified' : 'Email not verified',
              style: theme.textTheme.bodySmall?.copyWith(
                color: me.isEmailVerified
                    ? Colors.green
                    : theme.colorScheme.error,
              ),
            ),
            if (me.lastLoginAtUtc != null) ...[
              const SizedBox(height: 4),
              Text(
                'Last login: ${me.lastLoginAtUtc}',
                style: theme.textTheme.bodySmall,
              ),
            ],
          ],
        ),
      ),
    );
  }
}

class _HealthProfileSection extends StatelessWidget {
  final Me me;
  final String Function(DateTime?) formatDate;
  final VoidCallback onEditHealthProfile;

  const _HealthProfileSection({
    required this.me,
    required this.formatDate,
    required this.onEditHealthProfile,
  });

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('Health profile',
                style: theme.textTheme.titleMedium
                    ?.copyWith(fontWeight: FontWeight.bold)),
            const SizedBox(height: 8),
            Text(
              me.isHealthProfileCompleted
                  ? 'Completed'
                  : 'Not completed yet',
              style: theme.textTheme.bodySmall?.copyWith(
                color: me.isHealthProfileCompleted
                    ? Colors.green
                    : theme.colorScheme.error,
              ),
            ),
            const SizedBox(height: 12),
            _row(theme, 'Gender', me.gender),
            _row(theme, 'Activity level', me.activityLevel),
            _row(theme, 'Birthdate', formatDate(me.birthdate)),
            _row(theme, 'Height (cm)',
                me.heightCm != null ? '${me.heightCm!.toStringAsFixed(1)}' : '-'),
            _row(theme, 'Weight (kg)',
                me.weightKg != null ? '${me.weightKg!.toStringAsFixed(1)}' : '-'),
            const SizedBox(height: 12),
            Align(
              alignment: Alignment.centerRight,
              child: TextButton(
                onPressed: onEditHealthProfile,
                child: const Text('Edit health profile'),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _row(ThemeData theme, String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 2),
      child: Row(
        children: [
          Expanded(
            child: Text(label, style: theme.textTheme.bodySmall),
          ),
          Text(value, style: theme.textTheme.bodyMedium),
        ],
      ),
    );
  }
}

class _PasswordSection extends StatelessWidget {
  final TextEditingController currentCtrl;
  final TextEditingController newCtrl;
  final TextEditingController confirmCtrl;

  final bool obscureCurrent;
  final bool obscureNew;
  final bool obscureConfirm;

  final VoidCallback onToggleCurrent;
  final VoidCallback onToggleNew;
  final VoidCallback onToggleConfirm;

  final VoidCallback? onChangePassword;
  final bool changingPassword;

  const _PasswordSection({
    required this.currentCtrl,
    required this.newCtrl,
    required this.confirmCtrl,
    required this.obscureCurrent,
    required this.obscureNew,
    required this.obscureConfirm,
    required this.onToggleCurrent,
    required this.onToggleNew,
    required this.onToggleConfirm,
    required this.onChangePassword,
    required this.changingPassword,
  });

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('Password',
                style: theme.textTheme.titleMedium
                    ?.copyWith(fontWeight: FontWeight.bold)),
            const SizedBox(height: 12),
            TextField(
              controller: currentCtrl,
              obscureText: obscureCurrent,
              decoration: InputDecoration(
                labelText: 'Current password',
                suffixIcon: IconButton(
                  icon: Icon(
                    obscureCurrent ? Icons.visibility : Icons.visibility_off,
                  ),
                  onPressed: onToggleCurrent,
                ),
              ),
            ),
            const SizedBox(height: 12),
            TextField(
              controller: newCtrl,
              obscureText: obscureNew,
              decoration: InputDecoration(
                labelText: 'New password',
                suffixIcon: IconButton(
                  icon: Icon(
                    obscureNew ? Icons.visibility : Icons.visibility_off,
                  ),
                  onPressed: onToggleNew,
                ),
              ),
            ),
            const SizedBox(height: 12),
            TextField(
              controller: confirmCtrl,
              obscureText: obscureConfirm,
              decoration: InputDecoration(
                labelText: 'Confirm new password',
                suffixIcon: IconButton(
                  icon: Icon(
                    obscureConfirm ? Icons.visibility : Icons.visibility_off,
                  ),
                  onPressed: onToggleConfirm,
                ),
              ),
            ),
            const SizedBox(height: 16),
            Align(
              alignment: Alignment.centerRight,
              child: FilledButton(
                onPressed: onChangePassword,
                child: changingPassword
                    ? const SizedBox(
                  height: 20,
                  width: 20,
                  child: CircularProgressIndicator(strokeWidth: 2),
                )
                    : const Text('Update password'),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
