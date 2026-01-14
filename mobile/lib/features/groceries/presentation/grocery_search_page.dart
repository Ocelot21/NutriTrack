import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../meals/presentation/create_meal_item_page.dart';
import 'package:nutritrack_shared/groceries/data/grocery_repo.dart';
import 'package:nutritrack_shared/groceries/data/grocery_models.dart';
import 'grocery_providers.dart';

class GrocerySearchPageArgs {
  final String? mealId;
  final DateTime? mealLocalDate;

  GrocerySearchPageArgs({this.mealId, this.mealLocalDate});

  bool get selectionMode => mealId != null;
}

String per100UnitLabel(UnitOfMeasureUi u) {
  switch (u) {
    case UnitOfMeasureUi.gram:
      return '100 g';
    case UnitOfMeasureUi.milliliter:
      return '100 ml';
    case UnitOfMeasureUi.piece:
      return '100 g'; // canonical for piece too
  }
}

String unitChipLabel(UnitOfMeasureUi u) {
  switch (u) {
    case UnitOfMeasureUi.gram:
      return 'g';
    case UnitOfMeasureUi.milliliter:
      return 'ml';
    case UnitOfMeasureUi.piece:
      return 'pc';
  }
}

/// Normalizes raw scanner output (barcode/QR payload) into the code format our API expects.
///
/// Handles:
/// - whitespace/newlines
/// - common prefixes (EAN:, UPC:, barcode:)
/// - URLs that include the code as `?code=`/`?barcode=` or as the last path segment
String? normalizeScannedCode(String? raw) {
  if (raw == null) return null;
  var v = raw.trim();
  if (v.isEmpty) return null;

  // Some QR codes contain URLs or "barcode=..." query params.
  try {
    final uri = Uri.tryParse(v);
    if (uri != null && (uri.hasScheme || uri.host.isNotEmpty)) {
      final qp = uri.queryParameters;
      final fromQuery = qp['code'] ?? qp['barcode'] ?? qp['ean'] ?? qp['upc'];
      if (fromQuery != null && fromQuery.trim().isNotEmpty) {
        v = fromQuery.trim();
      } else if (uri.pathSegments.isNotEmpty) {
        v = uri.pathSegments.last.trim();
      }
    }
  } catch (_) {
    // ignore parse errors, keep original
  }

  v = v.trim();

  // Strip common prefixes from scanners / QR payloads.
  final lower = v.toLowerCase();
  const prefixes = [
    'ean-13:',
    'ean13:',
    'ean:',
    'upc-a:',
    'upca:',
    'upc:',
    'barcode:',
    'qr:',
    'code:',
  ];
  for (final p in prefixes) {
    if (lower.startsWith(p)) {
      v = v.substring(p.length).trim();
      break;
    }
  }

  // Remove whitespace and hyphens that might be introduced in some formats.
  v = v.replaceAll(RegExp(r'[\s-]'), '');

  return v.isEmpty ? null : v;
}

class GrocerySearchPage extends ConsumerStatefulWidget {
  final GrocerySearchPageArgs? args;
  const GrocerySearchPage({super.key, this.args});

  bool get selectionMode => args?.selectionMode ?? false;

  @override
  ConsumerState<GrocerySearchPage> createState() => _GrocerySearchPageState();
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

  GroceryCategory? _category; // null = All
  UnitOfMeasureUi? _unit; // null = Any

  Timer? _debounce;

  @override
  void initState() {
    super.initState();

    WidgetsBinding.instance.addPostFrameCallback((_) {
      ref.read(grocerySearchControllerProvider.notifier).loadInitialIfEmpty();
    });

    _searchCtrl.addListener(_onSearchChanged);
  }

