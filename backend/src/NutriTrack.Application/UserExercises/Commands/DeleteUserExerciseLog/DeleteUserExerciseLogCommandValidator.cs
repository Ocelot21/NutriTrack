using FluentValidation;

namespace NutriTrack.Application.UserExercises.Commands.DeleteUserExerciseLog;

public sealed class DeleteUserExerciseLogCommandValidator : AbstractValidator<DeleteUserExerciseLogCommand>
{
    public DeleteUserExerciseLogCommandValidator()
    {
        RuleFor(x => x.Id.Value).NotEmpty();
    }
}
