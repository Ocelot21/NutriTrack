using NutriTrack.Domain.Groceries;

namespace NutriTrack.Application.Groceries.Common;

public record GroceryRecommendationResult(
    GroceryId Id,
    string Name,
    GroceryCategory Category,
    string? Barcode,
    MacroNutrients MacrosPer100,
    int CaloriesPer100,
    UnitOfMeasure UnitOfMeasure,
    decimal? GramsPerPiece,
    string? ImageUrl,
    bool IsApproved,
    bool IsDeleted,
    double Score,
    string Explanation
);
