namespace Lavalink4NET.Protocol;

using System;

public readonly struct Optional<T>
{
    private readonly T _value;

    public static Optional<T> Default => default;

    public T Value
    {
        get
        {
            if (!IsPresent)
            {
                ThrowNotPresent();
            }

            return _value;

            static void ThrowNotPresent() => throw new InvalidOperationException("Optional not present.");
        }
    }

    public T? GetValueOrDefault()
    {
        return _value;
    }

    public T? GetValueOrDefault(T defaultValue)
    {
        return IsPresent ? _value : defaultValue;
    }

    public bool IsPresent { get; }

    public Optional(T value)
    {
        _value = value;
        IsPresent = true;
    }
}
