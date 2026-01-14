using ErrorOr;
using MediatR;

namespace NutriTrack.Application.Social.Commands.DeleteSocialPost;

public sealed record DeleteSocialPostCommand(
    Guid UserId,
    Guid PostId
) : IRequest<ErrorOr<Unit>>;
