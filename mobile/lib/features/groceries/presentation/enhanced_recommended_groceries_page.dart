import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:nutritrack_shared/core/models/paged_response.dart';
import 'package:nutritrack_shared/groceries/data/grocery_models.dart';
import 'package:nutritrack_shared/groceries/data/grocery_repo.dart';
import '../../../core/providers.dart';

final enhancedRecommendedGroceriesRepoProvider = Provider<GroceryRepo>((ref) {
  final api = ref.read(apiClientProvider);
  final store = ref.read(tokenStoreProvider);
  return GroceryRepo(api, store);
});

class EnhancedRecommendedGroceriesState {
  final bool isLoading;
  final String? error;
  final List<GroceryRecommendation> items;
  final int totalCount;
  final int page;
  final int pageSize;

  bool get canLoadMore => items.length < totalCount;

  const EnhancedRecommendedGroceriesState({
    this.isLoading = false,
    this.error,
    this.items = const [],
    this.totalCount = 0,
    this.page = 1,
    this.pageSize = 20,
  });

  EnhancedRecommendedGroceriesState copyWith({
    bool? isLoading,
    String? error,
    List<GroceryRecommendation>? items,
    int? totalCount,
    int? page,
    int? pageSize,
  }) {
    return EnhancedRecommendedGroceriesState(
      isLoading: isLoading ?? this.isLoading,
      error: error,
      items: items ?? this.items,
      totalCount: totalCount ?? this.totalCount,
      page: page ?? this.page,
      pageSize: pageSize ?? this.pageSize,
    );
  }
}

class EnhancedRecommendedGroceriesController
    extends Notifier<EnhancedRecommendedGroceriesState> {
  late final GroceryRepo _repo;

  @override
  EnhancedRecommendedGroceriesState build() {
    _repo = ref.read(enhancedRecommendedGroceriesRepoProvider);
    return const EnhancedRecommendedGroceriesState();
  }

  Future<void> loadInitial() async {
    if (state.items.isNotEmpty) return;
    state = state.copyWith(isLoading: true, error: null);
    await _loadPage(page: 1, append: false);
  }

  Future<void> loadMore() async {
    if (!state.canLoadMore || state.isLoading) return;
    final nextPage = state.page + 1;
    state = state.copyWith(isLoading: true, error: null);
    await _loadPage(page: nextPage, append: true);
  }

  Future<void> _loadPage({
    required int page,
    required bool append,
  }) async {
    try {
      final result = await _repo.enhancedRecommendedGroceries(
        page: page,
        pageSize: state.pageSize,
      );

      final newItems =
          append ? [...state.items, ...result.items] : result.items;

      state = state.copyWith(
        isLoading: false,
        items: newItems,
        totalCount: result.totalCount,
        page: result.page,
        pageSize: result.pageSize,
        error: null,
      );
    } catch (e) {
      state = state.copyWith(
        isLoading: false,
        error: e.toString(),
      );
    }
  }
}

final enhancedRecommendedGroceriesControllerProvider = NotifierProvider<
    EnhancedRecommendedGroceriesController,
    EnhancedRecommendedGroceriesState>(
  EnhancedRecommendedGroceriesController.new,
);

class EnhancedRecommendedGroceriesPage extends ConsumerStatefulWidget {
  const EnhancedRecommendedGroceriesPage({super.key});

  @override
  ConsumerState<EnhancedRecommendedGroceriesPage> createState() =>
      _EnhancedRecommendedGroceriesPageState();
}

class _EnhancedRecommendedGroceriesPageState
    extends ConsumerState<EnhancedRecommendedGroceriesPage> {
  final _scrollController = ScrollController();

  @override
  void initState() {
    super.initState();
    _scrollController.addListener(_onScroll);
    Future.microtask(() {
      ref
          .read(enhancedRecommendedGroceriesControllerProvider.notifier)
          .loadInitial();
    });
  }

  @override
  void dispose() {
    _scrollController.removeListener(_onScroll);
    _scrollController.dispose();
    super.dispose();
  }

  void _onScroll() {
    final st = ref.read(enhancedRecommendedGroceriesControllerProvider);
    if (st.isLoading || !st.canLoadMore) return;

    final threshold = 300.0;
    final max = _scrollController.position.maxScrollExtent;
    final current = _scrollController.position.pixels;

    if (current >= max - threshold) {
      ref
          .read(enhancedRecommendedGroceriesControllerProvider.notifier)
          .loadMore();
    }
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(enhancedRecommendedGroceriesControllerProvider);
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Enhanced Recommender'),
      ),
      body: Builder(
        builder: (context) {
          if (state.error != null && state.items.isEmpty) {
            return Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Text(
                    'Error loading recommendations',
                    style: theme.textTheme.titleMedium,
                  ),
                  const SizedBox(height: 8),
                  Text(
                    state.error!,
                    style: theme.textTheme.bodySmall?.copyWith(
                      color: theme.colorScheme.error,
                    ),
                  ),
                ],
              ),
            );
          }

          if (state.items.isEmpty && state.isLoading) {
            return const Center(child: CircularProgressIndicator());
          }

          if (state.items.isEmpty) {
            return const Center(
              child: Text('No recommendations available'),
            );
          }

          return ListView.builder(
            controller: _scrollController,
            padding: const EdgeInsets.all(12),
            itemCount: state.items.length +
                ((state.isLoading && state.items.isNotEmpty) ? 1 : 0),
            itemBuilder: (context, index) {
              if (index >= state.items.length) {
                return const Padding(
                  padding: EdgeInsets.all(16),
                  child: Center(child: CircularProgressIndicator()),
                );
              }

              final recommendation = state.items[index];
              return _RecommendationCard(
                recommendation: recommendation,
                onTap: () {
                  // Navigate to grocery search with this grocery selected
                  context.push(
                    '/groceries/search',
                    extra: recommendation.toGrocery(),
                  );
                },
              );
            },
          );
        },
      ),
    );
  }
}

