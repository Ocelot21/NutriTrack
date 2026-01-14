using ErrorOr;
using MediatR;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.TwoFactor.Queries.VerifyTotp;

public sealed record VerifyTotpQuery(UserId UserId, string Code)
    : IRequest<ErrorOr<Success>>;
