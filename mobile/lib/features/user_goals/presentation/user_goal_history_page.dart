import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../data/user_goal_models.dart';
import 'user_goal_providers.dart';


class UserGoalHistoryPage extends ConsumerStatefulWidget {
  const UserGoalHistoryPage({super.key});

  @override
  ConsumerState<UserGoalHistoryPage> createState() =>
      _UserGoalHistoryPageState();
}

class _UserGoalHistoryPageState extends ConsumerState<UserGoalHistoryPage> {
  @override
  void initState() {
    super.initState();
    Future.microtask(
          () => ref
          .read(userGoalHistoryControllerProvider.notifier)
          .loadInitial(),
    );
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(userGoalHistoryControllerProvider);
    final textTheme = Theme.of(context).textTheme;
    final colorScheme = Theme.of(context).colorScheme;

    return Scaffold(
      appBar: AppBar(
        title: const Text('Goals history'),
      ),
      body: RefreshIndicator(
        onRefresh: () => ref
            .read(userGoalHistoryControllerProvider.notifier)
            .refresh(),
        child: ListView.builder(
          physics: const AlwaysScrollableScrollPhysics(),
          padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 16),
          itemCount: state.items.length +
              1 + // footer (load more / empty)
              (state.isLoading && state.items.isEmpty ? 1 : 0),
          itemBuilder: (context, index) {
            // loading-only state
            if (state.isLoading && state.items.isEmpty) {
              return const Center(
                child: Padding(
                  padding: EdgeInsets.only(top: 32),
                  child: CircularProgressIndicator(),
                ),
              );
            }

            if (index < state.items.length) {
              final goal = state.items[index];
              return _GoalHistoryItem(goal: goal);
            }

            // footer
            if (state.canLoadMore) {
              return Padding(
                padding: const EdgeInsets.symmetric(vertical: 16),
                child: Center(
                  child: TextButton(
                    onPressed: () => ref
                        .read(userGoalHistoryControllerProvider.notifier)
                        .loadMore(),
                    child: const Text('Load more'),
                  ),
                ),
              );
            }

            if (state.items.isEmpty) {
              return Padding(
                padding: const EdgeInsets.only(top: 32),
                child: Center(
                  child: Text(
                    'You don\'t have any goals yet.',
                    style: textTheme.bodyMedium,
                  ),
                ),
              );
            }

            if (state.error != null) {
              return Padding(
                padding: const EdgeInsets.only(top: 16),
                child: Center(
                  child: Text(
                    state.error!,
                    style: textTheme.bodySmall
                        ?.copyWith(color: colorScheme.error),
                  ),
                ),
              );
            }

            return const SizedBox.shrink();
          },
        ),
      ),
    );
  }
}

class _GoalHistoryItem extends StatelessWidget {
  final UserGoalDto goal;

  const _GoalHistoryItem({required this.goal});

  @override
  Widget build(BuildContext context) {
    final textTheme = Theme.of(context).textTheme;
    final colorScheme = Theme.of(context).colorScheme;

    Color statusColor;
    switch (goal.status) {
      case UserGoalStatusUi.completed:
        statusColor = Colors.green;
        break;
      case UserGoalStatusUi.failed:
        statusColor = colorScheme.error;
        break;
      case UserGoalStatusUi.cancelled:
        statusColor = colorScheme.outline;
        break;
      case UserGoalStatusUi.inProgress:
      default:
        statusColor = colorScheme.primary;
        break;
    }

    return Card(
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // top row: type + status
            Row(
              children: [
                Expanded(
                  child: Text(
                    goal.type.label,
                    style: textTheme.titleMedium,
                  ),
                ),
                Container(
                  padding:
                  const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                  decoration: BoxDecoration(
                    borderRadius: BorderRadius.circular(999),
                    color: statusColor.withOpacity(0.12),
                  ),
                  child: Text(
                    goal.status.label,
                    style: textTheme.bodySmall?.copyWith(
                      color: statusColor,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                ),
              ],
            ),

            const SizedBox(height: 8),

            Row(
              children: [
                Expanded(
                  child: _InfoRow(
                    label: 'Start',
                    value:
                    '${goal.startWeightKg.toStringAsFixed(1)} kg',
                  ),
                ),
                Expanded(
                  child: _InfoRow(
                    label: 'Target',
                    value:
                    '${goal.targetWeightKg.toStringAsFixed(1)} kg',
                  ),
                ),
              ],
            ),

            const SizedBox(height: 4),

            Row(
              children: [
                Expanded(
                  child: _InfoRow(
                    label: 'Start date',
                    value: goal.startDate
                        .toLocal()
                        .toString()
                        .split(' ')
                        .first,
                  ),
                ),
                Expanded(
                  child: _InfoRow(
                    label: 'Target date',
                    value: goal.targetDate
                        .toLocal()
                        .toString()
                        .split(' ')
                        .first,
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

class _InfoRow extends StatelessWidget {
  final String label;
  final String value;

  const _InfoRow({
    required this.label,
    required this.value,
  });

  @override
  Widget build(BuildContext context) {
    final textTheme = Theme.of(context).textTheme;

    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 2),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            label,
            style: textTheme.bodySmall?.copyWith(
              fontWeight: FontWeight.w500,
            ),
          ),
          const SizedBox(height: 2),
          Text(
            value,
            style: textTheme.bodyMedium,
          ),
        ],
      ),
    );
  }
}