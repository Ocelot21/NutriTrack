using FluentValidation;

namespace NutriTrack.Application.Authentication.Commands.LoginTwoFactor;

public class LoginTwoFactorCommandValidator : AbstractValidator<LoginTwoFactorCommand>
{
    public LoginTwoFactorCommandValidator()
    {
        RuleFor(x => x.ChallengeId)
            .NotEmpty();

        RuleFor(x => x.Code)
            .NotEmpty()
            .Length(6);
    }
}
