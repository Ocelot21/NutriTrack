import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:nutritrack_shared/exercises/data/exercise_models.dart';
import 'exercise_providers.dart';
import 'create_exercise_dialog.dart';

class ExercisesAdminPage extends ConsumerStatefulWidget {
  const ExercisesAdminPage({super.key});

  @override
  ConsumerState<ExercisesAdminPage> createState() =>
      _ExercisesAdminPageState();
}

class _ExercisesAdminPageState extends ConsumerState<ExercisesAdminPage> {
  final _searchController = TextEditingController();

  @override
  void initState() {
    super.initState();
    Future.microtask(() {
      final state = ref.read(exerciseSearchControllerProvider);
      _searchController.text = state.filters.searchTerm ?? '';
      ref
          .read(exerciseSearchControllerProvider.notifier)
          .loadInitialIfEmpty();
    });
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  void _applySearch() {
    final controller =
    ref.read(exerciseSearchControllerProvider.notifier);
    final state = ref.read(exerciseSearchControllerProvider);

    final term = _searchController.text.trim();

    controller.applyFilters(
      searchTerm: term.isEmpty ? null : term,
      category: state.filters.category,
      minCaloriesPerMinute: state.filters.minCaloriesPerMinute,
      maxCaloriesPerMinute: state.filters.maxCaloriesPerMinute,
    );
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(exerciseSearchControllerProvider);
    final controller =
    ref.read(exerciseSearchControllerProvider.notifier);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Exercises'),
      ),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          children: [
            // header
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                const Text(
                  'Exercise catalog',
                  style: TextStyle(
                    fontSize: 20,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                Row(
                  children: [
                    OutlinedButton.icon(
                      onPressed: () {
                        showDialog(
                          context: context,
                          builder: (_) => const CreateExerciseDialog(),
                        );
                      },
                      icon: const Icon(Icons.add),
                      label: const Text('Add new exercise'),
                    ),
                    const SizedBox(width: 8),
                    IconButton(
                      tooltip: 'Reload',
                      onPressed: () {
                        controller.loadInitialIfEmpty();
                      },
                      icon: const Icon(Icons.refresh),
                    ),
                  ],
                ),
              ],
            ),
            const SizedBox(height: 16),

            // search bar
            Row(
              children: [
                Expanded(
                  child: TextField(
                    controller: _searchController,
                    decoration: const InputDecoration(
                      labelText: 'Search by name',
                      prefixIcon: Icon(Icons.search),
                    ),
                    onSubmitted: (_) => _applySearch(),
                  ),
                ),
                const SizedBox(width: 8),
                OutlinedButton(
                  onPressed: state.isLoading ? null : _applySearch,
                  child: const Text('Search'),
                ),
              ],
            ),
            const SizedBox(height: 16),

            if (state.error != null)
              Padding(
                padding: const EdgeInsets.only(bottom: 8),
                child: Align(
                  alignment: Alignment.centerLeft,
                  child: Text(
                    state.error!,
                    style: const TextStyle(color: Colors.red),
                  ),
                ),
              ),

            Expanded(
              child: Card(
                elevation: 2,
                child: state.isLoading && state.items.isEmpty
                    ? const Center(child: CircularProgressIndicator())
                    : _ExerciseTable(items: state.items),
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
                    if (state.isLoading && state.items.isNotEmpty)
                      const Padding(
                        padding: EdgeInsets.only(right: 12),
                        child: SizedBox(
                          width: 16,
                          height: 16,
                          child: CircularProgressIndicator(strokeWidth: 2),
                        ),
                      ),
                    TextButton(
                      onPressed: (!state.canLoadMore || state.isLoading)
                          ? null
                          : () => controller.loadMore(),
                      child: const Text('Load more'),
                    ),
                  ],
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

class _ExerciseTable extends StatelessWidget {
  final List<Exercise> items;

  const _ExerciseTable({required this.items});

  @override
  Widget build(BuildContext context) {
    if (items.isEmpty) {
      return const Center(
        child: Text('No exercises found.'),
      );
    }

    return SingleChildScrollView(
      scrollDirection: Axis.vertical,
      child: DataTable(
        columnSpacing: 16,
        headingRowHeight: 40,
        dataRowMinHeight: 40,
        dataRowMaxHeight: 56,
        columns: const [
          DataColumn(label: Text('Name')),
          DataColumn(label: Text('Category')),
          DataColumn(label: Text('Calories/min')),
          DataColumn(label: Text('Approved')),
          DataColumn(label: Text('Actions')),
        ],
        rows: items.map((e) {
          return DataRow(
            cells: [
              DataCell(Text(e.name)),
              DataCell(Text(e.category.label)),
              DataCell(Text(e.defaultCaloriesPerMinute.toStringAsFixed(1))),
              DataCell(
                Icon(e.isApproved ? Icons.check : Icons.close),
              ),
              DataCell(
                IconButton(
                  tooltip: 'Details',
                  icon: const Icon(Icons.open_in_new),
                  onPressed: () {
                    context.go('/exercises/${e.id}');
                  },
                ),
              ),
            ],
          );
        }).toList(),
      ),
    );
  }
}
