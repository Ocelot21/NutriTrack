using FluentValidation;
using NutriTrack.Domain.Common;
using NutriTrack.Domain.Groceries;

namespace NutriTrack.Application.Groceries.Commands.CreateGrocery;

public sealed class CreateGroceryCommandValidator : AbstractValidator<CreateGroceryCommand>
{
    public CreateGroceryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(DomainConstraints.Groceries.MaxNameLength);

        RuleFor(x => x.Category)
            .IsInEnum();

        RuleFor(x => x.ProteinPer100)
            .GreaterThanOrEqualTo(0).LessThan(DomainConstraints.Groceries.MaxMacroValuePer100);
        RuleFor(x => x.CarbsPer100)
            .GreaterThanOrEqualTo(0).LessThan(DomainConstraints.Groceries.MaxMacroValuePer100);
        RuleFor(x => x.FatPer100)
            .GreaterThanOrEqualTo(0).LessThan(DomainConstraints.Groceries.MaxMacroValuePer100);

        RuleFor(x => x.CaloriesPer100)
            .GreaterThanOrEqualTo(0).LessThan(DomainConstraints.Groceries.MaxCaloriesPer100);

        RuleFor(x => x.Barcode)
            .MaximumLength(DomainConstraints.Groceries.MaxBarcodeLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Barcode));
    }
}
