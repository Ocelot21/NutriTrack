import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../home/data/daily_overview_models.dart';
import 'edit_exercise_log_providers.dart';

class EditExerciseLogPageArgs {
  final UserExerciseLog log;

  EditExerciseLogPageArgs({required this.log});
}

class EditExerciseLogPage extends ConsumerStatefulWidget {
  final EditExerciseLogPageArgs args;

  const EditExerciseLogPage({
    super.key,
    required this.args,
  });

  @override
  ConsumerState<EditExerciseLogPage> createState() =>
      _EditExerciseLogPageState();
}

class _EditExerciseLogPageState extends ConsumerState<EditExerciseLogPage> {
  late TextEditingController _durationCtrl;
  late TextEditingController _notesCtrl;

  late DateTime _date;
  late TimeOfDay _time;

  @override
  void initState() {
    super.initState();
    final log = widget.args.log;
    final local = log.occurredAtLocal;

    _date = DateTime(local.year, local.month, local.day);
    _time = TimeOfDay(hour: local.hour, minute: local.minute);

    _durationCtrl = TextEditingController(
      text: log.durationMinutes.toString(),
    );
    _notesCtrl = TextEditingController(text: log.notes ?? '');
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
    final d = date.day.toString().padLeft(2, '0');
    final m = date.month.toString().padLeft(2, '0');
    return '$d.$m.${date.year}';
  }

  String _formatTime(TimeOfDay t) {
    final h = t.hour.toString().padLeft(2, '0');
    final m = t.minute.toString().padLeft(2, '0');
    return '$h:$m';
  }

  Future<void> _onSave() async {
    final log = widget.args.log;
    final controller =
    ref.read(editExerciseLogControllerProvider.notifier);

    final durationText = _durationCtrl.text.trim();
    final duration =
        double.tryParse(durationText.replaceAll(',', '.')) ?? 0;

    final occurredAtLocal = DateTime(
      _date.year,
      _date.month,
      _date.day,
      _time.hour,
      _time.minute,
    );

    final ok = await controller.updateLog(
      log: log,
      durationMinutes: duration,
      occurredAtLocal: occurredAtLocal,
      notes: _notesCtrl.text,
    );

    if (!ok) return;
    if (!mounted) return;

    Navigator.of(context).pop(true); // signal da se refresh-a Home
  }

  Future<void> _onDelete() async {
    final log = widget.args.log;
    final controller =
    ref.read(editExerciseLogControllerProvider.notifier);

    final confirm = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Delete log'),
        content: const Text(
          'Are you sure you want to delete this exercise log?',
        ),
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

    if (confirm != true) return;

    final ok = await controller.deleteLog(log);
    if (!ok || !mounted) return;

    Navigator.of(context).pop(true);
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final state = ref.watch(editExerciseLogControllerProvider);
    final log = widget.args.log;

    return Scaffold(
      appBar: AppBar(
        leading: IconButton(
          icon: const Icon(Icons.arrow_back),
          onPressed: () => Navigator.of(context).pop(false),
        ),
        title: const Text('Edit exercise log'),
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Center(
          child: ConstrainedBox(
            constraints: const BoxConstraints(maxWidth: 480),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                _LogHeader(log: log),
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
                        : const Text('Save changes'),
                  ),
                ),
                const SizedBox(height: 12),
                SizedBox(
                  height: 44,
                  child: OutlinedButton.icon(
                    icon: const Icon(Icons.delete),
                    style: OutlinedButton.styleFrom(
                      foregroundColor: theme.colorScheme.error,
                      side: BorderSide(color: theme.colorScheme.error),
                    ),
                    onPressed: state.isSubmitting ? null : _onDelete,
                    label: const Text('Delete log'),
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

class _LogHeader extends StatelessWidget {
  final UserExerciseLog log;

  const _LogHeader({required this.log});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          log.exerciseName,
          style: theme.textTheme.titleLarge
              ?.copyWith(fontWeight: FontWeight.bold),
        ),
        const SizedBox(height: 4),
        Text(
          log.category,
          style: theme.textTheme.bodyMedium,
        ),
        const SizedBox(height: 4),
        Text(
          '${log.totalCalories.toStringAsFixed(0)} kcal total',
          style: theme.textTheme.bodyMedium?.copyWith(
            color: theme.colorScheme.primary,
          ),
        ),
      ],
    );
  }
}
