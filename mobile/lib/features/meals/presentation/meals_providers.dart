import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/providers.dart';
import '../../../core/token_store.dart';
import '../data/meals_repo.dart';
import '../data/create_meal_request.dart';

final mealsRepoProvider = Provider<MealsRepo>((ref) {
  final api = ref.read(apiClientProvider);
  final store = ref.read(tokenStoreProvider);
  return MealsRepo(api, store);
});

class CreateMealState {
  final bool isSubmitting;
  final String? error;

  const CreateMealState({
    this.isSubmitting = false,
    this.error,
  });

  CreateMealState copyWith({
    bool? isSubmitting,
    String? error,
  }) {
    return CreateMealState(
      isSubmitting: isSubmitting ?? this.isSubmitting,
      error: error,
    );
  }
}

class CreateMealController extends Notifier<CreateMealState> {
  late final MealsRepo _repo;

  @override
  CreateMealState build() {
    _repo = ref.read(mealsRepoProvider);
    return const CreateMealState();
  }

  Future<bool> submit({
    required String name,
    String? description,
    required DateTime date,
    required TimeOfDay time,
  }) async {
    if (name.trim().isEmpty) {
      state = state.copyWith(error: 'Meal name is required.');
      return false;
    }

    final occurredAtLocal = DateTime(
      date.year,
      date.month,
      date.day,
      time.hour,
      time.minute,
    );

    state = state.copyWith(isSubmitting: true, error: null);

    try {
      final request = CreateMealRequest(
        name: name.trim(),
        occurredAtLocal: occurredAtLocal,
        description: description?.trim().isEmpty == true
            ? null
            : description?.trim(),
      );

      await _repo.createMeal(request);

      state = state.copyWith(isSubmitting: false, error: null);
      return true;
    } on MealsException catch (e) {
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

class EditMealState {
  final bool isSubmitting;
  final String? error;

  const EditMealState({
    this.isSubmitting = false,
    this.error,
  });

  EditMealState copyWith({
    bool? isSubmitting,
    String? error,
  }) {
    return EditMealState(
      isSubmitting: isSubmitting ?? this.isSubmitting,
      error: error,
    );
  }
}

class EditMealController extends Notifier<EditMealState> {
  late final MealsRepo _repo;

  @override
  EditMealState build() {
    _repo = ref.read(mealsRepoProvider);
    return const EditMealState();
  }

  Future<bool> submit({
    required String mealId,
    required String name,
    String? description,
    required DateTime date,
    required TimeOfDay time,
  }) async {
    if (name.trim().isEmpty) {
      state = state.copyWith(error: 'Meal name is required.');
      return false;
    }

    final occurredAtLocal = DateTime(
      date.year,
      date.month,
      date.day,
      time.hour,
      time.minute,
    );

    state = state.copyWith(isSubmitting: true, error: null);

    try {
      final request = CreateMealRequest(
        name: name.trim(),
        occurredAtLocal: occurredAtLocal,
        description: description?.trim().isEmpty == true
            ? null
            : description?.trim(),
      );

      await _repo.updateMeal(mealId, request);

      state = state.copyWith(isSubmitting: false, error: null);
      return true;
    } on MealsException catch (e) {
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

final createMealControllerProvider =
NotifierProvider.autoDispose<CreateMealController, CreateMealState>(
  CreateMealController.new,
);

final editMealControllerProvider =
NotifierProvider.autoDispose<EditMealController, EditMealState>(
  EditMealController.new,
);
