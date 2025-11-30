using FluentValidation;

namespace NutriTrack.Application.Me.Commands.UpdateHealthProfile;

public sealed class UpdateHealthProfileCommandValidator : AbstractValidator<UpdateHealthProfileCommand>
{
    public UpdateHealthProfileCommandValidator()
    {
        RuleFor(x => x.UserId)
            .Must(id => id.Value != Guid.Empty)
            .WithMessage("User id is required.");

        RuleFor(x => x.HeightCm)
            .GreaterThan(0).When(x => x.HeightCm.HasValue)
            .LessThanOrEqualTo(300).When(x => x.HeightCm.HasValue);

        RuleFor(x => x.WeightKg)
            .GreaterThan(0).When(x => x.WeightKg.HasValue)
            .LessThanOrEqualTo(500).When(x => x.WeightKg.HasValue);

        RuleFor(x => x.Birthdate)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow)).When(x => x.Birthdate.HasValue);

        RuleFor(x => x.Gender)
            .IsInEnum().When(x => x.Gender.HasValue);

        RuleFor(x => x.ActivityLevel)
            .IsInEnum().When(x => x.ActivityLevel.HasValue);
    }
}
