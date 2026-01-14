using ErrorOr;
using MediatR;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Me.Commands.ChangePassword;

public sealed record ChangePasswordCommand(
    UserId UserId,
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword
) : IRequest<ErrorOr<Unit>>;
