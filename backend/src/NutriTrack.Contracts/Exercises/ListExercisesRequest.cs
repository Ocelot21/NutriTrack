namespace NutriTrack.Contracts.Exercises;

public sealed record ListExercisesRequest(
    string? SearchTerm,
    string? Category,
    decimal? MinCaloriesPerMinute,
    decimal? MaxCaloriesPerMinute,
    int Page = 1,
    int PageSize = 10);
