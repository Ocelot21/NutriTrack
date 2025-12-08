using FluentValidation;

namespace NutriTrack.Application.WeightHistoryEntries.Commands.CreateWeightHistoryEntry;

public sealed class CreateWeightHistoryEntryCommandValidator : AbstractValidator<CreateWeightHistoryEntryCommand>
{
    public CreateWeightHistoryEntryCommandValidator()
    {
        RuleFor(x => x.UserId.Value).NotEmpty();
        RuleFor(x => x.Date).NotEmpty();
        RuleFor(x => x.WeightKg).GreaterThan(0);
    }
}
