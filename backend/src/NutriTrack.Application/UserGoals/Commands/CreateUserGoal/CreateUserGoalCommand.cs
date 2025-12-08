using ErrorOr;
using MediatR;
using NutriTrack.Application.UserGoals.Common;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.UserGoals.Commands.CreateUserGoal;

public sealed record CreateUserGoalCommand(
    UserId UserId,
    NutritionGoal Type,
    DateOnly TargetDate,
    decimal TargetWeightKg
) : IRequest<ErrorOr<UserGoalResult>>;
