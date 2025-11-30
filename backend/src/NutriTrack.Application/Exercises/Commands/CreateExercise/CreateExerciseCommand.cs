using ErrorOr;
using MediatR;
using NutriTrack.Application.Exercises.Common;
using NutriTrack.Domain.Exercises;

namespace NutriTrack.Application.Exercises.Commands.CreateExercise;

public record CreateExerciseCommand(
    string Name,
    ExerciseCategory Category,
    decimal DefaultCaloriesPerMinute,
    string? Description,
    string? ImageUrl
) : IRequest<ErrorOr<ExerciseResult>>;
