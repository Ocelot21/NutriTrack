import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../data/dashboard_models.dart';
import 'dashboard_providers.dart';

class DashboardPage extends ConsumerStatefulWidget {
  const DashboardPage({super.key});

  @override
  ConsumerState<DashboardPage> createState() => _DashboardPageState();
}

class _DashboardPageState extends ConsumerState<DashboardPage> {
  int _days = 7;
  int _top = 10;

  @override
  void initState() {
    super.initState();
    Future.microtask(() {
      ref
          .read(dashboardSummaryControllerProvider.notifier)
          .setRequest(GetAdminDashboardSummaryRequest(days: _days, top: _top));
    });
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    final state = ref.watch(dashboardSummaryControllerProvider);
    final controller = ref.read(dashboardSummaryControllerProvider.notifier);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Dashboard'),
        actions: [
          IconButton(
            tooltip: 'Refresh',
            onPressed: state.isLoading ? null : controller.refresh,
            icon: const Icon(Icons.refresh),
          ),
          const SizedBox(width: 8),
        ],
      ),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Wrap(
              spacing: 12,
              runSpacing: 12,
              crossAxisAlignment: WrapCrossAlignment.center,
              children: [
                Text('Admin overview', style: theme.textTheme.titleLarge),
                const SizedBox(width: 12),
                _DaysDropdown(
                  value: _days,
                  onChanged: (v) {
                    setState(() => _days = v);
                    controller.setRequest(
                      GetAdminDashboardSummaryRequest(days: _days, top: _top),
                    );
                  },
                ),
                _TopDropdown(
                  value: _top,
                  onChanged: (v) {
                    setState(() => _top = v);
                    controller.setRequest(
                      GetAdminDashboardSummaryRequest(days: _days, top: _top),
                    );
                  },
                ),
              ],
            ),
            const SizedBox(height: 16),
            if (state.error != null)
              Padding(
                padding: const EdgeInsets.only(bottom: 12),
                child: Text(
                  state.error!,
                  style: theme.textTheme.bodyMedium
                      ?.copyWith(color: theme.colorScheme.error),
                ),
              ),
            Expanded(
              child: state.isLoading && state.summary == null
                  ? const _DashboardLoading()
                  : (state.summary == null)
                      ? Center(
                          child: FilledButton.icon(
                            onPressed: controller.refresh,
                            icon: const Icon(Icons.refresh),
                            label: const Text('Load dashboard'),
                          ),
                        )
                      : LayoutBuilder(
                          builder: (context, constraints) {
                            final wide = constraints.maxWidth >= 980;
                            final summary = state.summary!;

                            return SingleChildScrollView(
                              child: Column(
                                crossAxisAlignment: CrossAxisAlignment.stretch,
                                children: [
                                  _KpisRow(summary: summary),
                                  const SizedBox(height: 16),
                                  if (wide)
                                    Row(
                                      crossAxisAlignment:
                                          CrossAxisAlignment.start,
                                      children: [
                                        Expanded(
                                          flex: 3,
                                          child: _DailyCountsCard(
                                              summary: summary),
                                        ),
                                        const SizedBox(width: 16),
                                        Expanded(
                                          flex: 2,
                                          child: Column(
                                            children: [
                                              _TopItemsCard(
                                                title: 'Top groceries',
                                                items: summary.topGroceries,
                                              ),
                                              const SizedBox(height: 16),
                                              _TopItemsCard(
                                                title: 'Top exercises',
                                                items: summary.topExercises,
                                              ),
                                            ],
                                          ),
                                        ),
                                      ],
                                    )
                                  else ...[
                                    _DailyCountsCard(summary: summary),
                                    const SizedBox(height: 16),
                                    _TopItemsCard(
                                      title: 'Top groceries',
                                      items: summary.topGroceries,
                                    ),
                                    const SizedBox(height: 16),
                                    _TopItemsCard(
                                      title: 'Top exercises',
                                      items: summary.topExercises,
                                    ),
                                  ],
                                ],
                              ),
                            );
                          },
                        ),
            ),
          ],
        ),
      ),
    );
  }
}

// -----------------------------------------------------------------------------
// Filters
// -----------------------------------------------------------------------------

class _DaysDropdown extends StatelessWidget {
  final int value;
  final ValueChanged<int> onChanged;

  const _DaysDropdown({required this.value, required this.onChanged});

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: 180,
      child: DropdownButtonFormField<int>(
        decoration: const InputDecoration(
          labelText: 'Last days',
          isDense: true,
        ),
        initialValue: value,
        items: const [7, 14, 30, 90]
            .map((v) => DropdownMenuItem(value: v, child: Text('$v days')))
            .toList(growable: false),
        onChanged: (v) {
          if (v == null) return;
          onChanged(v);
        },
      ),
    );
  }
}

