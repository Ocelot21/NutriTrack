class EnumOption {
  final int id;
  final String label;
  final String backendValue;

  const EnumOption({
    required this.id,
    required this.label,
    required this.backendValue,
  });
}

// Gender enum (backend: Unknown=0, Male=1, Female=2)
class GenderOptions {
  static const unknown = EnumOption(
    id: 0,
    label: 'Prefer not to say',
    backendValue: 'Unknown',
  );

  static const male = EnumOption(
    id: 1,
    label: 'Male',
    backendValue: 'Male',
  );

  static const female = EnumOption(
    id: 2,
    label: 'Female',
    backendValue: 'Female',
  );

  static const values = [unknown, male, female];
}

// ActivityLevel (Sedentary=0, Light=1, Moderate=2, Active=3, VeryActive=4)
class ActivityLevelOptions {
  static const sedentary = EnumOption(
    id: 0,
    label: 'Sedentary',
    backendValue: 'Sedentary',
  );
  static const light = EnumOption(
    id: 1,
    label: 'Light',
    backendValue: 'Light',
  );
  static const moderate = EnumOption(
    id: 2,
    label: 'Moderate',
    backendValue: 'Moderate',
  );
  static const active = EnumOption(
    id: 3,
    label: 'Active',
    backendValue: 'Active',
  );
  static const veryActive = EnumOption(
    id: 4,
    label: 'Very active',
    backendValue: 'VeryActive',
  );

  static const values = [
    sedentary,
    light,
    moderate,
    active,
    veryActive,
  ];
}

// NutritionGoal (LoseWeight=1, MaintainWeight=2, GainWeight=3)
class NutritionGoalOptions {
  static const lose = EnumOption(
    id: 1,
    label: 'Lose weight',
    backendValue: 'LoseWeight',
  );
  static const maintain = EnumOption(
    id: 2,
    label: 'Maintain weight',
    backendValue: 'MaintainWeight',
  );
  static const gain = EnumOption(
    id: 3,
    label: 'Gain weight',
    backendValue: 'GainWeight',
  );

  static const values = [lose, maintain, gain];
}
