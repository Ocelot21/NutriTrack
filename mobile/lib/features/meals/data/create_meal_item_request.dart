class CreateMealItemRequest {
  final String groceryId;
  final double quantity;

  CreateMealItemRequest({
    required this.groceryId,
    required this.quantity,
  });

  Map<String, dynamic> toJson() {
    return {
      'groceryId': groceryId,
      'quantity': quantity,
    };
  }
}
