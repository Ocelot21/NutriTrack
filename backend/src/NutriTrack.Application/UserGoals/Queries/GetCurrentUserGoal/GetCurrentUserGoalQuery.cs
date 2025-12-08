using ErrorOr;
using MediatR;
using NutriTrack.Application.UserGoals.Common;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.UserGoals.Queries.GetCurrentUserGoal;

public sealed record GetCurrentUserGoalQuery(UserId UserId) : IRequest<ErrorOr<UserGoalResult>>;
