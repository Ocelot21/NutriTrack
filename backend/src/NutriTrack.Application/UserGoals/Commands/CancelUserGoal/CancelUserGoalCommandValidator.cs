using FluentValidation;

namespace NutriTrack.Application.UserGoals.Commands.CancelUserGoal;

public sealed class CancelUserGoalCommandValidator : AbstractValidator<CancelUserGoalCommand>
{
    public CancelUserGoalCommandValidator()
    {
        RuleFor(x => x.Id.Value).NotEmpty();
    }
}
