import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:nutritrack_shared/groceries/data/grocery_models.dart';
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

String per100UnitLabel(UnitOfMeasureUi u) {
  switch (u) {
    case UnitOfMeasureUi.gram:
      return '100 g';
    case UnitOfMeasureUi.milliliter:
      return '100 ml';
    case UnitOfMeasureUi.piece:
      return '100 g'; // canonical for piece too (piece converts to grams)
  }
}

String quantityUnitLabel(UnitOfMeasureUi u) {
  switch (u) {
    case UnitOfMeasureUi.gram:
      return 'g';
    case UnitOfMeasureUi.milliliter:
      return 'ml';
    case UnitOfMeasureUi.piece:
      return 'pieces';
  }
}

class MealItemCreatePage extends ConsumerStatefulWidget {
  final MealItemCreatePageArgs args;

  const MealItemCreatePage({
    super.key,
    required this.args,
  });

  @override
  ConsumerState<MealItemCreatePage> createState() => _MealItemCreatePageState();
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
    final q = _parseQuantity();

    if (q == null || q <= 0) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Enter a valid quantity greater than 0.')),
      );
      return;
    }

    // Optional safety: if Piece but missing gramsPerPiece, warn (data bug)
    final g = widget.args.grocery;
    final isPiece = g.unitOfMeasure == UnitOfMeasureUi.piece;
    if (isPiece && g.gramsPerPiece == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('This grocery is missing grams per piece.'),
        ),
      );
      // You can "return;" if you want to block saving completely.
      // return;
    }

    final controller = ref.read(mealItemCreateControllerProvider.notifier);

    final ok = await controller.submit(
      mealId: meal.id,
      groceryId: g.id,
      quantity: q,
    );

    if (!ok || !mounted) return;
    Navigator.of(context).pop(true);
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(mealItemCreateControllerProvider);
    final theme = Theme.of(context);

    final grocery = widget.args.grocery;
    final isPiece = grocery.unitOfMeasure == UnitOfMeasureUi.piece;
    final qtyUnit = quantityUnitLabel(grocery.unitOfMeasure);

    final mealsAsync = ref.watch(mealsForDropdownProvider(widget.args.contextDate));

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

          _selectedMealId ??= (() {
            final preferred = widget.args.preferredMealId;
            if (preferred != null) {
              final found = meals.firstWhere(
                    (m) => m.id == preferred,
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

          // Optional computed info preview (approx per entered quantity)
          final q = _parseQuantity();
          final factor = (q != null && q > 0)
              ? (isPiece
              ? (grocery.gramsPerPiece == null ? null : (q * grocery.gramsPerPiece!) / 100.0)
              : q / 100.0)
              : null;

          final kcalApprox =
          factor == null ? null : (grocery.caloriesPer100 * factor).round();
          final pApprox = factor == null ? null : (grocery.proteinPer100 * factor);
          final cApprox = factor == null ? null : (grocery.carbsPer100 * factor);
          final fApprox = factor == null ? null : (grocery.fatPer100 * factor);

          return SingleChildScrollView(
            padding: const EdgeInsets.all(16),
            child: Center(
              child: ConstrainedBox(
                constraints: const BoxConstraints(maxWidth: 520),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    _GroceryHeader(grocery: grocery),
                    const SizedBox(height: 14),

                    // A slightly more prominent macro summary card
                    _MacroSummaryCard(grocery: grocery),

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
                          child: Text('${m.name} · ${_formatDate(m.localDate)}'),
                        ),
                      )
                          .toList(),
                      onChanged: (value) {
                        if (value == null) return;
                        setState(() => _selectedMealId = value);
                      },
                    ),

                    const SizedBox(height: 16),

                    TextField(
                      controller: _quantityCtrl,
                      keyboardType: const TextInputType.numberWithOptions(decimal: true),
                      onChanged: (_) => setState(() {}),
                      decoration: InputDecoration(
                        labelText: 'Quantity ($qtyUnit)',
                        helperText: isPiece
                            ? (grocery.gramsPerPiece != null
                            ? '1 piece ≈ ${grocery.gramsPerPiece!.toStringAsFixed(0)} g'
                            : 'This grocery is missing grams per piece.')
                            : null,
                      ),
                    ),

                    const SizedBox(height: 12),

                    // Preview card (optional but very useful UX)
                    _QuantityPreviewCard(
                      isVisible: q != null && q > 0,
                      calories: kcalApprox,
                      protein: pApprox,
                      carbs: cApprox,
                      fat: fApprox,
                    ),

                    const SizedBox(height: 18),

                    SizedBox(
                      height: 50,
                      child: FilledButton(
                        onPressed: state.isSubmitting ? null : () => _onSave(selectedMeal),
                        child: state.isSubmitting
                            ? const SizedBox(
                          height: 20,
                          width: 20,
                          child: CircularProgressIndicator(strokeWidth: 2),
                        )
                            : const Text('Save'),
                      ),
                    ),

                    const SizedBox(height: 8),

                    Text(
                      'Tip: nutritional values are shown per ${per100UnitLabel(grocery.unitOfMeasure)}.',
                      style: theme.textTheme.bodySmall?.copyWith(
                        color: theme.colorScheme.onSurfaceVariant,
                      ),
                      textAlign: TextAlign.center,
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
    final isPiece = grocery.unitOfMeasure == UnitOfMeasureUi.piece;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        ClipRRect(
          borderRadius: BorderRadius.circular(16),
          child: SizedBox(
            height: 170,
            width: double.infinity,
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
          style: theme.textTheme.titleLarge?.copyWith(fontWeight: FontWeight.bold),
        ),
        const SizedBox(height: 6),
        Wrap(
          spacing: 8,
          runSpacing: 8,
          children: [
            _InfoChip(icon: Icons.category, text: grocery.category.label),
            _InfoChip(icon: Icons.local_fire_department, text: '${grocery.caloriesPer100} kcal / ${per100UnitLabel(grocery.unitOfMeasure)}'),
            _InfoChip(icon: Icons.straighten, text: grocery.unitOfMeasure.displayLabel),
            if (isPiece)
              _InfoChip(
                icon: Icons.scale,
                text: grocery.gramsPerPiece != null
                    ? '1 piece ≈ ${grocery.gramsPerPiece!.toStringAsFixed(0)} g'
                    : 'Missing grams/piece',
              ),
          ],
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
          size: 52,
          color: theme.colorScheme.onPrimaryContainer,
        ),
      ),
    );
  }
}

class _MacroSummaryCard extends StatelessWidget {
  final Grocery grocery;

  const _MacroSummaryCard({required this.grocery});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    final p = grocery.proteinPer100;
    final c = grocery.carbsPer100;
    final f = grocery.fatPer100;

    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: theme.colorScheme.surfaceVariant,
        borderRadius: BorderRadius.circular(16),
      ),
      child: Row(
        children: [
          Expanded(
            child: _MacroTile(
              label: 'Protein',
              value: '${p.toStringAsFixed(1)} g',
            ),
          ),
          Expanded(
            child: _MacroTile(
              label: 'Carbs',
              value: '${c.toStringAsFixed(1)} g',
            ),
          ),
          Expanded(
            child: _MacroTile(
              label: 'Fat',
              value: '${f.toStringAsFixed(1)} g',
            ),
          ),
        ],
      ),
    );
  }
}

