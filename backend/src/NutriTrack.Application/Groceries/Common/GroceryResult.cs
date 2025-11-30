using NutriTrack.Domain.Groceries;

namespace NutriTrack.Application.Groceries.Common;

public record GroceryResult(
    GroceryId Id,
    string Name,
    GroceryCategory Category,
    string? Barcode,
    MacroNutrients MacrosPer100,
    int CaloriesPer100,
    UnitOfMeasure UnitOfMeasure,
    string? ImageUrl,
    bool IsApproved,
    bool IsDeleted
);