class _RecommendationCard extends StatelessWidget {
  final GroceryRecommendation recommendation;
  final VoidCallback onTap;

  const _RecommendationCard({
    required this.recommendation,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Card(
      margin: const EdgeInsets.only(bottom: 12),
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(12),
        child: Padding(
          padding: const EdgeInsets.all(12),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  if (recommendation.imageUrl != null &&
                      recommendation.imageUrl!.isNotEmpty)
                    ClipRRect(
                      borderRadius: BorderRadius.circular(8),
                      child: Image.network(
                        recommendation.imageUrl!,
                        width: 60,
                        height: 60,
                        fit: BoxFit.cover,
                        errorBuilder: (_, __, ___) => Container(
                          width: 60,
                          height: 60,
                          color: theme.colorScheme.surfaceContainerHighest,
                          child: Icon(
                            Icons.fastfood,
                            color: theme.colorScheme.onSurfaceVariant,
                          ),
                        ),
                      ),
                    )
                  else
                    Container(
                      width: 60,
                      height: 60,
                      decoration: BoxDecoration(
                        color: theme.colorScheme.surfaceContainerHighest,
                        borderRadius: BorderRadius.circular(8),
                      ),
                      child: Icon(
                        Icons.fastfood,
                        color: theme.colorScheme.onSurfaceVariant,
                      ),
                    ),
                  const SizedBox(width: 12),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Row(
                          children: [
                            Expanded(
                              child: Text(
                                recommendation.name,
                                style: theme.textTheme.titleMedium?.copyWith(
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                            ),
                            Container(
                              padding: const EdgeInsets.symmetric(
                                horizontal: 8,
                                vertical: 4,
                              ),
                              decoration: BoxDecoration(
                                color: theme.colorScheme.primaryContainer,
                                borderRadius: BorderRadius.circular(12),
                              ),
                              child: Row(
                                mainAxisSize: MainAxisSize.min,
                                children: [
                                  Icon(
                                    Icons.stars,
                                    size: 14,
                                    color:
                                        theme.colorScheme.onPrimaryContainer,
                                  ),
                                  const SizedBox(width: 4),
                                  Text(
                                    recommendation.score.toStringAsFixed(1),
                                    style: theme.textTheme.labelSmall?.copyWith(
                                      color:
                                          theme.colorScheme.onPrimaryContainer,
                                      fontWeight: FontWeight.bold,
                                    ),
                                  ),
                                ],
                              ),
                            ),
                          ],
                        ),
                        const SizedBox(height: 4),
                        Text(
                          recommendation.category.label,
                          style: theme.textTheme.bodySmall?.copyWith(
                            color: theme.colorScheme.onSurfaceVariant,
                          ),
                        ),
                      ],
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 12),
              Container(
                padding: const EdgeInsets.all(10),
                decoration: BoxDecoration(
                  color: theme.colorScheme.surfaceContainerHighest,
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Row(
                  children: [
                    Icon(
                      Icons.lightbulb_outline,
                      size: 16,
                      color: theme.colorScheme.primary,
                    ),
                    const SizedBox(width: 8),
                    Expanded(
                      child: Text(
                        recommendation.explanation,
                        style: theme.textTheme.bodySmall?.copyWith(
                          fontStyle: FontStyle.italic,
                        ),
                      ),
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 8),
              Wrap(
                spacing: 8,
                runSpacing: 4,
                children: [
                  _NutrientChip(
                    label: 'Protein',
                    value: '${recommendation.proteinPer100.toStringAsFixed(1)}g',
                  ),
                  _NutrientChip(
                    label: 'Carbs',
                    value: '${recommendation.carbsPer100.toStringAsFixed(1)}g',
                  ),
                  _NutrientChip(
                    label: 'Fat',
                    value: '${recommendation.fatPer100.toStringAsFixed(1)}g',
                  ),
                  _NutrientChip(
                    label: 'Calories',
                    value: '${recommendation.caloriesPer100} kcal',
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _NutrientChip extends StatelessWidget {
  final String label;
  final String value;

  const _NutrientChip({
    required this.label,
    required this.value,
  });

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
      decoration: BoxDecoration(
        border: Border.all(color: theme.dividerColor),
        borderRadius: BorderRadius.circular(8),
      ),
      child: Text(
        '$label: $value',
        style: theme.textTheme.bodySmall,
      ),
    );
  }
}
