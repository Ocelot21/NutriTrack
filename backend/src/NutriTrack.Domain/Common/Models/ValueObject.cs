namespace NutriTrack.Domain.Common.Models;

public abstract class ValueObject
{
    protected abstract IEnumerable<object?> GetAtomicValues();

    public override bool Equals(object? obj)
    {
        if (obj is not ValueObject other)
            return false;

        return GetAtomicValues().SequenceEqual(other.GetAtomicValues());
    }

    public override int GetHashCode()
        => GetAtomicValues()
            .Where(v => v is not null)
            .Aggregate(0, (hash, value) => HashCode.Combine(hash, value!));
}
