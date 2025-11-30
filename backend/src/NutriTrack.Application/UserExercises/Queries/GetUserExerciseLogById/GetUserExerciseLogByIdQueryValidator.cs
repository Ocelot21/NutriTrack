using FluentValidation;

namespace NutriTrack.Application.UserExercises.Queries.GetUserExerciseLogById;

public sealed class GetUserExerciseLogByIdQueryValidator : AbstractValidator<GetUserExerciseLogByIdQuery>
{
    public GetUserExerciseLogByIdQueryValidator()
    {
        RuleFor(x => x.Id.Value).NotEmpty();
    }
}
