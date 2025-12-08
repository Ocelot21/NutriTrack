using FluentValidation;
using NutriTrack.Domain.Common;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.UserGoals.Commands.CreateUserGoal;

public sealed class CreateUserGoalCommandValidator : AbstractValidator<CreateUserGoalCommand>
{
    public CreateUserGoalCommandValidator()
    {
        RuleFor(x => x.UserId.Value).NotEmpty();
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.TargetDate).NotEmpty();
        RuleFor(x => x.TargetWeightKg).GreaterThan(0);
    }
}
