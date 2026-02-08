import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:nutritrack_shared/groceries/data/grocery_models.dart';

import '../../meals/presentation/create_meal_item_page.dart';
import 'grocery_providers.dart' as providers;

/// A simple paged list of recommended groceries.
///
/// No search. No filters. Just page + pageSize.
class RecommendedGroceriesPage extends ConsumerStatefulWidget {
  const RecommendedGroceriesPage({super.key});

  @override
  ConsumerState<RecommendedGroceriesPage> createState() =>
      _RecommendedGroceriesPageState();
}

class _RecommendedGroceriesPageState
    extends ConsumerState<RecommendedGroceriesPage> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      ref
          .read(providers.recommendedGroceriesControllerProvider.notifier)
          .loadInitialIfEmpty();
    });
  }

  Future<void> _navigateToCreateMealItem(Grocery grocery) async {
    await context.push<bool>(
      '/meal-items/create',
      extra: MealItemCreatePageArgs(grocery: grocery),
    );
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final state = ref.watch(providers.recommendedGroceriesControllerProvider);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Recommended for you'),
      ),
      body: SafeArea(
        child: Column(
          children: [
            Expanded(
              child: state.isLoading && state.items.isEmpty
                  ? const Center(child: CircularProgressIndicator())
                  : RefreshIndicator(
                      onRefresh: () => ref
                          .read(
                            providers.recommendedGroceriesControllerProvider
                                .notifier,
                          )
                          .reload(),
                      child: ListView.separated(
                        padding: const EdgeInsets.symmetric(
                          horizontal: 12,
                          vertical: 10,
                        ),
                        itemCount:
                            state.items.length + (state.canLoadMore ? 1 : 0),
                        separatorBuilder: (_, _) => const SizedBox(height: 10),
                        itemBuilder: (context, index) {
                          if (index < state.items.length) {
                            final g = state.items[index];
                            return _RecommendedGroceryCard(
                              grocery: g,
                              onTap: () => _navigateToCreateMealItem(g),
                            );
                          }

                          if (!state.canLoadMore) {
                            return const SizedBox.shrink();
                          }

                          return Padding(
                            padding: const EdgeInsets.symmetric(vertical: 6),
                            child: Center(
                              child: OutlinedButton(
                                onPressed: state.isLoading
                                    ? null
                                    : () => ref
                                        .read(
                                          providers.recommendedGroceriesControllerProvider
                                              .notifier,
                                        )
                                        .loadMore(),
                                child: state.isLoading
                                    ? const SizedBox(
                                        height: 18,
                                        width: 18,
                                        child: CircularProgressIndicator(
                                          strokeWidth: 2,
                                        ),
                                      )
                                    : const Text('Load more'),
                              ),
                            ),
                          );
                        },
                      ),
                    ),
            ),
            if (state.error != null)
              Container(
                width: double.infinity,
                color: theme.colorScheme.errorContainer,
                padding:
                    const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
                child: Text(
                  state.error!,
                  style: theme.textTheme.bodyMedium?.copyWith(
                    color: theme.colorScheme.onErrorContainer,
                  ),
                ),
              ),
          ],
        ),
      ),
    );
  }
}

class _RecommendedGroceryCard extends StatelessWidget {
  final Grocery grocery;
  final VoidCallback? onTap;

  const _RecommendedGroceryCard({
    required this.grocery,
    this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Card(
      elevation: 0,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(16),
        side: BorderSide(
          color: theme.dividerColor.withValues(alpha: 0.6),
        ),
      ),
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(16),
        child: Padding(
          padding: const EdgeInsets.all(12),
          child: Row(
            children: [
              const Icon(Icons.local_grocery_store),
              const SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      grocery.name,
                      style: theme.textTheme.titleMedium,
                    ),
                    const SizedBox(height: 4),
                    Text(
                      grocery.category.label,
                      style: theme.textTheme.bodySmall?.copyWith(
                        color: theme.colorScheme.onSurfaceVariant,
                      ),
                    ),
                  ],
                ),
              ),
              const SizedBox(width: 6),
              Icon(
                Icons.chevron_right,
                color: theme.colorScheme.onSurfaceVariant,
              ),
            ],
          ),
        ),
      ),
    );
  }
}
