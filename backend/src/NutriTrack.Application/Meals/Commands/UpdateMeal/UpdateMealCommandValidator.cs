using FluentValidation;
using NutriTrack.Domain.Common;

namespace NutriTrack.Application.Meals.Commands.UpdateMeal;

public sealed class UpdateMealCommandValidator : AbstractValidator<UpdateMealCommand>
{
    public UpdateMealCommandValidator()
    {
        RuleFor(x => x.Id.Value).NotEmpty();

        When(x => x.Name is not null, () =>
        {
            RuleFor(x => x.Name!)
                .NotEmpty()
                .MaximumLength(DomainConstraints.Meals.MaxMealNameLength);
        });

        When(x => x.Description is not null, () =>
        {
            RuleFor(x => x.Description!)
                .MaximumLength(DomainConstraints.Meals.MaxMealDescriptionLength);
        });
    }
}
