using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.UserGoals.Common;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.UserGoals.Queries.ListUserGoals;

public sealed record ListUserGoalsQuery(UserId UserId) : IRequest<ErrorOr<PagedResult<UserGoalResult>>>;
