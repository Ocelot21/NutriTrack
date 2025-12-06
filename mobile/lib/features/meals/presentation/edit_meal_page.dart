import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../home/data/daily_overview_models.dart';
import 'meals_providers.dart';

class EditMealPage extends ConsumerStatefulWidget {
  final Meal meal;

  const EditMealPage({
    super.key,
    required this.meal,
  });

  @override
  ConsumerState<EditMealPage> createState() => _EditMealPageState();
}

class _EditMealPageState extends ConsumerState<EditMealPage> {
  late TextEditingController _nameCtrl;
  late TextEditingController _descriptionCtrl;
  late TimeOfDay _timeOfDay;
  late DateTime _date;

  @override
  void initState() {
    super.initState();
    final meal = widget.meal;

    _nameCtrl = TextEditingController(text: meal.name);
    _descriptionCtrl = TextEditingController(text: meal.description ?? '');

    final local = meal.occurredAtLocal.toLocal();
    _timeOfDay = TimeOfDay(hour: local.hour, minute: local.minute);
    _date = DateTime(local.year, local.month, local.day);
  }

  @override
  void dispose() {
    _nameCtrl.dispose();
    _descriptionCtrl.dispose();
    super.dispose();
  }

  Future<void> _pickTime() async {
    final picked = await showTimePicker(
      context: context,
      initialTime: _timeOfDay,
    );

    if (picked != null) {
      setState(() {
        _timeOfDay = picked;
      });
    }
  }

  String _formatTimeOfDay(TimeOfDay time) {
    final h = time.hour.toString().padLeft(2, '0');
    final m = time.minute.toString().padLeft(2, '0');
    return '$h:$m';
  }

  Future<void> _onSave() async {
    final controller = ref.read(editMealControllerProvider.notifier);

    final ok = await controller.submit(
      mealId: widget.meal.id,
      name: _nameCtrl.text,
      description: _descriptionCtrl.text,
      date: _date,
      time: _timeOfDay,
    );

    if (!ok) return;
    if (!mounted) return;
    Navigator.of(context).pop(true);

  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final state = ref.watch(editMealControllerProvider);

    return Scaffold(
      appBar: AppBar(
        leading: IconButton(
          icon: const Icon(Icons.arrow_back),
          onPressed: () => Navigator.of(context).pop(false),
        ),
        title: const Text('Edit meal'),
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Center(
          child: ConstrainedBox(
            constraints: const BoxConstraints(maxWidth: 420),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
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
                  controller: _nameCtrl,
                  decoration: const InputDecoration(
                    labelText: 'Meal name',
                  ),
                ),
                const SizedBox(height: 12),
                InkWell(
                  onTap: _pickTime,
                  borderRadius: BorderRadius.circular(12),
                  child: InputDecorator(
                    decoration: const InputDecoration(
                      labelText: 'Time',
                    ),
                    child: Text(
                      _formatTimeOfDay(_timeOfDay),
                      style: theme.textTheme.bodyMedium,
                    ),
                  ),
                ),
                const SizedBox(height: 12),
                TextField(
                  controller: _descriptionCtrl,
                  maxLines: 3,
                  decoration: const InputDecoration(
                    labelText: 'Description (optional)',
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
              ],
            ),
          ),
        ),
      ),
    );
  }
}