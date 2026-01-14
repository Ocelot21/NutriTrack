import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/providers.dart';
import 'package:nutritrack_shared/exercises/data/exercise_models.dart';
import 'package:nutritrack_shared/exercises/data/exercise_repo.dart';
import 'package:nutritrack_shared/exercises/data/create_user_exercise_log_request.dart';

class ExerciseLogState {
  final bool isSubmitting;
  final String? error;

  const ExerciseLogState({
    this.isSubmitting = false,
    this.error,
  });

  ExerciseLogState copyWith({
    bool? isSubmitting,
    String? error,
  }) {
    return ExerciseLogState(
      isSubmitting: isSubmitting ?? this.isSubmitting,
      error: error,
    );
  }
}

final exerciseLogRepoProvider = Provider<ExerciseRepo>((ref) {
  final api = ref.read(apiClientProvider);
  final store = ref.read(tokenStoreProvider);
  return ExerciseRepo(api, store);
});

class ExerciseLogController extends Notifier<ExerciseLogState> {
  late final ExerciseRepo _repo;

  @override
  ExerciseLogState build() {
    _repo = ref.read(exerciseLogRepoProvider);
    return const ExerciseLogState();
  }

  Future<bool> submit({
    required Exercise exercise,
    required double durationMinutes,
    required DateTime occurredAtLocal,
    String? notes,
  }) async {
    if (durationMinutes <= 0) {
      state = state.copyWith(error: 'Duration must be greater than 0.');
      return false;
    }

    state = state.copyWith(isSubmitting: true, error: null);

    try {
      final request = CreateUserExerciseLogRequest(
        exerciseId: exercise.id,
        durationMinutes: durationMinutes,
        occurredAtLocal: occurredAtLocal,
        notes: (notes?.trim().isEmpty ?? true) ? null : notes!.trim(),
      );

      await _repo.createUserExerciseLog(request);

      state = state.copyWith(isSubmitting: false, error: null);
      return true;
    } on ExerciseSearchException catch (e) {
      state = state.copyWith(isSubmitting: false, error: e.message);
      return false;
    } catch (e) {
      state = state.copyWith(
        isSubmitting: false,
        error: 'Unexpected error: $e',
      );
      return false;
    }
  }
}

final exerciseLogControllerProvider =
NotifierProvider.autoDispose<ExerciseLogController, ExerciseLogState>(
  ExerciseLogController.new,
);
