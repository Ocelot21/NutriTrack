namespace NutriTrack.Contracts.Groceries;

public sealed record ListGroceriesRequest(
    string? SearchTerm,
    string? Category,
    string? UnitOfMeasure,
    int? MinCaloriesPer100,
    int? MaxCaloriesPer100,
    decimal? MinProteinPer100,
    decimal? MaxProteinPer100,
    decimal? MinCarbsPer100,
    decimal? MaxCarbsPer100,
    decimal? MinFatPer100,
    decimal? MaxFatPer100,
    int? Page = 1,
    int? PageSize = 10);
