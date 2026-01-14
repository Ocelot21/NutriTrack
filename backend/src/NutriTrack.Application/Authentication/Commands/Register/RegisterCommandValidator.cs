using FluentValidation;
using NutriTrack.Domain.Common;

namespace NutriTrack.Application.Authentication.Commands.Register
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(DomainConstraints.Users.MaxNameLength);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(DomainConstraints.Users.MaxNameLength);

            RuleFor(x => x.Username)
                .NotEmpty()
                .MinimumLength(DomainConstraints.Users.MinUsernameLength)
                .MaximumLength(DomainConstraints.Users.MaxUsernameLength)
                .Matches(DomainPatterns.UsernamePattern)
                .WithMessage("Username format is invalid.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .MaximumLength(DomainConstraints.Users.MaxEmailLength)
                .Matches(DomainPatterns.BasicEmailPattern)
                .WithMessage("Email format is invalid.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(4)
                .MaximumLength(200);

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .Equal(x => x.Password)
                .WithMessage("Passwords do not match.");

            RuleFor(x => x.CountryIso2)
                .Cascade(CascadeMode.Stop)
                .Must(v => string.IsNullOrWhiteSpace(v) || DomainPatterns.CountryIso2Regex.IsMatch(v.Trim().ToUpperInvariant()))
                .WithMessage("Country must be ISO-3166 alpha-2 (e.g., BA, HR, US).");

            RuleFor(x => x.TimeZoneId)
                .NotEmpty();
        }
    }
}
