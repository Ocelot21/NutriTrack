import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:nutritrack_shared/exercises/data/exercise_models.dart';
import 'exercise_log_providers.dart';

class ExerciseLogPageArgs {
  final Exercise exercise;
  final DateTime initialDate;

  ExerciseLogPageArgs({
    required this.exercise,
    required this.initialDate,
  });
}

class ExerciseLogPage extends ConsumerStatefulWidget {
  final ExerciseLogPageArgs args;

  const ExerciseLogPage({
    super.key,
    required this.args,
  });

  @override
  ConsumerState<ExerciseLogPage> createState() => _ExerciseLogPageState();
}

class _ExerciseLogPageState extends ConsumerState<ExerciseLogPage> {
  late TextEditingController _durationCtrl;
  late TextEditingController _notesCtrl;

  late DateTime _date;
  late TimeOfDay _time;

  @override
  void initState() {
    super.initState();
    final initial = widget.args.initialDate;
    _date = DateTime(initial.year, initial.month, initial.day);
    _time = TimeOfDay(hour: initial.hour, minute: initial.minute);

    // možeš prepopuniti default trajanje, npr. 30min
    _durationCtrl = TextEditingController(text: '');
    _notesCtrl = TextEditingController();
  }

  @override
  void dispose() {
    _durationCtrl.dispose();
    _notesCtrl.dispose();
    super.dispose();
  }

  Future<void> _pickDate() async {
    final picked = await showDatePicker(
      context: context,
      initialDate: _date,
      firstDate: DateTime(2000),
      lastDate: DateTime(2100),
    );
    if (picked != null) {
      setState(() {
        _date = picked;
      });
    }
  }

  Future<void> _pickTime() async {
    final picked = await showTimePicker(
      context: context,
      initialTime: _time,
    );
    if (picked != null) {
      setState(() {
        _time = picked;
      });
    }
  }

  String _formatDate(DateTime date) {
    final day = date.day.toString().padLeft(2, '0');
    final month = date.month.toString().padLeft(2, '0');
    return '$day.$month.${date.year}';
  }

  String _formatTime(TimeOfDay time) {
    final h = time.hour.toString().padLeft(2, '0');
    final m = time.minute.toString().padLeft(2, '0');
    return '$h:$m';
  }

  Future<void> _onSave() async {
    final exercise = widget.args.exercise;
    final controller = ref.read(exerciseLogControllerProvider.notifier);

    final durationText = _durationCtrl.text.trim();
    final duration = double.tryParse(durationText.replaceAll(',', '.')) ?? 0;

    final occurredAtLocal = DateTime(
      _date.year,
      _date.month,
      _date.day,
      _time.hour,
      _time.minute,
    );

    final ok = await controller.submit(
      exercise: exercise,
      durationMinutes: duration,
      occurredAtLocal: occurredAtLocal,
      notes: _notesCtrl.text,
    );

    if (!ok) return;
    if (!mounted) return;
    Navigator.of(context).pop(true);
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final state = ref.watch(exerciseLogControllerProvider);
    final exercise = widget.args.exercise;

    return Scaffold(
      appBar: AppBar(
        leading: IconButton(
          icon: const Icon(Icons.arrow_back),
          onPressed: () => Navigator.of(context).pop(false),
        ),
        title: const Text('Log exercise'),
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Center(
          child: ConstrainedBox(
            constraints: const BoxConstraints(maxWidth: 480),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                _ExerciseHeader(exercise: exercise),
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
                TextField(
                  controller: _durationCtrl,
                  keyboardType:
                  const TextInputType.numberWithOptions(decimal: true),
                  decoration: const InputDecoration(
                    labelText: 'Duration (minutes)',
                    hintText: 'e.g. 30',
                  ),
                ),
                const SizedBox(height: 12),
                Row(
                  children: [
                    Expanded(
                      child: InkWell(
                        onTap: _pickDate,
                        borderRadius: BorderRadius.circular(12),
                        child: InputDecorator(
                          decoration: const InputDecoration(
                            labelText: 'Date',
                          ),
                          child: Text(_formatDate(_date)),
                        ),
                      ),
                    ),
                    const SizedBox(width: 12),
                    Expanded(
                      child: InkWell(
                        onTap: _pickTime,
                        borderRadius: BorderRadius.circular(12),
                        child: InputDecorator(
                          decoration: const InputDecoration(
                            labelText: 'Time',
                          ),
                          child: Text(_formatTime(_time)),
                        ),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 12),
                TextField(
                  controller: _notesCtrl,
                  maxLines: 3,
                  decoration: const InputDecoration(
                    labelText: 'Notes (optional)',
                  ),
                ),
                const SizedBox(height: 24),
                SizedBox(
                  height: 48,
                  child: FilledButton(
                    onPressed: state.isSubmitting ? null : _onSave,
                    child: state.isSubmitting
                        ? const SizedBox(
                      height: 20,
                      width: 20,
                      child: CircularProgressIndicator(strokeWidth: 2),
                    )
                        : const Text('Save'),
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

class _ExerciseHeader extends StatelessWidget {
  final Exercise exercise;

  const _ExerciseHeader({required this.exercise});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    final imageUrl = exercise.imageUrl;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        // slika / placeholder – ograničena visina da ne pojede ekran
        ClipRRect(
          borderRadius: BorderRadius.circular(16),
          child: SizedBox(
            height: 180,
            child: imageUrl != null && imageUrl.isNotEmpty
                ? Image.network(
              imageUrl,
              fit: BoxFit.cover,
              errorBuilder: (_, __, ___) => _placeholderImage(theme),
            )
                : _placeholderImage(theme),
          ),
        ),
        const SizedBox(height: 12),
        Text(
          exercise.name,
          style: theme.textTheme.titleLarge
              ?.copyWith(fontWeight: FontWeight.bold),
        ),
        const SizedBox(height: 4),
        Row(
          children: [
            Text(
              exercise.category.label,
              style: theme.textTheme.bodyMedium,
            ),
            const SizedBox(width: 8),
            const Text('·'),
            const SizedBox(width: 8),
            Text(
              '${exercise.defaultCaloriesPerMinute.toStringAsFixed(1)} kcal/min',
              style: theme.textTheme.bodyMedium?.copyWith(
                color: theme.colorScheme.primary,
              ),
            ),
            if (exercise.isApproved) ...[
              const SizedBox(width: 8),
              Icon(
                Icons.verified,
                color: theme.colorScheme.primary,
                size: 18,
              ),
            ],
          ],
        ),
        if (exercise.description != null &&
            exercise.description!.trim().isNotEmpty) ...[
          const SizedBox(height: 8),
          Text(
            exercise.description!,
            style: theme.textTheme.bodyMedium,
          ),
        ],
      ],
    );
  }

  Widget _placeholderImage(ThemeData theme) {
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
          Icons.fitness_center,
          size: 48,
          color: theme.colorScheme.onPrimaryContainer,
        ),
      ),
    );
  }
}

