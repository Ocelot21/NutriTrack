using ErrorOr;
using MediatR;
using NutriTrack.Application.UserExercises.Common;
using NutriTrack.Domain.Users;
using NutriTrack.Domain.Exercises;

namespace NutriTrack.Application.UserExercises.Commands.CreateUserExerciseLog;

public record CreateUserExerciseLogCommand(
    UserId UserId,
    Exercise Exercise,
    decimal DurationMinutes,
    DateTime OccurredAtUtc,
    DateTimeOffset OccurredAtLocal,
    DateOnly LocalDate,
    string? Notes
) : IRequest<ErrorOr<UserExerciseLogResult>>;
