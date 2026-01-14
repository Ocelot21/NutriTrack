using FluentValidation;

namespace NutriTrack.Application.Me.Commands.UpdateUserProfile;

public sealed class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotNull();

        RuleFor(x => x)
            .Must(x =>
                x.Email is not null ||
                x.Username is not null ||
                x.FirstName is not null ||
                x.LastName is not null ||
                x.TimeZoneId is not null ||
                x.CountryIso2 is not null)
            .WithMessage("At least one field must be provided.");

        When(x => x.Email is not null, () =>
        {
            RuleFor(x => x.Email!)
                .NotEmpty()
                .EmailAddress();
        });

        When(x => x.Username is not null, () =>
        {
            RuleFor(x => x.Username!)
                .NotEmpty();
        });

        When(x => x.FirstName is not null, () =>
        {
            RuleFor(x => x.FirstName!)
                .NotEmpty();
        });

        When(x => x.LastName is not null, () =>
        {
            RuleFor(x => x.LastName!)
                .NotEmpty();
        });

        When(x => x.TimeZoneId is not null, () =>
        {
            RuleFor(x => x.TimeZoneId!)
                .NotEmpty();
        });

        When(x => x.CountryIso2 is not null, () =>
        {
            RuleFor(x => x.CountryIso2!)
                .NotEmpty()
                .Length(2);
        });
    }
}
