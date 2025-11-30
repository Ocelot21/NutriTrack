namespace NutriTrack.Contracts.Groceries;

public sealed record GroceryResponse(
    Guid Id,
    string Name,
    string Category,
    string? Barcode,
    decimal ProteinGramsPer100g,
    decimal CarbsGramsPer100g,
    decimal FatGramsPer100g,
    int CaloriesPer100,
    string UnitOfMeasure,
    string? ImageUrl,
    bool IsApproved,
    bool IsDeleted
);
