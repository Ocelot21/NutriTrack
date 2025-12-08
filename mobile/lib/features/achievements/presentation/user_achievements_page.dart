import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../data/user_achievement_models.dart';
import 'user_achievement_providers.dart';

class UserAchievementsPage extends ConsumerStatefulWidget {
  const UserAchievementsPage({super.key});

  @override
  ConsumerState<UserAchievementsPage> createState() =>
      _UserAchievementsPageState();
}
class _UserAchievementsPageState extends ConsumerState<UserAchievementsPage> {
  @override
  void initState() {
    super.initState();
    Future.microtask(() {
      ref
          .read(userAchievementsControllerProvider.notifier)
          .loadInitial();
    });
  }
  @override
  Widget build(BuildContext context) {
    final state = ref.watch(userAchievementsControllerProvider);
    final controller = ref.read(userAchievementsControllerProvider.notifier);
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: const Text('My Achievements'),
      ),
      body: RefreshIndicator(
        onRefresh: () => controller.loadInitial(refresh: true),
        child: Builder(
          builder: (context) {
            if (state.isLoading && state.items.isEmpty) {
              return const Center(
                child: CircularProgressIndicator(),
              );
            }

            if (state.error != null && state.items.isEmpty) {
              return Center(
                child: Padding(
                  padding: const EdgeInsets.all(16),
                  child: Text(
                    state.error!,
                    textAlign: TextAlign.center,
                    style: theme.textTheme.bodyMedium?.copyWith(
                      color: theme.colorScheme.error,
                    ),
                  ),
                ),
              );
            }

            if (state.items.isEmpty) {
              return const Center(
                child: Text('You have no achievements yet.'),
              );
            }

            return ListView.builder(
              padding: const EdgeInsets.only(bottom: 16),
              itemCount: state.items.length + (state.canLoadMore ? 1 : 0),
              itemBuilder: (context, index) {
                if (index >= state.items.length) {
                  // load more
                  if (!state.isLoading) {
                    controller.loadMore();
                  }
                  return const Padding(
                    padding: EdgeInsets.all(16),
                    child: Center(
                      child: CircularProgressIndicator(),
                    ),
                  );
                }

                final a = state.items[index];
                return _AchievementCard(achievement: a);
              },
            );
          },
        ),
      ),
    );
  }
}

class _AchievementCard extends StatelessWidget {
  final UserAchievementModel achievement;

  const _AchievementCard({
    required this.achievement,
  });

  String _formatDate(DateTime dt) {
    final d = dt.day.toString().padLeft(2, '0');
    final m = dt.month.toString().padLeft(2, '0');
    final y = dt.year.toString();
    return '$d.$m.$y';
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colorScheme = theme.colorScheme;

    final icon = achievementIconForName(achievement.iconName);

    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Icon(
              icon,
              size: 32,
              color: colorScheme.primary,
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    achievement.title,
                    style: theme.textTheme.titleMedium?.copyWith(
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    achievement.description,
                    style: theme.textTheme.bodyMedium,
                  ),
                  const SizedBox(height: 8),
                  Row(
                    children: [
                      Chip(
                        label: Text(
                          achievement.category.label,
                        ),
                      ),
                      const SizedBox(width: 8),
                      Chip(
                        avatar: const Icon(
                          Icons.star,
                          size: 16,
                        ),
                        label: Text('${achievement.points} pts'),
                      ),
                      const Spacer(),
                      Text(
                        _formatDate(achievement.earnedAt),
                        style: theme.textTheme.bodySmall?.copyWith(
                          color: colorScheme.onSurfaceVariant,
                        ),
                      ),
                    ],
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}
