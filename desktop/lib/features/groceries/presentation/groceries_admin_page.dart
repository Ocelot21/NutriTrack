import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:nutritrack_shared/groceries/data/grocery_models.dart';
import 'package:nutritrack_shared/groceries/data/grocery_repo.dart';
import 'grocery_providers.dart';
import 'create_grocery_dialog.dart';

class GroceriesAdminPage extends ConsumerStatefulWidget {
  const GroceriesAdminPage({super.key});

  @override
  ConsumerState<GroceriesAdminPage> createState() =>
      _GroceriesAdminPageState();
}

class _GroceriesAdminPageState extends ConsumerState<GroceriesAdminPage> {
  final _searchController = TextEditingController();

  @override
  void initState() {
    super.initState();
    Future.microtask(() {
      final state = ref.read(grocerySearchControllerProvider);
      final filters = state.filters;

      _searchController.text = filters.searchTerm ?? '';

      ref
          .read(grocerySearchControllerProvider.notifier)
          .loadInitialIfEmpty();
    });
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  void _applySearchAndFilters() {
    final controller =
    ref.read(grocerySearchControllerProvider.notifier);
    final prevFilters = ref.read(grocerySearchControllerProvider).filters;

    final term = _searchController.text.trim();

    final newFilters = GrocerySearchFilters(
      searchTerm: term.isEmpty ? null : term,
      category: prevFilters.category,
      unitOfMeasure: prevFilters.unitOfMeasure,
      minCaloriesPer100: prevFilters.minCaloriesPer100,
      maxCaloriesPer100: prevFilters.maxCaloriesPer100,
      minProteinPer100: prevFilters.minProteinPer100,
      maxProteinPer100: prevFilters.maxProteinPer100,
      minCarbsPer100: prevFilters.minCarbsPer100,
      maxCarbsPer100: prevFilters.maxCarbsPer100,
      minFatPer100: prevFilters.minFatPer100,
      maxFatPer100: prevFilters.maxFatPer100,
    );

    controller.applyFilters(newFilters);
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(grocerySearchControllerProvider);
    final controller =
    ref.read(grocerySearchControllerProvider.notifier);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Groceries'),
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
                  'Grocery catalog',
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
                          builder: (_) => const CreateGroceryDialog(),
                        );
                      },
                      icon: const Icon(Icons.add),
                      label: const Text('Add new grocery'),
                    ),
                    const SizedBox(width: 8),
                    IconButton(
                      tooltip: 'Reload',
                      onPressed: () {
                        controller.applyFilters(state.filters);
                      },
                      icon: const Icon(Icons.refresh),
                    ),
                  ],
                ),
              ],
            ),
            const SizedBox(height: 16),

            // search
            Row(
              children: [
                Expanded(
                  child: TextField(
                    controller: _searchController,
                    decoration: const InputDecoration(
                      labelText: 'Search by name',
                      prefixIcon: Icon(Icons.search),
                    ),
                    onSubmitted: (_) => _applySearchAndFilters(),
                  ),
                ),
                const SizedBox(width: 8),
                OutlinedButton(
                  onPressed:
                  state.isLoading ? null : _applySearchAndFilters,
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

            // tabela
            Expanded(
              child: Card(
                elevation: 2,
                child: state.isLoading && state.items.isEmpty
                    ? const Center(
                  child: CircularProgressIndicator(),
                )
                    : _GroceryTable(
                  items: state.items,
                ),
              ),
            ),

            const SizedBox(height: 8),

            // paging info + load more
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

class _GroceryTable extends StatelessWidget {
  final List<Grocery> items;

  const _GroceryTable({required this.items});

  @override
  Widget build(BuildContext context) {
    if (items.isEmpty) {
      return const Center(
        child: Text('No groceries found.'),
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
          DataColumn(label: Text('Unit')),
          DataColumn(label: Text('Protein/100')),
          DataColumn(label: Text('Carbs/100')),
          DataColumn(label: Text('Fat/100')),
          DataColumn(label: Text('Calories/100')),
          DataColumn(label: Text('Actions')),
        ],
        rows: items.map((g) {
          return DataRow(
            cells: [
              DataCell(Text(g.name)),
              DataCell(Text(g.category.label)),
              DataCell(Text(g.unitOfMeasure.displayLabel)),
              DataCell(Text('${g.proteinPer100.toStringAsFixed(1)} g')),
              DataCell(Text('${g.carbsPer100.toStringAsFixed(1)} g')),
              DataCell(Text('${g.fatPer100.toStringAsFixed(1)} g')),
              DataCell(Text(g.caloriesPer100.toString())),
              DataCell(
                IconButton(
                  tooltip: 'Details',
                  icon: const Icon(Icons.open_in_new),
                  onPressed: () {
                    context.go('/groceries/${g.id}');
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
