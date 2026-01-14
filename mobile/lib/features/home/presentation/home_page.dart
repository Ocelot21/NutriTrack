import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:nutritrack_mobile/features/meals/presentation/meals_providers.dart';

import '../../../app/widgets/app_drawer.dart';
import '../../exercises/presentation/edit_exercise_log_page.dart';
import '../../groceries/presentation/grocery_search_page.dart';
import '../../meals/presentation/edit_meal_item_page.dart';
import 'home_providers.dart';
import '../data/daily_overview_models.dart';
import 'daily_overview_share_providers.dart';
import 'daily_overview_share_sheet.dart';

class HomePage extends ConsumerStatefulWidget {
  const HomePage({super.key});

  @override
  ConsumerState<HomePage> createState() => _HomePageState();
}

class _HomePageState extends ConsumerState<HomePage> {
  late DateTime _selectedDate;

  @override
  void initState() {
    super.initState();
    final now = DateTime.now();
    _selectedDate = DateTime(now.year, now.month, now.day);

    WidgetsBinding.instance.addPostFrameCallback((_) {
      _loadForSelectedDate();
    });
  }

  void _loadForSelectedDate() {
    ref
        .read(dailyOverviewControllerProvider.notifier)
        .loadForDate(_selectedDate, onHealthProfileNotCompleted: () {
      if (!mounted) return;
      context.go('/health-profile');
    });
  }

  void _goToPreviousDay() {
    setState(() {
      _selectedDate = _selectedDate.subtract(const Duration(days: 1));
    });
    _loadForSelectedDate();
  }

  void _goToNextDay() {
    setState(() {
      _selectedDate = _selectedDate.add(const Duration(days: 1));
    });
    _loadForSelectedDate();
  }

  String _formatDate(DateTime date) {
    const months = [
      'Jan',
      'Feb',
      'Mar',
      'Apr',
      'May',
      'Jun',
      'Jul',
      'Aug',
      'Sep',
      'Oct',
      'Nov',
      'Dec',
    ];
    final m = months[date.month - 1];
    return '${date.day} $m ${date.year}';
  }

  String _formatTime(DateTime dateTime) {
    final h = dateTime.hour.toString().padLeft(2, '0');
    final m = dateTime.minute.toString().padLeft(2, '0');
    return '$h:$m';
  }

  String _formatDateOnlyParam(DateTime date) {
    final y = date.year.toString().padLeft(4, '0');
    final m = date.month.toString().padLeft(2, '0');
    final d = date.day.toString().padLeft(2, '0');
    return '$y-$m-$d';
  }

  Future<void> _editMeal(Meal meal) async {
    final changed = await context.push<bool>(
      '/meals/edit',
      extra: meal,
    );
    if (changed == true) {
      _loadForSelectedDate();
    }
  }

