import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../data/role_models.dart';
import '../data/role_repo.dart';
import 'role_providers.dart';

class RolesPage extends ConsumerWidget {
  const RolesPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final theme = Theme.of(context);
    final rolesAsync = ref.watch(rolesListProvider);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Roles'),
        actions: [
          IconButton(
            tooltip: 'Refresh',
            onPressed: () => ref.invalidate(rolesListProvider),
            icon: const Icon(Icons.refresh),
          ),
          FilledButton.icon(
            onPressed: () async {
              await showDialog<void>(
                context: context,
                builder: (_) => const _CreateRoleDialog(),
              );
              ref.invalidate(rolesListProvider);
            },
            icon: const Icon(Icons.add),
            label: const Text('Create role'),
          ),
          const SizedBox(width: 12),
        ],
      ),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: rolesAsync.when(
          loading: () => const Center(child: CircularProgressIndicator()),
          error: (e, _) => Center(
            child: Text(
              e.toString(),
              style: theme.textTheme.bodyMedium
                  ?.copyWith(color: theme.colorScheme.error),
            ),
          ),
          data: (roles) {
            if (roles.isEmpty) {
              return const Center(child: Text('No roles found.'));
            }

            return Card(
              child: SingleChildScrollView(
                child: DataTable(
                  showCheckboxColumn: false,
                  columns: const [
                    DataColumn(label: Text('Name')),
                    DataColumn(label: Text('Description')),
                    DataColumn(label: Text('Permissions')),
                    DataColumn(label: Text('Actions')),
                  ],
                  rows: roles.map((r) {
                    return DataRow(
                      onSelectChanged: (_) => _openDetails(context, r.id),
                      cells: [
                        DataCell(Text(r.name)),
                        DataCell(Text((r.description ?? '').isEmpty
                            ? '-'
                            : r.description!)),
                        DataCell(Text('${r.permissions.length}')),
                        DataCell(
                          Row(
                            children: [
                              IconButton(
                                tooltip: 'Edit',
                                icon: const Icon(Icons.edit_outlined),
                                onPressed: () => _openDetails(context, r.id),
                              ),
                              IconButton(
                                tooltip: 'Delete',
                                icon: const Icon(Icons.delete_outline),
                                onPressed: () => _deleteRole(context, ref, r),
                              ),
                            ],
                          ),
                        ),
                      ],
                    );
                  }).toList(growable: false),
                ),
              ),
            );
          },
        ),
      ),
    );
  }

  Future<void> _openDetails(BuildContext context, String roleId) async {
    await showDialog<void>(
      context: context,
      builder: (_) => _RoleDetailsDialog(roleId: roleId),
    );
  }

  Future<void> _deleteRole(BuildContext context, WidgetRef ref, RoleResponse r) async {
    final ok = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Delete role?'),
        content: Text('Delete "${r.name}"? This can\'t be undone.'),
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
      ),
    );

    if (ok != true) return;

    final repo = ref.read(roleRepoProvider);
    try {
      await repo.deleteRole(r.id);
      if (!context.mounted) return;
      ScaffoldMessenger.of(context)
          .showSnackBar(SnackBar(content: Text('Deleted ${r.name}.')));
      ref.invalidate(rolesListProvider);
    } on RolesException catch (e) {
      if (!context.mounted) return;
      ScaffoldMessenger.of(context)
          .showSnackBar(SnackBar(content: Text(e.message)));
    } catch (e) {
      if (!context.mounted) return;
      ScaffoldMessenger.of(context)
          .showSnackBar(SnackBar(content: Text('Delete failed: $e')));
    }
  }
}

class _CreateRoleDialog extends ConsumerStatefulWidget {
  const _CreateRoleDialog();

  @override
  ConsumerState<_CreateRoleDialog> createState() => _CreateRoleDialogState();
}

class _CreateRoleDialogState extends ConsumerState<_CreateRoleDialog> {
  final _name = TextEditingController();
  final _description = TextEditingController();

  final Set<String> _selectedPermissions = <String>{};
  bool _saving = false;
  String? _error;

