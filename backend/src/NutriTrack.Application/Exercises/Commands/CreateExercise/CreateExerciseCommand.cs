using ErrorOr;
using MediatR;
using NutriTrack.Application.Exercises.Common;
using NutriTrack.Domain.Exercises;
using System.IO;

namespace NutriTrack.Application.Exercises.Commands.CreateExercise;

public record CreateExerciseCommand(
    string Name,
    ExerciseCategory Category,
    decimal DefaultCaloriesPerMinute,
    string? Description,
    Stream? Image,
    string? ImageFileName,
    string? ImageContentType
) : IRequest<ErrorOr<ExerciseResult>>;
