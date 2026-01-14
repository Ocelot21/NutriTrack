using FluentValidation;

namespace NutriTrack.Application.Social.Commands.ShareAchievementPost;

public sealed class ShareAchievementPostCommandValidator : AbstractValidator<ShareAchievementPostCommand>
{
    public ShareAchievementPostCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.UserAchievementId)
            .NotEmpty();

        RuleFor(x => x.Caption)
            .MaximumLength(500)
            .When(x => x.Caption is not null);

        RuleFor(x => x.Visibility)
            .InclusiveBetween(1, 3)
            .When(x => x.Visibility.HasValue);
    }
}
