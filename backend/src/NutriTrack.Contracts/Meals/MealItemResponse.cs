namespace NutriTrack.Contracts.Meals;

public sealed record MealItemResponse(
    Guid Id,
    Guid GroceryId,
    string GroceryName,
    int CaloriesPer100,
    decimal ProteinGramsPer100,
    decimal CarbsGramsPer100,
    decimal FatGramsPer100,
    string UnitOfMeasure,
    decimal? GramsPerPiece,
    decimal Quantity
);
