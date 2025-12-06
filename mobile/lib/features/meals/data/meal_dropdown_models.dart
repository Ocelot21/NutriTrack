class MealDropdownItem {
  final String id;
  final String name;
  final DateTime localDate;

  MealDropdownItem({
    required this.id,
    required this.name,
    required this.localDate,
  });

  factory MealDropdownItem.fromJson(Map<String, dynamic> json) {
    return MealDropdownItem(
      id: json['id'] as String,
      name: json['name'] as String,
      localDate: DateTime.parse(json['localDate'] as String),
    );
  }
}