using FluentValidation;

namespace NutriTrack.Application.Meals.Commands.DeleteMeal;

public sealed class DeleteMealCommandValidator : AbstractValidator<DeleteMealCommand>
{
    public DeleteMealCommandValidator()
    {
        RuleFor(x => x.Id.Value).NotEmpty();
    }
}
