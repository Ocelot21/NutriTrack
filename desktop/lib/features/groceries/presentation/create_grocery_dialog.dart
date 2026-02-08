import 'dart:io';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:cross_file/cross_file.dart';
import 'package:file_picker/file_picker.dart';

import 'package:nutritrack_shared/groceries/data/grocery_models.dart';
import 'package:nutritrack_shared/groceries/data/grocery_repo.dart';
import 'grocery_providers.dart';

class CreateGroceryDialog extends ConsumerStatefulWidget {
  const CreateGroceryDialog({super.key});

  @override
  ConsumerState<CreateGroceryDialog> createState() =>
      _CreateGroceryDialogState();
}

class _CreateGroceryDialogState extends ConsumerState<CreateGroceryDialog> {
  final _formKey = GlobalKey<FormState>();

  final _nameController = TextEditingController();
  final _barcodeController = TextEditingController();
  final _proteinController = TextEditingController();
  final _carbsController = TextEditingController();
  final _fatController = TextEditingController();
  final _caloriesController = TextEditingController();
  final _gramsPerPieceController = TextEditingController();

  GroceryCategory? _category = GroceryCategory.uncategorized;
  UnitOfMeasureUi? _unit = UnitOfMeasureUi.gram;
  XFile? _image;

  bool _saving = false;
  String? _error;

  @override
  void dispose() {
    _nameController.dispose();
    _barcodeController.dispose();
    _proteinController.dispose();
    _carbsController.dispose();
    _fatController.dispose();
    _caloriesController.dispose();
    _gramsPerPieceController.dispose();
    super.dispose();
  }