  @override
  void dispose() {
    _debounce?.cancel();
    _searchCtrl.removeListener(_onSearchChanged);

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

  void _onSearchChanged() {
    _debounce?.cancel();
    _debounce = Timer(const Duration(milliseconds: 350), () {
      _applyFilters();
    });
  }

  double? _parseDoubleOrNull(String? text) {
    if (text == null) return null;
    final t = text.trim();
    if (t.isEmpty) return null;
    return double.tryParse(t.replaceAll(',', '.'));
  }

  GrocerySearchFilters _buildFilters() {
    return GrocerySearchFilters(
      searchTerm: _searchCtrl.text,
      category: _category,
      unitOfMeasure: _unit,
      minCaloriesPer100: _parseDoubleOrNull(_minKcalCtrl.text),
      maxCaloriesPer100: _parseDoubleOrNull(_maxKcalCtrl.text),
      minProteinPer100: _parseDoubleOrNull(_minProteinCtrl.text),
      maxProteinPer100: _parseDoubleOrNull(_maxProteinCtrl.text),
      minCarbsPer100: _parseDoubleOrNull(_minCarbsCtrl.text),
      maxCarbsPer100: _parseDoubleOrNull(_maxCarbsCtrl.text),
      minFatPer100: _parseDoubleOrNull(_minFatCtrl.text),
      maxFatPer100: _parseDoubleOrNull(_maxFatCtrl.text),
    );
  }

  Future<void> _applyFilters() async {
    await ref
        .read(grocerySearchControllerProvider.notifier)
        .applyFilters(_buildFilters());
  }

  int _activeAdvancedCount() {
    int c = 0;
    if (_parseDoubleOrNull(_minKcalCtrl.text) != null) c++;
    if (_parseDoubleOrNull(_maxKcalCtrl.text) != null) c++;
    if (_parseDoubleOrNull(_minProteinCtrl.text) != null) c++;
    if (_parseDoubleOrNull(_maxProteinCtrl.text) != null) c++;
    if (_parseDoubleOrNull(_minCarbsCtrl.text) != null) c++;
    if (_parseDoubleOrNull(_maxCarbsCtrl.text) != null) c++;
    if (_parseDoubleOrNull(_minFatCtrl.text) != null) c++;
    if (_parseDoubleOrNull(_maxFatCtrl.text) != null) c++;
    return c;
  }

  Future<void> _clearAll() async {
    setState(() {
      _searchCtrl.clear();
      _category = null;
      _unit = null;
      _minKcalCtrl.clear();
      _maxKcalCtrl.clear();
      _minProteinCtrl.clear();
      _maxProteinCtrl.clear();
      _minCarbsCtrl.clear();
      _maxCarbsCtrl.clear();
      _minFatCtrl.clear();
      _maxFatCtrl.clear();
    });
    await _applyFilters();
  }

  Grocery? _findLoadedByBarcode(String code) {
    final state = ref.read(grocerySearchControllerProvider);
    final normalized = normalizeScannedCode(code) ?? code.trim();

    for (final g in state.items) {
      final b = g.barcode;
      if (b == null || b.trim().isEmpty) continue;
      final bn = normalizeScannedCode(b) ?? b.trim();
      if (bn == normalized) return g;
    }

    return null;
  }

  Future<void> _navigateToCreateMealItem(Grocery item) async {
    final created = await context.push<bool>(
      '/meal-items/create',
      extra: MealItemCreatePageArgs(
        grocery: item,
        preferredMealId: widget.selectionMode ? widget.args!.mealId : null,
        contextDate: widget.selectionMode ? widget.args!.mealLocalDate : null,
      ),
    );

    if (!mounted) return;
    if (widget.selectionMode && created == true) {
      context.pop(true);
    }
  }

  Future<void> _searchByCode(String code) async {
    final repo = ref.read(groceryRepoProvider);

    final normalized = normalizeScannedCode(code);
    final candidates = <String>{
      if (normalized != null) normalized,
      code.trim(),
    }..removeWhere((e) => e.isEmpty);

    // 1) Fast path: try to find it in already loaded items.
    for (final c in candidates) {
      final loaded = _findLoadedByBarcode(c);
      if (loaded != null) {
        await _navigateToCreateMealItem(loaded);
        return;
      }
    }

    // 2) Fallback: ask the API.
    try {
      Grocery? item;
      Object? lastError;

      for (final c in candidates) {
        try {
          item = await repo.getByCode(c);
          break;
        } catch (e) {
          lastError = e;
        }
      }

      if (!mounted) return;

      if (item == null) {
        throw lastError ?? Exception('Not found');
      }

      await _navigateToCreateMealItem(item);
    } catch (_) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Not found')),
      );
    }
  }

  void _openMoreFiltersSheet() {
    final theme = Theme.of(context);

    showModalBottomSheet<void>(
      context: context,
      isScrollControlled: true,
      showDragHandle: true,
      builder: (ctx) {
        return SafeArea(
          child: Padding(
            padding: EdgeInsets.only(
              left: 16,
              right: 16,
              bottom: 16 + MediaQuery.of(ctx).viewInsets.bottom,
              top: 8,
            ),
            child: SingleChildScrollView(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  Text('More filters', style: theme.textTheme.titleLarge),
                  const SizedBox(height: 12),
                  _RangeRow(
                    label: 'Calories (kcal / 100)',
                    minCtrl: _minKcalCtrl,
                    maxCtrl: _maxKcalCtrl,
                  ),
                  const SizedBox(height: 10),
                  _RangeRow(
                    label: 'Protein (g / 100)',
                    minCtrl: _minProteinCtrl,
                    maxCtrl: _maxProteinCtrl,
                  ),
                  const SizedBox(height: 10),
                  _RangeRow(
                    label: 'Carbs (g / 100)',
                    minCtrl: _minCarbsCtrl,
                    maxCtrl: _maxCarbsCtrl,
                  ),
                  const SizedBox(height: 10),
                  _RangeRow(
                    label: 'Fat (g / 100)',
                    minCtrl: _minFatCtrl,
                    maxCtrl: _maxFatCtrl,
                  ),
                  const SizedBox(height: 16),
                  Row(
                    children: [
                      Expanded(
                        child: OutlinedButton(
                          onPressed: () {
                            _minKcalCtrl.clear();
                            _maxKcalCtrl.clear();
                            _minProteinCtrl.clear();
                            _maxProteinCtrl.clear();
                            _minCarbsCtrl.clear();
                            _maxCarbsCtrl.clear();
                            _minFatCtrl.clear();
                            _maxFatCtrl.clear();
                            if (mounted) setState(() {});
                          },
                          child: const Text('Clear'),
                        ),
                      ),
                      const SizedBox(width: 12),
                      Expanded(
                        child: FilledButton.icon(
                          onPressed: () async {
                            Navigator.of(ctx).pop();
                            await _applyFilters();
                            if (mounted) setState(() {});
                          },
                          icon: const Icon(Icons.check),
                          label: const Text('Apply'),
                        ),
                      ),
                    ],
                  ),
                ],
              ),
            ),
          ),
        );
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final state = ref.watch(grocerySearchControllerProvider);

    final advancedCount = _activeAdvancedCount();

    return Scaffold(
      appBar: AppBar(
        title: const Text('Groceries'),
        actions: [
          IconButton(
            tooltip: 'Scan code',
            icon: const Icon(Icons.qr_code_scanner),
            onPressed: () async {
              final code = await context.push<String>('/scanner');
              if (code == null || code.isEmpty) return;
              await _searchByCode(code);
            },
          ),
          IconButton(
            tooltip: 'Clear all',
            icon: const Icon(Icons.clear_all),
            onPressed: state.isLoading ? null : _clearAll,
          ),
        ],
      ),
      body: SafeArea(
        child: Column(
          children: [
            Padding(
              padding: const EdgeInsets.fromLTRB(12, 12, 12, 8),
              child: TextField(
                controller: _searchCtrl,
                textInputAction: TextInputAction.search,
                onSubmitted: (_) => _applyFilters(),
                decoration: InputDecoration(
                  labelText: 'Search groceries',
                  prefixIcon: const Icon(Icons.search),
                  suffixIcon: _searchCtrl.text.trim().isEmpty
                      ? null
                      : IconButton(
                    tooltip: 'Clear search',
                    icon: const Icon(Icons.close),
                    onPressed: () {
                      _searchCtrl.clear();
                      _applyFilters();
                    },
                  ),
                ),
              ),
            ),

            // Compact filter pills
            Padding(
              padding: const EdgeInsets.fromLTRB(12, 0, 12, 10),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  _CategoryPills(
                    selected: _category,
                    onChanged: (c) async {
                      setState(() => _category = c);
                      await _applyFilters();
                    },
                  ),
                  const SizedBox(height: 8),
                  Row(
                    children: [
                      Expanded(
                        child: _UnitPills(
                          selected: _unit,
                          onChanged: (u) async {
                            setState(() => _unit = u);
                            await _applyFilters();
                          },
                        ),
                      ),
                      const SizedBox(width: 10),
                      Stack(
                        clipBehavior: Clip.none,
                        children: [
                          OutlinedButton.icon(
                            onPressed:
                            state.isLoading ? null : _openMoreFiltersSheet,
                            icon: const Icon(Icons.tune),
                            label: const Text('More'),
                          ),
                          if (advancedCount > 0)
                            Positioned(
                              right: -6,
                              top: -6,
                              child: Container(
                                padding: const EdgeInsets.symmetric(
                                  horizontal: 6,
                                  vertical: 2,
                                ),
                                decoration: BoxDecoration(
                                  color: theme.colorScheme.primary,
                                  borderRadius: BorderRadius.circular(999),
                                ),
                                child: Text(
                                  advancedCount.toString(),
                                  style: theme.textTheme.labelSmall?.copyWith(
                                    color: theme.colorScheme.onPrimary,
                                    fontWeight: FontWeight.bold,
                                  ),
                                ),
                              ),
                            ),
                        ],
                      ),
                    ],
                  ),
                ],
              ),
            ),

            const Divider(height: 1),

            Expanded(
              child: state.isLoading && state.items.isEmpty
                  ? const Center(child: CircularProgressIndicator())
                  : RefreshIndicator(
                onRefresh: _applyFilters,
                child: ListView.separated(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 12,
                    vertical: 10,
                  ),
                  itemCount: state.items.length +
                      (state.canLoadMore ? 1 : 0),
                  separatorBuilder: (_, __) =>
                  const SizedBox(height: 10),
                  itemBuilder: (context, index) {
                    if (index < state.items.length) {
                      final g = state.items[index];

                      return _GroceryCard(
                        grocery: g,
                        onTap: () async {
                          final created = await context.push<bool>(
                            '/meal-items/create',
                            extra: MealItemCreatePageArgs(
                              grocery: g,
                              preferredMealId: widget.selectionMode
                                  ? widget.args!.mealId
                                  : null,
                              contextDate: widget.selectionMode
                                  ? widget.args!.mealLocalDate
                                  : null,
                            ),
                          );

                          if (!context.mounted) return;

                          if (widget.selectionMode && created == true) {
                            Navigator.of(context).pop(true);
                          }
                        },
                      );
                    }

                    if (!state.canLoadMore) return const SizedBox.shrink();

                    return Padding(
                      padding: const EdgeInsets.symmetric(vertical: 6),
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
                const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
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

class _CategoryPills extends StatelessWidget {
  final GroceryCategory? selected; // null = All
  final ValueChanged<GroceryCategory?> onChanged;

  const _CategoryPills({
    required this.selected,
    required this.onChanged,
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

    FilterChip chip({
      required String label,
      required bool isSelected,
      required VoidCallback onTap,
    }) {
      return FilterChip(
        label: Text(label),
        selected: isSelected,
        showCheckmark: false,
        visualDensity: VisualDensity.compact,
        materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
        selectedColor: theme.colorScheme.secondaryContainer,
        onSelected: (_) => onTap(),
      );
    }

    return SingleChildScrollView(
      scrollDirection: Axis.horizontal,
      child: Row(
        children: [
          chip(
            label: 'All',
            isSelected: selected == null,
            onTap: () => onChanged(null),
          ),
          const SizedBox(width: 8),
          for (final c in categories) ...[
            chip(
              label: c.label,
              isSelected: selected == c,
              onTap: () => onChanged(selected == c ? null : c),
            ),
            const SizedBox(width: 8),
          ],
        ],
      ),
    );
  }
}

class _UnitPills extends StatelessWidget {
  final UnitOfMeasureUi? selected; // null = Any
  final ValueChanged<UnitOfMeasureUi?> onChanged;

  const _UnitPills({
    required this.selected,
    required this.onChanged,
  });

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    FilterChip chip({
      required String label,
      required bool isSelected,
      required VoidCallback onTap,
    }) {
      return FilterChip(
        label: Text(label),
        selected: isSelected,
        showCheckmark: false,
        visualDensity: VisualDensity.compact,
        materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
        selectedColor: theme.colorScheme.primaryContainer,
        onSelected: (_) => onTap(),
      );
    }

    final units = UnitOfMeasureUi.values;

    return SingleChildScrollView(
      scrollDirection: Axis.horizontal,
      child: Row(
        children: [
          chip(
            label: 'Any',
            isSelected: selected == null,
            onTap: () => onChanged(null),
          ),
          const SizedBox(width: 8),
          for (final u in units) ...[
            chip(
              label: unitChipLabel(u),
              isSelected: selected == u,
              onTap: () => onChanged(selected == u ? null : u),
            ),
            const SizedBox(width: 8),
          ],
        ],
      ),
    );
  }
}

class _RangeRow extends StatelessWidget {
  final String label;
  final TextEditingController minCtrl;
  final TextEditingController maxCtrl;

  const _RangeRow({
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
            decoration: InputDecoration(labelText: 'Min $label'),
          ),
        ),
        const SizedBox(width: 12),
        Expanded(
          child: TextField(
            controller: maxCtrl,
            keyboardType:
            const TextInputType.numberWithOptions(decimal: true),
            decoration: InputDecoration(labelText: 'Max $label'),
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
    final isPiece = grocery.unitOfMeasure == UnitOfMeasureUi.piece;

    return Card(
      elevation: 0,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(16),
        side: BorderSide(
          color: theme.dividerColor.withValues(alpha: 0.6),
        ),
      ),
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(16),
        child: Padding(
          padding: const EdgeInsets.all(12),
          child: Row(
            children: [
              _GroceryAvatar(grocery: grocery),
              const SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(grocery.name, style: theme.textTheme.titleMedium),
                    const SizedBox(height: 3),
                    Text(
                      grocery.category.label,
                      style: theme.textTheme.bodySmall?.copyWith(
                        color: theme.colorScheme.onSurfaceVariant,
                      ),
                    ),
                    const SizedBox(height: 8),
                    Wrap(
                      spacing: 8,
                      runSpacing: 8,
                      children: [
                        _Badge(
                          icon: Icons.local_fire_department,
                          text:
                          '${grocery.caloriesPer100} kcal / ${per100UnitLabel(grocery.unitOfMeasure)}',
                        ),
                        _Badge(
                          icon: Icons.pie_chart_outline,
                          text:
                          'P ${grocery.proteinPer100.toStringAsFixed(1)} · '
                              'C ${grocery.carbsPer100.toStringAsFixed(1)} · '
                              'F ${grocery.fatPer100.toStringAsFixed(1)}',
                        ),
                        if (isPiece)
                          _Badge(
                            icon: Icons.straighten,
                            text: grocery.gramsPerPiece != null
                                ? '1 pc ≈ ${grocery.gramsPerPiece!.toStringAsFixed(0)} g'
                                : '1 pc: unknown g',
                          ),
                      ],
                    ),
                  ],
                ),
              ),
              const SizedBox(width: 6),
              Icon(
                Icons.chevron_right,
                color: theme.colorScheme.onSurfaceVariant,
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _GroceryAvatar extends StatelessWidget {
  final Grocery grocery;

  const _GroceryAvatar({required this.grocery});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final imageUrl = grocery.imageUrl;

    return ClipRRect(
      borderRadius: BorderRadius.circular(14),
      child: SizedBox(
        width: 54,
        height: 54,
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
}

class _Badge extends StatelessWidget {
  final IconData icon;
  final String text;

  const _Badge({required this.icon, required this.text});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
      decoration: BoxDecoration(
        color: theme.colorScheme.surfaceContainerHighest,
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

