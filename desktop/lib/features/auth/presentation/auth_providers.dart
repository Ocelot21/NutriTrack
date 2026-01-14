import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/providers.dart';
import 'package:nutritrack_shared/auth/data/auth_repo.dart';
import 'package:nutritrack_shared/auth/data/login_result.dart';

class AuthState {
  final bool isLoading;
  final String? error;

  /// True when the backend requested a second factor.
  final bool requiresTwoFactor;

  /// Present only when [requiresTwoFactor] is true.
  final String? twoFactorChallengeId;

  const AuthState({
    this.isLoading = false,
    this.error,
    this.requiresTwoFactor = false,
    this.twoFactorChallengeId,
  });

  AuthState copyWith({
    bool? isLoading,
    String? error,
    bool? requiresTwoFactor,
    String? twoFactorChallengeId,
  }) {
    return AuthState(
      isLoading: isLoading ?? this.isLoading,
      error: error,
      requiresTwoFactor: requiresTwoFactor ?? this.requiresTwoFactor,
      twoFactorChallengeId: twoFactorChallengeId,
    );
  }
}

class AuthController extends Notifier<AuthState> {
  late final AuthRepo _repo;

  @override
  AuthState build() {
    _repo = ref.read(authRepoProvider);
    return const AuthState();
  }

  Future<void> login({
    required String email,
    required String password,
  }) async {
    state = state.copyWith(
      isLoading: true,
      error: null,
      requiresTwoFactor: false,
      twoFactorChallengeId: null,
    );

    try {
      final result = await _repo.login(
        emailOrUsername: email,
        password: password,
      );

      switch (result) {
        case LoginAuthenticated():
          state = state.copyWith(
            isLoading: false,
            error: null,
            requiresTwoFactor: false,
            twoFactorChallengeId: null,
          );
          return;
        case LoginTwoFactorRequired(:final challengeId):
          state = state.copyWith(
            isLoading: false,
            error: null,
            requiresTwoFactor: true,
            twoFactorChallengeId: challengeId,
          );
          return;
      }
    } on AuthException catch (e) {
      state = state.copyWith(
        isLoading: false,
        error: e.message,
        requiresTwoFactor: false,
        twoFactorChallengeId: null,
      );
    } catch (e) {
      state = state.copyWith(
        isLoading: false,
        error: 'Unexpected error: $e',
        requiresTwoFactor: false,
        twoFactorChallengeId: null,
      );
    }
  }

  Future<void> verifyTwoFactor({
    required String code,
  }) async {
    final challengeId = state.twoFactorChallengeId;
    if (challengeId == null || challengeId.isEmpty) {
      state = state.copyWith(error: 'No pending two-factor challenge. Please log in again.');
      return;
    }

    state = state.copyWith(isLoading: true, error: null);

    try {
      await _repo.verifyTwoFactor(
        challengeId: challengeId,
        code: code,
      );

      // If we got here, token is set/saved.
      state = state.copyWith(
        isLoading: false,
        error: null,
        requiresTwoFactor: false,
        twoFactorChallengeId: null,
      );
    } on AuthException catch (e) {
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

  void resetTwoFactor() {
    state = state.copyWith(
      error: null,
      requiresTwoFactor: false,
      twoFactorChallengeId: null,
    );
  }

  void clearError() {
    state = state.copyWith(error: null);
  }
}

final authRepoProvider = Provider<AuthRepo>((ref) {
  final api = ref.read(apiClientProvider);
  final store = ref.read(tokenStoreProvider);
  return AuthRepo(api, store);
});

final authControllerProvider =
    NotifierProvider<AuthController, AuthState>(AuthController.new);
