import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../data/user_models.dart';
import 'user_providers.dart';
import '../../roles/presentation/role_providers.dart';

class UsersPage extends ConsumerStatefulWidget {
  const UsersPage({super.key});

  @override
  ConsumerState<UsersPage> createState() => _UsersPageState();
}

class _UsersPageState extends ConsumerState<UsersPage> {
  @override
  void initState() {
    super.initState();
    Future.microtask(() =>
        ref.read(usersListControllerProvider.notifier).loadInitialIfEmpty());
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final state = ref.watch(usersListControllerProvider);
    final controller = ref.read(usersListControllerProvider.notifier);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Users'),
        actions: [
          IconButton(
            tooltip: 'Refresh',
            icon: const Icon(Icons.refresh),
            onPressed: state.isLoading ? null : controller.refresh,
          ),
        ],
      ),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Row(
              children: [
                Expanded(
                  child: Text(
                    'Manage users',
                    style: theme.textTheme.titleLarge,
                  ),
                ),
                SizedBox(
                  width: 260,
                  child: DropdownButtonFormField<int>(
                    decoration: const InputDecoration(
                      labelText: 'Page size',
                      isDense: true,
                    ),
                    initialValue: state.pageSize,
                    items: const [10, 20, 50, 100]
                        .map((v) => DropdownMenuItem(value: v, child: Text('$v')))
                        .toList(),
                    onChanged: state.isLoading
                        ? null
                        : (v) {
                            if (v == null) return;
                            controller.setPageSize(v);
                          },
                  ),
                ),
              ],
            ),
            const SizedBox(height: 12),

            if (state.error != null)
              Padding(
                padding: const EdgeInsets.only(bottom: 12),
                child: Text(
                  state.error!,
                  style: theme.textTheme.bodyMedium?.copyWith(
                    color: theme.colorScheme.error,
                  ),
                ),
              ),

