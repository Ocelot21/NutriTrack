class HealthProfileRequest {
  final String gender;
  final DateTime birthdate;
  final int heightCm;
  final double weightKg;
  final String activityLevel;
  final String nutritionGoal;

  HealthProfileRequest({
    required this.gender,
    required this.birthdate,
    required this.heightCm,
    required this.weightKg,
    required this.activityLevel,
    required this.nutritionGoal,
  });

  Map<String, dynamic> toJson() {
    final birthStr =
        '${birthdate.year.toString().padLeft(4, '0')}-${birthdate.month.toString().padLeft(2, '0')}-${birthdate.day.toString().padLeft(2, '0')}';

    return {
      'Gender': gender,
      'birthdate': birthStr,
      'heightCm': heightCm,
      'weightKg': weightKg,
      'ActivityLevel': activityLevel,
      'NutritionGoal': nutritionGoal,
    };
  }
}
