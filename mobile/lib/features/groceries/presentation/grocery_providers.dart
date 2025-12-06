import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/providers.dart';
import '../../../core/token_store.dart';
import '../data/grocery_models.dart';
import '../data/grocery_repo.dart';

final groceryRepoProvider = Provider<GroceryRepo>((ref) {
  final api = ref.read(apiClientProvider);
  final store = ref.read(tokenStoreProvider);
  return GroceryRepo(api, store);
});

class GrocerySearchState {
  final bool isLoading;
  final String? error;
  final List<Grocery> items;
  final int totalCount;
  final int page;
  final int pageSize;
  final GrocerySearchFilters filters;

  bool get canLoadMore => items.length < totalCount;

  const GrocerySearchState({
    this.isLoading = false,
    this.error,
    this.items = const [],
    this.totalCount = 0,
    this.page = 1,
    this.pageSize = 10,
    this.filters = const GrocerySearchFilters(),
  });

  GrocerySearchState copyWith({
    bool? isLoading,
    String? error,
    List<Grocery>? items,
    int? totalCount,
    int? page,
    int? pageSize,
    GrocerySearchFilters? filters,
  }) {
    return GrocerySearchState(
      isLoading: isLoading ?? this.isLoading,
      error: error,
      items: items ?? this.items,
      totalCount: totalCount ?? this.totalCount,
      page: page ?? this.page,
      pageSize: pageSize ?? this.pageSize,
      filters: filters ?? this.filters,
    );
  }
}

class GrocerySearchController extends Notifier<GrocerySearchState> {
  late final GroceryRepo _repo;

  @override
  GrocerySearchState build() {
    _repo = ref.read(groceryRepoProvider);
    return const GrocerySearchState();
  }

  Future<void> applyFilters(GrocerySearchFilters filters) async {
    state = state.copyWith(
      isLoading: true,
      error: null,
      filters: filters,
      items: [],
      totalCount: 0,
      page: 1,
    );

    await _loadPage(page: 1, append: false);
  }

  Future<void> loadInitialIfEmpty() async {
    if (state.items.isNotEmpty || state.isLoading) return;
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
      final result = await _repo.searchGroceries(
        filters: state.filters,
        page: page,
        pageSize: state.pageSize,
      );

      final newItems = append
          ? [...state.items, ...result.items]
          : result.items;

      state = state.copyWith(
        isLoading: false,
        items: newItems,
        totalCount: result.totalCount,
        page: result.page,
        pageSize: result.pageSize,
        error: null,
      );
    } on GrocerySearchException catch (e) {
      state = state.copyWith(
        isLoading: false,
        error: e.message,
      );
    } catch (e) {
      state = state.copyWith(
        isLoading: false,
        error: 'Unexpected error: $e',
      );
    }
  }
}

final grocerySearchControllerProvider =
NotifierProvider.autoDispose<GrocerySearchController, GrocerySearchState>(
  GrocerySearchController.new,
);
