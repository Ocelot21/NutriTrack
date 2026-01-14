import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/providers.dart';
import '../data/user_repo.dart';
import '../data/user_models.dart';

final userRepoProvider = Provider<UserRepo>((ref) {
  final api = ref.read(apiClientProvider);
  final store = ref.read(tokenStoreProvider);
  return UserRepo(api, store);
});

// --------------------
// Me / profile
// --------------------

class UserState {
  final Me? me;
  final bool isLoading;
  final String? error;

  const UserState({
    this.me,
    this.isLoading = false,
    this.error,
  });

  UserState copyWith({
    Me? me,
    bool? isLoading,
    String? error,
  }) {
    return UserState(
      me: me ?? this.me,
      isLoading: isLoading ?? this.isLoading,
      error: error,
    );
  }
}

class UserController extends Notifier<UserState> {
  late final UserRepo _repo;

  @override
  UserState build() {
    _repo = ref.read(userRepoProvider);

    // fire-and-forget initial load
    Future.microtask(loadMe);

    return const UserState();
  }

  Future<void> loadMe() async {
    state = state.copyWith(isLoading: true, error: null);
    try {
      final me = await _repo.getMe();
      state = UserState(me: me, isLoading: false, error: null);
    } catch (e) {
      state = UserState(
        me: null,
        isLoading: false,
        error: e.toString(),
      );
    }
  }

  Future<bool> updateField(String fieldName, dynamic value) async {
    try {
      await _repo.updateProfileFields({fieldName: value});
      await loadMe();
      return true;
    } catch (e) {
      state = state.copyWith(error: e.toString());
      return false;
    }
  }

  Future<bool> uploadAvatar(MultipartFile file) async {
    try {
      await _repo.uploadAvatar(file);
      await loadMe();
      return true;
    } catch (e) {
      state = state.copyWith(error: e.toString());
      return false;
    }
  }

  Future<bool> changePassword({
    required String currentPassword,
    required String newPassword,
    required String confirmPassword,
  }) async {
    try {
      await _repo.changePassword(
        currentPassword: currentPassword,
        newPassword: newPassword,
        confirmPassword: confirmPassword,
      );
      return true;
    } catch (e) {
      state = state.copyWith(error: e.toString());
      return false;
    }
  }
}

final userControllerProvider = NotifierProvider<UserController, UserState>(
  UserController.new,
);

// --------------------
// Admin - users list
// --------------------

class UsersListState {
  final bool isLoading;
  final String? error;
  final List<UserResponse> items;
  final int totalCount;
  final int page;
  final int pageSize;

  bool get canLoadMore => items.length < totalCount;

  const UsersListState({
    this.isLoading = false,
    this.error,
    this.items = const [],
    this.totalCount = 0,
    this.page = 1,
    this.pageSize = 20,
  });

  UsersListState copyWith({
    bool? isLoading,
    String? error,
    List<UserResponse>? items,
    int? totalCount,
    int? page,
    int? pageSize,
  }) {
    return UsersListState(
      isLoading: isLoading ?? this.isLoading,
      error: error,
      items: items ?? this.items,
      totalCount: totalCount ?? this.totalCount,
      page: page ?? this.page,
      pageSize: pageSize ?? this.pageSize,
    );
  }
}

class UsersListController extends Notifier<UsersListState> {
  late final UserRepo _repo;

  @override
  UsersListState build() {
    _repo = ref.read(userRepoProvider);
    return const UsersListState();
  }

  Future<void> loadInitialIfEmpty() async {
    if (state.items.isNotEmpty || state.isLoading) return;
    state = state.copyWith(isLoading: true, error: null);
    await _loadPage(page: 1, append: false);
  }

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

  Future<void> setPageSize(int pageSize) async {
    state = state.copyWith(
      pageSize: pageSize,
      isLoading: true,
      error: null,
      items: const [],
      totalCount: 0,
      page: 1,
    );
    await _loadPage(page: 1, append: false);
  }

  Future<void> loadMore() async {
    if (!state.canLoadMore || state.isLoading) return;
    final nextPage = state.page + 1;
    state = state.copyWith(isLoading: true, error: null);
    await _loadPage(page: nextPage, append: true);
  }

  Future<void> deleteUser(String userId) async {
    state = state.copyWith(isLoading: true, error: null);
    try {
      await _repo.deleteUserById(userId);
      final idx = state.items.indexWhere((u) => u.id == userId);
      if (idx >= 0) {
        final newItems = [...state.items]..removeAt(idx);
        final newTotal = state.totalCount > 0 ? state.totalCount - 1 : 0;
        state = state.copyWith(
          isLoading: false,
          items: newItems,
          totalCount: newTotal,
          error: null,
        );
      } else {
        state = state.copyWith(isLoading: false, error: null);
      }
    } catch (e) {
      state = state.copyWith(isLoading: false, error: e.toString());
      rethrow;
    }
  }

  Future<void> _loadPage({required int page, required bool append}) async {
    try {
      final result = await _repo.listUsers(
        pageNumber: page,
        pageSize: state.pageSize,
      );

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
}

final usersListControllerProvider = NotifierProvider<UsersListController, UsersListState>(
  UsersListController.new,
);

// --------------------
// Admin - user details
// --------------------

final userDetailsProvider = FutureProvider.autoDispose.family<UserResponse, String>(
  (ref, userId) async {
    final repo = ref.read(userRepoProvider);
    return repo.getUserById(userId);
  },
);

/// Helper for setting a user's role.
///
/// UI can `await` this and show its own loading state; we invalidate the
/// details provider so the screen refreshes after success.
Future<void> setUserRole(
  WidgetRef ref, {
  required String userId,
  required String roleId,
}) async {
  final repo = ref.read(userRepoProvider);
  await repo.setUserRole(userId: userId, roleId: roleId);

  // Refresh details after changing role.
  ref.invalidate(userDetailsProvider(userId));
}
