import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import '../../meals/presentation/create_meal_item_page.dart';
import '../data/grocery_repo.dart';
import '../data/grocery_models.dart';
import 'grocery_providers.dart';

class GrocerySearchPageArgs {
  final String? mealId;
  final DateTime? mealLocalDate;

  GrocerySearchPageArgs({
    this.mealId,
    this.mealLocalDate,
  });

  bool get selectionMode => mealId != null;
}

class GrocerySearchPage extends ConsumerStatefulWidget {
  final GrocerySearchPageArgs? args;

  const GrocerySearchPage({super.key, this.args});

  bool get selectionMode => args?.selectionMode ?? false;


  @override
  ConsumerState<GrocerySearchPage> createState() =>
      _GrocerySearchPageState();
}


class _GrocerySearchPageState extends ConsumerState<GrocerySearchPage> {
  final _searchCtrl = TextEditingController();
  final _minKcalCtrl = TextEditingController();
  final _maxKcalCtrl = TextEditingController();
  final _minProteinCtrl = TextEditingController();
  final _maxProteinCtrl = TextEditingController();
  final _minCarbsCtrl = TextEditingController();
  final _maxCarbsCtrl = TextEditingController();
  final _minFatCtrl = TextEditingController();
  final _maxFatCtrl = TextEditingController();

