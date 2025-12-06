import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/providers.dart';
import '../../../core/token_store.dart';
import '../data/exercise_models.dart';
import '../data/exercise_repo.dart';

final exerciseRepoProvider = Provider<ExerciseRepo>((ref) {
  final api = ref.read(apiClientProvider);
  final store = ref.read(tokenStoreProvider);
  return ExerciseRepo(api, store);
});

class ExerciseSearchState {
  final bool isLoading;
  final String? error;
  final List<Exercise> items;
  final int totalCount;
  final int page;
  final int pageSize;
  final ExerciseSearchFilters filters;

  bool get canLoadMore => items.length < totalCount;

  const ExerciseSearchState({
    this.isLoading = false,
    this.error,
    this.items = const [],
    this.totalCount = 0,
    this.page = 1,
    this.pageSize = 20,
    this.filters = const ExerciseSearchFilters(),
  });

  ExerciseSearchState copyWith({
    bool? isLoading,
    String? error,
    List<Exercise>? items,
    int? totalCount,
    int? page,
    int? pageSize,
    ExerciseSearchFilters? filters,
  }) {
    return ExerciseSearchState(
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

class ExerciseSearchController extends Notifier<ExerciseSearchState> {
  late final ExerciseRepo _repo;

  @override
  ExerciseSearchState build() {
    _repo = ref.read(exerciseRepoProvider);
    return const ExerciseSearchState();
  }

  Future<void> applyFilters({
    String? searchTerm,
    ExerciseCategory? category,
    double? minCaloriesPerMinute,
    double? maxCaloriesPerMinute,
  }) async {
    final newFilters = ExerciseSearchFilters(
      searchTerm: searchTerm,
      category: category,
      minCaloriesPerMinute: minCaloriesPerMinute,
      maxCaloriesPerMinute: maxCaloriesPerMinute,
    );

    state = state.copyWith(
      isLoading: true,
      error: null,
      filters: newFilters,
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
      final result = await _repo.searchExercises(
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
    } on ExerciseSearchException catch (e) {
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

final exerciseSearchControllerProvider =
NotifierProvider.autoDispose<ExerciseSearchController, ExerciseSearchState>(
  ExerciseSearchController.new,
);