  Future<void> _deleteMeal(String mealId) async {
    final theme = Theme.of(context);

    final ok = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Delete meal'),
        content: const Text(
          'Are you sure you want to delete this meal and all of its items?',
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(ctx, false),
            child: const Text('Cancel'),
          ),
          FilledButton(
            onPressed: () => Navigator.pop(ctx, true),
            child: const Text('Delete'),
          ),
        ],
      ),
    );

    if (ok != true) return;

    try {
      final repo = ref.read(mealsRepoProvider);
      await repo.deleteMeal(mealId);

      if (!mounted) return;

      _loadForSelectedDate();

      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Meal deleted')),
      );
    } catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            e.toString(),
            style: theme.textTheme.bodyMedium?.copyWith(
              color: theme.colorScheme.onError,
            ),
          ),
          backgroundColor: theme.colorScheme.error,
        ),
      );
    }
  }

  Future<void> _addMealItem(Meal meal) async {
    final mealDate = DateTime(
      meal.occurredAtLocal.year,
      meal.occurredAtLocal.month,
      meal.occurredAtLocal.day,
    );

    final created = await context.push<bool>(
      '/groceries',
      extra: GrocerySearchPageArgs(
        mealId: meal.id,
        mealLocalDate: mealDate,
      ),
    );

    if (created == true) {
      _loadForSelectedDate();
    }
  }

  Future<void> _editMealItem({
    required String mealId,
    required MealItem item,
  }) async {
    final changed = await context.push<bool>(
      '/meals/items/edit',
      extra: EditMealItemPageArgs(
        mealId: mealId,
        item: item,
      ),
    );

    if (changed == true) {
      _loadForSelectedDate();
    }
  }

  Future<void> _editExerciseLog(UserExerciseLog log) async {
    final changed = await context.push<bool>(
      '/exercise-logs/edit',
      extra: EditExerciseLogPageArgs(log: log),
    );

    if (changed == true) {
      _loadForSelectedDate();
    }
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final state = ref.watch(dailyOverviewControllerProvider);
    final overview = state.overview;

    final localDateParam = _formatDateOnlyParam(_selectedDate);
    final isShared = ref.watch(sharedDailyOverviewsProvider).contains(localDateParam);

    return Scaffold(
      appBar: AppBar(
        title: const Text('NutriTrack'),
      ),
      drawer: const AppDrawer(),
      body: SafeArea(
        child: state.isLoading && overview == null
            ? const Center(child: CircularProgressIndicator())
            : SingleChildScrollView(
          padding: const EdgeInsets.all(16),
          child: Center(
            child: ConstrainedBox(
              constraints: const BoxConstraints(maxWidth: 480),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  Row(
                    children: [
                      Expanded(
                        child: _DateSelector(
                          dateLabel: _formatDate(_selectedDate),
                          onPrev: _goToPreviousDay,
                          onNext: _goToNextDay,
                        ),
                      ),
                      const SizedBox(width: 8),
                      if (isShared)
                        Tooltip(
                          message: 'Already shared',
                          child: Container(
                            padding: const EdgeInsets.symmetric(
                                horizontal: 10, vertical: 10),
                            decoration: BoxDecoration(
                              color: theme.colorScheme.primary,
                              borderRadius: BorderRadius.circular(999),
                            ),
                            child: Icon(
                              Icons.check,
                              color: theme.colorScheme.onPrimary,
                              size: 18,
                            ),
                          ),
                        )
                      else
                        IconButton(
                          tooltip: 'Share daily overview',
                          onPressed: overview == null
                              ? null
                              : () {
                            showModalBottomSheet(
                              context: context,
                              isScrollControlled: true,
                              showDragHandle: true,
                              builder: (_) => DailyOverviewShareSheet(
                                localDate: localDateParam,
                                dateLabel: _formatDate(_selectedDate),
                              ),
                            );
                          },
                          icon: const Icon(Icons.share),
                        ),
                    ],
                  ),
                  const SizedBox(height: 24),
                  if (state.error != null) ...[
                    Text(
                      state.error!,
                      style: theme.textTheme.bodyMedium?.copyWith(
                        color: theme.colorScheme.error,
                      ),
                      textAlign: TextAlign.center,
                    ),
                  ] else if (overview != null) ...[
                    _CaloriesSummary(
                      snapshot: overview.snapshot,
                      targets: overview.targets,
                    ),
                    const SizedBox(height: 24),
                    _MacroCards(
                      snapshot: overview.snapshot,
                      targets: overview.targets,
                    ),
                    const SizedBox(height: 24),
                    _MealsSection(
                      meals: overview.meals,
                      formatTime: _formatTime,
                      onEditMeal: _editMeal,
                      onDeleteMeal: _deleteMeal,
                      onAddItem: _addMealItem,
                      onTapItem: (mealId, item) => _editMealItem(mealId: mealId, item: item),
                    ),
                    const SizedBox(height: 16),
                    _AddCard(
                      label: 'Add new meal',
                      onTap: () async {
                        final changed = await context.push<bool>(
                          '/meals/create',
                          extra: _selectedDate,
                        );
                        if (changed == true) {
                          _loadForSelectedDate();
                        }
                      },
                    ),
                    const SizedBox(height: 24),
                    _ExercisesSection(
                      exercises: overview.exercises,
                      formatTime: _formatTime,
                      onTapLog: _editExerciseLog,
                    ),
                    const SizedBox(height: 16),
                    _AddCard(
                      label: 'Add new exercise',
                      onTap: () async {
                        final changed = await context.push<bool>(
                          '/exercises',
                          extra: _selectedDate,
                        );

                        if (changed == true) {
                          _loadForSelectedDate();
                        }
                      },
                    ),
                  ] else ...[
                    Text(
                      'No overview data for this day.',
                      style: theme.textTheme.bodyMedium,
                      textAlign: TextAlign.center,
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

class _DateSelector extends StatelessWidget {
  final String dateLabel;
  final VoidCallback onPrev;
  final VoidCallback onNext;

  const _DateSelector({
    required this.dateLabel,
    required this.onPrev,
    required this.onNext,
  });

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Row(
      children: [
        IconButton(
          icon: const Icon(Icons.chevron_left),
          onPressed: onPrev,
        ),
        Expanded(
          child: Container(
            padding: const EdgeInsets.symmetric(vertical: 10),
            decoration: BoxDecoration(
              color: theme.colorScheme.surfaceContainerHighest,
              borderRadius: BorderRadius.circular(40),
            ),
            alignment: Alignment.center,
            child: Text(
              dateLabel,
              style: theme.textTheme.titleMedium,
            ),
          ),
        ),
        IconButton(
          icon: const Icon(Icons.chevron_right),
          onPressed: onNext,
        ),
      ],
    );
  }
}

class _CaloriesSummary extends StatelessWidget {
  final DailyNutritionSnapshot snapshot;
  final DailyNutritionTargets targets;

  const _CaloriesSummary({
    required this.snapshot,
    required this.targets,
  });

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceEvenly,
          children: [
            _CaloriesBlock(
              title: 'Consumed',
              value: snapshot.consumedCalories,
            ),
            _CaloriesBlock(
              title: 'Burned',
              value: snapshot.burnedCalories,
            ),
            _CaloriesBlock(
              title: 'Net',
              value: snapshot.netCalories,
              highlight: true,
            ),
          ],
        ),
        const SizedBox(height: 16),
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceEvenly,
          children: [
            _CaloriesBlock(
              title: 'Goal',
              value: targets.calories,
            ),
            _CaloriesBlock(
              title: 'Remaining',
              value: snapshot.remainingCalories,
            ),
          ],
        ),
      ],
    );
  }
}

class _CaloriesBlock extends StatelessWidget {
  final String title;
  final int value;
  final bool highlight;

  const _CaloriesBlock({
    required this.title,
    required this.value,
    this.highlight = false,
  });

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    final textStyle = highlight
        ? theme.textTheme.titleLarge?.copyWith(
      color: theme.colorScheme.secondary,
      fontWeight: FontWeight.bold,
    )
        : theme.textTheme.titleLarge?.copyWith(
      fontWeight: FontWeight.bold,
    );

    return Column(
      children: [
        Text(title, style: theme.textTheme.bodyMedium),
        const SizedBox(height: 4),
        Text(
          value.toString(),
          style: textStyle,
        ),
      ],
    );
  }
}

class _MacroCards extends StatelessWidget {
  final DailyNutritionSnapshot snapshot;
  final DailyNutritionTargets targets;

  const _MacroCards({
    required this.snapshot,
    required this.targets,
  });

  double _ratio(double consumed, double target) {
    if (target <= 0) return 0;
    final r = consumed / target;
    if (r < 0) return 0;
    if (r > 1) return 1;
    return r;
  }

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Expanded(
          child: _MacroCard(
            label: 'Protein',
            consumed: snapshot.consumedProteinGrams,
            target: targets.proteinGrams,
            ratio: _ratio(
              snapshot.consumedProteinGrams,
              targets.proteinGrams,
            ),
          ),
        ),
        const SizedBox(width: 12),
        Expanded(
          child: _MacroCard(
            label: 'Carbs',
            consumed: snapshot.consumedCarbohydrateGrams,
            target: targets.carbohydrateGrams,
            ratio: _ratio(
              snapshot.consumedCarbohydrateGrams,
              targets.carbohydrateGrams,
            ),
          ),
        ),
        const SizedBox(width: 12),
        Expanded(
          child: _MacroCard(
            label: 'Fats',
            consumed: snapshot.consumedFatGrams,
            target: targets.fatGrams,
            ratio: _ratio(
              snapshot.consumedFatGrams,
              targets.fatGrams,
            ),
          ),
        ),
      ],
    );
  }
}