  GroceryCategory? _selectedCategory;
  UnitOfMeasureUi? _selectedUnit;
  bool _showAdvanced = false;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      ref
          .read(grocerySearchControllerProvider.notifier)
          .loadInitialIfEmpty();
    });
  }

  @override
  void dispose() {
    _searchCtrl.dispose();
    _minKcalCtrl.dispose();
    _maxKcalCtrl.dispose();
    _minProteinCtrl.dispose();
    _maxProteinCtrl.dispose();
    _minCarbsCtrl.dispose();
    _maxCarbsCtrl.dispose();
    _minFatCtrl.dispose();
    _maxFatCtrl.dispose();
    super.dispose();
  }

  double? _parseDoubleOrNull(String? text) {
    if (text == null) return null;
    final trimmed = text.trim();
    if (trimmed.isEmpty) return null;
    return double.tryParse(trimmed.replaceAll(',', '.'));
  }

  Future<void> _applyFilters() async {
    final controller =
    ref.read(grocerySearchControllerProvider.notifier);

    final filters = GrocerySearchFilters(
      searchTerm: _searchCtrl.text,
      category: _selectedCategory,
      unitOfMeasure: _selectedUnit,
      minCaloriesPer100: _parseDoubleOrNull(_minKcalCtrl.text),
      maxCaloriesPer100: _parseDoubleOrNull(_maxKcalCtrl.text),
      minProteinPer100g: _parseDoubleOrNull(_minProteinCtrl.text),
      maxProteinPer100g: _parseDoubleOrNull(_maxProteinCtrl.text),
      minCarbsPer100g: _parseDoubleOrNull(_minCarbsCtrl.text),
      maxCarbsPer100g: _parseDoubleOrNull(_maxCarbsCtrl.text),
      minFatPer100g: _parseDoubleOrNull(_minFatCtrl.text),
      maxFatPer100g: _parseDoubleOrNull(_maxFatCtrl.text),
    );

    await controller.applyFilters(filters);
  }

  Future<void> _searchByCode(String code) async {
    final repo = ref.read(groceryRepoProvider);

    try {
      final item = await repo.getByCode(code);

      if (!mounted) return;

      if (widget.selectionMode) {
        final created = await context.push<bool>(
          '/meal-items/create',
          extra: MealItemCreatePageArgs(
            grocery: item,
            preferredMealId: widget.args?.mealId,
            contextDate: widget.args?.mealLocalDate,
          ),
        );

        if (created == true && mounted) {
          context.pop(true);
        }
        return;
      }

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Found: ${item.name}')),
      );

    } catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Not found')),
      );
    }
  }


  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final state = ref.watch(grocerySearchControllerProvider);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Groceries'),
        actions: [
          IconButton(
            icon: const Icon(Icons.qr_code_scanner),
            onPressed: () async {
              final code = await context.push<String>('/scanner');

              if (code == null) return;

              _searchByCode(code);
            },
          ),

        ],
      ),
      body: SafeArea(
        child: Column(
          children: [
            Padding(
              padding: const EdgeInsets.all(12),
              child: TextField(
                controller: _searchCtrl,
                textInputAction: TextInputAction.search,
                onSubmitted: (_) => _applyFilters(),
                decoration: const InputDecoration(
                  labelText: 'Search groceries',
                  prefixIcon: Icon(Icons.search),
                ),
              ),
            ),
            ExpansionTile(
              initiallyExpanded: false,
              tilePadding: const EdgeInsets.symmetric(horizontal: 12),
              childrenPadding:
              const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
              title: const Text('Advanced filters'),
              children: [
                const SizedBox(height: 8),
                _CategoryChips(
                  selected: _selectedCategory,
                  onSelected: (cat) {
                    setState(() {
                      _selectedCategory =
                      _selectedCategory == cat ? null : cat;
                    });
                  },
                ),
                const SizedBox(height: 12),
                _UnitChips(
                  selected: _selectedUnit,
                  onSelected: (unit) {
                    setState(() {
                      _selectedUnit =
                      _selectedUnit == unit ? null : unit;
                    });
                  },
                ),
                const SizedBox(height: 12),
                Row(
                  children: [
                    Expanded(
                      child: TextField(
                        controller: _minKcalCtrl,
                        keyboardType:
                        const TextInputType.numberWithOptions(decimal: true),
                        decoration: const InputDecoration(
                          labelText: 'Min kcal / 100',
                        ),
                      ),
                    ),
                    const SizedBox(width: 12),
                    Expanded(
                      child: TextField(
                        controller: _maxKcalCtrl,
                        keyboardType:
                        const TextInputType.numberWithOptions(decimal: true),
                        decoration: const InputDecoration(
                          labelText: 'Max kcal / 100',
                        ),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 8),
                _MacroFilterRow(
                  label: 'Protein (g / 100)',
                  minCtrl: _minProteinCtrl,
                  maxCtrl: _maxProteinCtrl,
                ),
                const SizedBox(height: 8),
                _MacroFilterRow(
                  label: 'Carbs (g / 100)',
                  minCtrl: _minCarbsCtrl,
                  maxCtrl: _maxCarbsCtrl,
                ),
                const SizedBox(height: 8),
                _MacroFilterRow(
                  label: 'Fat (g / 100)',
                  minCtrl: _minFatCtrl,
                  maxCtrl: _maxFatCtrl,
                ),
                const SizedBox(height: 12),
                Align(
                  alignment: Alignment.centerRight,
                  child: FilledButton.icon(
                    onPressed: state.isLoading ? null : _applyFilters,
                    icon: const Icon(Icons.filter_alt),
                    label: const Text('Apply filters'),
                  ),
                ),
                const SizedBox(height: 8),
              ],
            ),

            const Divider(height: 1),
            // LIST
            Expanded(
              child: state.isLoading && state.items.isEmpty
                  ? const Center(child: CircularProgressIndicator())
                  : RefreshIndicator(
                onRefresh: _applyFilters,
                child: ListView.separated(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 12,
                    vertical: 8,
                  ),
                  itemCount: state.items.length +
                      (state.canLoadMore ? 1 : 0),
                  separatorBuilder: (_, __) =>
                  const SizedBox(height: 8),
                  itemBuilder: (context, index) {
                    if (index < state.items.length) {
                      final g = state.items[index];
                      return _GroceryCard(
                        grocery: g,
                        onTap: () async {
                          final selectionArgs = widget.args;

                          if (!(widget.selectionMode)) {
                            // todo: details page
                            return;
                          }

                          final created = await context.push<bool>(
                            '/meal-items/create',
                            extra: MealItemCreatePageArgs(
                              grocery: g,
                              preferredMealId: selectionArgs!.mealId,
                              contextDate: selectionArgs.mealLocalDate,
                            ),
                          );

                          if (!context.mounted) return;

                          if (created == true) {
                            Navigator.of(context).pop(true);
                          }
                        },
                      );

                    }

                    if (!state.canLoadMore) {
                      return const SizedBox.shrink();
                    }

                    return Padding(
                      padding: const EdgeInsets.symmetric(vertical: 8),
                      child: Center(
                        child: OutlinedButton(
                          onPressed: state.isLoading
                              ? null
                              : () => ref
                              .read(
                            grocerySearchControllerProvider
                                .notifier,
                          )
                              .loadMore(),
                          child: state.isLoading
                              ? const SizedBox(
                            height: 18,
                            width: 18,
                            child: CircularProgressIndicator(
                              strokeWidth: 2,
                            ),
                          )
                              : const Text('Load more'),
                        ),
                      ),
                    );
                  },
                ),
              ),
            ),
            if (state.error != null)
              Container(
                width: double.infinity,
                color: theme.colorScheme.errorContainer,
                padding:
                const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                child: Text(
                  state.error!,
                  style: theme.textTheme.bodyMedium?.copyWith(
                    color: theme.colorScheme.onErrorContainer,
                  ),
                ),
              ),
          ],
        ),
      ),
    );
  }
}

class _CategoryChips extends StatelessWidget {
  final GroceryCategory? selected;
  final ValueChanged<GroceryCategory> onSelected;