class _TopDropdown extends StatelessWidget {
  final int value;
  final ValueChanged<int> onChanged;

  const _TopDropdown({required this.value, required this.onChanged});

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: 200,
      child: DropdownButtonFormField<int>(
        decoration: const InputDecoration(
          labelText: 'Top items',
          isDense: true,
        ),
        initialValue: value,
        items: const [5, 10, 20, 50]
            .map((v) => DropdownMenuItem(value: v, child: Text('Top $v')))
            .toList(growable: false),
        onChanged: (v) {
          if (v == null) return;
          onChanged(v);
        },
      ),
    );
  }
}

// -----------------------------------------------------------------------------
// KPI Cards
// -----------------------------------------------------------------------------

class _KpisRow extends StatelessWidget {
  final AdminDashboardSummaryResponse summary;

  const _KpisRow({required this.summary});

  @override
  Widget build(BuildContext context) {
    final k = summary.kpis;

    return Wrap(
      spacing: 16,
      runSpacing: 16,
      children: [
        _KpiCard(
          title: 'Active users',
          value: '${k.activeUsers}',
          icon: Icons.people_alt_outlined,
        ),
        _KpiCard(
          title: 'Meals logged',
          value: '${k.mealsLogged}',
          icon: Icons.restaurant_outlined,
        ),
        _KpiCard(
          title: 'Exercise logs',
          value: '${k.exerciseLogsLogged}',
          icon: Icons.fitness_center_outlined,
        ),
      ],
    );
  }
}

class _KpiCard extends StatelessWidget {
  final String title;
  final String value;
  final IconData icon;

