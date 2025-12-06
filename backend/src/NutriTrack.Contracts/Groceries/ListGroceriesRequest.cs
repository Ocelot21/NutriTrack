namespace NutriTrack.Contracts.Groceries;

public sealed record ListGroceriesRequest(
    string? SearchTerm,
    string? Category,
    string? UnitOfMeasure,
    int? MinCaloriesPer100,
    int? MaxCaloriesPer100,
    decimal? MinProteinPer100g,
    decimal? MaxProteinPer100g,
    decimal? MinCarbsPer100g,
    decimal? MaxCarbsPer100g,
    decimal? MinFatPer100g,
    decimal? MaxFatPer100g,
    int? Page = 1,
    int? PageSize = 10);
