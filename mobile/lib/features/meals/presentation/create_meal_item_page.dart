import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../groceries/data/grocery_models.dart';
import '../data/meal_dropdown_models.dart';
import 'meal_item_providers.dart';
import 'meals_dropdown_provider.dart';

class MealItemCreatePageArgs {
  final Grocery grocery;
  final String? preferredMealId;
  final DateTime? contextDate;

  MealItemCreatePageArgs({
    required this.grocery,
    this.preferredMealId,
    this.contextDate,
  });
}

class MealItemCreatePage extends ConsumerStatefulWidget {
  final MealItemCreatePageArgs args;

  const MealItemCreatePage({
    super.key,
    required this.args,
  });

  @override
  ConsumerState<MealItemCreatePage> createState() =>
      _MealItemCreatePageState();
}

class _MealItemCreatePageState extends ConsumerState<MealItemCreatePage> {
  final _quantityCtrl = TextEditingController();
  String? _selectedMealId;

  @override
  void dispose() {
    _quantityCtrl.dispose();
    super.dispose();
  }

  double? _parseQuantity() {
    final text = _quantityCtrl.text.trim();
    if (text.isEmpty) return null;
    return double.tryParse(text.replaceAll(',', '.'));
  }

  String _formatDate(DateTime d) {
    final day = d.day.toString().padLeft(2, '0');
    final month = d.month.toString().padLeft(2, '0');
    return '$day.$month.${d.year}';
  }

  Future<void> _onSave(MealDropdownItem meal) async {
    final quantity = _parseQuantity() ?? 0;

    final controller =
    ref.read(mealItemCreateControllerProvider.notifier);

    final ok = await controller.submit(
      mealId: meal.id,
      groceryId: widget.args.grocery.id,
      quantity: quantity,
    );

    if (!ok || !mounted) return;
    Navigator.of(context).pop(true); // javljamo da se refresh-a Home
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(mealItemCreateControllerProvider);
    final theme = Theme.of(context);
    final grocery = widget.args.grocery;

    final mealsAsync =
    ref.watch(mealsForDropdownProvider(widget.args.contextDate));

    return Scaffold(
      appBar: AppBar(
        leading: IconButton(
          icon: const Icon(Icons.close),
          onPressed: () => Navigator.of(context).pop(false),
        ),
        title: const Text('Add meal item'),
      ),
      body: mealsAsync.when(
        loading: () => const Center(child: CircularProgressIndicator()),
        error: (err, _) => Center(
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Text('Failed to load meals: $err'),
          ),
        ),
        data: (meals) {
          if (meals.isEmpty) {
            return const Center(
              child: Padding(
                padding: EdgeInsets.all(16),
                child: Text('No meals found for this period.'),
              ),
            );
          }

          // odaberi default meal (preferred ako postoji, inače prvi)
          _selectedMealId ??= (() {
            if (widget.args.preferredMealId != null) {
              final found = meals.firstWhere(
                    (m) => m.id == widget.args.preferredMealId,
                orElse: () => meals.first,
              );
              return found.id;
            }
            return meals.first.id;
          })();

          final selectedMeal = meals.firstWhere(
                (m) => m.id == _selectedMealId,
            orElse: () => meals.first,
          );

          final unitLabel = grocery.unitOfMeasure.label;

          return SingleChildScrollView(
            padding: const EdgeInsets.all(16),
            child: Center(
              child: ConstrainedBox(
                constraints: const BoxConstraints(maxWidth: 480),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    _GroceryHeader(grocery: grocery),
                    const SizedBox(height: 16),
                    if (state.error != null) ...[
                      Text(
                        state.error!,
                        style: theme.textTheme.bodyMedium?.copyWith(
                          color: theme.colorScheme.error,
                        ),
                      ),
                      const SizedBox(height: 12),
                    ],
                    DropdownButtonFormField<String>(
                      value: _selectedMealId,
                      decoration: const InputDecoration(
                        labelText: 'Meal',
                      ),
                      items: meals
                          .map(
                            (m) => DropdownMenuItem(
                          value: m.id,
                          child: Text(
                            '${m.name} · ${_formatDate(m.localDate)}',
                          ),
                        ),
                      )
                          .toList(),
                      onChanged: (value) {
                        if (value == null) return;
                        setState(() {
                          _selectedMealId = value;
                        });
                      },
                    ),
                    const SizedBox(height: 16),
                    TextField(
                      controller: _quantityCtrl,
                      keyboardType:
                      const TextInputType.numberWithOptions(decimal: true),
                      decoration: InputDecoration(
                        labelText: 'Quantity ($unitLabel)',
                      ),
                    ),
                    const SizedBox(height: 24),
                    SizedBox(
                      height: 48,
                      child: FilledButton(
                        onPressed: state.isSubmitting
                            ? null
                            : () => _onSave(selectedMeal),
                        child: state.isSubmitting
                            ? const SizedBox(
                          height: 20,
                          width: 20,
                          child: CircularProgressIndicator(
                            strokeWidth: 2,
                          ),
                        )
                            : const Text('Save'),
                      ),
                    ),
                  ],
                ),
              ),
            ),
          );
        },
      ),
    );
  }
}

class _GroceryHeader extends StatelessWidget {
  final Grocery grocery;

  const _GroceryHeader({required this.grocery});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final imageUrl = grocery.imageUrl;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        ClipRRect(
          borderRadius: BorderRadius.circular(16),
          child: SizedBox(
            height: 160,
            child: imageUrl != null && imageUrl.isNotEmpty
                ? Image.network(
              imageUrl,
              fit: BoxFit.cover,
              errorBuilder: (_, __, ___) => _placeholder(theme),
            )
                : _placeholder(theme),
          ),
        ),
        const SizedBox(height: 12),
        Text(
          grocery.name,
          style: theme.textTheme.titleLarge
              ?.copyWith(fontWeight: FontWeight.bold),
        ),
        const SizedBox(height: 4),
        Text(
          grocery.category.label,
          style: theme.textTheme.bodyMedium,
        ),
        const SizedBox(height: 4),
        Text(
          '${grocery.caloriesPer100} kcal / 100 ${grocery.unitOfMeasure.label}',
          style: theme.textTheme.bodyMedium?.copyWith(
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
          Icons.shopping_basket,
          size: 48,
          color: theme.colorScheme.onPrimaryContainer,
        ),
      ),
    );
  }
}

