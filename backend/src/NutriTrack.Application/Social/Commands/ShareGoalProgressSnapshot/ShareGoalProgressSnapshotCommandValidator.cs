using FluentValidation;

namespace NutriTrack.Application.Social.Commands.ShareGoalProgressSnapshot;

public sealed class ShareGoalProgressSnapshotCommandValidator : AbstractValidator<ShareGoalProgressSnapshotCommand>
{
    public ShareGoalProgressSnapshotCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.UserGoalId)
            .NotEmpty();

        RuleFor(x => x.Caption)
            .MaximumLength(500)
            .When(x => x.Caption is not null);

        RuleFor(x => x.Visibility)
            .InclusiveBetween(1, 3)
            .When(x => x.Visibility.HasValue);
    }
}
