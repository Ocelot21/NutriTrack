using FluentValidation;

namespace NutriTrack.Application.Meals.Queries.GetMealById;

public sealed class GetMealByIdQueryValidator : AbstractValidator<GetMealByIdQuery>
{
    public GetMealByIdQueryValidator()
    {
        RuleFor(x => x.Id.Value).NotEmpty();
    }
}
