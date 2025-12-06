enum ExerciseCategory {
  uncategorized('Uncategorized', 'Uncategorized'),
  cardio('Cardio', 'Cardio'),
  strength('Strength', 'Strength'),
  mobility('Mobility', 'Mobility'),
  flexibility('Flexibility', 'Flexibility'),
  hiit('HIIT', 'HIIT'),
  other('Other', 'Other');

  final String label;
  final String backendValue;

  const ExerciseCategory(this.label, this.backendValue);

  static ExerciseCategory fromBackend(String value) {
    return ExerciseCategory.values.firstWhere(
          (c) => c.backendValue.toLowerCase() == value.toLowerCase(),
      orElse: () => ExerciseCategory.uncategorized,
    );
  }
}

class Exercise {
  final String id;
  final String name;
  final ExerciseCategory category;
  final double defaultCaloriesPerMinute;
  final String? description;
  final String? imageUrl;
  final bool isApproved;

  Exercise({
    required this.id,
    required this.name,
    required this.category,
    required this.defaultCaloriesPerMinute,
    required this.description,
    required this.imageUrl,
    required this.isApproved,
  });

  factory Exercise.fromJson(Map<String, dynamic> json) {
    final categoryStr = json['category'] as String? ?? 'Uncategorized';

    return Exercise(
      id: json['id'] as String,
      name: json['name'] as String,
      category: ExerciseCategory.fromBackend(categoryStr),
      defaultCaloriesPerMinute:
      (json['defaultCaloriesPerMinute'] as num).toDouble(),
      description: json['description'] as String?,
      imageUrl: json['imageUrl'] as String?,
      isApproved: json['isApproved'] as bool? ?? false,
    );
  }
}

class PagedResponse<T> {
  final List<T> items;
  final int totalCount;
  final int page;
  final int pageSize;

  PagedResponse({
    required this.items,
    required this.totalCount,
    required this.page,
    required this.pageSize,
  });

  factory PagedResponse.fromJson(
      Map<String, dynamic> json,
      T Function(Map<String, dynamic>) fromJsonT,
      ) {
    final itemsJson = json['items'] as List<dynamic>? ?? const [];
    return PagedResponse<T>(
      items: itemsJson
          .map((e) => fromJsonT(e as Map<String, dynamic>))
          .toList(),
      totalCount: json['totalCount'] as int? ?? 0,
      page: json['page'] as int? ?? 1,
      pageSize: json['pageSize'] as int? ?? itemsJson.length,
    );
  }
}
