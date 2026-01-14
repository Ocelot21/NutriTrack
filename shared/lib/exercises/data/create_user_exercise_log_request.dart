class CreateUserExerciseLogRequest {
  final String exerciseId;
  final double durationMinutes;
  final DateTime occurredAtLocal;
  final String? notes;

  CreateUserExerciseLogRequest({
    required this.exerciseId,
    required this.durationMinutes,
    required this.occurredAtLocal,
    this.notes,
  });

  Map<String, dynamic> toJson() {
    final local = occurredAtLocal;
    final offset = DateTime.now().timeZoneOffset;
    final sign = offset.isNegative ? '-' : '+';
    final totalMinutes = offset.inMinutes.abs();
    final hours = (totalMinutes ~/ 60).toString().padLeft(2, '0');
    final minutes = (totalMinutes % 60).toString().padLeft(2, '0');

    final datePart = local.toIso8601String().split('.').first;
    final offsetStr = '$sign$hours:$minutes';

    return {
      'exerciseId': exerciseId,
      'durationMinutes': durationMinutes,
      'occurredAtLocal': '$datePart$offsetStr',
      'notes': notes,
    };
  }
}
