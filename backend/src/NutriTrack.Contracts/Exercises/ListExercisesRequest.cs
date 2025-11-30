namespace NutriTrack.Contracts.Exercises;

public sealed record ListExercisesRequest(int? Page = 1, int? PageSize = 10, bool ApprovedOnly = true);
