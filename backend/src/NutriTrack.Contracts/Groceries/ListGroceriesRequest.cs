namespace NutriTrack.Contracts.Groceries;

public sealed record ListGroceriesRequest(int? Page = null, int? PageSize = null, bool ApprovedOnly = true);
