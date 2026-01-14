import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:nutritrack_shared/core/models/paged_response.dart';
import 'package:nutritrack_shared/exercises/data/exercise_models.dart';
import 'package:nutritrack_shared/groceries/data/grocery_models.dart';

import '../../exercises/presentation/exercise_providers.dart';
import '../../groceries/presentation/grocery_providers.dart';

class SuggestionsListState<T> {
  final bool isLoading;
  final String? error;
  final List<T> items;
  final int totalCount;
  final int page;
  final int pageSize;

  bool get canLoadMore => items.length < totalCount;

  const SuggestionsListState({
    this.isLoading = false,
    this.error,
    this.items = const [],
    this.totalCount = 0,
    this.page = 1,
    this.pageSize = 20,
  });

  SuggestionsListState<T> copyWith({
    bool? isLoading,
    String? error,
    List<T>? items,
    int? totalCount,
    int? page,
    int? pageSize,
  }) {
    return SuggestionsListState<T>(
      isLoading: isLoading ?? this.isLoading,
      error: error,
      items: items ?? this.items,
      totalCount: totalCount ?? this.totalCount,
      page: page ?? this.page,
      pageSize: pageSize ?? this.pageSize,
    );
  }
}

abstract class _SuggestionsBaseController<T>
    extends Notifier<SuggestionsListState<T>> {
  Future<PagedResponse<T>> fetch({required int page, required int pageSize});

  @override
  SuggestionsListState<T> build() => SuggestionsListState<T>();

  Future<void> refresh() async {
    state = state.copyWith(
      isLoading: true,
      error: null,
      items: const [],
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

  Future<void> _loadPage({required int page, required bool append}) async {
    try {
      final result = await fetch(page: page, pageSize: state.pageSize);
      final newItems = append ? [...state.items, ...result.items] : result.items;
      state = state.copyWith(
        isLoading: false,
        error: null,
        items: newItems,
        totalCount: result.totalCount,
        page: result.page,
        pageSize: result.pageSize,
      );
    } catch (e) {
      state = state.copyWith(isLoading: false, error: e.toString());
    }
  }

  void _removeById(String id, String Function(T) getId) {
    final idx = state.items.indexWhere((x) => getId(x) == id);
    if (idx < 0) return;

    final newItems = [...state.items]..removeAt(idx);
    final newTotal = state.totalCount > 0 ? state.totalCount - 1 : 0;
    state = state.copyWith(items: newItems, totalCount: newTotal);
  }
}

class GrocerySuggestionsController
    extends _SuggestionsBaseController<Grocery> {
  @override
  Future<PagedResponse<Grocery>> fetch({
    required int page,
    required int pageSize,
  }) {
    final repo = ref.read(groceryRepoProvider);
    return repo.listSuggestionGroceries(page: page, pageSize: pageSize);
  }

  Future<void> approve(String groceryId) async {
    final repo = ref.read(groceryRepoProvider);

    state = state.copyWith(isLoading: true, error: null);
    try {
      await repo.approveSuggestion(groceryId);
      _removeById(groceryId, (g) => g.id);
      state = state.copyWith(isLoading: false, error: null);
    } catch (e) {
      state = state.copyWith(isLoading: false, error: e.toString());
      rethrow;
    }
  }

  Future<void> reject(String groceryId) async {
    final repo = ref.read(groceryRepoProvider);

    state = state.copyWith(isLoading: true, error: null);
    try {
      await repo.deleteGrocery(groceryId);
      _removeById(groceryId, (g) => g.id);
      state = state.copyWith(isLoading: false, error: null);
    } catch (e) {
      state = state.copyWith(isLoading: false, error: e.toString());
      rethrow;
    }
  }
}

final grocerySuggestionsControllerProvider = NotifierProvider.autoDispose<
    GrocerySuggestionsController,
    SuggestionsListState<Grocery>>(
  GrocerySuggestionsController.new,
);

class ExerciseSuggestionsController
    extends _SuggestionsBaseController<Exercise> {
  @override
  Future<PagedResponse<Exercise>> fetch({
    required int page,
    required int pageSize,
  }) {
    final repo = ref.read(exerciseRepoProvider);
    return repo.listSuggestionExercises(page: page, pageSize: pageSize);
  }

  Future<void> approve(String exerciseId) async {
    final repo = ref.read(exerciseRepoProvider);

    state = state.copyWith(isLoading: true, error: null);
    try {
      await repo.approveSuggestion(exerciseId);
      _removeById(exerciseId, (e) => e.id);
      state = state.copyWith(isLoading: false, error: null);
    } catch (e) {
      state = state.copyWith(isLoading: false, error: e.toString());
      rethrow;
    }
  }

  Future<void> reject(String exerciseId) async {
    final repo = ref.read(exerciseRepoProvider);

    state = state.copyWith(isLoading: true, error: null);
    try {
      await repo.deleteExercise(exerciseId);
      _removeById(exerciseId, (e) => e.id);
      state = state.copyWith(isLoading: false, error: null);
    } catch (e) {
      state = state.copyWith(isLoading: false, error: e.toString());
      rethrow;
    }
  }
}

final exerciseSuggestionsControllerProvider = NotifierProvider.autoDispose<
    ExerciseSuggestionsController,
    SuggestionsListState<Exercise>>(
  ExerciseSuggestionsController.new,
);
