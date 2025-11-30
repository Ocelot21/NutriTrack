using FluentValidation;
using NutriTrack.Domain.Common;

namespace NutriTrack.Application.Groceries.Commands.UpdateGrocery;

public sealed class UpdateGroceryCommandValidator : AbstractValidator<UpdateGroceryCommand>
{
    public UpdateGroceryCommandValidator()
    {
        RuleFor(x => x.Id.Value).NotEmpty();

        When(x => x.Name is not null, () =>
        {
            RuleFor(x => x.Name!)
                .NotEmpty()
                .MaximumLength(DomainConstraints.Groceries.MaxNameLength);
        });

        When(x => x.Barcode is not null, () =>
        {
            RuleFor(x => x.Barcode!)
                .MaximumLength(DomainConstraints.Groceries.MaxBarcodeLength);
        });

        When(x => x.ProteinPer100g.HasValue, () =>
        {
            RuleFor(x => x.ProteinPer100g!.Value)
                .GreaterThanOrEqualTo(0).LessThan(DomainConstraints.Groceries.MaxMacroValuePer100g);
        });

        When(x => x.CarbsPer100g.HasValue, () =>
        {
            RuleFor(x => x.CarbsPer100g!.Value)
                .GreaterThanOrEqualTo(0).LessThan(DomainConstraints.Groceries.MaxMacroValuePer100g);
        });

        When(x => x.FatPer100g.HasValue, () =>
        {
            RuleFor(x => x.FatPer100g!.Value)
                .GreaterThanOrEqualTo(0).LessThan(DomainConstraints.Groceries.MaxMacroValuePer100g);
        });

        When(x => x.CaloriesPer100.HasValue, () =>
        {
            RuleFor(x => x.CaloriesPer100!.Value)
                .GreaterThanOrEqualTo(0).LessThan(DomainConstraints.Groceries.MaxCaloriesPer100g);
        });
    }
}