class _MacroCard extends StatelessWidget {
  final String label;
  final double consumed;
  final double target;
  final double ratio;

  const _MacroCard({
    required this.label,
    required this.consumed,
    required this.target,
    required this.ratio,
  });

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: theme.colorScheme.surfaceContainerHighest,
        borderRadius: BorderRadius.circular(16),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(label, style: theme.textTheme.bodyMedium),
          const SizedBox(height: 8),
          SizedBox(
            height: 6,
            child: ClipRRect(
              borderRadius: BorderRadius.circular(4),
              child: LinearProgressIndicator(
                value: ratio,
              ),
            ),
          ),
          const SizedBox(height: 8),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text('${consumed.toStringAsFixed(0)} g',
                  style: theme.textTheme.bodyMedium),
              Text('${target.toStringAsFixed(0)} g',
                  style: theme.textTheme.bodyMedium),
            ],
          ),
        ],
      ),
    );
  }
}

class _MealsSection extends StatelessWidget {
  final List<Meal> meals;
  final String Function(DateTime) formatTime;
  final void Function(Meal meal)? onEditMeal;
  final void Function(String mealId)? onDeleteMeal;
  final void Function(Meal meal)? onAddItem;
  final void Function(String mealId, MealItem item)? onTapItem;


  const _MealsSection({
    required this.meals,
    required this.formatTime,
    this.onEditMeal,
    this.onDeleteMeal,
    this.onAddItem,
    this.onTapItem
  });


  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Container(
          padding: const EdgeInsets.symmetric(vertical: 10, horizontal: 16),
          decoration: BoxDecoration(
            color: theme.colorScheme.secondary,
            borderRadius: BorderRadius.circular(20),
          ),
          child: Text(
            'Meals',
            style: theme.textTheme.titleMedium?.copyWith(
              color: theme.colorScheme.onSecondary,
            ),
          ),
        ),
        const SizedBox(height: 12),
        for (final meal in meals) ...[
          _MealBlock(
            meal: meal,
            formatTime: formatTime,
            onEdit: onEditMeal == null ? null : () => onEditMeal!(meal),
            onDelete: onDeleteMeal == null
                ? null
                : () => onDeleteMeal!(meal.id),
            onAddItem: onAddItem == null ? null : () => onAddItem!(meal),
            onTapItem: onTapItem == null ? null : (it) => onTapItem!(meal.id, it),
          ),


          const SizedBox(height: 16),
        ],
      ],
    );
  }
}