  const _CategoryChips({
    required this.selected,
    required this.onSelected,
  });

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    final categories = [
      GroceryCategory.vegetable,
      GroceryCategory.fruit,
      GroceryCategory.grain,
      GroceryCategory.protein,
      GroceryCategory.dairy,
      GroceryCategory.snack,
      GroceryCategory.drink,
      GroceryCategory.other,
    ];

    return SingleChildScrollView(
      scrollDirection: Axis.horizontal,
      child: Row(
        children: [
          FilterChip(
            label: const Text('All'),
            selected: selected == null,
            onSelected: (_) => onSelected(GroceryCategory.uncategorized),
          ),
          const SizedBox(width: 8),
          for (final cat in categories) ...[
            FilterChip(
              label: Text(cat.label),
              selected: selected == cat,
              onSelected: (_) => onSelected(cat),
              selectedColor: theme.colorScheme.secondaryContainer,
            ),
            const SizedBox(width: 8),
          ],
        ],
      ),
    );
  }
}

class _UnitChips extends StatelessWidget {
  final UnitOfMeasureUi? selected;
  final ValueChanged<UnitOfMeasureUi> onSelected;

  const _UnitChips({
    required this.selected,
    required this.onSelected,
  });

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    final units = UnitOfMeasureUi.values;

    return SingleChildScrollView(
      scrollDirection: Axis.horizontal,
      child: Row(
        children: [
          FilterChip(
            label: const Text('Any unit'),
            selected: selected == null,
            onSelected: (_) {},
          ),
          const SizedBox(width: 8),
          for (final u in units) ...[
            FilterChip(
              label: Text(u.label),
              selected: selected == u,
              onSelected: (_) => onSelected(u),
              selectedColor: theme.colorScheme.primaryContainer,
            ),
            const SizedBox(width: 8),
          ],
        ],
      ),
    );
  }
}

class _MacroFilterRow extends StatelessWidget {
  final String label;
  final TextEditingController minCtrl;
  final TextEditingController maxCtrl;

  const _MacroFilterRow({
    required this.label,
    required this.minCtrl,
    required this.maxCtrl,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Expanded(
          child: TextField(
            controller: minCtrl,
            keyboardType:
            const TextInputType.numberWithOptions(decimal: true),
            decoration: InputDecoration(
              labelText: 'Min $label',
            ),
          ),
        ),
        const SizedBox(width: 12),
        Expanded(
          child: TextField(
            controller: maxCtrl,
            keyboardType:
            const TextInputType.numberWithOptions(decimal: true),
            decoration: InputDecoration(
              labelText: 'Max $label',
            ),
          ),
        ),
      ],
    );
  }
}

class _GroceryCard extends StatelessWidget {
  final Grocery grocery;
  final VoidCallback? onTap;

  const _GroceryCard({
    required this.grocery,
    this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(16),
      child: Container(
        padding: const EdgeInsets.all(12),
        decoration: BoxDecoration(
          color: theme.colorScheme.surfaceVariant,
          borderRadius: BorderRadius.circular(16),
        ),
        child: Row(
          children: [
            _buildImage(theme),
            const SizedBox(width: 12),
            Expanded(
              child: _buildInfo(theme),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildImage(ThemeData theme) {
    final imageUrl = grocery.imageUrl;
    return ClipRRect(
      borderRadius: BorderRadius.circular(12),
      child: SizedBox(
        width: 56,
        height: 56,
        child: imageUrl != null && imageUrl.isNotEmpty
            ? Image.network(
          imageUrl,
          fit: BoxFit.cover,
          errorBuilder: (_, __, ___) => _placeholder(theme),
        )
            : _placeholder(theme),
      ),
    );
  }

  Widget _placeholder(ThemeData theme) {
    return Container(
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: [
            theme.colorScheme.primaryContainer,
            theme.colorScheme.secondaryContainer,
          ],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
      ),
      child: Center(
        child: Icon(
          Icons.local_grocery_store,
          color: theme.colorScheme.onPrimaryContainer,
        ),
      ),
    );
  }

  Widget _buildInfo(ThemeData theme) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          grocery.name,
          style: theme.textTheme.titleMedium,
        ),
        const SizedBox(height: 4),
        Text(
          grocery.category.label,
          style: theme.textTheme.bodySmall,
        ),
        const SizedBox(height: 4),
        Text(
          '${grocery.caloriesPer100} kcal / 100 ${grocery.unitOfMeasure.label}',
          style: theme.textTheme.bodySmall?.copyWith(
            color: theme.colorScheme.primary,
          ),
        ),
        const SizedBox(height: 2),
        Text(
          'P ${grocery.proteinPer100g.toStringAsFixed(1)} · '
              'C ${grocery.carbsPer100g.toStringAsFixed(1)} · '
              'F ${grocery.fatPer100g.toStringAsFixed(1)}',
          style: theme.textTheme.bodySmall,
        ),
      ],
    );
  }
}
