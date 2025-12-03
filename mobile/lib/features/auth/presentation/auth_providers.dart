import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/providers.dart';
import '../data/auth_repo.dart';
import '../data/register_request.dart';

class AuthState {
  final bool isLoading;
  final String? error;

  const AuthState({
    this.isLoading = false,
    this.error,
  });

  AuthState copyWith({
    bool? isLoading,
    String? error,
  }) {
    return AuthState(
      isLoading: isLoading ?? this.isLoading,
      error: error,
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
    state = state.copyWith(isLoading: true, error: null);

    try {
      final success = await _repo.login(email: email, password: password);

      if (!success) {
        state = state.copyWith(
          isLoading: false,
          error: 'Login failed for unknown reason.',
        );
        return;
      }

      state = state.copyWith(isLoading: false, error: null);
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

enum RegisterStep {
  credentials, // username, email, password, confirm
  profile,     // firstName, lastName, country, timeZone
}

class RegisterState {
  final RegisterStep step;
  final RegisterRequest data;
  final bool isSubmitting;
  final String? error;

  const RegisterState({
    required this.step,
    required this.data,
    this.isSubmitting = false,
    this.error,
  });

  RegisterState copyWith({
    RegisterStep? step,
    RegisterRequest? data,
    bool? isSubmitting,
    String? error,
  }) {
    return RegisterState(
      step: step ?? this.step,
      data: data ?? this.data,
      isSubmitting: isSubmitting ?? this.isSubmitting,
      error: error,
    );
  }
}

class RegisterController extends Notifier<RegisterState> {
  late final AuthRepo _repo;

  @override
  RegisterState build() {
    _repo = ref.read(authRepoProvider);
    return RegisterState(
      step: RegisterStep.credentials,
      data: RegisterRequest(),
    );
  }

  void setCredentials({
    required String username,
    required String email,
    required String password,
    required String confirmPassword,
  }) {
    if (username.isEmpty ||
        email.isEmpty ||
        password.isEmpty ||
        confirmPassword.isEmpty) {
      state = state.copyWith(
        error: 'All fields are required.',
      );
      return;
    }

    if (!email.contains('@')) {
      state = state.copyWith(
        error: 'Please enter a valid email address.',
      );
      return;
    }

    if (password.length < 8) {
      state = state.copyWith(
        error: 'Password must be at least 8 characters long.',
      );
      return;
    }

    if (password != confirmPassword) {
      state = state.copyWith(
        error: 'Password and confirmation do not match.',
      );
      return;
    }

    final updated = state.data.copyWith(
      username: username,
      email: email,
      password: password,
      confirmPassword: confirmPassword,
    );

    state = state.copyWith(
      data: updated,
      step: RegisterStep.profile,
      error: null,
    );
  }

  void setProfileInfo({
    required String firstName,
    required String lastName,
    required String countryIso2,
    required String timeZoneId,
  }) {
    if (firstName.isEmpty || lastName.isEmpty) {
      state = state.copyWith(error: 'First name and last name are required.');
      return;
    }

    if (countryIso2.isEmpty || timeZoneId.isEmpty) {
      state = state.copyWith(error: 'Country and time zone are required.');
      return;
    }

    final updated = state.data.copyWith(
      firstName: firstName,
      lastName: lastName,
      countryIso2: countryIso2,
      timeZoneId: timeZoneId,
    );

    state = state.copyWith(
      data: updated,
      error: null,
    );
  }

  void goBackToCredentials() {
    state = state.copyWith(
      step: RegisterStep.credentials,
      error: null,
    );
  }

  Future<void> submit() async {
    state = state.copyWith(isSubmitting: true, error: null);

    try {
      await _repo.register(state.data);
      
      state = state.copyWith(isSubmitting: false);
    } on AuthException catch (e) {
      state = state.copyWith(
        isSubmitting: false,
        error: e.message,
      );
    } catch (e) {
      state = state.copyWith(
        isSubmitting: false,
        error: 'Unexpected error: $e',
      );
    }
  }
}

final registerControllerProvider =
NotifierProvider<RegisterController, RegisterState>(
  RegisterController.new,
);
