using NutriTrack.Domain.Common.Errors;
using NutriTrack.Domain.Common.Models;
using NutriTrack.Domain.Common; // DomainConstraints

namespace NutriTrack.Domain.Groceries;

public sealed class MacroNutrients : ValueObject
{
    public decimal ProteinGramsPer100g { get; }
    public decimal CarbsGramsPer100g { get; }
    public decimal FatGramsPer100g { get; }

    public MacroNutrients(decimal proteinGramsPer100g, decimal carbsGramsPer100g, decimal fatGramsPer100g)
    {
        // TODO: Validation boundaries may be adjusted in the future.
        ValidateNonNegativeAndUpperBound(proteinGramsPer100g, nameof(ProteinGramsPer100g));
        ValidateNonNegativeAndUpperBound(carbsGramsPer100g, nameof(CarbsGramsPer100g));
        ValidateNonNegativeAndUpperBound(fatGramsPer100g, nameof(FatGramsPer100g));

        ProteinGramsPer100g = proteinGramsPer100g;
        CarbsGramsPer100g = carbsGramsPer100g;
        FatGramsPer100g = fatGramsPer100g;
    }

    private static void ValidateNonNegativeAndUpperBound(decimal value, string name)
    {
        if (value < 0 || value >= DomainConstraints.Groceries.MaxMacroValuePer100g)
        {
            throw new DomainException(
                DomainErrorCodes.Groceries.InvalidMacros,
                $"{name} must be between 0 and less than {DomainConstraints.Groceries.MaxMacroValuePer100g} grams per 100g."
                );
        }
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        // Equality based on atomic values.
        yield return ProteinGramsPer100g;
        yield return CarbsGramsPer100g;
        yield return FatGramsPer100g;
    }
}