            Expanded(
              child: Card(
                elevation: 2,
                child: state.isLoading && state.items.isEmpty
                    ? const Center(child: CircularProgressIndicator())
                    : SingleChildScrollView(
                        child: DataTable(
                          showCheckboxColumn: false,
                          columns: const [
                            DataColumn(label: Text('Username')),
                            DataColumn(label: Text('Email')),
                            DataColumn(label: Text('Name')),
                            DataColumn(label: Text('Role')),
                            DataColumn(label: Text('2FA')),
                            DataColumn(label: Text('Last login')),
                            DataColumn(label: Text('Actions')),
                          ],
                          rows: state.items.map((u) => _userRow(context, u)).toList(),
                        ),
                      ),
              ),
            ),
            const SizedBox(height: 8),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text('Showing ${state.items.length} of ${state.totalCount} (page ${state.page})'),
                Row(
                  children: [
                    if (state.isLoading)
                      const Padding(
                        padding: EdgeInsets.only(right: 12),
                        child: SizedBox(
                          width: 16,
                          height: 16,
                          child: CircularProgressIndicator(strokeWidth: 2),
                        ),
                      ),
                    TextButton(
                      onPressed: state.canLoadMore && !state.isLoading ? controller.loadMore : null,
                      child: const Text('Load more'),
                    ),
                  ],
                )
              ],
            ),
          ],
        ),
      ),
    );
  }

  DataRow _userRow(BuildContext context, UserResponse u) {
    final theme = Theme.of(context);
    final isBusy = ref.watch(usersListControllerProvider).isLoading;

    String formatDate(DateTime? dt) {
      if (dt == null) return '-';
      return '${dt.year.toString().padLeft(4, '0')}-${dt.month.toString().padLeft(2, '0')}-${dt.day.toString().padLeft(2, '0')} '
          '${dt.hour.toString().padLeft(2, '0')}:${dt.minute.toString().padLeft(2, '0')}';
    }

    final fullName = '${u.firstName} ${u.lastName}'.trim();

    return DataRow(
      onSelectChanged: (_) => _openDetails(context, u.id),
      cells: [
        DataCell(Text(u.username)),
        DataCell(Text(u.email)),
        DataCell(Text(fullName.isEmpty ? '-' : fullName)),
        DataCell(Text(u.roleName ?? '-')),
        DataCell(Icon(u.isTwoFactorEnabled ? Icons.check : Icons.close,
            color: u.isTwoFactorEnabled ? Colors.green : theme.colorScheme.error)),
        DataCell(Text(formatDate(u.lastLoginAtUtc))),
        DataCell(
          Row(
            children: [
              IconButton(
                tooltip: 'Details',
                icon: const Icon(Icons.open_in_new),
                onPressed: () => _openDetails(context, u.id),
              ),
              IconButton(
                tooltip: 'Delete',
                icon: const Icon(Icons.delete_outline),
                onPressed: isBusy ? null : () => _deleteUser(context, u),
              ),
            ],
          ),
        ),
      ],
    );
  }

  Future<void> _openDetails(BuildContext context, String userId) async {
    await showDialog<void>(
      context: context,
      builder: (ctx) => _UserDetailsDialog(userId: userId),
    );
  }

  Future<void> _deleteUser(BuildContext context, UserResponse u) async {
    final ok = await showDialog<bool>(
      context: context,
      builder: (ctx) {
        return AlertDialog(
          title: const Text('Delete user?'),
          content: Text('Delete ${u.username} (${u.email})? This can\'t be undone.'),
          actions: [
            TextButton(
              onPressed: () => Navigator.of(ctx).pop(false),
              child: const Text('Cancel'),
            ),
            FilledButton(
              style: FilledButton.styleFrom(
                backgroundColor: Theme.of(ctx).colorScheme.error,
                foregroundColor: Theme.of(ctx).colorScheme.onError,
              ),
              onPressed: () => Navigator.of(ctx).pop(true),
              child: const Text('Delete'),
            ),
          ],
        );
      },
    );

    if (ok != true) return;

    try {
      await ref.read(usersListControllerProvider.notifier).deleteUser(u.id);
      if (!context.mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Deleted ${u.username}.')),
      );
    } catch (e) {
      if (!context.mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Delete failed: $e')),
      );
    }
  }
}

class _UserDetailsDialog extends ConsumerStatefulWidget {
  final String userId;
  const _UserDetailsDialog({required this.userId});

  @override
  ConsumerState<_UserDetailsDialog> createState() => _UserDetailsDialogState();
}

class _UserDetailsDialogState extends ConsumerState<_UserDetailsDialog> {
  String? _selectedRoleId; // null = no change yet / or clear role explicitly via action

  bool _isUpdatingRole = false;
  String? _roleUpdateError;

  @override
  void initState() {
    super.initState();
  }

