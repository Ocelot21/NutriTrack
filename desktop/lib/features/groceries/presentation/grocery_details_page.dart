import 'dart:io';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:cross_file/cross_file.dart';
import 'package:file_picker/file_picker.dart';
import 'package:go_router/go_router.dart';

import 'package:nutritrack_shared/groceries/data/grocery_models.dart';
import 'package:nutritrack_shared/groceries/data/grocery_repo.dart';
import 'grocery_providers.dart';
import '../../../core/widgets/sas_network_image.dart';

class GroceryDetailsPage extends ConsumerStatefulWidget {
  final String id;

  const GroceryDetailsPage({super.key, required this.id});

  @override
  ConsumerState<GroceryDetailsPage> createState() =>
      _GroceryDetailsPageState();
}

class _GroceryDetailsPageState extends ConsumerState<GroceryDetailsPage> {
  Grocery? _grocery;
  bool _isLoading = true;
  String? _error;
  bool _isEditing = false;
  bool _isSaving = false;
  bool _isDeleting = false;

  final _nameController = TextEditingController();
  final _barcodeController = TextEditingController();
  final _proteinController = TextEditingController();
  final _carbsController = TextEditingController();
  final _fatController = TextEditingController();
  final _caloriesController = TextEditingController();
  final _gramsPerPieceController = TextEditingController();

  GroceryCategory? _selectedCategory;
  UnitOfMeasureUi? _selectedUnit;
  XFile? _newImage;

  final _formKey = GlobalKey<FormState>();

  @override
  void initState() {
    super.initState();
    _load();
  }

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

  Future<void> _load() async {
    setState(() {
      _isLoading = true;
      _error = null;
    });

    final repo = ref.read(groceryRepoProvider);

    try {
      final g = await repo.getById(widget.id);
      _grocery = g;

      _nameController.text = g.name;
      _barcodeController.text = g.barcode ?? '';
      _proteinController.text = g.proteinPer100.toString();
      _carbsController.text = g.carbsPer100.toString();
      _fatController.text = g.fatPer100.toString();
      _caloriesController.text = g.caloriesPer100.toString();
      _gramsPerPieceController.text = g.gramsPerPiece?.toString() ?? '';

      _selectedCategory = g.category;
      _selectedUnit = g.unitOfMeasure;

      setState(() {
        _isLoading = false;
      });
    } on GrocerySearchException catch (e) {
      setState(() {
        _isLoading = false;
        _error = e.message;
      });
    } catch (e) {
      setState(() {
        _isLoading = false;
        _error = 'Unexpected error: $e';
      });
    }
  }

  void _startEdit() {
    setState(() {
      _isEditing = true;
      _newImage = null;
    });
  }

  void _cancelEdit() {
    if (_grocery != null) {
      _nameController.text = _grocery!.name;
      _barcodeController.text = _grocery!.barcode ?? '';
      _proteinController.text = _grocery!.proteinPer100.toString();
      _carbsController.text = _grocery!.carbsPer100.toString();
      _fatController.text = _grocery!.fatPer100.toString();
      _caloriesController.text = _grocery!.caloriesPer100.toString();
      _gramsPerPieceController.text = _grocery!.gramsPerPiece?.toString() ?? '';
      _selectedCategory = _grocery!.category;
      _selectedUnit = _grocery!.unitOfMeasure;
      _newImage = null;
    }
    setState(() {
      _isEditing = false;
    });
  }

  Future<void> _pickImage() async {
    final result = await FilePicker.platform.pickFiles(
      type: FileType.image,
    );

    if (result != null && result.files.single.path != null) {
      final path = result.files.single.path!;
      setState(() {
        _newImage = XFile(path);
      });
    }
  }

  Future<void> _save() async {
    if (!_formKey.currentState!.validate()) return;
    if (_selectedCategory == null || _selectedUnit == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Category and unit of measure are required.')),
      );
      return;
    }

