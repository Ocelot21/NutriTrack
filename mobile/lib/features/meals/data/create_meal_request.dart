class CreateMealRequest {
  final String name;
  final DateTime occurredAtLocal;
  final String? description;

  CreateMealRequest({
    required this.name,
    required this.occurredAtLocal,
    this.description,
  });

  Map<String, dynamic> toJson() {
    final local = occurredAtLocal;
    final offset = DateTime.now().timeZoneOffset;
    final sign = offset.isNegative ? '-' : '+';
    final totalMinutes = offset.inMinutes.abs();
    final hours = (totalMinutes ~/ 60).toString().padLeft(2, '0');
    final minutes = (totalMinutes % 60).toString().padLeft(2, '0');

    // drop microseconds for ISO 8601 format
    final datePart = local.toIso8601String().split('.').first;

    final offsetStr = '$sign$hours:$minutes';

    return {
      'name': name,
      'occurredAtLocal': '$datePart$offsetStr',
      'description': description,
    };
  }
}