  @override
  void dispose() {
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    final detailsAsync = ref.watch(userDetailsProvider(widget.userId));
    final rolesAsync = ref.watch(rolesListProvider);

    final isWorking = detailsAsync.isLoading || _isUpdatingRole;

    return AlertDialog(
      title: const Text('User details'),
      content: SizedBox(
        width: 760,
        child: detailsAsync.when(
          loading: () => const Padding(
            padding: EdgeInsets.all(24),
            child: Center(child: CircularProgressIndicator()),
          ),
          error: (e, _) => Padding(
            padding: const EdgeInsets.all(12),
            child: Text(
              e.toString(),
              style: theme.textTheme.bodyMedium?.copyWith(
                color: theme.colorScheme.error,
              ),
            ),
          ),
          data: (u) {
            final currentRoleId = u.roleId;
            _selectedRoleId ??= currentRoleId; // initialize once

            return Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                if (_roleUpdateError != null) ...[
                  Padding(
                    padding: const EdgeInsets.only(bottom: 12),
                    child: Text(
                      _roleUpdateError!,
                      style: theme.textTheme.bodyMedium?.copyWith(
                        color: theme.colorScheme.error,
                      ),
                    ),
                  ),
                ],

                _kv('Username', u.username),
                _kv('Email', u.email),
                _kv('Name', '${u.firstName} ${u.lastName}'.trim()),
                _kv('Role', u.roleName ?? '-'),
                _kv('2FA enabled', u.isTwoFactorEnabled ? 'Yes' : 'No'),
                _kv('Timezone', u.timeZoneId),
                _kv('Country', u.countryCode ?? '-'),
                _kv('Health profile completed',
                    u.isHealthProfileCompleted ? 'Yes' : 'No'),
                _kv('Gender', u.gender),
                _kv('Activity level', u.activityLevel),
                _kv(
                  'Birthdate',
                  u.birthdate?.toIso8601String().split('T').first ?? '-',
                ),
                _kv('Height (cm)', u.heightCm?.toString() ?? '-'),
                _kv('Weight (kg)', u.weightKg?.toString() ?? '-'),
                const Divider(height: 28),

                Text(
                  'Role',
                  style: theme.textTheme.titleMedium,
                ),
                const SizedBox(height: 8),

                rolesAsync.when(
                  loading: () => const Padding(
                    padding: EdgeInsets.symmetric(vertical: 12),
                    child: LinearProgressIndicator(),
                  ),
                  error: (e, _) => Text(
                    e.toString(),
                    style: theme.textTheme.bodyMedium
                        ?.copyWith(color: theme.colorScheme.error),
                  ),
                  data: (roles) {
                    final sorted = [...roles]..sort((a, b) => a.name.compareTo(b.name));

                    return DropdownButtonFormField<String?>(
                      decoration: const InputDecoration(
                        labelText: 'Assigned role',
                        isDense: true,
                      ),
                      initialValue: _selectedRoleId,
                      items: [
                        const DropdownMenuItem<String?>(
                          value: null,
                          child: Text('No role'),
                        ),
                        ...sorted.map(
                          (r) => DropdownMenuItem<String?>(
                            value: r.id,
                            child: Text(r.name),
                          ),
                        ),
                      ],
                      onChanged: isWorking
                          ? null
                          : (v) {
                              setState(() {
                                _selectedRoleId = v;
                              });
                            },
                    );
                  },
                ),
              ],
            );
          },
        ),
      ),
      actions: [
        TextButton(
          onPressed: isWorking ? null : () => Navigator.of(context).pop(),
          child: const Text('Close'),
        ),
        FilledButton(
          onPressed: isWorking
              ? null
              : () async {
                  final desired = _selectedRoleId; // can be null
                  await _setRoleForUser(widget.userId, desired);
                },
          child: isWorking
              ? const SizedBox(
                  width: 16,
                  height: 16,
                  child: CircularProgressIndicator(strokeWidth: 2),
                )
              : const Text('Save role'),
        ),
      ],
    );
  }

  Future<void> _setRoleForUser(String userId, String? roleId) async {
    setState(() {
      _isUpdatingRole = true;
      _roleUpdateError = null;
    });

    try {
      // Backend currently requires a RoleId GUID; "no role" isn't supported.
      // We'll guard and show a message instead of sending null.
      if (roleId == null || roleId.isEmpty) {
        throw Exception('Clearing a role is not supported by the backend yet.');
      }

      await setUserRole(ref, userId: userId, roleId: roleId);
      // Keep list in sync.
      ref.read(usersListControllerProvider.notifier).refresh();

      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Role updated.')),
      );
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _roleUpdateError = e.toString();
      });
    } finally {
      if (!mounted) return;
      setState(() {
        _isUpdatingRole = false;
      });
    }
  }

  Widget _kv(String k, String v) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 6),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(width: 200, child: Text(k)),
          Expanded(child: Text(v)),
        ],
      ),
    );
  }
}
