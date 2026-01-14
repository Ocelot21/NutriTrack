using FluentValidation;

namespace NutriTrack.Application.Social.Commands.ShareDailyOverviewSnapshot;

public sealed class ShareDailyOverviewSnapshotCommandValidator : AbstractValidator<ShareDailyOverviewSnapshotCommand>
{
    public ShareDailyOverviewSnapshotCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Caption)
            .MaximumLength(500)
            .When(x => x.Caption is not null);

        RuleFor(x => x.Visibility)
            .InclusiveBetween(1, 3)
            .When(x => x.Visibility.HasValue);
    }
}
