import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:image_picker/image_picker.dart';
import 'package:go_router/go_router.dart';

import '../data/grocery_models.dart';
import '../data/grocery_repo.dart';
import '../../../core/providers.dart';
import '../../../core/token_store.dart';
import 'grocery_providers.dart';


class GrocerySuggestPage extends ConsumerStatefulWidget {
  const GrocerySuggestPage({super.key});

  @override
  ConsumerState<GrocerySuggestPage> createState() =>
      _GrocerySuggestPageState();
}

class _GrocerySuggestPageState extends ConsumerState<GrocerySuggestPage> {
  final _formKey = GlobalKey<FormState>();

  final _nameCtrl = TextEditingController();
  final _proteinCtrl = TextEditingController();
  final _carbsCtrl = TextEditingController();
  final _fatCtrl = TextEditingController();
  final _caloriesCtrl = TextEditingController();
  final _barcodeCtrl = TextEditingController();

  GroceryCategory _selectedCategory = GroceryCategory.uncategorized;
  UnitOfMeasureUi _selectedUnit = UnitOfMeasureUi.gram;

  XFile? _imageFile;
  bool _isSubmitting = false;
  String? _error;

  @override
  void dispose() {
    _nameCtrl.dispose();
    _proteinCtrl.dispose();
    _carbsCtrl.dispose();
    _fatCtrl.dispose();
    _caloriesCtrl.dispose();
    _barcodeCtrl.dispose();
    super.dispose();
  }

  Future<void> _pickImage() async {
    final picker = ImagePicker();
    final picked = await picker.pickImage(source: ImageSource.gallery);
    if (picked != null) {
      setState(() => _imageFile = picked);
    }
  }

  Future<void> _scanBarcode() async {
    final code = await context.push<String>('/scanner');
    if (!mounted) return;
    if (code != null && code.isNotEmpty) {
      setState(() {
        _barcodeCtrl.text = code;
      });
    }
  }

  double _parseDouble(String text) {
    final t = text.trim();
    if (t.isEmpty) return 0;
    return double.tryParse(t.replaceAll(',', '.')) ?? 0;
  }

