using NutriTrack.Domain.Common.Errors;
using NutriTrack.Domain.Common.Models;
using NutriTrack.Domain.Common;

namespace NutriTrack.Domain.Groceries;

public sealed class MacroNutrients : ValueObject
{
    public decimal ProteinGramsPer100 { get; }
    public decimal CarbsGramsPer100 { get; }
    public decimal FatGramsPer100 { get; }

    public MacroNutrients(decimal proteinGramsPer100, decimal carbsGramsPer100, decimal fatGramsPer100)
    {
        ValidateNonNegativeAndUpperBound(proteinGramsPer100, nameof(ProteinGramsPer100));
        ValidateNonNegativeAndUpperBound(carbsGramsPer100, nameof(CarbsGramsPer100));
        ValidateNonNegativeAndUpperBound(fatGramsPer100, nameof(FatGramsPer100));

        ProteinGramsPer100 = proteinGramsPer100;
        CarbsGramsPer100 = carbsGramsPer100;
        FatGramsPer100 = fatGramsPer100;
    }

    private static void ValidateNonNegativeAndUpperBound(decimal value, string name)
    {
        if (value < 0 || value >= DomainConstraints.Groceries.MaxMacroValuePer100)
        {
            throw new DomainException(
                DomainErrorCodes.Groceries.InvalidMacros,
                $"{name} must be between 0 and less than {DomainConstraints.Groceries.MaxMacroValuePer100} grams per 100g."
                );
        }
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        // Equality based on atomic values.
        yield return ProteinGramsPer100;
        yield return CarbsGramsPer100;
        yield return FatGramsPer100;
    }
}
