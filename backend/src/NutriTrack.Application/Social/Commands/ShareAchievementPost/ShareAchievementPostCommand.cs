using ErrorOr;
using MediatR;

namespace NutriTrack.Application.Social.Commands.ShareAchievementPost;

public sealed record ShareAchievementPostCommand(
    Guid UserId,
    Guid UserAchievementId,
    string? Caption,
    int? Visibility
) : IRequest<ErrorOr<Unit>>;
