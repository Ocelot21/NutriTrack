import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:nutritrack_shared/exercises/data/exercise_models.dart';
import 'package:nutritrack_shared/groceries/data/grocery_models.dart';

import 'suggestions_providers.dart';

class SuggestionsPage extends ConsumerStatefulWidget {
  const SuggestionsPage({super.key});

  @override
  ConsumerState<SuggestionsPage> createState() => _SuggestionsPageState();
}

class _SuggestionsPageState extends ConsumerState<SuggestionsPage>
    with SingleTickerProviderStateMixin {
  late final TabController _tabs;

  @override
  void initState() {
    super.initState();
    _tabs = TabController(length: 2, vsync: this);

    // Load the initially visible tab only. The other tab will lazy-load
    // when it becomes visible.
    Future.microtask(() {
      ref.read(grocerySuggestionsControllerProvider.notifier).loadInitialIfEmpty();
    });

    _tabs.addListener(() {
      if (!_tabs.indexIsChanging) {
        if (_tabs.index == 0) {
          ref
              .read(grocerySuggestionsControllerProvider.notifier)
              .loadInitialIfEmpty();
        } else {
          ref
              .read(exerciseSuggestionsControllerProvider.notifier)
              .loadInitialIfEmpty();
        }
      }
    });
  }

  @override
  void dispose() {
    _tabs.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Suggestions'),
        bottom: TabBar(
          controller: _tabs,
          tabs: const [
            Tab(text: 'Groceries'),
            Tab(text: 'Exercises'),
          ],
        ),
        actions: [
          IconButton(
            tooltip: 'Refresh',
            onPressed: () {
              ref.read(grocerySuggestionsControllerProvider.notifier).refresh();
              ref.read(exerciseSuggestionsControllerProvider.notifier).refresh();
            },
            icon: const Icon(Icons.refresh),
          )
        ],
      ),
      body: TabBarView(
        controller: _tabs,
        children: const [
          _GrocerySuggestionsTab(),
          _ExerciseSuggestionsTab(),
        ],
      ),
    );
  }
}

class _GrocerySuggestionsTab extends ConsumerStatefulWidget {
  const _GrocerySuggestionsTab();

  @override
  ConsumerState<_GrocerySuggestionsTab> createState() =>
      _GrocerySuggestionsTabState();
}