    final protein = double.tryParse(_proteinController.text.replaceAll(',', '.'));
    final carbs = double.tryParse(_carbsController.text.replaceAll(',', '.'));
    final fat = double.tryParse(_fatController.text.replaceAll(',', '.'));
    final calories = int.tryParse(_caloriesController.text);
    final gramsPerPiece = _selectedUnit == UnitOfMeasureUi.piece
        ? double.tryParse(_gramsPerPieceController.text.replaceAll(',', '.'))
        : null;

    if (protein == null || carbs == null || fat == null || calories == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Invalid numeric values.')),
      );
      return;
    }
    if (_selectedUnit == UnitOfMeasureUi.piece && (gramsPerPiece == null || gramsPerPiece <= 0)) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Grams per piece is required and must be > 0 for Piece unit.')),
      );
      return;
    }

    setState(() {
      _isSaving = true;
      _error = null;
    });

    final repo = ref.read(groceryRepoProvider);

    try {
      await repo.updateGrocery(
        id: widget.id,
        name: _nameController.text.trim(),
        category: _selectedCategory!,
        unitOfMeasure: _selectedUnit!,
        proteinPer100: protein,
        carbsPer100: carbs,
        fatPer100: fat,
        caloriesPer100: calories,
        gramsPerPiece: gramsPerPiece,
        barcode: _barcodeController.text.trim().isEmpty
            ? null
            : _barcodeController.text.trim(),
        imageFile: _newImage,
      );

      await _load();
      if (!mounted) return;
      setState(() {
        _isSaving = false;
        _isEditing = false;
      });

      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Grocery updated.')),
      );
    } on GrocerySearchException catch (e) {
      setState(() {
        _isSaving = false;
        _error = e.message;
      });
    } catch (e) {
      setState(() {
        _isSaving = false;
        _error = 'Unexpected error: $e';
      });
    }
  }

  Future<void> _confirmDelete() async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Delete grocery'),
        content: const Text(
          'Are you sure you want to delete this grocery? This action cannot be undone.',
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(false),
            child: const Text('Cancel'),
          ),
          FilledButton(
            onPressed: () => Navigator.of(context).pop(true),
            child: const Text('Delete'),
          ),
        ],
      ),
    );

    if (confirmed == true) {
      await _delete();
    }
  }

  Future<void> _delete() async {
    setState(() {
      _isDeleting = true;
      _error = null;
    });

    final repo = ref.read(groceryRepoProvider);

    try {
      await repo.deleteGrocery(widget.id);

      if (mounted) {
        context.go('/groceries');
      }
    } on GrocerySearchException catch (e) {
      setState(() {
        _isDeleting = false;
        _error = e.message;
      });
    } catch (e) {
      setState(() {
        _isDeleting = false;
        _error = 'Unexpected error: $e';
      });
    }
  }

  void _goBack() {
    // Prefer returning to the actual previous page when possible.
    if (Navigator.of(context).canPop()) {
      context.pop();
      return;
    }

    // Fallback if opened as a deep link.
    context.go('/groceries');
  }

  @override
  Widget build(BuildContext context) {
    if (_isLoading && _grocery == null) {
      return const Scaffold(
        body: Center(child: CircularProgressIndicator()),
      );
    }

    if (_error != null && _grocery == null) {
      return Scaffold(
        appBar: AppBar(
          title: const Text('Grocery details'),
        ),
        body: Center(
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Text(_error!, style: const TextStyle(color: Colors.red)),
              const SizedBox(height: 8),
              ElevatedButton(
                onPressed: _load,
                child: const Text('Retry'),
              ),
            ],
          ),
        ),
      );
    }

    final g = _grocery!;
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: Text('Grocery: ${g.name}'),
        leading: IconButton(
          tooltip: 'Back',
          icon: const Icon(Icons.arrow_back),
          onPressed: _goBack,
        ),
        actions: [
          if (!_isEditing)
            IconButton(
              tooltip: 'Edit',
              onPressed: _startEdit,
              icon: const Icon(Icons.edit),
            ),
          if (_isEditing) ...[
            TextButton(
              onPressed: _isSaving ? null : _cancelEdit,
              child: const Text('Cancel'),
            ),
            TextButton(
              onPressed: _isSaving ? null : _save,
              child: _isSaving
                  ? const SizedBox(
                      width: 16,
                      height: 16,
                      child: CircularProgressIndicator(strokeWidth: 2),
                    )
                  : const Text('Save'),
            ),
          ],
          const SizedBox(width: 8),
          IconButton(
            tooltip: 'Delete',
            onPressed: _isDeleting ? null : _confirmDelete,
            icon: const Icon(Icons.delete),
          ),
          const SizedBox(width: 8),
        ],
      ),
      body: Center(
        child: ConstrainedBox(
          constraints: const BoxConstraints(maxWidth: 1050),
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Form(
              key: _formKey,
              child: SingleChildScrollView(
                child: LayoutBuilder(
                  builder: (context, constraints) {
                    final isNarrow = constraints.maxWidth < 820;

                    final imagePanel = SizedBox(
                      width: isNarrow ? double.infinity : 260,
                      child: Column(
                        children: [
                          AspectRatio(
                            aspectRatio: 1,
                            child: _buildImagePreview(g),
                          ),
                          const SizedBox(height: 12),
                          if (_isEditing)
                            OutlinedButton.icon(
                              onPressed: _pickImage,
                              icon: const Icon(Icons.image),
                              label: const Text('Change image'),
                            ),
                        ],
                      ),
                    );

                    final detailsPanel = Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        if (_error != null)
                          Padding(
                            padding: const EdgeInsets.only(bottom: 8),
                            child: Text(
                              _error!,
                              style: const TextStyle(color: Colors.red),
                            ),
                          ),
                        Wrap(
                          spacing: 16,
                          runSpacing: 16,
                          children: [
                            SizedBox(
                              width: isNarrow ? double.infinity : 360,
                              child: TextFormField(
                                controller: _nameController,
                                enabled: _isEditing,
                                decoration: const InputDecoration(
                                  labelText: 'Name *',
                                ),
                                validator: (v) {
                                  if (v == null || v.trim().isEmpty) return 'Name is required';
                                  if (!RegExp(r'[a-zA-Z]').hasMatch(v)) {
                                    return 'Name must contain at least one letter';
                                  }
                                  return null;
                                },
                              ),
                            ),
                            SizedBox(
                              width: isNarrow ? double.infinity : 220,
                              child: DropdownButtonFormField<GroceryCategory>(
                                initialValue: _selectedCategory,
                                items: GroceryCategory.values.map((c) {
                                  return DropdownMenuItem(
                                    value: c,
                                    child: Text(c.label),
                                  );
                                }).toList(),
                                onChanged: _isEditing
                                    ? (v) => setState(() {
                                          _selectedCategory = v;
                                        })
                                    : null,
                                decoration: const InputDecoration(
                                  labelText: 'Category',
                                ),
                              ),
                            ),
                            SizedBox(
                              width: isNarrow ? double.infinity : 220,
                              child: DropdownButtonFormField<UnitOfMeasureUi>(
                                initialValue: _selectedUnit,
                                items: UnitOfMeasureUi.values.map((u) {
                                  return DropdownMenuItem(
                                    value: u,
                                    child: Text(u.displayLabel),
                                  );
                                }).toList(),
                                onChanged: _isEditing
                                    ? (v) => setState(() {
                                          _selectedUnit = v;
                                          if (_selectedUnit != UnitOfMeasureUi.piece) {
                                            _gramsPerPieceController.clear();
                                          }
                                        })
                                    : null,
                                decoration: const InputDecoration(
                                  labelText: 'Unit of measure',
                                ),
                              ),
                            ),
                            SizedBox(
                              width: isNarrow ? double.infinity : 240,
                              child: TextFormField(
                                controller: _barcodeController,
                                enabled: _isEditing,
                                decoration: const InputDecoration(
                                  labelText: 'Barcode / code',
                                ),
                              ),
                            ),
                          ],
                        ),
                        const SizedBox(height: 24),
                        Text(
                          'Nutrition (${per100LabelSuffix()})',
                          style: theme.textTheme.titleMedium,
                        ),
                        const SizedBox(height: 12),
                        Wrap(
                          spacing: 16,
                          runSpacing: 16,
                          children: [
                            if (_isEditing && _selectedUnit == UnitOfMeasureUi.piece)
                              SizedBox(
                                width: isNarrow ? double.infinity : 220,
                                child: TextFormField(
                                  controller: _gramsPerPieceController,
                                  enabled: _isEditing,
                                  decoration: const InputDecoration(
                                    labelText: 'Grams per piece',
                                    helperText: 'Example: 1 egg ≈ 60',
                                  ),
                                  keyboardType: TextInputType.number,
                                  validator: (v) {
                                    if (_selectedUnit != UnitOfMeasureUi.piece) return null;
                                    final t = (v ?? '').trim();
                                    if (t.isEmpty) return 'Required';
                                    final parsed = double.tryParse(t.replaceAll(',', '.'));
                                    if (parsed == null || parsed <= 0) return 'Enter a value > 0';
                                    return null;
                                  },
                                ),
                              ),
                            SizedBox(
                              width: isNarrow ? double.infinity : 180,
                              child: TextFormField(
                                controller: _proteinController,
                                enabled: _isEditing,
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
                            SizedBox(
                              width: isNarrow ? double.infinity : 180,
                              child: TextFormField(
                                controller: _carbsController,
                                enabled: _isEditing,
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
                            SizedBox(
                              width: isNarrow ? double.infinity : 180,
                              child: TextFormField(
                                controller: _fatController,
                                enabled: _isEditing,
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
                            SizedBox(
                              width: isNarrow ? double.infinity : 180,
                              child: TextFormField(
                                controller: _caloriesController,
                                enabled: _isEditing,
                                decoration: InputDecoration(
                                  labelText: 'Calories ${per100LabelSuffix()} *',
                                ),
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
                        const SizedBox(height: 24),
                        Row(
                          children: [
                            Text(
                              'Approved: ',
                              style: theme.textTheme.bodyLarge,
                            ),
                            Icon(
                              g.isApproved ? Icons.check_circle : Icons.cancel,
                              color: g.isApproved ? Colors.green : Colors.red,
                            ),
                          ],
                        ),
                      ],
                    );

                    if (isNarrow) {
                      return Column(
                        crossAxisAlignment: CrossAxisAlignment.stretch,
                        children: [
                          imagePanel,
                          const SizedBox(height: 16),
                          detailsPanel,
                        ],
                      );
                    }

                    return Row(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        imagePanel,
                        const SizedBox(width: 24),
                        Expanded(child: detailsPanel),
                      ],
                    );
                  },
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildImagePreview(Grocery g) {
    if (_newImage != null) {
      return ClipRRect(
        borderRadius: BorderRadius.circular(8),
        child: Image.file(
          File(_newImage!.path),
          fit: BoxFit.cover,
        ),
      );
    }

    if (g.imageUrl != null && g.imageUrl!.isNotEmpty) {
      return SasNetworkImage(
        url: g.imageUrl,
        fit: BoxFit.cover,
        borderRadius: BorderRadius.circular(8),
      );
    }

    return Container(
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(8),
        color: Colors.grey.shade200,
      ),
      child: const Center(
        child: Icon(Icons.image_not_supported, size: 40),
      ),
    );
  }

  String per100LabelSuffix() {
    switch (_selectedUnit) {
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
