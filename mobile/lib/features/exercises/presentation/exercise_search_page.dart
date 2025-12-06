import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'exercise_log_page.dart';
import 'exercise_providers.dart';
import '../data/exercise_models.dart';

class ExerciseSearchPage extends ConsumerStatefulWidget {
  final DateTime? contextDate;

  const ExerciseSearchPage({
    super.key,
    this.contextDate,
  });

  @override
  ConsumerState<ExerciseSearchPage> createState() =>
      _ExerciseSearchPageState();
}

class _ExerciseSearchPageState extends ConsumerState<ExerciseSearchPage> {
  final _searchCtrl = TextEditingController();
  final _minKcalCtrl = TextEditingController();
  final _maxKcalCtrl = TextEditingController();
  ExerciseCategory? _selectedCategory;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      ref
          .read(exerciseSearchControllerProvider.notifier)
          .loadInitialIfEmpty();
    });
  }

  @override
  void dispose() {
    _searchCtrl.dispose();
    _minKcalCtrl.dispose();
    _maxKcalCtrl.dispose();
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
    ref.read(exerciseSearchControllerProvider.notifier);

    final minKcal = _parseDoubleOrNull(_minKcalCtrl.text);
    final maxKcal = _parseDoubleOrNull(_maxKcalCtrl.text);

    await controller.applyFilters(
      searchTerm: _searchCtrl.text,
      category: _selectedCategory,
      minCaloriesPerMinute: minKcal,
      maxCaloriesPerMinute: maxKcal,
    );
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final state = ref.watch(exerciseSearchControllerProvider);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Exercises'),
      ),
      body: SafeArea(
        child: Column(
          children: [
            // FILTERS
            Padding(
              padding: const EdgeInsets.all(12),
              child: Column(
                children: [
                  TextField(
                    controller: _searchCtrl,
                    textInputAction: TextInputAction.search,
                    onSubmitted: (_) => _applyFilters(),
                    decoration: const InputDecoration(
                      labelText: 'Search exercises',
                      prefixIcon: Icon(Icons.search),
                    ),
                  ),
                  const SizedBox(height: 12),
                  _CategoryFilterChips(
                    selected: _selectedCategory,
                    onSelected: (cat) {
                      setState(() {
                        _selectedCategory =
                        _selectedCategory == cat ? null : cat;
                      });
                    },
                  ),
                  const SizedBox(height: 12),
                  Row(
                    children: [
                      Expanded(
                        child: TextField(
                          controller: _minKcalCtrl,
                          keyboardType: const TextInputType.numberWithOptions(
                            decimal: true,
                          ),
                          decoration: const InputDecoration(
                            labelText: 'Min kcal/min',
                          ),
                        ),
                      ),
                      const SizedBox(width: 12),
                      Expanded(
                        child: TextField(
                          controller: _maxKcalCtrl,
                          keyboardType: const TextInputType.numberWithOptions(
                            decimal: true,
                          ),
                          decoration: const InputDecoration(
                            labelText: 'Max kcal/min',
                          ),
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 8),
                  Align(
                    alignment: Alignment.centerRight,
                    child: FilledButton.icon(
                      onPressed: state.isLoading ? null : _applyFilters,
                      icon: const Icon(Icons.filter_alt),
                      label: const Text('Apply filters'),
                    ),
                  ),
                ],
              ),
            ),
            const Divider(height: 1),
            // LIST
            Expanded(
              child: state.isLoading && state.items.isEmpty
                  ? const Center(child: CircularProgressIndicator())
                  : RefreshIndicator(
                onRefresh: () => _applyFilters(),
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
                      final ex = state.items[index];
                      return _ExerciseCard(
                        exercise: ex,
                        onTap: () async {
                          final initial = widget.contextDate ?? DateTime.now();

                          final args = ExerciseLogPageArgs(
                            exercise: ex,
                            initialDate: initial,
                          );

                          final logged = await context.push<bool>(
                            '/exercises/log',
                            extra: args,
                          );

                          if (logged == true) {
                            // zatvaramo search i javljamo Home-u da se nešto promijenilo
                            Navigator.of(context).pop(true);
                          }
                        },
                      );
                    }

                    // "Load more" row
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
                              .read(exerciseSearchControllerProvider
                              .notifier)
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

class _CategoryFilterChips extends StatelessWidget {
  final ExerciseCategory? selected;
  final ValueChanged<ExerciseCategory> onSelected;

  const _CategoryFilterChips({
    required this.selected,
    required this.onSelected,
  });

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    final categories = [
      ExerciseCategory.cardio,
      ExerciseCategory.strength,
      ExerciseCategory.mobility,
      ExerciseCategory.flexibility,
      ExerciseCategory.hiit,
      ExerciseCategory.other,
    ];

    return SingleChildScrollView(
      scrollDirection: Axis.horizontal,
      child: Row(
        children: [
          FilterChip(
            label: const Text('All'),
            selected: selected == null,
            onSelected: (_) {
              onSelected(ExerciseCategory.uncategorized);
            },
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

class _ExerciseCard extends StatelessWidget {
  final Exercise exercise;
  final VoidCallback? onTap;

  const _ExerciseCard({
    required this.exercise,
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
            if (exercise.imageUrl != null) ...[
              ClipRRect(
                borderRadius: BorderRadius.circular(12),
                child: SizedBox(
                  height: 48,
                  width: 48,
                  child: Image.network(
                    exercise.imageUrl!,
                    fit: BoxFit.cover,
                    errorBuilder: (_, __, ___) =>
                    const Icon(Icons.fitness_center),
                  ),
                ),
              ),
              const SizedBox(width: 12),
            ] else ...[
              const Icon(Icons.fitness_center, size: 32),
              const SizedBox(width: 12),
            ],
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    exercise.name,
                    style: theme.textTheme.titleMedium,
                  ),
                  const SizedBox(height: 4),
                  Text(
                    exercise.category.label,
                    style: theme.textTheme.bodySmall,
                  ),
                  const SizedBox(height: 4),
                  Text(
                    '${exercise.defaultCaloriesPerMinute.toStringAsFixed(1)} kcal / min',
                    style: theme.textTheme.bodySmall?.copyWith(
                      color: theme.colorScheme.primary,
                    ),
                  ),
                ],
              ),
            ),
            if (exercise.isApproved)
              Icon(
                Icons.verified,
                color: theme.colorScheme.primary,
                size: 20,
              ),
          ],
        ),
      ),
    );
  }
}
