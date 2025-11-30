using FluentValidation;
using NutriTrack.Domain.Common;

namespace NutriTrack.Application.Exercises.Commands.UpdateExercise;

public sealed class UpdateExerciseCommandValidator : AbstractValidator<UpdateExerciseCommand>
{
    public UpdateExerciseCommandValidator()
    {
        RuleFor(x => x.Id.Value).NotEmpty();

        When(x => x.Name is not null, () =>
        {
            RuleFor(x => x.Name!)
                .NotEmpty()
                .MaximumLength(DomainConstraints.Exercises.MaxNameLength);
        });

        When(x => x.Description is not null, () =>
        {
            RuleFor(x => x.Description!)
                .MaximumLength(DomainConstraints.Exercises.MaxDescriptionLength);
        });

        When(x => x.DefaultCaloriesPerMinute.HasValue, () =>
        {
            RuleFor(x => x.DefaultCaloriesPerMinute!.Value)
                .GreaterThanOrEqualTo(0).LessThan(DomainConstraints.Exercises.MaxDefaultCaloriesPerMinute);
        });
    }
}
