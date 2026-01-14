import 'dart:io';

import 'package:cross_file/cross_file.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:nutritrack_shared/exercises/data/exercise_models.dart';
import 'exercise_providers.dart';
import 'package:file_picker/file_picker.dart';

class CreateExerciseDialog extends ConsumerStatefulWidget {
  const CreateExerciseDialog({super.key});

  @override
  ConsumerState<CreateExerciseDialog> createState() =>
      _CreateExerciseDialogState();
}

class _CreateExerciseDialogState extends ConsumerState<CreateExerciseDialog> {
  final _formKey = GlobalKey<FormState>();

  final _nameController = TextEditingController();
  final _caloriesController = TextEditingController();
  final _descriptionController = TextEditingController();

  ExerciseCategory? _category = ExerciseCategory.uncategorized;
  XFile? _image;

  bool _saving = false;
  String? _error;

  @override
  void dispose() {
    _nameController.dispose();
    _caloriesController.dispose();
    _descriptionController.dispose();
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

    final calories = double.tryParse(
      _caloriesController.text.replaceAll(',', '.'),
    );

    if (calories == null) {
      setState(() => _error = 'Invalid calories per minute.');
      return;
    }

    setState(() {
      _saving = true;
      _error = null;
    });

    final repo = ref.read(exerciseRepoProvider);

    try {
      await repo.createExercise(
        name: _nameController.text.trim(),
        category: _category!,
        defaultCaloriesPerMinute: calories,
        description: _descriptionController.text.trim().isEmpty
            ? null
            : _descriptionController.text.trim(),
        imageFile: _image,
      );

      // reload liste
      ref.read(exerciseSearchControllerProvider.notifier).applyFilters();

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
                    'Create Exercise',
                    style: TextStyle(
                      fontSize: 20,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: 20),

                  Row(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
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
                      Expanded(
                        child: Column(
                          children: [
                            TextFormField(
                              controller: _nameController,
                              decoration:
                              const InputDecoration(labelText: 'Name'),
                              validator: (v) =>
                              v == null || v.trim().isEmpty ? 'Required' : null,
                            ),
                            const SizedBox(height: 12),
                            DropdownButtonFormField<ExerciseCategory>(
                              value: _category,
                              items: ExerciseCategory.values
                                  .map(
                                    (c) => DropdownMenuItem(
                                  value: c,
                                  child: Text(c.label),
                                ),
                              )
                                  .toList(),
                              onChanged: _saving
                                  ? null
                                  : (v) => setState(() => _category = v),
                              decoration:
                              const InputDecoration(labelText: 'Category'),
                            ),
                            const SizedBox(height: 12),
                            TextFormField(
                              controller: _caloriesController,
                              decoration: const InputDecoration(
                                labelText: 'Default calories per minute',
                              ),
                              keyboardType: TextInputType.number,
                            ),
                            const SizedBox(height: 12),
                            TextFormField(
                              controller: _descriptionController,
                              decoration: const InputDecoration(
                                labelText: 'Description (optional)',
                              ),
                              minLines: 2,
                              maxLines: 4,
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
                        onPressed:
                        _saving ? null : () => Navigator.of(context).pop(false),
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
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}
