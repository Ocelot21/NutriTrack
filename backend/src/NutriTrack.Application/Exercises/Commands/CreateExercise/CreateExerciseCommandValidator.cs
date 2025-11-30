using FluentValidation;
using NutriTrack.Domain.Common;

namespace NutriTrack.Application.Exercises.Commands.CreateExercise;

public sealed class CreateExerciseCommandValidator : AbstractValidator<CreateExerciseCommand>
{
    public CreateExerciseCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(DomainConstraints.Exercises.MaxNameLength);

        RuleFor(x => x.Description)
            .MaximumLength(DomainConstraints.Exercises.MaxDescriptionLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Category)
            .IsInEnum();

        RuleFor(x => x.DefaultCaloriesPerMinute)
            .GreaterThanOrEqualTo(0).LessThan(DomainConstraints.Exercises.MaxDefaultCaloriesPerMinute);
    }
}
