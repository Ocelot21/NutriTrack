import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../app/router.dart';
import '../data/user_goal_models.dart';
import 'user_goal_providers.dart';
import 'user_goal_share_providers.dart';
import 'user_goal_share_sheet.dart';

class UserGoalPage extends ConsumerStatefulWidget {
  const UserGoalPage({super.key});

  @override
  ConsumerState<UserGoalPage> createState() => _UserGoalPageState();
}

class _UserGoalPageState extends ConsumerState<UserGoalPage> {
  @override
  void initState() {
    super.initState();
    Future.microtask(
          () => ref.read(userGoalControllerProvider.notifier).load(),
    );
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(userGoalControllerProvider);
    final colorScheme = Theme.of(context).colorScheme;
    final textTheme = Theme.of(context).textTheme;

    return Scaffold(
      appBar: AppBar(
      leading: IconButton(
        icon: const Icon(Icons.arrow_back),
        onPressed: () => Navigator.of(context).pop(),
      ),
      title: const Text('Nutrition goal'),
    ),
      body: RefreshIndicator(
        onRefresh: () =>
            ref.read(userGoalControllerProvider.notifier).refresh(),
        child: Builder(
          builder: (context) {
            if (state.isLoading && state.currentGoal == null) {
              return const Center(child: CircularProgressIndicator());
            }

            if (state.error != null && state.currentGoal == null) {
              return ListView(
                physics: const AlwaysScrollableScrollPhysics(),
                children: [
                  const SizedBox(height: 32),
                  Center(
                    child: Padding(
                      padding: const EdgeInsets.symmetric(horizontal: 24),
                      child: Column(
                        children: [
                          Text(
                            'Something went wrong',
                            style: textTheme.titleLarge,
                            textAlign: TextAlign.center,
                          ),
                          const SizedBox(height: 8),
                          Text(
                            state.error!,
                            style: textTheme.bodyMedium,
                            textAlign: TextAlign.center,
                          ),
                          const SizedBox(height: 16),
                          FilledButton(
                            onPressed: () {
                              ref
                                  .read(userGoalControllerProvider.notifier)
                                  .load();
                            },
                            child: const Text('Try again'),
                          ),
                        ],
                      ),
                    ),
                  ),
                ],
              );
            }

            if (state.currentGoal == null) {
              // nema goal-a
              return ListView(
                physics: const AlwaysScrollableScrollPhysics(),
                padding:
                const EdgeInsets.symmetric(horizontal: 16, vertical: 24),
                children: [
                  Card(
                    child: Padding(
                      padding: const EdgeInsets.all(16),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            'No active goal',
                            style: textTheme.titleLarge,
                          ),
                          const SizedBox(height: 8),
                          Text(
                            'You currently don\'t have a nutrition goal in progress. '
                                'Create a goal to track your progress more easily.',
                            style: textTheme.bodyMedium,
                          ),
                          const SizedBox(height: 16),
                          Row(
                            children: [
                              ElevatedButton(
                                onPressed: () => _showCreateGoalDialog(context),
                                child: const Text('Create goal'),
                              ),
                              const SizedBox(width: 12),
                              if (state.isCreatingGoal)
                                const SizedBox(
                                  height: 20,
                                  width: 20,
                                  child: CircularProgressIndicator(strokeWidth: 2),
                                ),
                            ],
                          ),
                          if (state.error != null) ...[
                            const SizedBox(height: 12),
                            Text(
                              state.error!,
                              style: textTheme.bodySmall?.copyWith(
                                color: colorScheme.error,
                              ),
                            ),
                          ],
                        ],
                      ),
                    ),
                  ),
                ],
              );
            }

            final goal = state.currentGoal!;
            final history = state.history;

            return ListView(
              physics: const AlwaysScrollableScrollPhysics(),
              padding:
              const EdgeInsets.symmetric(horizontal: 16, vertical: 24),
              children: [
                // Goal summary
                Card(
                  child: Padding(
                    padding: const EdgeInsets.all(16),
                    child: _GoalSummary(goal: goal),
                  ),
                ),

                const SizedBox(height: 8),

                // Weight chart
                Card(
                  child: Padding(
                    padding: const EdgeInsets.all(16),
                    child: _WeightChartSection(goal: goal, history: history),
                  ),
                ),

                const SizedBox(height: 8),

                // Actions
                _GoalActions(
                  goal: goal,
                  isSavingWeight: state.isSavingWeight,
                  onAddWeight: () => _showAddWeightDialog(context),
                  onViewHistory: () {
                    context.pushNamed(AppRoute.userGoalHistory.name);
                  },
                  canCancel: goal.status == UserGoalStatusUi.inProgress,
                  onCancel: () {
                    ref
                        .read(userGoalControllerProvider.notifier)
                        .cancelCurrentGoal();
                  },
                ),

                if (state.error != null) ...[
                  const SizedBox(height: 12),
                  Text(
                    state.error!,
                    style: textTheme.bodySmall?.copyWith(
                      color: colorScheme.error,
                    ),
                  ),
                ],

                const SizedBox(height: 24),
              ],
            );
          },
        ),
      ),
    );
  }

  Future<void> _showAddWeightDialog(BuildContext context) async {
    final controller = TextEditingController();
    final formKey = GlobalKey<FormState>();

    final result = await showDialog<double?>(
      context: context,
      builder: (context) {
        final textTheme = Theme.of(context).textTheme;
        final colorScheme = Theme.of(context).colorScheme;

        return AlertDialog(
          title: const Text('Update weight'),
          content: Form(
            key: formKey,
            child: TextFormField(
              controller: controller,
              keyboardType:
              const TextInputType.numberWithOptions(decimal: true),
              decoration: const InputDecoration(
                labelText: 'Current weight (kg)',
                hintText: 'e.g. 97.5',
              ),
              validator: (value) {
                if (value == null || value.trim().isEmpty) {
                  return 'Please enter your weight';
                }
                final parsed = double.tryParse(
                    value.replaceAll(',', '.'));
                if (parsed == null || parsed <= 0 || parsed > 300) {
                  return 'Enter a valid weight';
                }
                return null;
              },
            ),
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.of(context).pop(null),
              child: Text(
                'Cancel',
                style: textTheme.labelLarge
                    ?.copyWith(color: colorScheme.onSurfaceVariant),
              ),
            ),
            ElevatedButton(
              onPressed: () {
                if (!formKey.currentState!.validate()) return;
                final value = double.parse(
                    controller.text.replaceAll(',', '.'));
                Navigator.of(context).pop(value);
              },
              child: const Text('Save'),
            ),
          ],
        );
      },
    );

    if (result != null) {
      await ref
          .read(userGoalControllerProvider.notifier)
          .addWeightEntry(weightKg: result);
    }
  }

  Future<void> _showCreateGoalDialog(BuildContext context) async {
    final type = ValueNotifier<NutritionGoalTypeUi>(
      NutritionGoalTypeUi.loseWeight,
    );
    DateTime? targetDate;
    final targetWeightController = TextEditingController();
    final formKey = GlobalKey<FormState>();

    final result = await showDialog<bool>(
      context: context,
      builder: (context) {
        final textTheme = Theme.of(context).textTheme;
        final colorScheme = Theme.of(context).colorScheme;

        return AlertDialog(
          title: const Text('Create goal'),
          content: Form(
            key: formKey,
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                // Goal type
                ValueListenableBuilder<NutritionGoalTypeUi>(
                  valueListenable: type,
                  builder: (context, value, _) {
                    return DropdownButtonFormField<NutritionGoalTypeUi>(
                      initialValue: value,
                      items: NutritionGoalTypeUi.values
                          .map(
                            (t) => DropdownMenuItem(
                          value: t,
                          child: Text(t.label),
                        ),
                      )
                          .toList(),
                      onChanged: (v) {
                        if (v != null) type.value = v;
                      },
                      decoration: const InputDecoration(
                        labelText: 'Goal type',
                      ),
                    );
                  },
                ),
                const SizedBox(height: 12),

                // Target date
                Align(
                  alignment: Alignment.centerLeft,
                  child: TextButton.icon(
                    onPressed: () async {
                      final now = DateTime.now();
                      final picked = await showDatePicker(
                        context: context,
                        initialDate: now.add(const Duration(days: 30)),
                        firstDate: now,
                        lastDate: now.add(const Duration(days: 365)),
                      );
                      if (picked != null) {
                        setState(() {
                          targetDate = picked;
                        });
                      }
                    },
                    icon: const Icon(Icons.calendar_today),
                    label: Text(
                      targetDate == null
                          ? 'Select target date'
                          : 'Target date: ${targetDate!.toLocal().toString().split(' ').first}',
                      style: textTheme.bodyMedium,
                    ),
                  ),
                ),
                const SizedBox(height: 8),

                // Target weight
                TextFormField(
                  controller: targetWeightController,
                  keyboardType:
                  const TextInputType.numberWithOptions(decimal: true),
                  decoration: const InputDecoration(
                    labelText: 'Target weight (kg)',
                    hintText: 'e.g. 92.0',
                  ),
                  validator: (value) {
                    if (value == null || value.trim().isEmpty) {
                      return 'Please enter target weight';
                    }
                    final parsed =
                    double.tryParse(value.replaceAll(',', '.'));
                    if (parsed == null || parsed <= 0 || parsed > 300) {
                      return 'Enter a valid target weight';
                    }
                    return null;
                  },
                ),
              ],
            ),
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.of(context).pop(false),
              child: Text(
                'Cancel',
                style: textTheme.labelLarge
                    ?.copyWith(color: colorScheme.onSurfaceVariant),
              ),
            ),
            ElevatedButton(
              onPressed: () {
                if (targetDate == null) {
                  ScaffoldMessenger.of(context).showSnackBar(
                    const SnackBar(
                      content: Text('Please select a target date'),
                    ),
                  );
                  return;
                }
                if (!formKey.currentState!.validate()) return;
                Navigator.of(context).pop(true);
              },
              child: const Text('Create'),
            ),
          ],
        );
      },
    );

    if (result == true) {
      final weight =
      double.parse(targetWeightController.text.replaceAll(',', '.'));

      await ref.read(userGoalControllerProvider.notifier).createGoal(
        type: type.value,
        targetDate: targetDate!,
        targetWeightKg: weight,
      );
    }
  }
}

