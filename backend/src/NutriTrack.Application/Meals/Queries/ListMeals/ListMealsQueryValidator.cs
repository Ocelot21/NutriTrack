using FluentValidation;

namespace NutriTrack.Application.Meals.Queries.ListMeals;

public sealed class ListMealsQueryValidator : AbstractValidator<ListMealsQuery>
{
    public ListMealsQueryValidator()
    {
        RuleFor(x => x.UserId.Value).NotEmpty();

        When(x => x.From.HasValue && x.To.HasValue, () =>
        {
            RuleFor(x => x.To!.Value)
                .GreaterThanOrEqualTo(x => x.From!.Value);
        });
    }
}
