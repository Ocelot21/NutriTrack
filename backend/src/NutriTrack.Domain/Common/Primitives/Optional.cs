namespace NutriTrack.Domain.Common.Primitives;

public readonly struct Optional<T>
{
    public bool IsSet { get; }
    private readonly T? _value;

    public T Value => IsSet
        ? _value!
        : throw new InvalidOperationException("Optional has no value.");

    private Optional(T value)
    {
        IsSet = true;
        _value = value;
    }

    public static Optional<T> None() => default;

    public static implicit operator Optional<T>(T value) => new(value);
}