class _MealBlock extends StatelessWidget {
  final Meal meal;
  final String Function(DateTime) formatTime;
  final VoidCallback? onEdit;
  final VoidCallback? onDelete;
  final VoidCallback? onAddItem;
  final void Function(MealItem item)? onTapItem;


  const _MealBlock({
    required this.meal,
    required this.formatTime,
    this.onEdit,
    this.onDelete,
    this.onAddItem,
    this.onTapItem
  });



  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Container(
          padding: const EdgeInsets.symmetric(vertical: 10, horizontal: 16),
          decoration: BoxDecoration(
            color: theme.colorScheme.secondaryContainer,
            borderRadius: BorderRadius.circular(16),
          ),
          child: Row(
            children: [
              Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    meal.name,
                    style: theme.textTheme.titleMedium,
                  ),
                  const SizedBox(height: 2),
                  Text(
                    formatTime(meal.occurredAtLocal),
                    style: theme.textTheme.bodySmall,
                  ),
                ],
              ),
              const Spacer(),
              IconButton(
                icon: const Icon(Icons.edit),
                onPressed: onEdit,
              ),
              IconButton(
                icon: const Icon(Icons.delete),
                onPressed: onDelete,
              ),
            ],
          ),
        ),
        const SizedBox(height: 8),
        // items
        Column(
          children: [
            for (final item in meal.items) ...[
              _MealItemCard(
                item: item,
                onTap: onTapItem == null ? null : () => onTapItem!(item),
              ),
              const SizedBox(height: 8),
            ],

            _AddCard(
              label: 'Add item',
              onTap: onAddItem ?? () {},
            ),

          ],
        ),
      ],
    );
  }
}

