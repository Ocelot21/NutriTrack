using ErrorOr;
using MediatR;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.ActivityLevelHistoryEntries.Commands.CreateActivityLevelHistoryEntry;

public sealed record CreateActivityLevelHistoryEntryCommand(
    UserId UserId,
    DateOnly EffectiveFrom,
    ActivityLevel ActivityLevel
) : IRequest<ErrorOr<Unit>>;
