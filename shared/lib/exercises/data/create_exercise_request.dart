import 'package:cross_file/cross_file.dart';

import 'exercise_models.dart';

/// Matches the API's [FromForm] CreateExerciseRequest used by the
/// /exercises and /exercises/suggestions endpoints.
class CreateExerciseRequest {
  final String name;
  final ExerciseCategory category;
  final double defaultCaloriesPerMinute;
  final String? description;
  final XFile? imageFile;

  const CreateExerciseRequest({
    required this.name,
    required this.category,
    required this.defaultCaloriesPerMinute,
    this.description,
    this.imageFile,
  });
}

