using FluentValidation;

namespace NutriTrack.Application.Exercises.Commands.DeleteExercise;

public sealed class DeleteExerciseCommandValidator : AbstractValidator<DeleteExerciseCommand>
{
    public DeleteExerciseCommandValidator()
    {
        RuleFor(x => x.Id.Value).NotEmpty();
    }
}