  const _KpiCard({
    required this.title,
    required this.value,
    required this.icon,
  });

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return ConstrainedBox(
      constraints: const BoxConstraints(minWidth: 260, maxWidth: 360),
      child: Card(
        elevation: 2,
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Row(
            children: [
              Container(
                width: 44,
                height: 44,
                decoration: BoxDecoration(
                  color: theme.colorScheme.primaryContainer,
                  borderRadius: BorderRadius.circular(12),
                ),
                child: Icon(icon, color: theme.colorScheme.onPrimaryContainer),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(title, style: theme.textTheme.labelLarge),
                    const SizedBox(height: 6),
                    Text(
                      value,
                      style: theme.textTheme.headlineSmall
                          ?.copyWith(fontWeight: FontWeight.w700),
                    ),
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

// -----------------------------------------------------------------------------
// Daily counts chart
// -----------------------------------------------------------------------------

class _DailyCountsCard extends StatelessWidget {
  final AdminDashboardSummaryResponse summary;

  const _DailyCountsCard({required this.summary});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final counts = [...summary.dailyCounts]
      ..sort((a, b) => a.date.compareTo(b.date));

    return Card(
      elevation: 2,
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Row(
              children: [
                Expanded(
                  child: Text('Daily activity', style: theme.textTheme.titleMedium),
                ),
                _LegendDot(color: theme.colorScheme.primary, label: 'Meals'),
                const SizedBox(width: 12),
                _LegendDot(color: theme.colorScheme.tertiary, label: 'Exercises'),
              ],
            ),
            const SizedBox(height: 12),
            SizedBox(
              height: 220,
              child: counts.isEmpty
                  ? const Center(child: Text('No activity in this period.'))
                  : _MiniBarChart(counts: counts),
            ),
            const SizedBox(height: 12),
            Divider(color: theme.dividerColor),
            const SizedBox(height: 8),
            SingleChildScrollView(
              scrollDirection: Axis.horizontal,
              child: DataTable(
                columns: const [
                  DataColumn(label: Text('Date')),
                  DataColumn(label: Text('Meals logged')),
                  DataColumn(label: Text('Exercise logs')),
                ],
                rows: counts
                    .map(
                      (c) => DataRow(
                        cells: [
                          DataCell(Text(_formatDateOnly(c.date))),
                          DataCell(Text('${c.mealsLogged}')),
                          DataCell(Text('${c.exerciseLogsLogged}')),
                        ],
                      ),
                    )
                    .toList(growable: false),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _LegendDot extends StatelessWidget {
  final Color color;
  final String label;

  const _LegendDot({required this.color, required this.label});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Container(
          width: 10,
          height: 10,
          decoration: BoxDecoration(
            color: color,
            borderRadius: BorderRadius.circular(99),
          ),
        ),
        const SizedBox(width: 6),
        Text(label, style: theme.textTheme.labelMedium),
      ],
    );
  }
}

class _MiniBarChart extends StatelessWidget {
  final List<AdminDashboardDailyCountResponse> counts;

  const _MiniBarChart({required this.counts});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    final maxMeals =
        counts.map((e) => e.mealsLogged).fold<int>(0, (a, b) => a > b ? a : b);
    final maxExercises = counts
        .map((e) => e.exerciseLogsLogged)
        .fold<int>(0, (a, b) => a > b ? a : b);
    final maxValue = [maxMeals, maxExercises, 1].reduce((a, b) => a > b ? a : b);

    return Row(
      crossAxisAlignment: CrossAxisAlignment.end,
      children: [
        for (final c in counts)
          Expanded(
            child: Padding(
              padding: const EdgeInsets.symmetric(horizontal: 4),
              child: Column(
                mainAxisAlignment: MainAxisAlignment.end,
                children: [
                  Expanded(
                    child: Align(
                      alignment: Alignment.bottomCenter,
                      child: FractionallySizedBox(
                        heightFactor: c.mealsLogged / maxValue,
                        widthFactor: 1,
                        child: DecoratedBox(
                          decoration: BoxDecoration(
                            color: theme.colorScheme.primary,
                            borderRadius: BorderRadius.circular(8),
                          ),
                        ),
                      ),
                    ),
                  ),
                  const SizedBox(height: 6),
                  Expanded(
                    child: Align(
                      alignment: Alignment.bottomCenter,
                      child: FractionallySizedBox(
                        heightFactor: c.exerciseLogsLogged / maxValue,
                        widthFactor: 1,
                        child: DecoratedBox(
                          decoration: BoxDecoration(
                            color: theme.colorScheme.tertiary,
                            borderRadius: BorderRadius.circular(8),
                          ),
                        ),
                      ),
                    ),
                  ),
                  const SizedBox(height: 8),
                  Text(
                    _formatWeekday(c.date),
                    style: theme.textTheme.labelSmall,
                  ),
                ],
              ),
            ),
          ),
      ],
    );
  }
}

// -----------------------------------------------------------------------------
// Top items
// -----------------------------------------------------------------------------

class _TopItemsCard extends StatelessWidget {
  final String title;
  final List<AdminDashboardTopItemResponse> items;

  const _TopItemsCard({required this.title, required this.items});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Card(
      elevation: 2,
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Text(title, style: theme.textTheme.titleMedium),
            const SizedBox(height: 12),
            if (items.isEmpty)
              const Text('No items.')
            else
              DataTable(
                columns: const [
                  DataColumn(label: Text('Name')),
                  DataColumn(label: Text('Count'), numeric: true),
                ],
                rows: items
                    .map(
                      (i) => DataRow(
                        cells: [
                          DataCell(Text(i.name)),
                          DataCell(Text('${i.count}')),
                        ],
                      ),
                    )
                    .toList(growable: false),
              ),
          ],
        ),
      ),
    );
  }
}

// -----------------------------------------------------------------------------
// Formatting helpers
// -----------------------------------------------------------------------------

String _formatDateOnly(DateTime dt) {
  // Handles both full ISO strings and DateOnly-like strings.
  final y = dt.year.toString().padLeft(4, '0');
  final m = dt.month.toString().padLeft(2, '0');
  final d = dt.day.toString().padLeft(2, '0');
  return '$y-$m-$d';
}

String _formatWeekday(DateTime dt) {
  const names = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];
  final idx = dt.weekday.clamp(1, 7) - 1;
  return names[idx];
}

class _DashboardLoading extends StatefulWidget {
  const _DashboardLoading();

  @override
  State<_DashboardLoading> createState() => _DashboardLoadingState();
}

class _DashboardLoadingState extends State<_DashboardLoading> {
  bool _timedOut = false;

  @override
  void initState() {
    super.initState();
    Future.delayed(const Duration(seconds: 12), () {
      if (!mounted) return;
      setState(() => _timedOut = true);
    });
  }

  @override
  Widget build(BuildContext context) {
    if (!_timedOut) {
      return const Center(child: CircularProgressIndicator());
    }

    return Center(
      child: ConstrainedBox(
        constraints: const BoxConstraints(maxWidth: 520),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            const Text(
              'Still loading…',
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.w600),
            ),
            const SizedBox(height: 8),
            const Text(
              'This usually means the request is blocked (auth/token, network, or server).\n'
              'Try Refresh. If it persists, check the API base URL and login token.',
              textAlign: TextAlign.center,
            ),
            const SizedBox(height: 16),
            OutlinedButton.icon(
              onPressed: () {
                // Close and reopen the page rebuilds providers; also user can press Refresh in app bar.
                setState(() => _timedOut = false);
              },
              icon: const Icon(Icons.refresh),
              label: const Text('Keep waiting'),
            )
          ],
        ),
      ),
    );
  }
}
