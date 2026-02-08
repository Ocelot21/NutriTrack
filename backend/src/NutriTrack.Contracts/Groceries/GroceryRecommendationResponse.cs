namespace NutriTrack.Contracts.Groceries;

public sealed record GroceryRecommendationResponse(
    Guid Id,
    string Name,
    string Category,
    decimal ProteinGramsPer100,
    decimal CarbsGramsPer100,
    decimal FatGramsPer100,
    int CaloriesPer100,
    string UnitOfMeasure,
    decimal? GramsPerPiece,
    string? Barcode,
    string? ImageUrl,
    bool IsApproved,
    bool IsDeleted,
    double Score,
    string Explanation
);
