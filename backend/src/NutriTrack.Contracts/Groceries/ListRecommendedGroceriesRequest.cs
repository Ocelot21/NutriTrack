namespace NutriTrack.Contracts.Groceries;

public sealed record ListRecommendedGroceriesRequest(
    int? Page = 1,
    int? PageSize = 20);