class _GoalSummary extends StatelessWidget {
  final UserGoalDto goal;

  const _GoalSummary({required this.goal});

  @override
  Widget build(BuildContext context) {
    final textTheme = Theme.of(context).textTheme;
    final colorScheme = Theme.of(context).colorScheme;

    final statusColor = switch (goal.status) {
      UserGoalStatusUi.completed => Colors.green,
      UserGoalStatusUi.failed => colorScheme.error,
      UserGoalStatusUi.cancelled => colorScheme.outline,
      UserGoalStatusUi.inProgress => colorScheme.primary,
    };

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text('Current goal', style: textTheme.titleLarge),
        const SizedBox(height: 8),
        Row(
          children: [
            Chip(
              label: Text(goal.type.label),
              backgroundColor:
              colorScheme.primaryContainer.withAlpha(77),
            ),
            const SizedBox(width: 8),
            Chip(
              label: Text(goal.status.label),
              backgroundColor: statusColor.withAlpha(38),
              labelStyle: textTheme.bodyMedium?.copyWith(
                color: statusColor,
                fontWeight: FontWeight.w600,
              ),
            ),
          ],
        ),
        const SizedBox(height: 12),
        Row(
          children: [
            Expanded(
              child: _SummaryItem(
                label: 'Start',
                value:
                '${goal.startWeightKg.toStringAsFixed(1)} kg',
              ),
            ),
            Expanded(
              child: _SummaryItem(
                label: 'Target',
                value:
                '${goal.targetWeightKg.toStringAsFixed(1)} kg',
              ),
            ),
          ],
        ),
        const SizedBox(height: 8),
        Row(
          children: [
            Expanded(
              child: _SummaryItem(
                label: 'Start date',
                value:
                goal.startDate.toLocal().toString().split(' ').first,
              ),
            ),
            Expanded(
              child: _SummaryItem(
                label: 'Target date',
                value:
                goal.targetDate.toLocal().toString().split(' ').first,
              ),
            ),
          ],
        ),
      ],
    );
  }
}

