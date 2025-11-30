using FluentValidation;

namespace NutriTrack.Application.Exercises.Queries.ListExercises;

public sealed class ListExercisesQueryValidator : AbstractValidator<ListExercisesQuery>
{
    public ListExercisesQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0).When(x => x.Page.HasValue);
        RuleFor(x => x.PageSize).GreaterThan(0).When(x => x.PageSize.HasValue);
    }
}
