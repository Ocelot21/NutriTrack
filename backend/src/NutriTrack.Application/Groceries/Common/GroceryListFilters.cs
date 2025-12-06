using NutriTrack.Domain.Groceries;

namespace NutriTrack.Application.Groceries.Common
{
    public sealed record GroceryListFilters(
    string? SearchTerm,
    GroceryCategory? Category,
    UnitOfMeasure? UnitOfMeasure,
    int? MinCaloriesPer100,
    int? MaxCaloriesPer100,
    decimal? MinProteinPer100g,
    decimal? MaxProteinPer100g,
    decimal? MinCarbsPer100g,
    decimal? MaxCarbsPer100g,
    decimal? MinFatPer100g,
    decimal? MaxFatPer100g
    );

}
