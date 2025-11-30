using FluentValidation;

namespace NutriTrack.Application.UserExercises.Commands.UpdateUserExerciseLog;

public sealed class UpdateUserExerciseLogCommandValidator : AbstractValidator<UpdateUserExerciseLogCommand>
{
    public UpdateUserExerciseLogCommandValidator()
    {
        RuleFor(x => x.Id.Value).NotEmpty();
        When(x => x.DurationMinutes.HasValue, () =>
        {
            RuleFor(x => x.DurationMinutes!.Value).GreaterThan(0).LessThanOrEqualTo(1440);
        });
        When(x => x.Notes is not null, () =>
        {
            RuleFor(x => x.Notes!).MaximumLength(1000);
        });
    }
}
