using FluentValidation;

namespace NutriTrack.Application.Exercises.Queries.ListExercises;

public sealed class ListExercisesQueryValidator : AbstractValidator<ListExercisesQuery>
{
    public ListExercisesQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0);
    }
}