class _SummaryItem extends StatelessWidget {
  final String label;
  final String value;

  const _SummaryItem({
    required this.label,
    required this.value,
  });

  @override
  Widget build(BuildContext context) {
    final textTheme = Theme.of(context).textTheme;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(label,
            style:
            textTheme.bodySmall?.copyWith(fontWeight: FontWeight.w500)),
        const SizedBox(height: 4),
        Text(value, style: textTheme.bodyLarge),
      ],
    );
  }
}

class _GoalActions extends ConsumerWidget {
  final UserGoalDto goal;

  final bool isSavingWeight;
  final VoidCallback onAddWeight;
  final VoidCallback onViewHistory;

  final bool canCancel;
  final VoidCallback onCancel;

  const _GoalActions({
    required this.goal,
    required this.isSavingWeight,
    required this.onAddWeight,
    required this.onViewHistory,
    required this.canCancel,
    required this.onCancel,
  });

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final textTheme = Theme.of(context).textTheme;
    final colorScheme = Theme.of(context).colorScheme;

    final isShared = ref.watch(sharedGoalsProvider).contains(goal.id);

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text('Actions', style: textTheme.titleMedium),
        const SizedBox(height: 12),
        Row(
          children: [
            ElevatedButton.icon(
              onPressed: isSavingWeight ? null : onAddWeight,
              icon: const Icon(Icons.monitor_weight),
              label: const Text('Update weight'),
            ),
            const SizedBox(width: 12),
            if (isSavingWeight)
              const SizedBox(
                height: 20,
                width: 20,
                child: CircularProgressIndicator(strokeWidth: 2),
              ),
          ],
        ),

