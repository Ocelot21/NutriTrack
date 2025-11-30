using FluentValidation;

namespace NutriTrack.Application.Exercises.Queries.GetExerciseById;

public sealed class GetExerciseByIdQueryValidator : AbstractValidator<GetExerciseByIdQuery>
{
    public GetExerciseByIdQueryValidator()
    {
        RuleFor(x => x.Id.Value).NotEmpty();
    }
}
