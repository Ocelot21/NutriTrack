import 'package:flutter/material.dart';

import '../../data/social_models.dart';
import '../../../achievements/data/user_achievement_models.dart';

class SocialPostCard extends StatelessWidget {
  final SocialPostModel post;
  final VoidCallback? onAuthorTap;
  final VoidCallback? onDelete;

  const SocialPostCard({
    super.key,
    required this.post,
    this.onAuthorTap,
    this.onDelete,
  });

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Card(
      clipBehavior: Clip.antiAlias,
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            _Header(
              author: post.author,
              localTime: post.localTime,
              onTap: onAuthorTap,
              onDelete: onDelete,
            ),
            const SizedBox(height: 10),
            if (post.text != null && post.text!.trim().isNotEmpty)
              Text(
                post.text!,
                style: theme.textTheme.bodyLarge,
              ),
            if (post.type == SocialPostTypeUi.achievementShare &&
                post.userAchievement != null) ...[
              const SizedBox(height: 10),
              _AchievementBlock(ua: post.userAchievement!),
            ],
            if (post.type == SocialPostTypeUi.dailyOverviewShare &&
                post.dailyOverviewSnapshot != null) ...[
              const SizedBox(height: 10),
              _DailyOverviewSnapshotBlock(snapshot: post.dailyOverviewSnapshot!),
            ],
            if (post.type == SocialPostTypeUi.goalProgressShare &&
                post.goalProgressSnapshot != null) ...[
              const SizedBox(height: 10),
              _GoalProgressSnapshotBlock(snapshot: post.goalProgressSnapshot!),
            ],
          ],
        ),
      ),
    );
  }
}

class _Header extends StatelessWidget {
  final SocialPostAuthorModel author;
  final DateTime localTime;
  final VoidCallback? onTap;
  final VoidCallback? onDelete;

  const _Header({
    required this.author,
    required this.localTime,
    this.onTap,
    this.onDelete,
  });

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Row(
      children: [
        GestureDetector(
          onTap: onTap,
          child: CircleAvatar(
            radius: 20,
            backgroundImage: (author.avatarUrl != null && author.avatarUrl!.isNotEmpty)
                ? NetworkImage(author.avatarUrl!)
                : null,
            child: (author.avatarUrl == null || author.avatarUrl!.isEmpty)
                ? const Icon(Icons.person, size: 20)
                : null,
          ),
        ),
        const SizedBox(width: 10),
        Expanded(
          child: GestureDetector(
            onTap: onTap,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  author.username,
                  style: theme.textTheme.titleSmall?.copyWith(fontWeight: FontWeight.w600),
                ),
                const SizedBox(height: 2),
                Text(
                  _formatLocalTime(localTime),
                  style: theme.textTheme.bodySmall,
                ),
              ],
            ),
          ),
        ),
        if (onDelete != null)
          IconButton(
            tooltip: 'Delete',
            onPressed: onDelete,
            icon: const Icon(Icons.more_vert),
          ),
      ],
    );
  }

  static String _formatLocalTime(DateTime dt) {
    final two = (int v) => v.toString().padLeft(2, '0');
    return '${two(dt.day)}.${two(dt.month)}.${dt.year} ${two(dt.hour)}:${two(dt.minute)}';
  }
}

class _AchievementBlock extends StatelessWidget {
  final UserAchievementModel ua;

