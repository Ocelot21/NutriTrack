import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:image_picker/image_picker.dart';

import '../data/exercise_models.dart';
import '../data/exercise_repo.dart';
import '../../../core/providers.dart';
import '../../../core/token_store.dart';

final exerciseRepoProvider = Provider<ExerciseRepo>((ref) {
  final api = ref.read(apiClientProvider);
  final store = ref.read(tokenStoreProvider);
  return ExerciseRepo(api, store);
});

class ExerciseSuggestPage extends ConsumerStatefulWidget {
  const ExerciseSuggestPage({super.key});

  @override
  ConsumerState<ExerciseSuggestPage> createState() =>
      _ExerciseSuggestPageState();
}

class _ExerciseSuggestPageState extends ConsumerState<ExerciseSuggestPage> {
  final _formKey = GlobalKey<FormState>();

  final _nameCtrl = TextEditingController();
  final _caloriesCtrl = TextEditingController();
  final _descriptionCtrl = TextEditingController();

  ExerciseCategory _selectedCategory = ExerciseCategory.uncategorized;
  XFile? _imageFile;
  bool _isSubmitting = false;
  String? _error;

  @override
  void dispose() {
    _nameCtrl.dispose();
    _caloriesCtrl.dispose();
    _descriptionCtrl.dispose();
    super.dispose();
  }

  Future<void> _pickImage() async {
    final picker = ImagePicker();
    final picked = await picker.pickImage(source: ImageSource.gallery);
    if (picked != null) {
      setState(() {
        _imageFile = picked;
      });
    }
  }

  Future<void> _submit() async {
    final form = _formKey.currentState;
    if (form == null) return;

    if (!form.validate()) return;

    final caloriesText = _caloriesCtrl.text.trim();
    final calories =
        double.tryParse(caloriesText.replaceAll(',', '.')) ?? 0;

    if (calories <= 0) {
      setState(() {
        _error = 'Default calories per minute must be greater than 0.';
      });
      return;
    }

    setState(() {
      _isSubmitting = true;
      _error = null;
    });

    final repo = ref.read(exerciseRepoProvider);

    try {
      await repo.createExercise(
        name: _nameCtrl.text.trim(),
        category: _selectedCategory,
        defaultCaloriesPerMinute: calories,
        description: _descriptionCtrl.text.trim().isEmpty
            ? null
            : _descriptionCtrl.text.trim(),
        imageFile: _imageFile,
      );

      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Exercise suggestion submitted.'),
        ),
      );
      Navigator.of(context).pop();
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _isSubmitting = false;
        _error = e.toString();
      });
    } finally {
      if (mounted) {
        setState(() {
          _isSubmitting = false;
        });
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Suggest an exercise'),
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
                      'Help us improve NutriTrack by suggesting an exercise.',
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
                        labelText: 'Exercise name',
                      ),
                      validator: (value) {
                        if (value == null || value.trim().isEmpty) {
                          return 'Name is required.';
                        }
                        return null;
                      },
                    ),
                    const SizedBox(height: 12),
                    DropdownButtonFormField<ExerciseCategory>(
                      value: _selectedCategory,
                      decoration: const InputDecoration(
                        labelText: 'Category',
                      ),
                      items: ExerciseCategory.values
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
                    TextFormField(
                      controller: _caloriesCtrl,
                      keyboardType:
                      const TextInputType.numberWithOptions(decimal: true),
                      decoration: const InputDecoration(
                        labelText: 'Default calories per minute',
                      ),
                      validator: (value) {
                        if (value == null || value.trim().isEmpty) {
                          return 'Default calories per minute is required.';
                        }
                        final parsed = double.tryParse(
                            value.trim().replaceAll(',', '.'));
                        if (parsed == null || parsed <= 0) {
                          return 'Enter a valid positive number.';
                        }
                        return null;
                      },
                    ),
                    const SizedBox(height: 12),
                    TextFormField(
                      controller: _descriptionCtrl,
                      maxLines: 3,
                      decoration: const InputDecoration(
                        labelText: 'Description (optional)',
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
