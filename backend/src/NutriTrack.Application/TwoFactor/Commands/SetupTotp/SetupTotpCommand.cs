using ErrorOr;
using MediatR;
using NutriTrack.Application.TwoFactor.Common;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.TwoFactor.Commands.SetupTotp;

public sealed record SetupTotpCommand(UserId UserId)
    : IRequest<ErrorOr<TotpSetupResult>>;
