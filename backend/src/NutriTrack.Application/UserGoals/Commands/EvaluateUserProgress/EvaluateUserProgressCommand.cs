using ErrorOr;
using MediatR;
using NutriTrack.Domain.UserGoals;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.UserGoals.Commands.EvaluateUserProgress;

public sealed record EvaluateUserProgressCommand(UserGoalId GoalId, UserId UserId) : IRequest<ErrorOr<bool>>;
