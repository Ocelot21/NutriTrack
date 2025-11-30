using FluentValidation;
using NutriTrack.Domain.Common;

namespace NutriTrack.Application.Authentication.Commands.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(DomainConstraints.Users.MaxEmailLength)
            .Matches(DomainPatterns.BasicEmailPattern)
            .WithMessage("Email format is invalid.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(200);
    }
}
