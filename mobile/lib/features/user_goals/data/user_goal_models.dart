enum NutritionGoalTypeUi {
  loseWeight(1, 'Lose weight'),
  gainWeight(2, 'Gain weight'),
  maintainWeight(3, 'Maintain weight');

  final int backendValue;
  final String label;

  const NutritionGoalTypeUi(this.backendValue, this.label);

  static NutritionGoalTypeUi fromBackend(int value) {
    return NutritionGoalTypeUi.values.firstWhere(
          (t) => t.backendValue == value,
      orElse: () => NutritionGoalTypeUi.loseWeight,
    );
  }
}

enum UserGoalStatusUi {
  inProgress(1, 'In progress'),
  completed(2, 'Completed'),
  failed(3, 'Failed'),
  cancelled(4, 'Cancelled');

  final int backendValue;
  final String label;

  const UserGoalStatusUi(this.backendValue, this.label);

  static UserGoalStatusUi fromBackend(int value) {
    return UserGoalStatusUi.values.firstWhere(
          (s) => s.backendValue == value,
      orElse: () => UserGoalStatusUi.inProgress,
    );
  }
}

class UserGoalDto {
  final String id;
  final NutritionGoalTypeUi type;
  final UserGoalStatusUi status;
  final DateTime startDate;
  final DateTime targetDate;
  final double startWeightKg;
  final double targetWeightKg;

  final double? currentWeightKg;
  final double? progressPercent;

  UserGoalDto({
    required this.id,
    required this.type,
    required this.status,
    required this.startDate,
    required this.targetDate,
    required this.startWeightKg,
    required this.targetWeightKg,
    this.currentWeightKg,
    this.progressPercent,
  });

  factory UserGoalDto.fromJson(Map<String, dynamic> json) {
    return UserGoalDto(
      id: json['id'] as String,
      type: NutritionGoalTypeUi.fromBackend(json['type'] as int),
      status: UserGoalStatusUi.fromBackend(json['status'] as int),
      startDate: DateTime.parse(json['startDate'] as String),
      targetDate: DateTime.parse(json['targetDate'] as String),
      startWeightKg: (json['startWeightKg'] as num).toDouble(),
      targetWeightKg: (json['targetWeightKg'] as num).toDouble(),
      currentWeightKg: json['currentWeightKg'] == null
          ? null
          : (json['currentWeightKg'] as num).toDouble(),
      progressPercent: json['progressPercent'] == null
          ? null
          : (json['progressPercent'] as num).toDouble(),
    );
  }
}

class WeightHistoryEntryDto {
  final DateTime date;
  final double weightKg;

  WeightHistoryEntryDto({
    required this.date,
    required this.weightKg,
  });

  factory WeightHistoryEntryDto.fromJson(Map<String, dynamic> json) {
    return WeightHistoryEntryDto(
      date: DateTime.parse(json['date'] as String),
      weightKg: (json['weightKg'] as num).toDouble(),
    );
  }
}
