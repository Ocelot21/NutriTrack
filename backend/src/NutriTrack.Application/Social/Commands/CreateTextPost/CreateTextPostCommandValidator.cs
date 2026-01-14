using FluentValidation;

namespace NutriTrack.Application.Social.Commands.CreateTextPost;

public sealed class CreateTextPostCommandValidator : AbstractValidator<CreateTextPostCommand>
{
    public CreateTextPostCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Text)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(x => x.Visibility)
            .InclusiveBetween(1, 3)
            .When(x => x.Visibility.HasValue);
    }
}
