using FluentValidation;

namespace NutriTrack.Application.UserExercises.Queries.ListUserExerciseLogs;

public sealed class ListUserExerciseLogsQueryValidator : AbstractValidator<ListUserExerciseLogsQuery>
{
    public ListUserExerciseLogsQueryValidator()
    {
        RuleFor(x => x.UserId.Value).NotEmpty();

        When(x => x.From.HasValue && x.To.HasValue, () =>
        {
            RuleFor(x => x.To!.Value).GreaterThanOrEqualTo(x => x.From!.Value);
        });
    }
}
