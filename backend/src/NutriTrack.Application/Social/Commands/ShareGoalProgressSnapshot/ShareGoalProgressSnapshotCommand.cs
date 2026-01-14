using ErrorOr;
using MediatR;

namespace NutriTrack.Application.Social.Commands.ShareGoalProgressSnapshot;

public sealed record ShareGoalProgressSnapshotCommand(
    Guid UserId,
    Guid UserGoalId,
    string? Caption,
    int? Visibility
) : IRequest<ErrorOr<Unit>>;
