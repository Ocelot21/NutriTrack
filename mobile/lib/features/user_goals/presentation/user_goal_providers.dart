import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/providers.dart';
import '../../../core/token_store.dart';
import '../data/user_goal_models.dart';
import '../data/user_goal_repo.dart';

final userGoalRepoProvider = Provider<UserGoalRepo>((ref) {
  final api = ref.read(apiClientProvider);
  final store = ref.read(tokenStoreProvider);
  return UserGoalRepo(api, store);
});

class UserGoalState {
  final bool isLoading;
  final String? error;

  final UserGoalDto? currentGoal;
  final List<WeightHistoryEntryDto> history;

  final bool isSavingWeight;
  final bool isCreatingGoal;

  const UserGoalState({
    this.isLoading = false,
    this.error,
    this.currentGoal,
    this.history = const [],
    this.isSavingWeight = false,
    this.isCreatingGoal = false,
  });

  UserGoalState copyWith({
    bool? isLoading,
    String? error,
    UserGoalDto? currentGoal,
    List<WeightHistoryEntryDto>? history,
    bool? isSavingWeight,
    bool? isCreatingGoal,
  }) {
    return UserGoalState(
      isLoading: isLoading ?? this.isLoading,
      error: error,
      currentGoal: currentGoal ?? this.currentGoal,
      history: history ?? this.history,
      isSavingWeight: isSavingWeight ?? this.isSavingWeight,
      isCreatingGoal: isCreatingGoal ?? this.isCreatingGoal,
    );
  }
}

class UserGoalController extends Notifier<UserGoalState> {
  late final UserGoalRepo _repo;

  @override
  UserGoalState build() {
    _repo = ref.read(userGoalRepoProvider);
    return const UserGoalState();
  }

  Future<void> load() async {
    state = state.copyWith(isLoading: true, error: null);

    try {
      final goal = await _repo.getCurrentGoal();

      if (goal == null) {
        // nema goal-a
        state = state.copyWith(
          isLoading: false,
          currentGoal: null,
          history: const [],
        );
        return;
      }

      final now = DateTime.now();
      final from = goal.startDate;
      final to = now;

      final history =
      await _repo.listWeightHistory(from: from, to: to);

      state = state.copyWith(
        isLoading: false,
        currentGoal: goal,
        history: history,
      );
    } on UserGoalException catch (e) {
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

  Future<void> refresh() async => load();

  Future<void> createGoal({
    required NutritionGoalTypeUi type,
    required DateTime targetDate,
    required double targetWeightKg,
  }) async {
    state = state.copyWith(
      isCreatingGoal: true,
      error: null,
    );

    try {
      await _repo.createGoal(
        type: type,
        targetDate: targetDate,
        targetWeightKg: targetWeightKg,
      );

      // nakon kreiranja goal-a, refrešujemo sve
      state = state.copyWith(isCreatingGoal: false);
      await load();
    } on UserGoalException catch (e) {
      state = state.copyWith(
        isCreatingGoal: false,
        error: e.message,
      );
    } catch (e) {
      state = state.copyWith(
        isCreatingGoal: false,
        error: 'Unexpected error: $e',
      );
    }
  }

  Future<void> addWeightEntry({
    required double weightKg,
    DateTime? date,
  }) async {
    state = state.copyWith(
      isSavingWeight: true,
      error: null,
    );

    try {
      final entryDate = date ?? DateTime.now();

      await _repo.createWeightEntry(
        date: entryDate,
        weightKg: weightKg,
      );

      // backend u handleru može:
      // - updateat userov current weight
      // - evaluirati goal (Completed/Failed)
      // pa nakon toga samo refrešujemo state
      state = state.copyWith(isSavingWeight: false);
      await load();
    } on UserGoalException catch (e) {
      state = state.copyWith(
        isSavingWeight: false,
        error: e.message,
      );
    } catch (e) {
      state = state.copyWith(
        isSavingWeight: false,
        error: 'Unexpected error: $e',
      );
    }
  }

  Future<void> cancelCurrentGoal() async {
    final goal = state.currentGoal;
    if (goal == null) return;

    try {
      await _repo.cancelGoal(goal.id);
      await load();
    } on UserGoalException catch (e) {
      state = state.copyWith(error: e.message);
    } catch (e) {
      state = state.copyWith(error: 'Unexpected error: $e');
    }
  }
}

class UserGoalHistoryState {
  final bool isLoading;
  final String? error;
  final List<UserGoalDto> items;
  final int totalCount;
  final int page;
  final int pageSize;

  bool get canLoadMore => items.length < totalCount;

  const UserGoalHistoryState({
    this.isLoading = false,
    this.error,
    this.items = const [],
    this.totalCount = 0,
    this.page = 1,
    this.pageSize = 10,
  });

  UserGoalHistoryState copyWith({
    bool? isLoading,
    String? error,
    List<UserGoalDto>? items,
    int? totalCount,
    int? page,
    int? pageSize,
  }) {
    return UserGoalHistoryState(
      isLoading: isLoading ?? this.isLoading,
      error: error,
      items: items ?? this.items,
      totalCount: totalCount ?? this.totalCount,
      page: page ?? this.page,
      pageSize: pageSize ?? this.pageSize,
    );
  }
}

class UserGoalHistoryController extends Notifier<UserGoalHistoryState> {
  late final UserGoalRepo _repo;

  @override
  UserGoalHistoryState build() {
    _repo = ref.read(userGoalRepoProvider);
    return const UserGoalHistoryState();
  }

  Future<void> loadInitial() async {
    if (state.items.isNotEmpty || state.isLoading) return;

    state = state.copyWith(isLoading: true, error: null);
    await _loadPage(page: 1, append: false);
  }

  Future<void> refresh() async {
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
      final result = await _repo.listGoals(
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
    } on UserGoalException catch (e) {
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

final userGoalHistoryControllerProvider =
NotifierProvider.autoDispose<UserGoalHistoryController, UserGoalHistoryState>(
  UserGoalHistoryController.new,
);

final userGoalControllerProvider =
NotifierProvider.autoDispose<UserGoalController, UserGoalState>(
  UserGoalController.new,
);
