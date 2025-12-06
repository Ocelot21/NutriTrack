import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../domain/health_enums.dart';
import 'health_profile_providers.dart';

class HealthProfilePage extends ConsumerWidget {
  const HealthProfilePage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    ref.listen(
      healthProfileControllerProvider,
          (previous, next) {
        final wasSubmitted = previous?.submitted ?? false;
        final isSubmittedNow = next.submitted;

        if (!wasSubmitted && isSubmittedNow) {
          context.go('/home');
        }
      },
    );

    final theme = Theme.of(context);
    final state = ref.watch(healthProfileControllerProvider);
    final controller = ref.read(healthProfileControllerProvider.notifier);

    final heightCtrl = TextEditingController(
      text: state.heightCm?.toString() ?? '',
    );
    final weightCtrl = TextEditingController(
      text: state.weightKg?.toString() ?? '',
    );

    void updateHeightFromText() {
      final text = heightCtrl.text.trim();
      final value = int.tryParse(text);
      controller.updateHeight(value);
    }

    void updateWeightFromText() {
      final text = weightCtrl.text.trim().replaceAll(',', '.');
      final value = double.tryParse(text);
      controller.updateWeight(value);
    }

    Future<void> pickBirthdate() async {
      final now = DateTime.now();
      final initial = state.birthdate ??
          DateTime(now.year - 20, now.month, now.day);
      final first = DateTime(now.year - 100);
      final last = now;

      final picked = await showDatePicker(
        context: context,
        initialDate: initial,
        firstDate: first,
        lastDate: last,
      );

      if (picked != null) {
        final normalized = DateTime(picked.year, picked.month, picked.day);
        controller.updateBirthdate(normalized);
      }
    }

    final birthdateText = state.birthdate != null
        ? '${state.birthdate!.year}-${state.birthdate!.month.toString().padLeft(2, '0')}-${state.birthdate!.day.toString().padLeft(2, '0')}'
        : 'Select birthdate';


    return Scaffold(
      appBar: AppBar(
        title: const Text('Health profile'),
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Center(
          child: ConstrainedBox(
            constraints: const BoxConstraints(maxWidth: 420),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                Text(
                  'Set up your health profile',
                  style: theme.textTheme.headlineMedium,
                ),
                const SizedBox(height: 8),
                Text(
                  'This helps NutriTrack calculate your needs more accurately.',
                  style: theme.textTheme.bodyMedium,
                ),
                const SizedBox(height: 24),
                if (state.error != null) ...[
                  Text(
                    state.error!,
                    style: theme.textTheme.bodyMedium?.copyWith(
                      color: theme.colorScheme.error,
                    ),
                  ),
                  const SizedBox(height: 12),
                ],
                if (state.submitted) ...[
                  Text(
                    'Health profile saved successfully.',
                    style: theme.textTheme.bodyMedium?.copyWith(
                      color: theme.colorScheme.primary,
                    ),
                  ),
                  const SizedBox(height: 12),
                ],
                // Gender
                DropdownButtonFormField<EnumOption>(
                  value: state.gender,
                  items: GenderOptions.values
                      .map(
                        (e) => DropdownMenuItem(
                      value: e,
                      child: Text(e.label),
                    ),
                  )
                      .toList(),
                  onChanged: (value) {
                    if (value != null) {
                      controller.updateGender(value);
                    }
                  },
                  decoration: const InputDecoration(
                    labelText: 'Gender',
                  ),
                ),
                const SizedBox(height: 12),
                // Birthdate
                InkWell(
                  onTap: pickBirthdate,
                  borderRadius: BorderRadius.circular(12),
                  child: InputDecorator(
                    decoration: const InputDecoration(
                      labelText: 'Birthdate',
                    ),
                    child: Text(
                      birthdateText,
                      style: theme.textTheme.bodyMedium?.copyWith(
                        color: state.birthdate == null
                            ? theme.hintColor
                            : theme.textTheme.bodyMedium?.color,
                      ),
                    ),
                  ),
                ),
                const SizedBox(height: 12),
                // Height
                TextField(
                  controller: heightCtrl,
                  keyboardType: TextInputType.number,
                  decoration: const InputDecoration(
                    labelText: 'Height (cm)',
                    hintText: 'e.g. 185',
                  ),
                  onChanged: (_) => updateHeightFromText(),
                ),
                const SizedBox(height: 12),
                // Weight
                TextField(
                  controller: weightCtrl,
                  keyboardType:
                  const TextInputType.numberWithOptions(decimal: true),
                  decoration: const InputDecoration(
                    labelText: 'Weight (kg)',
                    hintText: 'e.g. 100',
                  ),
                  onChanged: (_) => updateWeightFromText(),
                ),
                const SizedBox(height: 12),
                // Activity level
                DropdownButtonFormField<EnumOption>(
                  value: state.activityLevel,
                  items: ActivityLevelOptions.values
                      .map(
                        (e) => DropdownMenuItem(
                      value: e,
                      child: Text(e.label),
                    ),
                  )
                      .toList(),
                  onChanged: (value) {
                    if (value != null) {
                      controller.updateActivityLevel(value);
                    }
                  },
                  decoration: const InputDecoration(
                    labelText: 'Activity level',
                  ),
                ),
                const SizedBox(height: 12),
                // Nutrition goal
                DropdownButtonFormField<EnumOption>(
                  value: state.nutritionGoal,
                  items: NutritionGoalOptions.values
                      .map(
                        (e) => DropdownMenuItem(
                      value: e,
                      child: Text(e.label),
                    ),
                  )
                      .toList(),
                  onChanged: (value) {
                    if (value != null) {
                      controller.updateNutritionGoal(value);
                    }
                  },
                  decoration: const InputDecoration(
                    labelText: 'Nutrition goal',
                  ),
                ),
                const SizedBox(height: 24),
                SizedBox(
                  height: 48,
                  child: FilledButton(
                    onPressed:
                    state.isSubmitting ? null : () => controller.submit(),
                    child: state.isSubmitting
                        ? const SizedBox(
                      height: 20,
                      width: 20,
                      child: CircularProgressIndicator(strokeWidth: 2),
                    )
                        : const Text('Save profile'),
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