        const SizedBox(height: 8),

        // Share progress
        if (isShared)
          Container(
            width: double.infinity,
            padding: const EdgeInsets.symmetric(vertical: 10, horizontal: 12),
            decoration: BoxDecoration(
              borderRadius: BorderRadius.circular(12),
              color: colorScheme.surfaceContainerHighest,
            ),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Icon(Icons.check_circle, color: colorScheme.primary),
                const SizedBox(width: 8),
                Text(
                  'Shared to Social',
                  style: textTheme.titleSmall?.copyWith(
                    fontWeight: FontWeight.w600,
                  ),
                ),
              ],
            ),
          )
        else
          OutlinedButton.icon(
            onPressed: () {
              showModalBottomSheet(
                context: context,
                isScrollControlled: true,
                showDragHandle: true,
                builder: (_) => GoalProgressShareSheet(goal: goal),
              );
            },
            icon: const Icon(Icons.share),
            label: const Text('Share progress'),
          ),

        const SizedBox(height: 8),

        if (canCancel)
          OutlinedButton.icon(
            onPressed: onCancel,
            icon: const Icon(Icons.cancel_outlined),
            label: const Text('Cancel goal'),
            style: OutlinedButton.styleFrom(
              foregroundColor: colorScheme.error,
            ),
          ),

        const SizedBox(height: 8),

        TextButton(
          onPressed: onViewHistory,
          child: Text(
            'View previous goals',
            style: textTheme.bodyMedium?.copyWith(
              color: colorScheme.primary,
            ),
          ),
        ),
      ],
    );
  }
}


class _WeightChartSection extends StatelessWidget {
  final UserGoalDto goal;
  final List<WeightHistoryEntryDto> history;

  const _WeightChartSection({
    required this.goal,
    required this.history,
  });

  @override
  Widget build(BuildContext context) {
    final textTheme = Theme.of(context).textTheme;

    if (history.length < 2) {
      return Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text('Progress', style: textTheme.titleMedium),
          const SizedBox(height: 8),
          Text(
            'Not enough data to show a chart yet. '
                'Log a few weights to see your progress over time.',
            style: textTheme.bodyMedium,
          ),
        ],
      );
    }

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text('Progress', style: textTheme.titleMedium),
        const SizedBox(height: 12),
        SizedBox(
          height: 220,
          child: _WeightLineChart(
            goal: goal,
            history: history,
          ),
        ),
      ],
    );
  }
}

class _WeightLineChart extends StatelessWidget {
  final UserGoalDto goal;
  final List<WeightHistoryEntryDto> history;

  const _WeightLineChart({
    required this.goal,
    required this.history,
  });

  @override
  Widget build(BuildContext context) {
    final colorScheme = Theme.of(context).colorScheme;

    // sort by date to be safe
    final sorted = [...history]
      ..sort((a, b) => a.date.compareTo(b.date));

    return LayoutBuilder(
      builder: (context, constraints) {
        return CustomPaint(
          painter: _WeightChartPainter(
            goal: goal,
            history: sorted,
            lineColor: colorScheme.primary,
            targetColor: Colors.green,
            axisColor: colorScheme.outlineVariant,
          ),
          size: Size(constraints.maxWidth, constraints.maxHeight),
        );
      },
    );
  }
}

class _WeightChartPainter extends CustomPainter {
  final UserGoalDto goal;
  final List<WeightHistoryEntryDto> history;
  final Color lineColor;
  final Color targetColor;
  final Color axisColor;

  _WeightChartPainter({
    required this.goal,
    required this.history,
    required this.lineColor,
    required this.targetColor,
    required this.axisColor,
  });

