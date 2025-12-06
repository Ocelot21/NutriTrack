enum GroceryCategory {
  uncategorized('Uncategorized', 'Uncategorized'),
  vegetable('Vegetable', 'Vegetable'),
  fruit('Fruit', 'Fruit'),
  grain('Grain', 'Grain'),
  protein('Protein', 'Protein'),
  dairy('Dairy', 'Dairy'),
  snack('Snack', 'Snack'),
  drink('Drink', 'Drink'),
  other('Other', 'Other');

  final String label;
  final String backendValue;

  const GroceryCategory(this.label, this.backendValue);

  static GroceryCategory fromBackend(String value) {
    return GroceryCategory.values.firstWhere(
          (c) => c.backendValue.toLowerCase() == value.toLowerCase(),
      orElse: () => GroceryCategory.uncategorized,
    );
  }
}

enum UnitOfMeasureUi {
  gram('g', 'Gram'),
  milliliter('ml', 'Milliliter'),
  piece('piece', 'Piece');

  final String label;
  final String backendValue;

  const UnitOfMeasureUi(this.label, this.backendValue);

  static UnitOfMeasureUi? fromBackend(String value) {
    for (final u in UnitOfMeasureUi.values) {
      if (u.backendValue.toLowerCase() == value.toLowerCase()) {
        return u;
      }
    }
    return null;
  }
}

class Grocery {
  final String id;
  final String name;
  final GroceryCategory category;
  final String? barcode;
  final double proteinPer100g;
  final double carbsPer100g;
  final double fatPer100g;
  final int caloriesPer100;
  final UnitOfMeasureUi unitOfMeasure;
  final String? imageUrl;
  final bool isApproved;

  Grocery({
    required this.id,
    required this.name,
    required this.category,
    required this.barcode,
    required this.proteinPer100g,
    required this.carbsPer100g,
    required this.fatPer100g,
    required this.caloriesPer100,
    required this.unitOfMeasure,
    required this.imageUrl,
    required this.isApproved,
  });

  factory Grocery.fromJson(Map<String, dynamic> json) {
    final categoryStr = json['category'] as String? ?? 'Uncategorized';
    final unitStr = json['unitOfMeasure'] as String? ?? 'Gram';

    return Grocery(
      id: json['id'] as String,
      name: json['name'] as String,
      category: GroceryCategory.fromBackend(categoryStr),
      barcode: json['barcode'] as String?,
      proteinPer100g: (json['proteinGramsPer100g'] as num).toDouble(),
      carbsPer100g: (json['carbsGramsPer100g'] as num).toDouble(),
      fatPer100g: (json['fatGramsPer100g'] as num).toDouble(),
      caloriesPer100: json['caloriesPer100'] as int,
      unitOfMeasure:
      UnitOfMeasureUi.fromBackend(unitStr) ?? UnitOfMeasureUi.gram,
      imageUrl: json['imageUrl'] as String?,
      isApproved: json['isApproved'] as bool? ?? false,
    );
  }
}