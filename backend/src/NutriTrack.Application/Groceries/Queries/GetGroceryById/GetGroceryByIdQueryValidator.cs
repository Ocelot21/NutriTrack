using FluentValidation;

namespace NutriTrack.Application.Groceries.Queries.GetGroceryById;

public sealed class GetGroceryByIdQueryValidator : AbstractValidator<GetGroceryByIdQuery>
{
    public GetGroceryByIdQueryValidator()
    {
        RuleFor(x => x.Id.Value)
            .NotEmpty();
    }
}
