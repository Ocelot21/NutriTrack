import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../achievements/data/user_achievement_models.dart';
import '../../user/presentation/user_providers.dart';
import '../data/social_models.dart';
import 'social_providers.dart';
import 'widgets/social_post_card.dart';

class SocialProfilePageArgs {
  final String userId;

  const SocialProfilePageArgs({required this.userId});
}

class SocialProfilePage extends ConsumerStatefulWidget {
  final SocialProfilePageArgs args;

  const SocialProfilePage({super.key, required this.args});

  @override
  ConsumerState<SocialProfilePage> createState() => _SocialProfilePageState();
}

class _SocialProfilePageState extends ConsumerState<SocialProfilePage>
    with SingleTickerProviderStateMixin {
  late final TabController _tabController;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 2, vsync: this);

    WidgetsBinding.instance.addPostFrameCallback((_) {
      ref
          .read(socialProfileControllerProvider.notifier)
          .load(widget.args.userId);
    });
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(socialProfileControllerProvider);
    final userState = ref.watch(userControllerProvider);
    final currentUserId = userState.me?.id;

    final profile = state.profile;

    return Scaffold(
      appBar: AppBar(
        title: const Text('Profile'),
        bottom: TabBar(
          controller: _tabController,
          tabs: const [
            Tab(text: 'Posts'),
            Tab(text: 'Achievements'),
          ],
        ),
      ),
      body: SafeArea(
        child: Column(
          children: [
            if (state.error != null)
              MaterialBanner(
                content: Text(state.error!),
                actions: [
                  TextButton(
                    onPressed: () => ref
                        .read(socialProfileControllerProvider.notifier)
                        .load(widget.args.userId),
                    child: const Text('Retry'),
                  ),
                ],
              ),
            _Header(profile: profile),
            const Divider(height: 1),
            Expanded(
              child: state.isLoading && profile == null
                  ? const Center(child: CircularProgressIndicator())
                  : TabBarView(
                      controller: _tabController,
                      children: [
                        _PostsTab(
                          posts: profile?.posts ?? const [],
                          currentUserId: currentUserId,
                        ),
                        _AchievementsTab(
                          achievements: profile?.achievements ?? const [],
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

class _Header extends StatelessWidget {
  final SocialProfileModel? profile;

  const _Header({required this.profile});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    final username = profile?.username ?? '...';
    final points = profile?.totalPoints;
    final avatarUrl = profile?.avatarUrl;

    return Padding(
      padding: const EdgeInsets.fromLTRB(16, 16, 16, 12),
      child: Row(
        children: [
          CircleAvatar(
            radius: 28,
            backgroundImage: (avatarUrl != null && avatarUrl.isNotEmpty)
                ? NetworkImage(avatarUrl)
                : null,
            child: (avatarUrl == null || avatarUrl.isEmpty)
                ? const Icon(Icons.person)
                : null,
          ),
          const SizedBox(width: 14),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  username,
                  style: theme.textTheme.titleMedium
                      ?.copyWith(fontWeight: FontWeight.w700),
                ),
                const SizedBox(height: 4),
                Text(
                  points == null ? 'Points: ...' : 'Points: $points',
                  style: theme.textTheme.bodySmall?.copyWith(
                    color: theme.colorScheme.onSurfaceVariant,
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _PostsTab extends StatelessWidget {
  final List<SocialPostModel> posts;
  final String? currentUserId;

  const _PostsTab({required this.posts, this.currentUserId});

  @override
  Widget build(BuildContext context) {
    if (posts.isEmpty) {
      return const Center(
        child: Text('No posts yet.'),
      );
    }

    return ListView.separated(
      padding: const EdgeInsets.fromLTRB(12, 12, 12, 24),
      itemCount: posts.length,
      separatorBuilder: (_, _) => const SizedBox(height: 10),
      itemBuilder: (context, index) {
        final post = posts[index];
        return SocialPostCard(
          post: post,
          currentUserId: currentUserId,
          onAuthorTap: null,
          onDelete: null,
        );
      },
    );
  }
}

class _AchievementsTab extends StatelessWidget {
  final List<SocialProfileAchievementModel> achievements;

  const _AchievementsTab({required this.achievements});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    if (achievements.isEmpty) {
      return const Center(
        child: Text('No achievements yet.'),
      );
    }

    return ListView.separated(
      padding: const EdgeInsets.fromLTRB(12, 12, 12, 24),
      itemCount: achievements.length,
      separatorBuilder: (_, _) => const SizedBox(height: 10),
      itemBuilder: (context, index) {
        final a = achievements[index];
        final iconData = achievementIconForName(a.icon);
        return Card(
          elevation: 0,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(14),
            side: BorderSide(
              color: theme.dividerColor.withValues(alpha: 0.6),
            ),
          ),
          child: ListTile(
            leading: CircleAvatar(
              backgroundColor: theme.colorScheme.primaryContainer,
              child: Icon(
                iconData,
                color: theme.colorScheme.onPrimaryContainer,
              ),
            ),
            title: Text(a.name),
          ),
        );
      },
    );
  }
}

