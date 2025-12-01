namespace NutriTrack.Contracts.Meals;

public sealed record MealItemResponse(
    Guid Id,
    Guid GroceryId,
    string GroceryName,
    int CaloriesPer100,
    decimal ProteinGramsPer100g,
    decimal CarbsGramsPer100g,
    decimal FatGramsPer100g,
    string UnitOfMeasure,
    decimal Quantity
);
