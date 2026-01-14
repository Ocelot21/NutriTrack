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
    decimal? GramsPerPiece,
    string? ImageUrl,
    bool IsApproved,
    bool IsDeleted
);

public record GroceryResultList(
    IReadOnlyList<GroceryResult> Items,
    int Page,
    int PageSize,
    int TotalCount
);