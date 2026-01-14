using FluentValidation;

namespace NutriTrack.Application.Social.Commands.DeleteSocialPost;

public sealed class DeleteSocialPostCommandValidator : AbstractValidator<DeleteSocialPostCommand>
{
    public DeleteSocialPostCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.PostId)
            .NotEmpty();
    }
}
