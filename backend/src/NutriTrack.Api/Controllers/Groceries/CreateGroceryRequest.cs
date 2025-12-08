namespace NutriTrack.Api.Controllers.Groceries;

public sealed record CreateGroceryRequest(
    string Name,
    string Category,
    decimal ProteinPer100g,
    decimal CarbsPer100g,
    decimal FatPer100g,
    int CaloriesPer100,
    string UnitOfMeasure,
    string? Barcode,
    IFormFile? Image
);
