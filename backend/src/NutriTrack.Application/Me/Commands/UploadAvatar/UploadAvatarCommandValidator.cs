using FluentValidation;

namespace NutriTrack.Application.Me.Commands.UploadAvatar;

public sealed class UploadAvatarCommandValidator : AbstractValidator<UploadAvatarCommand>
{
    public UploadAvatarCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotNull();

        RuleFor(x => x.Image)
            .NotNull();

        RuleFor(x => x.FileName)
            .NotEmpty();

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .Must(ct => ct.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            .WithMessage("ContentType must be an image/* content type.");
    }
}
