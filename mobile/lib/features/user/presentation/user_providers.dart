import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:dio/dio.dart';
import 'package:flutter_riverpod/legacy.dart';

import '../../../core/providers.dart';
import '../../../core/token_store.dart';
import '../data/user_repo.dart';
import '../data/user_models.dart';

final userRepoProvider = Provider<UserRepo>((ref) {
  final api = ref.read(apiClientProvider);
  final store = ref.read(tokenStoreProvider);
  return UserRepo(api, store);
});

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

class UserController extends StateNotifier<UserState> {
  UserController(this._repo) : super(const UserState()) {
    loadMe();
  }

  final UserRepo _repo;

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

final userControllerProvider =
StateNotifierProvider<UserController, UserState>((ref) {
  final repo = ref.read(userRepoProvider);
  return UserController(repo);
});