  const _AchievementBlock({required this.ua});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final icon = achievementIconForName(ua.iconName);

    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: theme.dividerColor),
      ),
      child: Row(
        children: [
          Icon(icon, size: 28),
          const SizedBox(width: 10),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  ua.title,
                  style: theme.textTheme.titleSmall?.copyWith(fontWeight: FontWeight.w600),
                ),
                const SizedBox(height: 2),
                Text(
                  ua.description,
                  style: theme.textTheme.bodySmall,
                ),
                const SizedBox(height: 6),
                Wrap(
                  spacing: 8,
                  runSpacing: 6,
                  children: [
                    _Chip(text: ua.category.label),
                    _Chip(text: '+${ua.points} pts'),
                  ],
                )
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _DailyOverviewSnapshotBlock extends StatelessWidget {
  final DailyOverviewSnapshotModel snapshot;

  const _DailyOverviewSnapshotBlock({required this.snapshot});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final textTheme = theme.textTheme;
    final colorScheme = theme.colorScheme;

    String fmtDate(DateTime dt) {
      final two = (int v) => v.toString().padLeft(2, '0');
      return '${two(dt.day)}.${two(dt.month)}.${dt.year}';
    }

    double ratio(double consumed, double target) {
      if (target <= 0) return 0;
      final r = consumed / target;
      if (r < 0) return 0;
      if (r > 1) return 1;
      return r;
    }

    final calProgress = ratio(snapshot.consumedCalories.toDouble(), snapshot.targetCalories.toDouble());

    final pProgress = ratio(snapshot.consumedProteinGrams, snapshot.targetProteinGrams);
    final cProgress = ratio(snapshot.consumedCarbohydrateGrams, snapshot.targetCarbohydrateGrams);
    final fProgress = ratio(snapshot.consumedFatGrams, snapshot.targetFatGrams);

    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: theme.dividerColor),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Expanded(
                child: Text(
                  'Daily overview',
                  style: textTheme.titleSmall?.copyWith(fontWeight: FontWeight.w600),
                ),
              ),
              Text(
                fmtDate(snapshot.date),
                style: textTheme.bodySmall?.copyWith(color: colorScheme.onSurfaceVariant),
              ),
            ],
          ),
          const SizedBox(height: 10),

          // Calories summary
          Text(
            'Calories',
            style: textTheme.bodySmall?.copyWith(
              fontWeight: FontWeight.w700,
              color: colorScheme.onSurfaceVariant,
            ),
          ),
          const SizedBox(height: 6),
          ClipRRect(
            borderRadius: BorderRadius.circular(999),
            child: LinearProgressIndicator(
              value: calProgress,
              minHeight: 8,
            ),
          ),
          const SizedBox(height: 6),
          Wrap(
            spacing: 10,
            runSpacing: 8,
            children: [
              _Chip(text: '${snapshot.consumedCalories}/${snapshot.targetCalories} kcal'),
              _Chip(text: 'Burned ${snapshot.burnedCalories}'),
              _Chip(text: 'Net ${snapshot.netCalories}'),
              _Chip(text: 'Remaining ${snapshot.remainingCalories}'),
            ],
          ),

          const SizedBox(height: 12),

          // Macros
          Text(
            'Macros',
            style: textTheme.bodySmall?.copyWith(
              fontWeight: FontWeight.w700,
              color: colorScheme.onSurfaceVariant,
            ),
          ),
          const SizedBox(height: 8),

          _MacroRow(
            label: 'Protein',
            consumed: snapshot.consumedProteinGrams,
            target: snapshot.targetProteinGrams,
            progress: pProgress,
          ),
          const SizedBox(height: 8),
          _MacroRow(
            label: 'Carbs',
            consumed: snapshot.consumedCarbohydrateGrams,
            target: snapshot.targetCarbohydrateGrams,
            progress: cProgress,
          ),
          const SizedBox(height: 8),
          _MacroRow(
            label: 'Fats',
            consumed: snapshot.consumedFatGrams,
            target: snapshot.targetFatGrams,
            progress: fProgress,
          ),

          const SizedBox(height: 12),

          Wrap(
            spacing: 10,
            runSpacing: 8,
            children: [
              _Chip(text: 'Meals ${snapshot.mealCount}'),
              _Chip(text: 'Exercises ${snapshot.exerciseCount}'),
            ],
          ),
        ],
      ),
    );
  }
}

class _MacroRow extends StatelessWidget {
  final String label;
  final double consumed;
  final double target;
  final double progress;

  const _MacroRow({
    required this.label,
    required this.consumed,
    required this.target,
    required this.progress,
  });

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final textTheme = theme.textTheme;
    final colorScheme = theme.colorScheme;

    String fmtNum(num v) {
      final s = v.toStringAsFixed(1);
      return s.endsWith('.0') ? s.substring(0, s.length - 2) : s;
    }

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Row(
          children: [
            Expanded(
              child: Text(
                label,
                style: textTheme.bodySmall?.copyWith(fontWeight: FontWeight.w600),
              ),
            ),
            Text(
              '${fmtNum(consumed)}/${fmtNum(target)} g',
              style: textTheme.bodySmall?.copyWith(color: colorScheme.onSurfaceVariant),
            ),
          ],
        ),
        const SizedBox(height: 4),
        ClipRRect(
          borderRadius: BorderRadius.circular(999),
          child: LinearProgressIndicator(
            value: progress,
            minHeight: 6,
          ),
        ),
      ],
    );
  }
}

class _GoalProgressSnapshotBlock extends StatelessWidget {
  final GoalProgressSnapshotModel snapshot;

  const _GoalProgressSnapshotBlock({required this.snapshot});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final textTheme = theme.textTheme;
    final colorScheme = theme.colorScheme;

    final sorted = [...snapshot.points]..sort((a, b) => a.date.compareTo(b.date));
    final hasEnoughPoints = sorted.length >= 2;

