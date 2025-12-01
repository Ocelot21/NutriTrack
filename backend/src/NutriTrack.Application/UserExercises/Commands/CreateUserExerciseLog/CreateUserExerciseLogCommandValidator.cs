using FluentValidation;
using NutriTrack.Domain.Common;

namespace NutriTrack.Application.UserExercises.Commands.CreateUserExerciseLog;

public sealed class CreateUserExerciseLogCommandValidator : AbstractValidator<CreateUserExerciseLogCommand>
{
    public CreateUserExerciseLogCommandValidator()
    {
        RuleFor(x => x.UserId.Value).NotEmpty();
        RuleFor(x => x.DurationMinutes).GreaterThan(0).LessThanOrEqualTo(1440);
        RuleFor(x => x.LocalDate).NotEmpty();
        When(x => !string.IsNullOrWhiteSpace(x.Notes), () =>
        {
            RuleFor(x => x.Notes!).MaximumLength(DomainConstraints.UserExerciseLogs.MaxNotesLength);
        });
    }
}
