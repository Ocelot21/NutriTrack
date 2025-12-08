namespace NutriTrack.Api.Controllers.Exercises;

public sealed record CreateExerciseRequest(
    string Name,
    string Category,
    decimal DefaultCaloriesPerMinute,
    string? Description,
    IFormFile? Image
);