    final DateTime? fromDate = sorted.isNotEmpty ? sorted.first.date : null;
    final DateTime? toDate = sorted.isNotEmpty ? sorted.last.date : null;

    final double distanceTotal = (snapshot.targetWeightKg - snapshot.startWeightKg).abs();
    final double distanceDone = (snapshot.currentWeightKg - snapshot.startWeightKg).abs();
    final double progress = distanceTotal <= 0.0001
        ? 0.0
        : (distanceDone / distanceTotal).clamp(0.0, 1.0);

    final bool isLossGoal = snapshot.targetWeightKg < snapshot.startWeightKg;
    final double remainingKg = (snapshot.targetWeightKg - snapshot.currentWeightKg).abs();

    String fmtDate(DateTime dt) {
      final two = (int v) => v.toString().padLeft(2, '0');
      return '${two(dt.day)}.${two(dt.month)}.${dt.year}';
    }

    String fmtKg(double v) => '${v.toStringAsFixed(1)} kg';

    String fmtProgressPct(double v) => '${(v * 100).toStringAsFixed(0)}%';

    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: theme.dividerColor),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Expanded(
                child: Text(
                  'Goal progress',
                  style: textTheme.titleSmall?.copyWith(
                    fontWeight: FontWeight.w600,
                  ),
                ),
              ),
              Text(
                fmtDate(snapshot.snapshotDate),
                style: textTheme.bodySmall?.copyWith(
                  color: colorScheme.onSurfaceVariant,
                ),
              ),
            ],
          ),
          if (snapshot.goalType.trim().isNotEmpty) ...[
            const SizedBox(height: 2),
            Text(
              snapshot.goalType,
              style: textTheme.bodySmall?.copyWith(
                color: colorScheme.onSurfaceVariant,
                fontWeight: FontWeight.w500,
              ),
            ),
          ],
          const SizedBox(height: 10),

          // compact summary
          Wrap(
            spacing: 10,
            runSpacing: 8,
            children: [
              _Chip(text: 'Start ${fmtKg(snapshot.startWeightKg)}'),
              _Chip(text: 'Current ${fmtKg(snapshot.currentWeightKg)}'),
              _Chip(text: 'Target ${fmtKg(snapshot.targetWeightKg)}'),
            ],
          ),

          const SizedBox(height: 10),

          // progress line
          Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Expanded(
                    child: Text(
                      isLossGoal
                          ? 'Progress towards loss'
                          : 'Progress towards gain',
                      style: textTheme.bodySmall?.copyWith(
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                  ),
                  Text(
                    fmtProgressPct(progress),
                    style: textTheme.bodySmall?.copyWith(
                      fontWeight: FontWeight.w700,
                      color: colorScheme.primary,
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 6),
              ClipRRect(
                borderRadius: BorderRadius.circular(999),
                child: LinearProgressIndicator(
                  value: progress.isNaN ? 0.0 : progress,
                  minHeight: 8,
                ),
              ),
              const SizedBox(height: 6),
              Text(
                remainingKg <= 0.05
                    ? 'At target.'
                    : '${remainingKg.toStringAsFixed(1)} kg to target',
                style: textTheme.bodySmall?.copyWith(
                  color: colorScheme.onSurfaceVariant,
                ),
              ),
            ],
          ),

          const SizedBox(height: 10),

          if (!hasEnoughPoints)
            Text(
              'Not enough data to show a chart yet.',
              style: textTheme.bodySmall,
            )
          else ...[
            Row(
              children: [
                Expanded(
                  child: Text(
                    (fromDate != null && toDate != null)
                        ? '${fmtDate(fromDate)} → ${fmtDate(toDate)}'
                        : 'Progress over time',
                    style: textTheme.bodySmall?.copyWith(
                      color: colorScheme.onSurfaceVariant,
                    ),
                  ),
                ),
                Text(
                  '${sorted.length} pts',
                  style: textTheme.bodySmall?.copyWith(
                    color: colorScheme.onSurfaceVariant,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 8),
            SizedBox(
              height: 180,
              child: _GoalProgressMiniChart(snapshot: snapshot),
            ),
          ],
        ],
      ),
    );
  }
}

class _GoalProgressMiniChart extends StatelessWidget {
  final GoalProgressSnapshotModel snapshot;

  const _GoalProgressMiniChart({required this.snapshot});

