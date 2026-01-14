using ErrorOr;
using MediatR;
using NutriTrack.Application.Authentication.Common;

namespace NutriTrack.Application.Authentication.Commands.LoginTwoFactor;

public sealed record LoginTwoFactorCommand(
    Guid ChallengeId,
    string Code
) : IRequest<ErrorOr<AuthenticationResult>>;

