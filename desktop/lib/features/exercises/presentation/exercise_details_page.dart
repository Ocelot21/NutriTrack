import 'dart:io';

import 'package:cross_file/cross_file.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:file_picker/file_picker.dart';
import 'package:go_router/go_router.dart';

import 'package:nutritrack_shared/exercises/data/exercise_models.dart';
import 'package:nutritrack_shared/exercises/data/exercise_repo.dart';
import 'exercise_providers.dart';
import '../../../core/widgets/sas_network_image.dart';

class ExerciseDetailsPage extends ConsumerStatefulWidget {
  final String id;

  const ExerciseDetailsPage({super.key, required this.id});

  @override
  ConsumerState<ExerciseDetailsPage> createState() =>
      _ExerciseDetailsPageState();
}

class _ExerciseDetailsPageState extends ConsumerState<ExerciseDetailsPage> {
  Exercise? _exercise;
  bool _isLoading = true;
  String? _error;
  bool _isEditing = false;
  bool _isSaving = false;
  bool _isDeleting = false;

  final _nameController = TextEditingController();
  final _caloriesController = TextEditingController();
  final _descriptionController = TextEditingController();

  ExerciseCategory? _category;
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
    _caloriesController.dispose();
    _descriptionController.dispose();
    super.dispose();
  }

  Future<void> _load() async {
    setState(() {
      _isLoading = true;
      _error = null;
    });

    final repo = ref.read(exerciseRepoProvider);

    try {
      final ex = await repo.getById(widget.id);
      _exercise = ex;

      _nameController.text = ex.name;
      _caloriesController.text = ex.defaultCaloriesPerMinute.toString();
      _descriptionController.text = ex.description ?? '';
      _category = ex.category;
      _newImage = null;

      setState(() {
        _isLoading = false;
      });
    } on ExerciseSearchException catch (e) {
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
    if (_exercise != null) {
      _nameController.text = _exercise!.name;
      _caloriesController.text =
          _exercise!.defaultCaloriesPerMinute.toString();
      _descriptionController.text = _exercise!.description ?? '';
      _category = _exercise!.category;
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

  void _goBack() {
    if (Navigator.of(context).canPop()) {
      context.pop();
      return;
    }
    context.go('/exercises');
  }

  Future<void> _save() async {
    if (!_formKey.currentState!.validate()) return;
    if (_category == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Category is required.')),
      );
      return;
    }

    final calories = double.tryParse(
      _caloriesController.text.replaceAll(',', '.'),
    );

    if (calories == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Invalid calories value.')),
      );
      return;
    }

    setState(() {
      _isSaving = true;
      _error = null;
    });

    final repo = ref.read(exerciseRepoProvider);

    try {
      await repo.updateExercise(
        id: widget.id,
        name: _nameController.text.trim(),
        category: _category!,
        defaultCaloriesPerMinute: calories,
        description: _descriptionController.text.trim().isEmpty
            ? null
            : _descriptionController.text.trim(),
        imageFile: _newImage,
      );

      await _load();
      setState(() {
        _isSaving = false;
        _isEditing = false;
      });

      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Exercise updated.')),
      );
    } on ExerciseSearchException catch (e) {
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
        title: const Text('Delete exercise'),
        content: const Text(
          'Are you sure you want to delete this exercise? '
              'This action cannot be undone.',
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

    final repo = ref.read(exerciseRepoProvider);

    try {
      await repo.deleteExercise(widget.id);
      if (mounted) {
        context.go('/exercises');
      }
    } on ExerciseSearchException catch (e) {
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

  @override
  Widget build(BuildContext context) {
    if (_isLoading && _exercise == null) {
      return const Scaffold(
        body: Center(child: CircularProgressIndicator()),
      );
    }

    if (_error != null && _exercise == null) {
      return Scaffold(
        appBar: AppBar(
          title: const Text('Exercise details'),
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

    final ex = _exercise!;
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: Text('Exercise: ${ex.name}'),
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
                            child: _buildImagePreview(ex),
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
                                  labelText: 'Name',
                                ),
                                validator: (v) => v == null || v.trim().isEmpty
                                    ? 'Required'
                                    : null,
                              ),
                            ),
                            SizedBox(
                              width: isNarrow ? double.infinity : 220,
                              child: DropdownButtonFormField<ExerciseCategory>(
                                initialValue: _category,
                                items: ExerciseCategory.values
                                    .map(
                                      (c) => DropdownMenuItem(
                                        value: c,
                                        child: Text(c.label),
                                      ),
                                    )
                                    .toList(),
                                onChanged: _isEditing
                                    ? (v) => setState(() => _category = v)
                                    : null,
                                decoration: const InputDecoration(
                                  labelText: 'Category',
                                ),
                              ),
                            ),
                            SizedBox(
                              width: isNarrow ? double.infinity : 260,
                              child: TextFormField(
                                controller: _caloriesController,
                                enabled: _isEditing,
                                decoration: const InputDecoration(
                                  labelText: 'Default calories per minute',
                                ),
                                keyboardType: TextInputType.number,
                              ),
                            ),
                          ],
                        ),
                        const SizedBox(height: 24),
                        Text(
                          'Description',
                          style: theme.textTheme.titleMedium,
                        ),
                        const SizedBox(height: 8),
                        TextFormField(
                          controller: _descriptionController,
                          enabled: _isEditing,
                          minLines: 4,
                          maxLines: 10,
                          decoration: const InputDecoration(
                            border: OutlineInputBorder(),
                            hintText: 'Description of the exercise...',
                          ),
                        ),
                        const SizedBox(height: 24),
                        Row(
                          children: [
                            Text(
                              'Approved: ',
                              style: theme.textTheme.bodyLarge,
                            ),
                            Icon(
                              ex.isApproved ? Icons.check_circle : Icons.cancel,
                              color: ex.isApproved ? Colors.green : Colors.red,
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

  Widget _buildImagePreview(Exercise ex) {
    if (_newImage != null) {
      return ClipRRect(
        borderRadius: BorderRadius.circular(8),
        child: Image.file(
          File(_newImage!.path),
          fit: BoxFit.cover,
        ),
      );
    }

    if (ex.imageUrl != null && ex.imageUrl!.isNotEmpty) {
      return SasNetworkImage(
        url: ex.imageUrl,
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
}
