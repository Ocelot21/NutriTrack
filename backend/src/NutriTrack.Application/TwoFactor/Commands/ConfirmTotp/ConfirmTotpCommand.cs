using ErrorOr;
using MediatR;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.TwoFactor.Commands.ConfirmTotp;

public sealed record ConfirmTotpCommand(UserId UserId, string Code)
    : IRequest<ErrorOr<Success>>;
