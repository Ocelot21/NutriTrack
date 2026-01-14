import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../home/data/daily_overview_models.dart';
import 'meals_providers.dart';

class EditMealItemPageArgs {
  final String mealId;
  final MealItem item;

  const EditMealItemPageArgs({
    required this.mealId,
    required this.item,
  });
}

class EditMealItemPage extends ConsumerStatefulWidget {
  final EditMealItemPageArgs args;

  const EditMealItemPage({super.key, required this.args});

  @override
  ConsumerState<EditMealItemPage> createState() => _EditMealItemPageState();
}

class _EditMealItemPageState extends ConsumerState<EditMealItemPage> {
  late final TextEditingController _qtyCtrl;

  bool _saving = false;
  String? _error;

  @override
  void initState() {
    super.initState();
    _qtyCtrl = TextEditingController(
      text: widget.args.item.quantity.toStringAsFixed(0),
    );
  }

  @override
  void dispose() {
    _qtyCtrl.dispose();
    super.dispose();
  }

  double? _parseQuantity() {
    final t = _qtyCtrl.text.trim();
    if (t.isEmpty) return null;
    return double.tryParse(t.replaceAll(',', '.'));
  }

  int _calcCalories(double quantity) {
    // caloriesPer100 * quantity / 100
    return (widget.args.item.caloriesPer100 * quantity / 100).round();
  }

  Future<void> _onSave() async {
    final q = _parseQuantity();
    if (q == null || q <= 0) {
      setState(() => _error = 'Enter a valid quantity greater than 0.');
      return;
    }

    setState(() {
      _saving = true;
      _error = null;
    });

    try {
      final repo = ref.read(mealsRepoProvider);
      await repo.updateMealItemQuantity(
        mealId: widget.args.mealId,
        itemId: widget.args.item.id,
        quantity: q,
      );

      if (!mounted) return;
      Navigator.of(context).pop(true);
    } catch (e) {
      if (!mounted) return;
      setState(() => _error = e.toString());
    } finally {
      if (mounted) setState(() => _saving = false);
    }
  }

  Future<void> _onDelete() async {
    final ok = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Delete item'),
        content: const Text('Are you sure you want to delete this meal item?'),
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

    setState(() {
      _saving = true;
      _error = null;
    });

    try {
      final repo = ref.read(mealsRepoProvider);
      await repo.deleteMealItem(
        mealId: widget.args.mealId,
        itemId: widget.args.item.id,
      );

      if (!mounted) return;
      Navigator.of(context).pop(true);
    } catch (e) {
      if (!mounted) return;
      setState(() => _error = e.toString());
    } finally {
      if (mounted) setState(() => _saving = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final item = widget.args.item;

    final q = _parseQuantity();
    final kcal = (q != null && q > 0) ? _calcCalories(q) : null;

    return Scaffold(
      appBar: AppBar(
        title: const Text('Edit meal item'),
        actions: [
          IconButton(
            onPressed: _saving ? null : _onDelete,
            icon: const Icon(Icons.delete),
            tooltip: 'Delete',
          ),
        ],
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Center(
          child: ConstrainedBox(
            constraints: const BoxConstraints(maxWidth: 520),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                // Header card
                Container(
                  padding: const EdgeInsets.all(12),
                  decoration: BoxDecoration(
                    color: theme.colorScheme.surfaceVariant,
                    borderRadius: BorderRadius.circular(16),
                  ),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        item.groceryName,
                        style: theme.textTheme.titleLarge?.copyWith(
                          fontWeight: FontWeight.w800,
                        ),
                      ),
                      const SizedBox(height: 6),
                      Text(
                        'Per 100: ${item.caloriesPer100} kcal • P ${item.proteinGramsPer100.toStringAsFixed(1)} g • C ${item.carbsGramsPer100.toStringAsFixed(1)} g • F ${item.fatGramsPer100.toStringAsFixed(1)} g',
                        style: theme.textTheme.bodySmall?.copyWith(
                          color: theme.colorScheme.onSurfaceVariant,
                        ),
                      ),
                    ],
                  ),
                ),

                const SizedBox(height: 16),

                if (_error != null) ...[
                  Text(
                    _error!,
                    style: theme.textTheme.bodyMedium?.copyWith(
                      color: theme.colorScheme.error,
                    ),
                  ),
                  const SizedBox(height: 12),
                ],

                TextField(
                  controller: _qtyCtrl,
                  keyboardType:
                  const TextInputType.numberWithOptions(decimal: true),
                  onChanged: (_) => setState(() {}),
                  decoration: InputDecoration(
                    labelText: 'Quantity (${item.unitOfMeasure})',
                    helperText: kcal == null ? null : '≈ $kcal kcal',
                  ),
                ),

                const SizedBox(height: 18),

                SizedBox(
                  height: 50,
                  child: FilledButton(
                    onPressed: _saving ? null : _onSave,
                    child: _saving
                        ? const SizedBox(
                      height: 20,
                      width: 20,
                      child: CircularProgressIndicator(strokeWidth: 2),
                    )
                        : const Text('Save'),
                  ),
                ),

                const SizedBox(height: 10),

                OutlinedButton.icon(
                  onPressed: _saving ? null : _onDelete,
                  icon: const Icon(Icons.delete),
                  label: const Text('Delete item'),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
