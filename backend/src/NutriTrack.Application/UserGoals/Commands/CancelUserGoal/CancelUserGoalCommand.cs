using ErrorOr;
using MediatR;
using NutriTrack.Domain.UserGoals;

namespace NutriTrack.Application.UserGoals.Commands.CancelUserGoal;

public sealed record CancelUserGoalCommand(UserGoalId Id) : IRequest<ErrorOr<Unit>>;