  @override
  void dispose() {
    _name.dispose();
    _description.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final permissionsAsync = ref.watch(permissionKeysProvider);

    return AlertDialog(
      title: const Text('Create role'),
      content: SizedBox(
        width: 760,
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            if (_error != null) ...[
              Text(_error!,
                  style: theme.textTheme.bodyMedium
                      ?.copyWith(color: theme.colorScheme.error)),
              const SizedBox(height: 12),
            ],
            TextField(
              controller: _name,
              decoration: const InputDecoration(labelText: 'Name'),
            ),
            const SizedBox(height: 12),
            TextField(
              controller: _description,
              decoration: const InputDecoration(labelText: 'Description'),
            ),
            const SizedBox(height: 16),
            Text('Permissions', style: theme.textTheme.titleMedium),
            const SizedBox(height: 8),
            SizedBox(
              height: 300,
              child: permissionsAsync.when(
                loading: () => const Center(child: CircularProgressIndicator()),
                error: (e, _) => Text(
                  e.toString(),
                  style: theme.textTheme.bodyMedium
                      ?.copyWith(color: theme.colorScheme.error),
                ),
                data: (keys) {
                  keys.sort();
                  return _PermissionPicker(
                    allKeys: keys,
                    initiallySelected: _selectedPermissions,
                    onChanged: (sel) {
                      setState(() {
                        _selectedPermissions
                          ..clear()
                          ..addAll(sel);
                      });
                    },
                  );
                },
              ),
            ),
          ],
        ),
      ),
      actions: [
        TextButton(
          onPressed: _saving ? null : () => Navigator.of(context).pop(),
          child: const Text('Cancel'),
        ),
        FilledButton(
          onPressed: _saving ? null : _save,
          child: _saving
              ? const SizedBox(
                  width: 16,
                  height: 16,
                  child: CircularProgressIndicator(strokeWidth: 2),
                )
              : const Text('Create'),
        ),
      ],
    );
  }

  Future<void> _save() async {
    final name = _name.text.trim();
    if (name.isEmpty) {
      setState(() => _error = 'Name is required.');
      return;
    }

    setState(() {
      _saving = true;
      _error = null;
    });

    final repo = ref.read(roleRepoProvider);

    try {
      await repo.createRole(
        CreateRoleRequest(
          name: name,
          description: _description.text.trim().isEmpty
              ? null
              : _description.text.trim(),
          permissions: _selectedPermissions.toList(growable: false),
        ),
      );

      if (!mounted) return;
      Navigator.of(context).pop();
    } on RolesException catch (e) {
      if (!mounted) return;
      setState(() => _error = e.message);
    } catch (e) {
      if (!mounted) return;
      setState(() => _error = 'Unexpected error: $e');
    } finally {
      if (!mounted) return;
      setState(() => _saving = false);
    }
  }
}

class _RoleDetailsDialog extends ConsumerStatefulWidget {
  final String roleId;
  const _RoleDetailsDialog({required this.roleId});

  @override
  ConsumerState<_RoleDetailsDialog> createState() => _RoleDetailsDialogState();
}

class _RoleDetailsDialogState extends ConsumerState<_RoleDetailsDialog> {
  bool _saving = false;
  String? _error;

  final Set<String> _selectedPermissions = <String>{};

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    final roleAsync = ref.watch(roleDetailsProvider(widget.roleId));
    final permKeysAsync = ref.watch(permissionKeysProvider);

