import 'package:cross_file/cross_file.dart';

import 'grocery_models.dart';

/// Matches the API's [FromForm] CreateGroceryRequest used by the
/// /groceries and /groceries/suggestions endpoints.
class CreateGroceryRequest {
  final String name;
  final GroceryCategory category;
  final UnitOfMeasureUi unitOfMeasure;

  /// Required when [unitOfMeasure] is [UnitOfMeasureUi.piece].
  final double? gramsPerPiece;

  final double proteinPer100;
  final double carbsPer100;
  final double fatPer100;
  final int caloriesPer100;

  final String? barcode;
  final XFile? imageFile;

  const CreateGroceryRequest({
    required this.name,
    required this.category,
    required this.unitOfMeasure,
    this.gramsPerPiece,
    required this.proteinPer100,
    required this.carbsPer100,
    required this.fatPer100,
    required this.caloriesPer100,
    this.barcode,
    this.imageFile,
  });
}

