namespace NutriTrack.Contracts.Groceries;

public sealed record UpdateGroceryRequest(
    string? Name,
    string? Category,
    decimal? ProteinPer100g,
    decimal? CarbsPer100g,
    decimal? FatPer100g,
    int? CaloriesPer100,
    string? UnitOfMeasure,
    string? Barcode
);
