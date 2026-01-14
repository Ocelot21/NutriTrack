import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:nutritrack_mobile/features/achievements/presentation/user_achievement_share_providers.dart';
import 'package:nutritrack_mobile/features/achievements/presentation/widgets/user_achievement_share_sheet.dart';

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

                return GestureDetector(
                  onTap: () {
                    showModalBottomSheet(
                      context: context,
                      isScrollControlled: true,
                      showDragHandle: true,
                      builder: (_) => AchievementShareSheet(
                        achievement: a,
                      ),
                    );
                  },
                  child: _AchievementCard(
                    achievement: a,
                  ),
                );

              },
            );
          },
        ),
      ),
    );
  }
}

class _AchievementCard extends ConsumerWidget {
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
  Widget build(BuildContext context, WidgetRef ref) {
    final theme = Theme.of(context);
    final colorScheme = theme.colorScheme;

    final icon = achievementIconForName(achievement.iconName);

    final isShared =
    ref.watch(sharedAchievementsProvider).contains(achievement.id);

    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
      child: Stack(
        children: [
          Padding(
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

                      Wrap(
                        spacing: 8,
                        runSpacing: 8,
                        crossAxisAlignment: WrapCrossAlignment.center,
                        children: [
                          Chip(label: Text(achievement.category.label)),
                          Chip(
                            avatar: const Icon(Icons.star, size: 16),
                            label: Text('${achievement.points} pts'),
                          ),
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

          if (isShared)
            Positioned(
              top: 10,
              right: 10,
              child: Container(
                padding:
                const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
                decoration: BoxDecoration(
                  color: colorScheme.primary,
                  borderRadius: BorderRadius.circular(999),
                ),
                child: Row(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Icon(Icons.check, size: 16, color: colorScheme.onPrimary),
                    const SizedBox(width: 6),
                    Text(
                      'Shared',
                      style: theme.textTheme.labelMedium?.copyWith(
                        color: colorScheme.onPrimary,
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                  ],
                ),
              ),
            ),
        ],
      ),
    );
  }
}

