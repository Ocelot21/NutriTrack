using FluentValidation;
using NutriTrack.Domain.Common;

namespace NutriTrack.Application.Meals.Commands.CreateMeal;

public sealed class CreateMealCommandValidator : AbstractValidator<CreateMealCommand>
{
    public CreateMealCommandValidator()
    {
        RuleFor(x => x.UserId.Value).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(DomainConstraints.Meals.MaxMealNameLength);
        RuleFor(x => x.LocalDate).NotEmpty();
    }
}