  @override
  Widget build(BuildContext context) {
    final colorScheme = Theme.of(context).colorScheme;

    final sorted = [...snapshot.points]
      ..sort((a, b) => a.date.compareTo(b.date));

    return LayoutBuilder(
      builder: (context, constraints) {
        return CustomPaint(
          painter: _GoalProgressChartPainter(
            points: sorted,
            startWeightKg: snapshot.startWeightKg,
            currentWeightKg: snapshot.currentWeightKg,
            targetWeightKg: snapshot.targetWeightKg,
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

class _GoalProgressChartPainter extends CustomPainter {
  final List<GoalProgressSnapshotPointModel> points;
  final double startWeightKg;
  final double currentWeightKg;
  final double targetWeightKg;
  final Color lineColor;
  final Color targetColor;
  final Color axisColor;

  _GoalProgressChartPainter({
    required this.points,
    required this.startWeightKg,
    required this.currentWeightKg,
    required this.targetWeightKg,
    required this.lineColor,
    required this.targetColor,
    required this.axisColor,
  });

  @override
  void paint(Canvas canvas, Size size) {
    if (points.isEmpty) return;

    const paddingLeft = 28.0;
    const paddingRight = 8.0;
    const paddingTop = 12.0;
    const paddingBottom = 20.0;

    final chartWidth = size.width - paddingLeft - paddingRight;
    final chartHeight = size.height - paddingTop - paddingBottom;

    if (chartWidth <= 0 || chartHeight <= 0) return;

    final values = points.map((e) => e.weightKg).toList();

    final double minWeight = values
        .followedBy([targetWeightKg])
        .reduce((a, b) => a < b ? a : b);

    final double maxWeight = values
        .followedBy([startWeightKg])
        .reduce((a, b) => a > b ? a : b);

    const double margin = 0.8;

    final double yMin = (minWeight - margin) <= 0 ? 0.0 : (minWeight - margin);
    final double yMax = maxWeight + margin;
    final double rawRange = yMax - yMin;
    final double yRange = rawRange.abs() < 0.1 ? 1.0 : rawRange;

    final startDate = points.first.date;
    final endDate = points.last.date;

    final int totalDaysInt = endDate.difference(startDate).inDays;
    final double totalDays = totalDaysInt <= 0 ? 1.0 : totalDaysInt.toDouble();

    double xForDate(DateTime date) {
      final double days = date.difference(startDate).inDays.toDouble();
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

    // target dashed line
    final double targetY = yForWeight(targetWeightKg);
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

    // progress line
    final linePaint = Paint()
      ..color = lineColor
      ..strokeWidth = 2
      ..style = PaintingStyle.stroke;

    final path = Path();
    for (var i = 0; i < points.length; i++) {
      final double x = xForDate(points[i].date);
      final double y = yForWeight(points[i].weightKg);

      if (i == 0) {
        path.moveTo(x, y);
      } else {
        path.lineTo(x, y);
      }
    }
    canvas.drawPath(path, linePaint);

    // points
    final pointPaint = Paint()
      ..color = lineColor
      ..style = PaintingStyle.fill;

    for (final e in points) {
      final double x = xForDate(e.date);
      final double y = yForWeight(e.weightKg);
      canvas.drawCircle(Offset(x, y), 3.0, pointPaint);
    }

    // y labels (min/max + target)
    final textPainter = TextPainter(
      textAlign: TextAlign.right,
      textDirection: TextDirection.ltr,
    );

    void drawLabel(String text, double y) {
      textPainter.text = TextSpan(
        text: text,
        style: TextStyle(
          fontSize: 10,
          color: axisColor,
        ),
      );
      textPainter.layout(minWidth: 0, maxWidth: paddingLeft - 4);
      textPainter.paint(
        canvas,
        Offset(paddingLeft - textPainter.width - 4, y - 7),
      );
    }

    drawLabel(yMax.toStringAsFixed(1), yForWeight(yMax));
    drawLabel(yMin.toStringAsFixed(1), yForWeight(yMin));
    drawLabel(targetWeightKg.toStringAsFixed(1), targetY);
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
        final next = distance + dashWidth;
        final extractPath = metric.extractPath(
          distance,
          next.clamp(0, metric.length),
        );
        canvas.drawPath(extractPath, paint);
        distance += dashWidth + gapWidth;
      }
    }
  }

  @override
  bool shouldRepaint(covariant _GoalProgressChartPainter oldDelegate) {
    return oldDelegate.points != points ||
        oldDelegate.startWeightKg != startWeightKg ||
        oldDelegate.currentWeightKg != currentWeightKg ||
        oldDelegate.targetWeightKg != targetWeightKg ||
        oldDelegate.lineColor != lineColor;
  }
}

class _Chip extends StatelessWidget {
  final String text;
  const _Chip({required this.text});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
      decoration: BoxDecoration(
        color: theme.colorScheme.surfaceContainerHighest,
        borderRadius: BorderRadius.circular(999),
      ),
      child: Text(
        text,
        style: theme.textTheme.bodySmall,
      ),
    );
  }
}
