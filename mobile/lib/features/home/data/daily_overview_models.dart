class MealItem {
  final String id;
  final String groceryId;
  final String groceryName;
  final int caloriesPer100;
  final double proteinGramsPer100;
  final double carbsGramsPer100;
  final double fatGramsPer100;
  final String unitOfMeasure;
  final double quantity;

  MealItem({
    required this.id,
    required this.groceryId,
    required this.groceryName,
    required this.caloriesPer100,
    required this.proteinGramsPer100,
    required this.carbsGramsPer100,
    required this.fatGramsPer100,
    required this.unitOfMeasure,
    required this.quantity,
  });

  factory MealItem.fromJson(Map<String, dynamic> json) {
    return MealItem(
      id: json['id'] as String,
      groceryId: json['groceryId'] as String,
      groceryName: json['groceryName'] as String,
      caloriesPer100: json['caloriesPer100'] as int,
      proteinGramsPer100: (json['proteinGramsPer100'] as num).toDouble(),
      carbsGramsPer100: (json['carbsGramsPer100'] as num).toDouble(),
      fatGramsPer100: (json['fatGramsPer100'] as num).toDouble(),
      unitOfMeasure: json['unitOfMeasure'] as String,
      quantity: (json['quantity'] as num).toDouble(),
    );
  }
}

class Meal {
  final String id;
  final String name;
  final String? description;
  final DateTime occurredAtUtc;
  final DateTime occurredAtLocal;
  final DateTime localDate;
  final int totalCalories;
  final double totalProtein;
  final double totalCarbohydrates;
  final double totalFats;
  final List<MealItem> items;

  Meal({
    required this.id,
    required this.name,
    required this.description,
    required this.occurredAtUtc,
    required this.occurredAtLocal,
    required this.localDate,
    required this.totalCalories,
    required this.totalProtein,
    required this.totalCarbohydrates,
    required this.totalFats,
    required this.items,
  });

  factory Meal.fromJson(Map<String, dynamic> json) {
    return Meal(
      id: json['id'] as String,
      name: json['name'] as String,
      description: json['description'] as String?,
      occurredAtUtc: DateTime.parse(json['occurredAtUtc'] as String),
      occurredAtLocal: DateTime.parse(json['occurredAtLocal'] as String).toLocal(),
      localDate: DateTime.parse(json['localDate'] as String),
      totalCalories: json['totalCalories'] as int,
      totalProtein: (json['totalProtein'] as num).toDouble(),
      totalCarbohydrates: (json['totalCarbohydrates'] as num).toDouble(),
      totalFats: (json['totalFats'] as num).toDouble(),
      items: (json['items'] as List<dynamic>)
          .map((e) => MealItem.fromJson(e as Map<String, dynamic>))
          .toList(),
    );
  }
}

class UserExerciseLog {
  final String id;
  final String exerciseId;
  final String exerciseName;
  final String category;
  final double durationMinutes;
  final DateTime occurredAtUtc;
  final DateTime occurredAtLocal;
  final DateTime localDate;
  final double totalCalories;
  final String? notes;

  UserExerciseLog({
    required this.id,
    required this.exerciseId,
    required this.exerciseName,
    required this.category,
    required this.durationMinutes,
    required this.occurredAtUtc,
    required this.occurredAtLocal,
    required this.localDate,
    required this.totalCalories,
    required this.notes,
  });

  factory UserExerciseLog.fromJson(Map<String, dynamic> json) {
    return UserExerciseLog(
      id: json['id'] as String,
      exerciseId: json['exerciseId'] as String,
      exerciseName: json['exerciseName'] as String,
      category: json['category'] as String,
      durationMinutes: (json['durationMinutes'] as num).toDouble(),
      occurredAtUtc: DateTime.parse(json['occurredAtUtc'] as String),
      occurredAtLocal: DateTime.parse(json['occurredAtLocal'] as String).toLocal(),
      localDate: DateTime.parse(json['localDate'] as String),
      totalCalories: (json['totalCalories'] as num).toDouble(),
      notes: json['notes'] as String?,
    );
  }
}

class DailyNutritionTargets {
  final int calories;
  final double proteinGrams;
  final double fatGrams;
  final double carbohydrateGrams;

  DailyNutritionTargets({
    required this.calories,
    required this.proteinGrams,
    required this.fatGrams,
    required this.carbohydrateGrams,
  });

  factory DailyNutritionTargets.fromJson(Map<String, dynamic> json) {
    return DailyNutritionTargets(
      calories: json['calories'] as int,
      proteinGrams: (json['proteinGrams'] as num).toDouble(),
      fatGrams: (json['fatGrams'] as num).toDouble(),
      carbohydrateGrams: (json['carbohydrateGrams'] as num).toDouble(),
    );
  }
}

class DailyNutritionSnapshot {
  final int consumedCalories;
  final int burnedCalories;
  final int netCalories;
  final int remainingCalories;
  final double consumedProteinGrams;
  final double consumedFatGrams;
  final double consumedCarbohydrateGrams;

  DailyNutritionSnapshot({
    required this.consumedCalories,
    required this.burnedCalories,
    required this.netCalories,
    required this.remainingCalories,
    required this.consumedProteinGrams,
    required this.consumedFatGrams,
    required this.consumedCarbohydrateGrams,
  });

  factory DailyNutritionSnapshot.fromJson(Map<String, dynamic> json) {
    return DailyNutritionSnapshot(
      consumedCalories: json['consumedCalories'] as int,
      burnedCalories: json['burnedCalories'] as int,
      netCalories: json['netCalories'] as int,
      remainingCalories: json['remainingCalories'] as int,
      consumedProteinGrams:
      (json['consumedProteinGrams'] as num).toDouble(),
      consumedFatGrams: (json['consumedFatGrams'] as num).toDouble(),
      consumedCarbohydrateGrams:
      (json['consumedCarbohydrateGrams'] as num).toDouble(),
    );
  }
}

class DailyOverview {
  final List<Meal> meals;
  final List<UserExerciseLog> exercises;
  final DailyNutritionTargets targets;
  final DailyNutritionSnapshot snapshot;

  DailyOverview({
    required this.meals,
    required this.exercises,
    required this.targets,
    required this.snapshot,
  });

  factory DailyOverview.fromJson(Map<String, dynamic> json) {
    return DailyOverview(
      meals: (json['meals'] as List<dynamic>)
          .map((e) => Meal.fromJson(e as Map<String, dynamic>))
          .toList(),
      exercises: (json['exercises'] as List<dynamic>)
          .map((e) => UserExerciseLog.fromJson(e as Map<String, dynamic>))
          .toList(),
      targets: DailyNutritionTargets.fromJson(
        json['targets'] as Map<String, dynamic>,
      ),
      snapshot: DailyNutritionSnapshot.fromJson(
        json['snapshot'] as Map<String, dynamic>,
      ),
    );
  }
}