class _GrocerySuggestionsTabState extends ConsumerState<_GrocerySuggestionsTab>
    with AutomaticKeepAliveClientMixin {
  @override
  bool get wantKeepAlive => true;

  Future<void> _approve(BuildContext context, Grocery g) async {
    final ok = await _confirm(
      context,
      title: 'Approve suggestion?',
      message: 'Approve “${g.name}”?',
      confirmLabel: 'Approve',
    );
    if (ok != true) return;

    try {
      await ref.read(grocerySuggestionsControllerProvider.notifier).approve(g.id);
      if (!context.mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Approved “${g.name}”.')),
      );
    } catch (e) {
      if (!context.mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Failed to approve: $e')),
      );
    }
  }

  Future<void> _reject(BuildContext context, Grocery g) async {
    final ok = await _confirm(
      context,
      title: 'Delete suggestion?',
      message: 'Delete “${g.name}”? This can\'t be undone.',
      confirmLabel: 'Delete',
      isDestructive: true,
    );
    if (ok != true) return;

    try {
      await ref.read(grocerySuggestionsControllerProvider.notifier).reject(g.id);
      if (!context.mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Deleted “${g.name}”.')),
      );
    } catch (e) {
      if (!context.mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Failed to delete: $e')),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    super.build(context);

    final state = ref.watch(grocerySuggestionsControllerProvider);
    final controller = ref.read(grocerySuggestionsControllerProvider.notifier);

    return _SuggestionsScaffold<Grocery>(
      title: 'Pending grocery suggestions',
      state: state,
      onRetry: controller.refresh,
      onLoadMore: controller.loadMore,
      itemBuilder: (g) {
        return DataRow(
          cells: [
            DataCell(Text(g.name)),
            DataCell(Text(g.category.label)),
            DataCell(Text(g.unitOfMeasure.displayLabel)),
            DataCell(Text(g.caloriesPer100.toString())),
            DataCell(Text(g.proteinPer100.toStringAsFixed(2))),
            DataCell(Text(g.carbsPer100.toStringAsFixed(2))),
            DataCell(Text(g.fatPer100.toStringAsFixed(2))),
            DataCell(
              Row(
                children: [
                  IconButton(
                    tooltip: 'View',
                    icon: const Icon(Icons.open_in_new),
                    onPressed: () => context.go('/groceries/${g.id}'),
                  ),
                  IconButton(
                    tooltip: 'Approve',
                    icon: const Icon(Icons.check_circle_outline),
                    onPressed: state.isLoading ? null : () => _approve(context, g),
                  ),
                  IconButton(
                    tooltip: 'Delete',
                    icon: const Icon(Icons.delete_outline),
                    onPressed: state.isLoading ? null : () => _reject(context, g),
                  ),
                ],
              ),
            ),
          ],
        );
      },
      columns: const [
        DataColumn(label: Text('Name')),
        DataColumn(label: Text('Category')),
        DataColumn(label: Text('Unit')),
        DataColumn(label: Text('Calories/100')),
        DataColumn(label: Text('Protein/100')),
        DataColumn(label: Text('Carbs/100')),
        DataColumn(label: Text('Fat/100')),
        DataColumn(label: Text('Actions')),
      ],
    );
  }
}

class _ExerciseSuggestionsTab extends ConsumerStatefulWidget {
  const _ExerciseSuggestionsTab();

  @override
  ConsumerState<_ExerciseSuggestionsTab> createState() =>
      _ExerciseSuggestionsTabState();
}

class _ExerciseSuggestionsTabState extends ConsumerState<_ExerciseSuggestionsTab>
    with AutomaticKeepAliveClientMixin {
  @override
  bool get wantKeepAlive => true;

  Future<void> _approve(BuildContext context, Exercise e) async {
    final ok = await _confirm(
      context,
      title: 'Approve suggestion?',
      message: 'Approve “${e.name}”?',
      confirmLabel: 'Approve',
    );
    if (ok != true) return;

    try {
      await ref.read(exerciseSuggestionsControllerProvider.notifier).approve(e.id);
      if (!context.mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Approved “${e.name}”.')),
      );
    } catch (err) {
      if (!context.mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Failed to approve: $err')),
      );
    }
  }

  Future<void> _reject(BuildContext context, Exercise e) async {
    final ok = await _confirm(
      context,
      title: 'Delete suggestion?',
      message: 'Delete “${e.name}”? This can\'t be undone.',
      confirmLabel: 'Delete',
      isDestructive: true,
    );
    if (ok != true) return;

    try {
      await ref.read(exerciseSuggestionsControllerProvider.notifier).reject(e.id);
      if (!context.mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Deleted “${e.name}”.')),
      );
    } catch (err) {
      if (!context.mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Failed to delete: $err')),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    super.build(context);

    final state = ref.watch(exerciseSuggestionsControllerProvider);
    final controller = ref.read(exerciseSuggestionsControllerProvider.notifier);

    return _SuggestionsScaffold<Exercise>(
      title: 'Pending exercise suggestions',
      state: state,
      onRetry: controller.refresh,
      onLoadMore: controller.loadMore,
      itemBuilder: (e) {
        return DataRow(
          cells: [
            DataCell(Text(e.name)),
            DataCell(Text(e.category.label)),
            DataCell(Text(e.defaultCaloriesPerMinute.toStringAsFixed(2))),
            DataCell(
              Row(
                children: [
                  IconButton(
                    tooltip: 'View',
                    icon: const Icon(Icons.open_in_new),
                    onPressed: () => context.go('/exercises/${e.id}'),
                  ),
                  IconButton(
                    tooltip: 'Approve',
                    icon: const Icon(Icons.check_circle_outline),
                    onPressed: state.isLoading ? null : () => _approve(context, e),
                  ),
                  IconButton(
                    tooltip: 'Delete',
                    icon: const Icon(Icons.delete_outline),
                    onPressed: state.isLoading ? null : () => _reject(context, e),
                  ),
                ],
              ),
            ),
          ],
        );
      },
      columns: const [
        DataColumn(label: Text('Name')),
        DataColumn(label: Text('Category')),
        DataColumn(label: Text('Cal/min')),
        DataColumn(label: Text('Actions')),
      ],
    );
  }
}

class _SuggestionsScaffold<T> extends StatelessWidget {
  final String title;
  final SuggestionsListState<T> state;
  final VoidCallback onRetry;
  final VoidCallback onLoadMore;
  final List<DataColumn> columns;
  final DataRow Function(T item) itemBuilder;

  const _SuggestionsScaffold({
    required this.title,
    required this.state,
    required this.onRetry,
    required this.onLoadMore,
    required this.columns,
    required this.itemBuilder,
  });

  @override
  Widget build(BuildContext context) {
    if (state.isLoading && state.items.isEmpty) {
      return const Center(child: CircularProgressIndicator());
    }

    if (state.error != null && state.items.isEmpty) {
      return Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text(
              state.error!,
              style: const TextStyle(color: Colors.red),
              textAlign: TextAlign.center,
            ),
            const SizedBox(height: 12),
            ElevatedButton(
              onPressed: onRetry,
              child: const Text('Retry'),
            ),
          ],
        ),
      );
    }

    if (state.items.isEmpty) {
      return const Center(
        child: Text('No pending suggestions.'),
      );
    }

    return Padding(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Text(
            title,
            style: Theme.of(context).textTheme.titleLarge,
          ),
          const SizedBox(height: 12),
          Expanded(
            child: Card(
              elevation: 2,
              child: SingleChildScrollView(
                scrollDirection: Axis.vertical,
                child: DataTable(
                  columns: columns,
                  rows: state.items.map(itemBuilder).toList(),
                ),
              ),
            ),
          ),
          const SizedBox(height: 8),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                'Showing ${state.items.length} of ${state.totalCount} '
                '(page ${state.page}, size ${state.pageSize})',
              ),
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
                    onPressed:
                        state.canLoadMore && !state.isLoading ? onLoadMore : null,
                    child: const Text('Load more'),
                  )
                ],
              )
            ],
          )
        ],
      ),
    );
  }
}

Future<bool?> _confirm(
  BuildContext context, {
  required String title,
  required String message,
  required String confirmLabel,
  bool isDestructive = false,
}) {
  return showDialog<bool>(
    context: context,
    builder: (ctx) {
      return AlertDialog(
        title: Text(title),
        content: Text(message),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(ctx).pop(false),
            child: const Text('Cancel'),
          ),
          FilledButton(
            onPressed: () => Navigator.of(ctx).pop(true),
            style: isDestructive
                ? FilledButton.styleFrom(
                    backgroundColor: Theme.of(ctx).colorScheme.error,
                    foregroundColor: Theme.of(ctx).colorScheme.onError,
                  )
                : null,
            child: Text(confirmLabel),
          ),
        ],
      );
    },
  );
}