  Future<void> _pickImage() async {
    final result = await FilePicker.platform.pickFiles(
      type: FileType.image,
    );

    if (result != null && result.files.single.path != null) {
      final path = result.files.single.path!;
      setState(() {
        _image = XFile(path); // cross_file XFile
      });
    }
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;

    final protein = double.tryParse(_proteinController.text.replaceAll(",", "."));
    final carbs = double.tryParse(_carbsController.text.replaceAll(",", "."));
    final fat = double.tryParse(_fatController.text.replaceAll(",", "."));
    final calories = int.tryParse(_caloriesController.text);

    final gramsPerPiece = _unit == UnitOfMeasureUi.piece
        ? double.tryParse(_gramsPerPieceController.text.replaceAll(',', '.'))
        : null;

    if (_unit == UnitOfMeasureUi.piece && (gramsPerPiece == null || gramsPerPiece <= 0)) {
      setState(() => _error = 'Grams per piece is required and must be > 0 for Piece unit.');
      return;
    }

    if (protein == null || carbs == null || fat == null || calories == null) {
      setState(() => _error = 'Invalid nutritional values.');
      return;
    }

    setState(() {
      _saving = true;
      _error = null;
    });

    final repo = ref.read(groceryRepoProvider);

    try {
      await repo.createGrocery(
        name: _nameController.text.trim(),
        category: _category!,
        unitOfMeasure: _unit!,
        gramsPerPiece: gramsPerPiece,
        proteinPer100: protein,
        carbsPer100: carbs,
        fatPer100: fat,
        caloriesPer100: calories,
        barcode: _barcodeController.text.trim().isEmpty
            ? null
            : _barcodeController.text.trim(),
        imageFile: _image,
      );

      // reload list
      ref.read(grocerySearchControllerProvider.notifier).applyFilters(
        const GrocerySearchFilters(),
      );

      if (mounted) Navigator.of(context).pop(true);
    } catch (e) {
      setState(() {
        _saving = false;
        _error = e.toString();
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Dialog(
      child: ConstrainedBox(
        constraints: const BoxConstraints(maxWidth: 650),
        child: Padding(
          padding: const EdgeInsets.all(24),
          child: SingleChildScrollView(
            child: Form(
              key: _formKey,
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  const Text(
                    'Create Grocery',
                    style: TextStyle(
                      fontSize: 20,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: 20),

                  Row(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      // Image block
                      Column(
                        children: [
                          SizedBox(
                            width: 150,
                            height: 150,
                            child: _image == null
                                ? Container(
                              decoration: BoxDecoration(
                                borderRadius: BorderRadius.circular(8),
                                color: Colors.grey.shade300,
                              ),
                              child: const Icon(Icons.image, size: 40),
                            )
                                : ClipRRect(
                              borderRadius: BorderRadius.circular(8),
                              child: Image.file(
                                File(_image!.path),
                                fit: BoxFit.cover,
                              ),
                            ),
                          ),
                          const SizedBox(height: 8),
                          OutlinedButton.icon(
                            onPressed: _saving ? null : _pickImage,
                            icon: const Icon(Icons.upload),
                            label: const Text('Select image'),
                          ),
                        ],
                      ),

                      const SizedBox(width: 24),

                      // Form fields
                      Expanded(
                        child: Column(
                          children: [
                            TextFormField(
                              controller: _nameController,
                              decoration:
                              const InputDecoration(labelText: 'Name *'),
                              validator: (v) {
                                if (v == null || v.trim().isEmpty) return 'Name is required';
                                if (!RegExp(r'[a-zA-Z]').hasMatch(v)) {
                                  return 'Name must contain at least one letter';
                                }
                                return null;
                              },
                            ),
                            const SizedBox(height: 12),

                            DropdownButtonFormField<GroceryCategory>(
                              initialValue: _category,
                              items: GroceryCategory.values
                                  .map((c) => DropdownMenuItem(
                                value: c,
                                child: Text(c.label),
                              ))
                                  .toList(),
                              onChanged: _saving
                                  ? null
                                  : (v) => setState(() => _category = v),
                              decoration:
                              const InputDecoration(labelText: 'Category'),
                            ),
                            const SizedBox(height: 12),

                            DropdownButtonFormField<UnitOfMeasureUi>(
                              initialValue: _unit,
                              items: UnitOfMeasureUi.values
                                  .map((u) => DropdownMenuItem(
                                value: u,
                                child: Text(u.displayLabel),
                              ))
                                  .toList(),
                              onChanged: _saving
                                  ? null
                                  : (v) => setState(() {
                                _unit = v;
                                if (_unit != UnitOfMeasureUi.piece) {
                                  _gramsPerPieceController.clear();
                                }
                              }),
                              decoration: const InputDecoration(
                                  labelText: 'Unit of measure'),
                            ),
                            if (_unit == UnitOfMeasureUi.piece) ...[
                              const SizedBox(height: 8),
                              Text(
                                'For "Piece", please set grams per piece so NutriTrack can convert pieces into grams.',
                                style: Theme.of(context).textTheme.bodySmall,
                              ),
                              const SizedBox(height: 12),
                              TextFormField(
                                controller: _gramsPerPieceController,
                                keyboardType: const TextInputType.numberWithOptions(decimal: true),
                                decoration: const InputDecoration(
                                  labelText: 'Grams per piece',
                                  helperText: 'Example: 1 egg ≈ 60',
                                ),
                                validator: (value) {
                                  if (_unit != UnitOfMeasureUi.piece) return null;
                                  final t = (value ?? '').trim();
                                  if (t.isEmpty) return 'Grams per piece is required for Piece.';
                                  final v = double.tryParse(t.replaceAll(',', '.'));
                                  if (v == null || v <= 0) return 'Enter a value greater than 0.';
                                  return null;
                                },
                              ),
                              const SizedBox(height: 12),
                            ],

                            TextFormField(
                              controller: _barcodeController,
                              decoration:
                              const InputDecoration(labelText: 'Barcode (optional)'),
                            ),
                            const SizedBox(height: 12),

                            Row(
                              children: [
                                Expanded(
                                  child: TextFormField(
                                    controller: _proteinController,
                                    decoration: InputDecoration(
                                      labelText: 'Protein ${per100LabelSuffix()} *',
                                    ),
                                    keyboardType: TextInputType.number,
                                    validator: (v) {
                                      if (v == null || v.trim().isEmpty) return 'Required';
                                      final val = double.tryParse(v.replaceAll(',', '.'));
                                      if (val == null || val < 0) return 'Enter valid number ≥ 0';
                                      return null;
                                    },
                                  ),
                                ),
                                const SizedBox(width: 8),
                                Expanded(
                                  child: TextFormField(
                                    controller: _carbsController,
                                    decoration: InputDecoration(
                                      labelText: 'Carbs ${per100LabelSuffix()} *',
                                    ),
                                    keyboardType: TextInputType.number,
                                    validator: (v) {
                                      if (v == null || v.trim().isEmpty) return 'Required';
                                      final val = double.tryParse(v.replaceAll(',', '.'));
                                      if (val == null || val < 0) return 'Enter valid number ≥ 0';
                                      return null;
                                    },
                                  ),
                                ),
                              ],
                            ),
                            const SizedBox(height: 12),

                            Row(
                              children: [
                                Expanded(
                                  child: TextFormField(
                                    controller: _fatController,
                                    decoration: InputDecoration(
                                      labelText: 'Fat ${per100LabelSuffix()} *',
                                    ),
                                    keyboardType: TextInputType.number,
                                    validator: (v) {
                                      if (v == null || v.trim().isEmpty) return 'Required';
                                      final val = double.tryParse(v.replaceAll(',', '.'));
                                      if (val == null || val < 0) return 'Enter valid number ≥ 0';
                                      return null;
                                    },
                                  ),
                                ),
                                const SizedBox(width: 8),
                                Expanded(
                                  child: TextFormField(
                                    controller: _caloriesController,
                                    decoration: InputDecoration(labelText: 'Calories ${per100LabelSuffix()} *'),
                                    keyboardType: TextInputType.number,
                                    validator: (v) {
                                      if (v == null || v.trim().isEmpty) return 'Required';
                                      final val = int.tryParse(v);
                                      if (val == null || val < 0) return 'Enter valid integer ≥ 0';
                                      return null;
                                    },
                                  ),
                                ),
                              ],
                            ),
                          ],
                        ),
                      ),
                    ],
                  ),

                  const SizedBox(height: 20),

                  if (_error != null)
                    Text(
                      _error!,
                      style: const TextStyle(color: Colors.red),
                    ),

                  const SizedBox(height: 16),

                  Row(
                    mainAxisAlignment: MainAxisAlignment.end,
                    children: [
                      TextButton(
                        onPressed: _saving
                            ? null
                            : () => Navigator.of(context).pop(false),
                        child: const Text('Cancel'),
                      ),
                      const SizedBox(width: 12),
                      FilledButton(
                        onPressed: _saving ? null : _submit,
                        child: _saving
                            ? const SizedBox(
                          width: 18,
                          height: 18,
                          child: CircularProgressIndicator(strokeWidth: 2),
                        )
                            : const Text('Create'),
                      ),
                    ],
                  )
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }

  String per100LabelSuffix() {
    switch (_unit) {
      case UnitOfMeasureUi.gram:
        return 'per 100 g';
      case UnitOfMeasureUi.milliliter:
        return 'per 100 ml';
      case UnitOfMeasureUi.piece:
        return 'per 100 g';
      default:
        return 'per 100 g';
    }
  }
}
