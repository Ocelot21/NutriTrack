using FluentValidation;

namespace NutriTrack.Application.Meals.Commands.RemoveMealItem;

public sealed class RemoveMealItemCommandValidator : AbstractValidator<RemoveMealItemCommand>
{
    public RemoveMealItemCommandValidator()
    {
        RuleFor(x => x.MealId.Value).NotEmpty();
        RuleFor(x => x.MealItemId.Value).NotEmpty();
    }
}