  @override
  @override
  void paint(Canvas canvas, Size size) {
    if (history.isEmpty) return;

    const paddingLeft = 32.0;
    const paddingRight = 12.0;
    const paddingTop = 16.0;
    const paddingBottom = 24.0;

    final chartWidth = size.width - paddingLeft - paddingRight;
    final chartHeight = size.height - paddingTop - paddingBottom;

    if (chartWidth <= 0 || chartHeight <= 0) return;

    // min/max weight kao double
    final double minWeight = history
        .map((e) => e.weightKg)
        .followedBy([goal.targetWeightKg])
        .reduce((a, b) => a < b ? a : b);

    final double maxWeight = history
        .map((e) => e.weightKg)
        .followedBy([goal.startWeightKg])
        .reduce((a, b) => a > b ? a : b);

    const double margin = 1.0;

    // bez clamp da izbjegnemo num -> double
    final double yMin =
    (minWeight - margin) <= 0 ? 0.0 : (minWeight - margin);
    final double yMax = maxWeight + margin;
    final double rawRange = yMax - yMin;
    final double yRange = rawRange.abs() < 0.1 ? 1.0 : rawRange;

    // datumi
    final dates = history.map((e) => e.date).toList();
    final startDate = dates.first;
    final endDate = dates.last;

    final int totalDaysInt = endDate.difference(startDate).inDays;
    final double totalDays =
    totalDaysInt <= 0 ? 1.0 : totalDaysInt.toDouble();

    double xForDate(DateTime date) {
      final double days =
      date.difference(startDate).inDays.toDouble();
      final double t = days / totalDays;
      return paddingLeft + t * chartWidth;
    }

    double yForWeight(double weight) {
      final double t = (weight - yMin) / yRange;
      final double inverted = 1 - t;
      return paddingTop + inverted * chartHeight;
    }

    final axisPaint = Paint()
      ..color = axisColor
      ..strokeWidth = 1;

    // y-axis
    canvas.drawLine(
      const Offset(paddingLeft, paddingTop),
      Offset(paddingLeft, paddingTop + chartHeight),
      axisPaint,
    );

    // x-axis
    canvas.drawLine(
      Offset(paddingLeft, paddingTop + chartHeight),
      Offset(paddingLeft + chartWidth, paddingTop + chartHeight),
      axisPaint,
    );

    // target horizontal line
    final double targetY = yForWeight(goal.targetWeightKg);
    final targetPaint = Paint()
      ..color = targetColor.withAlpha(179)
      ..strokeWidth = 1.5
      ..style = PaintingStyle.stroke;

    final dashedPath = Path()
      ..moveTo(paddingLeft, targetY)
      ..lineTo(paddingLeft + chartWidth, targetY);

    _drawDashedLine(
      canvas,
      dashedPath,
      targetPaint,
      dashWidth: 6,
      gapWidth: 4,
    );

    // line kroz history
    final linePaint = Paint()
      ..color = lineColor
      ..strokeWidth = 2
      ..style = PaintingStyle.stroke;

    final path = Path();
    for (var i = 0; i < history.length; i++) {
      final double x = xForDate(history[i].date);
      final double y = yForWeight(history[i].weightKg);

      if (i == 0) {
        path.moveTo(x, y);
      } else {
        path.lineTo(x, y);
      }
    }
    canvas.drawPath(path, linePaint);

    // tačke
    final pointPaint = Paint()
      ..color = lineColor
      ..style = PaintingStyle.fill;

    for (final e in history) {
      final double x = xForDate(e.date);
      final double y = yForWeight(e.weightKg);
      canvas.drawCircle(Offset(x, y), 3.5, pointPaint);
    }

    // y-axis labele
    final textPainter = TextPainter(
      textAlign: TextAlign.right,
      textDirection: TextDirection.ltr,
    );

    void drawLabel(String text, double y) {
      textPainter.text = TextSpan(
        text: text,
        style: const TextStyle(fontSize: 10),
      );
      textPainter.layout(minWidth: 0, maxWidth: paddingLeft - 4);
      textPainter.paint(
        canvas,
        Offset(paddingLeft - textPainter.width - 4, y - 7),
      );
    }

    drawLabel(yMax.toStringAsFixed(1), yForWeight(yMax));
    drawLabel(yMin.toStringAsFixed(1), yForWeight(yMin));
    drawLabel(goal.targetWeightKg.toStringAsFixed(1), targetY);
  }

  void _drawDashedLine(
      Canvas canvas,
      Path path,
      Paint paint, {
        required double dashWidth,
        required double gapWidth,
      }) {
    final metrics = path.computeMetrics();
    for (final metric in metrics) {
      double distance = 0.0;
      while (distance < metric.length) {
        final length = dashWidth;
        final next = distance + length;
        final extractPath =
        metric.extractPath(distance, next.clamp(0, metric.length));
        canvas.drawPath(extractPath, paint);
        distance += dashWidth + gapWidth;
      }
    }
  }

  @override
  bool shouldRepaint(covariant _WeightChartPainter oldDelegate) {
    return oldDelegate.history != history ||
        oldDelegate.goal != goal ||
        oldDelegate.lineColor != lineColor;
  }
}
