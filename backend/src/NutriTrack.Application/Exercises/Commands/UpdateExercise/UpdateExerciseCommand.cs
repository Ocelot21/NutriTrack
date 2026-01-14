using ErrorOr;
using MediatR;
using NutriTrack.Application.Exercises.Common;
using NutriTrack.Domain.Exercises;

namespace NutriTrack.Application.Exercises.Commands.UpdateExercise;

public record UpdateExerciseCommand(
    ExerciseId Id,
    string? Name,
    ExerciseCategory? Category,
    decimal? DefaultCaloriesPerMinute,
    string? Description,
    Stream? Image,
    string? ImageFileName,
    string? ImageContentType,
    bool? IsApproved,
    bool? IsDeleted
) : IRequest<ErrorOr<ExerciseResult>>;
