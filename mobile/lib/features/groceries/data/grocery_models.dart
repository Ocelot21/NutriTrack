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
    final rawCategory = json['category'];
    final rawUnit = json['unitOfMeasure'];

    return Grocery(
      id: json['id'] as String,
      name: json['name'] as String,
      category: GroceryCategoryMapper.fromBackendDynamic(rawCategory),
      barcode: json['barcode'] as String?,
      proteinPer100g: (json['proteinGramsPer100g'] as num).toDouble(),
      carbsPer100g: (json['carbsGramsPer100g'] as num).toDouble(),
      fatPer100g: (json['fatGramsPer100g'] as num).toDouble(),
      caloriesPer100: json['caloriesPer100'] as int,
      unitOfMeasure: UnitOfMeasureUiMapper.fromBackendDynamic(rawUnit),
      imageUrl: json['imageUrl'] as String?,
      isApproved: json['isApproved'] as bool? ?? false,
    );
  }
}

extension GroceryCategoryX on GroceryCategory {
  int get backendInt {
    switch (this) {
      case GroceryCategory.uncategorized:
        return 0;
      case GroceryCategory.vegetable:
        return 1;
      case GroceryCategory.fruit:
        return 2;
      case GroceryCategory.grain:
        return 3;
      case GroceryCategory.protein:
        return 4;
      case GroceryCategory.dairy:
        return 5;
      case GroceryCategory.snack:
        return 6;
      case GroceryCategory.drink:
        return 7;
      case GroceryCategory.other:
        return 8;
    }
  }
}

extension GroceryCategoryMapper on GroceryCategory {
  static GroceryCategory fromBackendDynamic(dynamic value) {
    if (value == null) return GroceryCategory.uncategorized;

    if (value is int) {
      switch (value) {
        case 1:
          return GroceryCategory.vegetable;
        case 2:
          return GroceryCategory.fruit;
        case 3:
          return GroceryCategory.grain;
        case 4:
          return GroceryCategory.protein;
        case 5:
          return GroceryCategory.dairy;
        case 6:
          return GroceryCategory.snack;
        case 7:
          return GroceryCategory.drink;
        case 8:
          return GroceryCategory.other;
        default:
          return GroceryCategory.uncategorized;
      }
    }

    if (value is String) {
      return GroceryCategory.fromBackend(value);
    }

    return GroceryCategory.uncategorized;
  }
}

extension UnitOfMeasureUiX on UnitOfMeasureUi {
  int get backendInt {
    switch (this) {
      case UnitOfMeasureUi.gram:
        return 0;
      case UnitOfMeasureUi.milliliter:
        return 1;
      case UnitOfMeasureUi.piece:
        return 2;
    }
  }

  String get displayLabel {
    switch (this) {
      case UnitOfMeasureUi.gram:
        return 'Gram (g)';
      case UnitOfMeasureUi.milliliter:
        return 'Milliliter (ml)';
      case UnitOfMeasureUi.piece:
        return 'Piece';
    }
  }
}

extension UnitOfMeasureUiMapper on UnitOfMeasureUi {
  static UnitOfMeasureUi fromBackendDynamic(dynamic value) {
    if (value == null) return UnitOfMeasureUi.gram;

    if (value is int) {
      switch (value) {
        case 1:
          return UnitOfMeasureUi.milliliter;
        case 2:
          return UnitOfMeasureUi.piece;
        default:
          return UnitOfMeasureUi.gram;
      }
    }

    if (value is String) {
      return UnitOfMeasureUi.values.firstWhere(
            (u) => u.backendValue.toLowerCase() == value.toLowerCase(),
        orElse: () => UnitOfMeasureUi.gram,
      );
    }

    return UnitOfMeasureUi.gram;
  }
}