class _MacroTile extends StatelessWidget {
  final String label;
  final String value;

  const _MacroTile({required this.label, required this.value});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Column(
      children: [
        Text(
          label,
          style: theme.textTheme.bodySmall?.copyWith(
            color: theme.colorScheme.onSurfaceVariant,
          ),
        ),
        const SizedBox(height: 4),
        Text(
          value,
          style: theme.textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w700),
        ),
      ],
    );
  }
}

class _QuantityPreviewCard extends StatelessWidget {
  final bool isVisible;
  final int? calories;
  final double? protein;
  final double? carbs;
  final double? fat;

  const _QuantityPreviewCard({
    required this.isVisible,
    required this.calories,
    required this.protein,
    required this.carbs,
    required this.fat,
  });

  @override
  Widget build(BuildContext context) {
    if (!isVisible) return const SizedBox.shrink();

    final theme = Theme.of(context);

    String fmt(double? v) => v == null ? '—' : v.toStringAsFixed(1);

    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: theme.dividerColor),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Approx. for entered quantity',
            style: theme.textTheme.titleSmall?.copyWith(fontWeight: FontWeight.w700),
          ),
          const SizedBox(height: 8),
          Wrap(
            spacing: 10,
            runSpacing: 10,
            children: [
              _InfoChip(
                icon: Icons.local_fire_department,
                text: calories == null ? 'kcal —' : 'kcal $calories',
              ),
              _InfoChip(icon: Icons.fitness_center, text: 'P ${fmt(protein)} g'),
              _InfoChip(icon: Icons.bakery_dining, text: 'C ${fmt(carbs)} g'),
              _InfoChip(icon: Icons.opacity, text: 'F ${fmt(fat)} g'),
            ],
          ),
        ],
      ),
    );
  }
}

class _InfoChip extends StatelessWidget {
  final IconData icon;
  final String text;

  const _InfoChip({required this.icon, required this.text});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 7),
      decoration: BoxDecoration(
        color: theme.colorScheme.surfaceVariant,
        borderRadius: BorderRadius.circular(999),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(icon, size: 16, color: theme.colorScheme.onSurfaceVariant),
          const SizedBox(width: 6),
          Text(text, style: theme.textTheme.bodySmall),
        ],
      ),
    );
  }
}