  Future<void> _submit() async {
    final form = _formKey.currentState;
    if (form == null) return;
    if (!form.validate()) return;

    final protein = _parseDouble(_proteinCtrl.text);
    final carbs = _parseDouble(_carbsCtrl.text);
    final fat = _parseDouble(_fatCtrl.text);
    final caloriesText = _caloriesCtrl.text.trim();
    final calories = int.tryParse(caloriesText) ?? 0;

    if (calories <= 0) {
      setState(() {
        _error = 'Calories per 100 must be greater than 0.';
      });
      return;
    }

    setState(() {
      _isSubmitting = true;
      _error = null;
    });

    final repo = ref.read(groceryRepoProvider);

    try {
      await repo.createGrocery(
        name: _nameCtrl.text.trim(),
        category: _selectedCategory,
        unitOfMeasure: _selectedUnit,
        proteinPer100g: protein,
        carbsPer100g: carbs,
        fatPer100g: fat,
        caloriesPer100: calories,
        barcode: _barcodeCtrl.text.trim().isEmpty
            ? null
            : _barcodeCtrl.text.trim(),
        imageFile: _imageFile,
      );

      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Grocery suggestion submitted.'),
        ),
      );
      Navigator.of(context).pop();
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _error = e.toString();
      });
    } finally {
      if (mounted) {
        setState(() => _isSubmitting = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Suggest a grocery'),
      ),
      body: SafeArea(
        child: Center(
          child: ConstrainedBox(
            constraints: const BoxConstraints(maxWidth: 480),
            child: SingleChildScrollView(
              padding: const EdgeInsets.all(16),
              child: Form(
                key: _formKey,
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    Text(
                      'Propose a new food item. Our admins will review it before it appears in NutriTrack.',
                      style: theme.textTheme.bodyMedium,
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
                    TextFormField(
                      controller: _nameCtrl,
                      decoration: const InputDecoration(
                        labelText: 'Name',
                      ),
                      validator: (value) {
                        if (value == null || value.trim().isEmpty) {
                          return 'Name is required.';
                        }
                        return null;
                      },
                    ),
                    const SizedBox(height: 12),
                    DropdownButtonFormField<GroceryCategory>(
                      value: _selectedCategory,
                      decoration: const InputDecoration(
                        labelText: 'Category',
                      ),
                      items: GroceryCategory.values
                          .map(
                            (c) => DropdownMenuItem(
                          value: c,
                          child: Text(c.label),
                        ),
                      )
                          .toList(),
                      onChanged: (value) {
                        if (value == null) return;
                        setState(() => _selectedCategory = value);
                      },
                    ),
                    const SizedBox(height: 12),
                    DropdownButtonFormField<UnitOfMeasureUi>(
                      value: _selectedUnit,
                      decoration: const InputDecoration(
                        labelText: 'Unit of measure',
                      ),
                      items: UnitOfMeasureUi.values
                          .map(
                            (u) => DropdownMenuItem(
                          value: u,
                          child: Text(u.displayLabel),
                        ),
                      )
                          .toList(),
                      onChanged: (value) {
                        if (value == null) return;
                        setState(() => _selectedUnit = value);
                      },
                    ),
                    const SizedBox(height: 12),
                    TextFormField(
                      controller: _caloriesCtrl,
                      keyboardType: TextInputType.number,
                      decoration: const InputDecoration(
                        labelText: 'Calories per 100',
                      ),
                      validator: (value) {
                        if (value == null || value.trim().isEmpty) {
                          return 'Calories are required.';
                        }
                        final parsed = int.tryParse(value.trim());
                        if (parsed == null || parsed <= 0) {
                          return 'Enter a valid positive integer.';
                        }
                        return null;
                      },
                    ),
                    const SizedBox(height: 12),
                    TextFormField(
                      controller: _proteinCtrl,
                      keyboardType:
                      const TextInputType.numberWithOptions(decimal: true),
                      decoration: const InputDecoration(
                        labelText: 'Protein per 100 g (optional)',
                      ),
                    ),
                    const SizedBox(height: 12),
                    TextFormField(
                      controller: _carbsCtrl,
                      keyboardType:
                      const TextInputType.numberWithOptions(decimal: true),
                      decoration: const InputDecoration(
                        labelText: 'Carbs per 100 g (optional)',
                      ),
                    ),
                    const SizedBox(height: 12),
                    TextFormField(
                      controller: _fatCtrl,
                      keyboardType:
                      const TextInputType.numberWithOptions(decimal: true),
                      decoration: const InputDecoration(
                        labelText: 'Fat per 100 g (optional)',
                      ),
                    ),
                    const SizedBox(height: 12),
                    TextFormField(
                      controller: _barcodeCtrl,
                      decoration: InputDecoration(
                        labelText: 'Barcode / QR (optional)',
                        suffixIcon: IconButton(
                          icon: const Icon(Icons.qr_code_scanner),
                          onPressed: _scanBarcode,
                        ),
                      ),
                    ),
                    const SizedBox(height: 16),
                    Row(
                      children: [
                        ElevatedButton.icon(
                          onPressed: _pickImage,
                          icon: const Icon(Icons.image),
                          label: const Text('Choose image'),
                        ),
                        const SizedBox(width: 12),
                        if (_imageFile != null)
                          Expanded(
                            child: Text(
                              _imageFile!.name,
                              overflow: TextOverflow.ellipsis,
                            ),
                          ),
                      ],
                    ),
                    const SizedBox(height: 24),
                    SizedBox(
                      height: 48,
                      child: FilledButton(
                        onPressed: _isSubmitting ? null : _submit,
                        child: _isSubmitting
                            ? const SizedBox(
                          height: 20,
                          width: 20,
                          child: CircularProgressIndicator(
                            strokeWidth: 2,
                          ),
                        )
                            : const Text('Submit'),
                      ),
                    ),
                  ],
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }
}
