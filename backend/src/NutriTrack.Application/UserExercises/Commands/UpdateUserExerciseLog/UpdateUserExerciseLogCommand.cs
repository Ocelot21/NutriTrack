using ErrorOr;
using MediatR;
using NutriTrack.Domain.UserExercises;

namespace NutriTrack.Application.UserExercises.Commands.UpdateUserExerciseLog;

public record UpdateUserExerciseLogCommand(
    UserExerciseLogId Id,
    decimal? DurationMinutes,
    DateTime? OccurredAtUtc,
    DateTimeOffset? OccurredAtLocal,
    DateOnly? LocalDate,
    string? Notes
) : IRequest<ErrorOr<Unit>>;
