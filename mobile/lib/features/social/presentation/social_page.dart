import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'social_providers.dart';
import 'widgets/social_post_card.dart';

class SocialPage extends ConsumerStatefulWidget {
  const SocialPage({super.key});

  @override
  ConsumerState<SocialPage> createState() => _SocialPageState();
}

class _SocialPageState extends ConsumerState<SocialPage> {
  final _scrollController = ScrollController();

  @override
  void initState() {
    super.initState();

    _scrollController.addListener(_onScroll);

    Future.microtask(() {
      ref.read(socialFeedControllerProvider.notifier).loadInitialIfEmpty();
    });
  }

  @override
  void dispose() {
    _scrollController.removeListener(_onScroll);
    _scrollController.dispose();
    super.dispose();
  }

  void _onScroll() {
    final st = ref.read(socialFeedControllerProvider);

    if (st.isLoading || !st.canLoadMore) return;

    final threshold = 300.0;
    final max = _scrollController.position.maxScrollExtent;
    final current = _scrollController.position.pixels;

    if (current >= max - threshold) {
      ref.read(socialFeedControllerProvider.notifier).loadMore();
    }
  }

  Future<void> _openCreateTextPostDialog() async {
    final controller = TextEditingController();

    final created = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('New post'),
        content: TextField(
          controller: controller,
          maxLines: 5,
          maxLength: 280,
          decoration: const InputDecoration(hintText: "What's on your mind?"),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(ctx).pop(false),
            child: const Text('Cancel'),
          ),
          FilledButton(
            onPressed: () {
              if (controller.text.trim().isEmpty) return;
              Navigator.of(ctx).pop(true);
            },
            child: const Text('Post'),
          ),
        ],
      ),
    );

    if (created == true) {
      await ref
          .read(socialFeedControllerProvider.notifier)
          .createTextPost(controller.text);
    }
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(socialFeedControllerProvider);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Social'),
        actions: [
          IconButton(
            tooltip: 'Refresh',
            onPressed: state.isLoading
                ? null
                : () => ref.read(socialFeedControllerProvider.notifier).refresh(),
            icon: const Icon(Icons.refresh),
          ),
        ],
      ),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: _openCreateTextPostDialog,
        icon: const Icon(Icons.edit),
        label: const Text('Post'),
      ),
      body: Column(
        children: [
          if (state.error != null)
            MaterialBanner(
              content: Text(state.error!),
              actions: [
                TextButton(
                  onPressed: () => ref.read(socialFeedControllerProvider.notifier).refresh(),
                  child: const Text('Retry'),
                ),
              ],
            ),
          Expanded(
            child: state.isLoading && state.items.isEmpty
                ? const Center(child: CircularProgressIndicator())
                : RefreshIndicator(
              onRefresh: () =>
                  ref.read(socialFeedControllerProvider.notifier).refresh(),
              child: ListView.separated(
                controller: _scrollController,
                padding: const EdgeInsets.only(bottom: 96, top: 12),
                itemCount: state.items.length +
                    ((state.isLoading && state.items.isNotEmpty) ? 1 : 0),
                separatorBuilder: (_, _) => const SizedBox(height: 10),
                itemBuilder: (context, index) {
                  if (index >= state.items.length) {
                    return const Padding(
                      padding: EdgeInsets.symmetric(vertical: 16),
                      child: Center(child: CircularProgressIndicator()),
                    );
                  }

                  final post = state.items[index];
                  return Padding(
                    padding: const EdgeInsets.symmetric(horizontal: 12),
                    child: SocialPostCard(
                      post: post,
                      onAuthorTap: () {
                        context.push('/social/profile/${post.author.id}');
                      },
                      onDelete: () async {
                        await ref
                            .read(socialFeedControllerProvider.notifier)
                            .deletePost(post.id);
                      },
                    ),
                  );
                },
              ),
            ),
          ),
        ],
      ),
    );
  }
}
