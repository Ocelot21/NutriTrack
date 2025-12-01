using ErrorOr;
using MediatR;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Me.Queries.GetDailyOverview;

public sealed record GetDailyOverviewQuery(UserId UserId) : IRequest<ErrorOr<Unit>>;
