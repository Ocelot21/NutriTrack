using ErrorOr;
using MediatR;
using NutriTrack.Application.Me.Common;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Me.Queries.GetDailyOverview;

public sealed record GetDailyOverviewQuery(UserId UserId, DateOnly Date) : IRequest<ErrorOr<DailyOverviewResult>>;
