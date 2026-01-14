using ErrorOr;
using MediatR;

namespace NutriTrack.Application.Social.Commands.ShareDailyOverviewSnapshot;

public sealed record ShareDailyOverviewSnapshotCommand(
    Guid UserId,
    DateOnly LocalDate,
    string? Caption,
    int? Visibility
) : IRequest<ErrorOr<Unit>>;
