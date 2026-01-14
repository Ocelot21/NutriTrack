using ErrorOr;
using MediatR;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Me.Commands.UploadAvatar;

public sealed record UploadAvatarCommand(
    UserId UserId,
    Stream Image,
    string FileName,
    string ContentType
) : IRequest<ErrorOr<Unit>>;
