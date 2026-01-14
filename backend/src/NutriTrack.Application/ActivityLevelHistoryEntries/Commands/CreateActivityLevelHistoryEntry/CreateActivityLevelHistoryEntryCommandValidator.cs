using FluentValidation;

namespace NutriTrack.Application.ActivityLevelHistoryEntries.Commands.CreateActivityLevelHistoryEntry;

public sealed class CreateActivityLevelHistoryEntryCommandValidator : AbstractValidator<CreateActivityLevelHistoryEntryCommand>
{
    public CreateActivityLevelHistoryEntryCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotNull();

        RuleFor(x => x.EffectiveFrom)
            .NotEmpty();

        RuleFor(x => x.ActivityLevel)
            .IsInEnum();
    }
}
