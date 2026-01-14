namespace NutriTrack.Contracts.Groceries;

public sealed record UpdateGroceryRequest(
    string? Name,
    string? Category,
    decimal? ProteinPer100,
    decimal? CarbsPer100,
    decimal? FatPer100,
    int? CaloriesPer100,
    string? UnitOfMeasure,
    string? GramsPerPiece,
    string? Barcode
);