    return AlertDialog(
      title: const Text('Role details'),
      content: SizedBox(
        width: 820,
        child: roleAsync.when(
          loading: () => const Padding(
            padding: EdgeInsets.all(24),
            child: Center(child: CircularProgressIndicator()),
          ),
          error: (e, _) => Text(
            e.toString(),
            style: theme.textTheme.bodyMedium
                ?.copyWith(color: theme.colorScheme.error),
          ),
          data: (role) {
            // Initialize selected permissions on first successful load.
            if (_selectedPermissions.isEmpty && role.permissions.isNotEmpty) {
              _selectedPermissions.addAll(role.permissions);
            }

            return Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                if (_error != null) ...[
                  Text(
                    _error!,
                    style: theme.textTheme.bodyMedium
                        ?.copyWith(color: theme.colorScheme.error),
                  ),
                  const SizedBox(height: 12),
                ],
                _kv('Id', role.id),
                _kv('Name', role.name),
                _kv('Description', role.description ?? '-'),
                const Divider(height: 28),
                Text('Permissions', style: theme.textTheme.titleMedium),
                const SizedBox(height: 8),
                SizedBox(
                  height: 340,
                  child: permKeysAsync.when(
                    loading: () => const Center(
                      child: CircularProgressIndicator(),
                    ),
                    error: (e, _) => Text(
                      e.toString(),
                      style: theme.textTheme.bodyMedium
                          ?.copyWith(color: theme.colorScheme.error),
                    ),
                    data: (keys) {
                      keys.sort();
                      return _PermissionPicker(
                        allKeys: keys,
                        initiallySelected: _selectedPermissions,
                        onChanged: (sel) {
                          setState(() {
                            _selectedPermissions
                              ..clear()
                              ..addAll(sel);
                          });
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
      actions: [
        TextButton(
          onPressed: _saving ? null : () => Navigator.of(context).pop(),
          child: const Text('Close'),
        ),
        FilledButton(
          onPressed: _saving ? null : _save,
          child: _saving
              ? const SizedBox(
                  width: 16,
                  height: 16,
                  child: CircularProgressIndicator(strokeWidth: 2),
                )
              : const Text('Save permissions'),
        ),
      ],
    );
  }

  Future<void> _save() async {
    setState(() {
      _saving = true;
      _error = null;
    });

    final repo = ref.read(roleRepoProvider);

    try {
      // Compute diff vs current role permissions.
      final current = await ref.read(roleDetailsProvider(widget.roleId).future);
      final currentSet = current.permissions.toSet();

      final add = _selectedPermissions.difference(currentSet).toList();
      final remove = currentSet.difference(_selectedPermissions).toList();

      await repo.updateRolePermissions(
        roleId: widget.roleId,
        add: add.isEmpty ? null : add,
        remove: remove.isEmpty ? null : remove,
      );

      // Refresh screens.
      ref.invalidate(roleDetailsProvider(widget.roleId));
      ref.invalidate(rolesListProvider);

      if (!mounted) return;
      Navigator.of(context).pop();
    } on RolesException catch (e) {
      if (!mounted) return;
      setState(() => _error = e.message);
    } catch (e) {
      if (!mounted) return;
      setState(() => _error = 'Unexpected error: $e');
    } finally {
      if (!mounted) return;
      setState(() => _saving = false);
    }
  }

  Widget _kv(String k, String v) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 6),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(width: 160, child: Text(k)),
          Expanded(child: Text(v)),
        ],
      ),
    );
  }
}

class _PermissionPicker extends StatefulWidget {
  final List<String> allKeys;
  final Set<String> initiallySelected;
  final ValueChanged<Set<String>> onChanged;

  const _PermissionPicker({
    required this.allKeys,
    required this.initiallySelected,
    required this.onChanged,
  });

  @override
  State<_PermissionPicker> createState() => _PermissionPickerState();
}

class _PermissionPickerState extends State<_PermissionPicker> {
  late Set<String> _selected;
  String _query = '';

  @override
  void initState() {
    super.initState();
    _selected = {...widget.initiallySelected};
  }

  @override
  void didUpdateWidget(covariant _PermissionPicker oldWidget) {
    super.didUpdateWidget(oldWidget);
    // Keep selection in sync if parent changes it.
    _selected = {...widget.initiallySelected};
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    final filtered = widget.allKeys
        .where((k) => _query.isEmpty || k.toLowerCase().contains(_query))
        .toList(growable: false);

    return Column(
      children: [
        Row(
          children: [
            Expanded(
              child: TextField(
                decoration: const InputDecoration(
                  labelText: 'Search permission keys',
                  isDense: true,
                ),
                onChanged: (v) => setState(() => _query = v.toLowerCase()),
              ),
            ),
            const SizedBox(width: 12),
            TextButton(
              onPressed: () {
                setState(() => _selected = {...widget.allKeys});
                widget.onChanged(_selected);
              },
              child: const Text('Select all'),
            ),
            TextButton(
              onPressed: () {
                setState(() => _selected.clear());
                widget.onChanged(_selected);
              },
              child: const Text('Clear'),
            ),
          ],
        ),
        const SizedBox(height: 8),
        Expanded(
          child: Material(
            color: theme.colorScheme.surfaceContainerLowest,
            borderRadius: BorderRadius.circular(8),
            child: ListView.builder(
              itemCount: filtered.length,
              itemBuilder: (context, i) {
                final keyName = filtered[i];
                final checked = _selected.contains(keyName);

                return CheckboxListTile(
                  dense: true,
                  title: Text(keyName),
                  value: checked,
                  onChanged: (v) {
                    setState(() {
                      if (v == true) {
                        _selected.add(keyName);
                      } else {
                        _selected.remove(keyName);
                      }
                    });
                    widget.onChanged(_selected);
                  },
                );
              },
            ),
          ),
        ),
      ],
    );
  }
}

