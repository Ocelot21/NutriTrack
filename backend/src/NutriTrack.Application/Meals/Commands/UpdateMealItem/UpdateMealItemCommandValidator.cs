using FluentValidation;

namespace NutriTrack.Application.Meals.Commands.UpdateMealItem;

public sealed class UpdateMealItemCommandValidator : AbstractValidator<UpdateMealItemCommand>
{
    public UpdateMealItemCommandValidator()
    {
        RuleFor(x => x.MealId.Value).NotEmpty();
        RuleFor(x => x.MealItemId.Value).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
