import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/providers.dart';
import '../../../core/token_store.dart';
import '../../../core/models/paged_response.dart';
import '../data/user_achievement_models.dart';
import '../data/user_achievement_repo.dart';

final userAchievementRepoProvider = Provider<UserAchievementRepo>((ref) {
  final api = ref.read(apiClientProvider);
  final store = ref.read(tokenStoreProvider);
  return UserAchievementRepo(api, store);
});

class UserAchievementsState {
  final bool isLoading;
  final String? error;
  final List<UserAchievementModel> items;
  final int totalCount;
  final int page;
  final int pageSize;

  bool get canLoadMore => items.length < totalCount;

  const UserAchievementsState({
    this.isLoading = false,
    this.error,
    this.items = const [],
    this.totalCount = 0,
    this.page = 1,
    this.pageSize = 10,
  });

  UserAchievementsState copyWith({
    bool? isLoading,
    String? error,
    List<UserAchievementModel>? items,
    int? totalCount,
    int? page,
    int? pageSize,
  }) {
    return UserAchievementsState(
      isLoading: isLoading ?? this.isLoading,
      error: error,
      items: items ?? this.items,
      totalCount: totalCount ?? this.totalCount,
      page: page ?? this.page,
      pageSize: pageSize ?? this.pageSize,
    );
  }
}

class UserAchievementsController extends Notifier<UserAchievementsState> {
  late final UserAchievementRepo _repo;

  @override
  UserAchievementsState build() {
    _repo = ref.read(userAchievementRepoProvider);
    return const UserAchievementsState();
  }

  Future<void> loadInitial({bool refresh = false}) async {
    if (state.items.isNotEmpty && !refresh) return;

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
      final result = await _repo.getUserAchievements(
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
    } catch (e) {
      state = state.copyWith(
        isLoading: false,
        error: e.toString(),
      );
    }
  }
}

final userAchievementsControllerProvider =
NotifierProvider.autoDispose<UserAchievementsController, UserAchievementsState>(
  UserAchievementsController.new,
);
