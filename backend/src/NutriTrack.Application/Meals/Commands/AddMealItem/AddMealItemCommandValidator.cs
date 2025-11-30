using FluentValidation;

namespace NutriTrack.Application.Meals.Commands.AddMealItem;

public sealed class AddMealItemCommandValidator : AbstractValidator<AddMealItemCommand>
{
    public AddMealItemCommandValidator()
    {
        RuleFor(x => x.MealId.Value).NotEmpty();
        RuleFor(x => x.Grocery).NotNull();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
