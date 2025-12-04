import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/providers.dart';
import '../data/health_profile_repo.dart';
import '../data/health_profile_request.dart';
import '../domain/health_enums.dart';

final healthProfileRepoProvider = Provider<HealthProfileRepo>((ref) {
  final api = ref.read(apiClientProvider);
  final store = ref.read(tokenStoreProvider);

  return HealthProfileRepo(api, store);
});

class HealthProfileState {
  final bool isSubmitting;
  final String? error;
  final bool submitted;

  final EnumOption gender;
  final DateTime? birthdate;
  final int? heightCm;
  final double? weightKg;
  final EnumOption activityLevel;
  final EnumOption nutritionGoal;

  const HealthProfileState({
    this.isSubmitting = false,
    this.error,
    this.submitted = false,
    required this.gender,
    this.birthdate,
    this.heightCm,
    this.weightKg,
    required this.activityLevel,
    required this.nutritionGoal,
  });

  HealthProfileState copyWith({
    bool? isSubmitting,
    String? error,
    bool? submitted,
    EnumOption? gender,
    DateTime? birthdate,
    int? heightCm,
    double? weightKg,
    EnumOption? activityLevel,
    EnumOption? nutritionGoal,
  }) {
    return HealthProfileState(
      isSubmitting: isSubmitting ?? this.isSubmitting,
      error: error,
      submitted: submitted ?? this.submitted,
      gender: gender ?? this.gender,
      birthdate: birthdate ?? this.birthdate,
      heightCm: heightCm ?? this.heightCm,
      weightKg: weightKg ?? this.weightKg,
      activityLevel: activityLevel ?? this.activityLevel,
      nutritionGoal: nutritionGoal ?? this.nutritionGoal,
    );
  }
}

class HealthProfileController extends Notifier<HealthProfileState> {
  late final HealthProfileRepo _repo;

  @override
  HealthProfileState build() {
    _repo = ref.read(healthProfileRepoProvider);
    return HealthProfileState(
      gender: GenderOptions.unknown,
      activityLevel: ActivityLevelOptions.light,
      nutritionGoal: NutritionGoalOptions.maintain,
    );
  }

  void updateGender(EnumOption value) {
    state = state.copyWith(gender: value, error: null);
  }

  void updateBirthdate(DateTime value) {
    state = state.copyWith(birthdate: value, error: null);
  }

  void updateHeight(int? value) {
    state = state.copyWith(heightCm: value, error: null);
  }

  void updateWeight(double? value) {
    state = state.copyWith(weightKg: value, error: null);
  }

  void updateActivityLevel(EnumOption value) {
    state = state.copyWith(activityLevel: value, error: null);
  }

  void updateNutritionGoal(EnumOption value) {
    state = state.copyWith(nutritionGoal: value, error: null);
  }

  Future<void> submit() async {
    final s = state;

    if (s.birthdate == null) {
      state = state.copyWith(error: 'Please select your birthdate.');
      return;
    }

    final now = DateTime.now();
    final age = now.year - s.birthdate!.year -
        ((now.month < s.birthdate!.month ||
            (now.month == s.birthdate!.month &&
                now.day < s.birthdate!.day))
            ? 1
            : 0);
    if (age < 10 || age > 100) {
      state = state.copyWith(
        error: 'Please enter a realistic age (10–100 years).',
      );
      return;
    }

    if (s.heightCm == null || s.heightCm! < 120 || s.heightCm! > 250) {
      state = state.copyWith(
        error: 'Please enter a realistic height (120–250 cm).',
      );
      return;
    }

    if (s.weightKg == null || s.weightKg! < 30 || s.weightKg! > 300) {
      state = state.copyWith(
        error: 'Please enter a realistic weight (30–300 kg).',
      );
      return;
    }

    state = state.copyWith(isSubmitting: true, error: null, submitted: false);

    try {
      final req = HealthProfileRequest(
        gender: s.gender.backendValue,
        birthdate: s.birthdate!,
        heightCm: s.heightCm!,
        weightKg: s.weightKg!,
        activityLevel: s.activityLevel.backendValue,
        nutritionGoal: s.nutritionGoal.backendValue,
      );

      await _repo.upsertHealthProfile(req);

      state = state.copyWith(
        isSubmitting: false,
        submitted: true,
        error: null,
      );
    } on HealthProfileException catch (e) {
      state = state.copyWith(
        isSubmitting: false,
        error: e.message,
        submitted: false,
      );
    } catch (e) {
      state = state.copyWith(
        isSubmitting: false,
        error: 'Unexpected error: $e',
        submitted: false,
      );
    }
  }
}

final healthProfileControllerProvider =
NotifierProvider<HealthProfileController, HealthProfileState>(
  HealthProfileController.new,
);