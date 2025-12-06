using FluentValidation;

namespace NutriTrack.Application.Groceries.Queries.ListGroceries;

public sealed class ListGroceriesQueryValidator : AbstractValidator<ListGroceriesQuery>
{
    public ListGroceriesQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).When(x => x.Page.HasValue);

        RuleFor(x => x.PageSize)
            .GreaterThan(0).When(x => x.PageSize.HasValue);
    }
}