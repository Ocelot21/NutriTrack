import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/providers.dart';
import '../../../core/token_store.dart';
import '../../home/data/daily_overview_models.dart'; // zbog UserExerciseLog
import '../data/exercise_repo.dart';
import '../data/update_user_exercise_log_request.dart';

class EditExerciseLogState {
  final bool isSubmitting;
  final String? error;

  const EditExerciseLogState({
    this.isSubmitting = false,
    this.error,
  });

  EditExerciseLogState copyWith({
    bool? isSubmitting,
    String? error,
  }) {
    return EditExerciseLogState(
      isSubmitting: isSubmitting ?? this.isSubmitting,
      error: error,
    );
  }
}

final editExerciseLogRepoProvider = Provider<ExerciseRepo>((ref) {
  final api = ref.read(apiClientProvider);
  final store = ref.read(tokenStoreProvider);
  return ExerciseRepo(api, store);
});

class EditExerciseLogController extends Notifier<EditExerciseLogState> {
  late final ExerciseRepo _repo;

  @override
  EditExerciseLogState build() {
    _repo = ref.read(editExerciseLogRepoProvider);
    return const EditExerciseLogState();
  }

  Future<bool> updateLog({
    required UserExerciseLog log,
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
      final request = UpdateUserExerciseLogRequest(
        durationMinutes: durationMinutes,
        occurredAtLocal: occurredAtLocal,
        notes: notes,
      );

      await _repo.updateUserExerciseLog(log.id, request);

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

  Future<bool> deleteLog(UserExerciseLog log) async {
    state = state.copyWith(isSubmitting: true, error: null);

    try {
      await _repo.deleteUserExerciseLog(log.id);
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

final editExerciseLogControllerProvider =
NotifierProvider.autoDispose<EditExerciseLogController, EditExerciseLogState>(
  EditExerciseLogController.new,
);
