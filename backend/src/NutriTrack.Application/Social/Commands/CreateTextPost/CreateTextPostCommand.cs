using ErrorOr;
using MediatR;

namespace NutriTrack.Application.Social.Commands.CreateTextPost;

public sealed record CreateTextPostCommand(
    Guid UserId,
    string Text,
    int? Visibility
) : IRequest<ErrorOr<Unit>>;