class _MealItemCard extends StatelessWidget {
  final MealItem item;
  final VoidCallback? onTap;

  const _MealItemCard({
    required this.item,
    this.onTap,
  });

  int _calculateCalories() {
    // caloriesPer100 * quantity / 100
    return (item.caloriesPer100 * item.quantity / 100).round();
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final kcal = _calculateCalories();

    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(12),
      child: Container(
        padding: const EdgeInsets.symmetric(vertical: 10, horizontal: 16),
        decoration: BoxDecoration(
          color: theme.colorScheme.surfaceContainerHighest,
          borderRadius: BorderRadius.circular(12),
        ),
        child: Row(
          children: [
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    item.groceryName,
                    style: theme.textTheme.bodyLarge,
                  ),
                  const SizedBox(height: 2),
                  Text(
                    '${item.quantity.toStringAsFixed(0)} ${item.unitOfMeasure}',
                    style: theme.textTheme.bodySmall,
                  ),
                ],
              ),
            ),
            Text(
              '$kcal kcal',
              style: theme.textTheme.bodyMedium,
            ),
          ],
        ),
      ),
    );
  }
}

class _ExercisesSection extends StatelessWidget {
  final List<UserExerciseLog> exercises;
  final String Function(DateTime) formatTime;
  final void Function(UserExerciseLog log)? onTapLog;

  const _ExercisesSection({
    required this.exercises,
    required this.formatTime,
    this.onTapLog,
  });


  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Container(
          padding: const EdgeInsets.symmetric(vertical: 10, horizontal: 16),
          decoration: BoxDecoration(
            color: theme.colorScheme.secondary,
            borderRadius: BorderRadius.circular(20),
          ),
          child: Text(
            'Exercises',
            style: theme.textTheme.titleMedium?.copyWith(
              color: theme.colorScheme.onSecondary,
            ),
          ),
        ),
        const SizedBox(height: 12),
        for (final log in exercises) ...[
          _ExerciseCard(
            log: log,
            formatTime: formatTime,
            onTap: onTapLog == null ? null : () => onTapLog!(log),
          ),
          const SizedBox(height: 8),
        ],

      ],
    );
  }
}

class _ExerciseCard extends StatelessWidget {
  final UserExerciseLog log;
  final String Function(DateTime) formatTime;
  final VoidCallback? onTap;

  const _ExerciseCard({
    required this.log,
    required this.formatTime,
    this.onTap,
  });


  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    final kcal = log.totalCalories.round();

    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(12),
      child: Container(
        padding: const EdgeInsets.symmetric(vertical: 10, horizontal: 16),
        decoration: BoxDecoration(
          color: theme.colorScheme.surfaceContainerHighest,
          borderRadius: BorderRadius.circular(12),
        ),
        child: Row(
          children: [
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    log.exerciseName,
                    style: theme.textTheme.bodyLarge,
                  ),
                  const SizedBox(height: 2),
                  Text(
                    formatTime(log.occurredAtLocal),
                    style: theme.textTheme.bodySmall,
                  ),
                ],
              ),
            ),
            Text(
              '-$kcal kcal',
              style: theme.textTheme.bodyMedium,
            ),
          ],
        ),
      ),
    );
  }
}

class _AddCard extends StatelessWidget {
  final String label;
  final VoidCallback onTap;

  const _AddCard({
    required this.label,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(16),
      child: Container(
        padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 16),
        decoration: BoxDecoration(
          color: theme.colorScheme.surfaceContainerHighest,
          borderRadius: BorderRadius.circular(16),
          border: Border.all(
            color: theme.colorScheme.secondary,
          ),
        ),
        child: Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            const Icon(Icons.add),
            const SizedBox(width: 8),
            Text(
              label,
              style: theme.textTheme.bodyMedium,
            ),
          ],
        ),
      ),
    );
  }
}
