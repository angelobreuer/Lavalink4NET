namespace Lavalink4NET.Filters;

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit, Pack = 0, Size = Bands * 2)]
public readonly record struct Equalizer
{
    public const int Bands = 15; // 0-14

    [FieldOffset(0)]
    private readonly ushort _data;

    public static Equalizer Default => default;

    public float Band0 { get => this[0]; init => this[0] = value; }
    public float Band1 { get => this[1]; init => this[1] = value; }
    public float Band2 { get => this[2]; init => this[2] = value; }
    public float Band3 { get => this[3]; init => this[3] = value; }
    public float Band4 { get => this[4]; init => this[4] = value; }
    public float Band5 { get => this[5]; init => this[5] = value; }
    public float Band6 { get => this[6]; init => this[6] = value; }
    public float Band7 { get => this[7]; init => this[7] = value; }
    public float Band8 { get => this[8]; init => this[8] = value; }
    public float Band9 { get => this[9]; init => this[9] = value; }
    public float Band10 { get => this[10]; init => this[10] = value; }
    public float Band11 { get => this[11]; init => this[11] = value; }
    public float Band12 { get => this[12]; init => this[12] = value; }
    public float Band13 { get => this[13]; init => this[13] = value; }
    public float Band14 { get => this[14]; init => this[14] = value; }

    public float this[int band]
    {
        get
        {
            CheckIndex(band);

            var rawValue = Unsafe.Add(
                source: ref Unsafe.AsRef(_data),
                elementOffset: band);

            if (rawValue is 0)
            {
                return 0F; // fast path
            }

            var mappedValue = rawValue / (float)ushort.MaxValue * 1.25F;
            return mappedValue > 1.0F ? mappedValue - 1.5F : mappedValue;
        }

        init
        {
            CheckIndex(band);

            // normalize
            value = Math.Max(-0.25F, value);
            value = Math.Min(1.0F, value);

            // map
            if (value < 0F)
            {
                value = -value + 1.0F;
            }

            // stretch
            var rawValue = (ushort)Math.Round(value * ushort.MaxValue);

            // encode
            Unsafe.Add(
                source: ref _data,
                elementOffset: band) = rawValue;
        }
    }

    private static void CheckIndex(int band)
    {
        if (band is < 0 or >= Bands)
        {
            throw new ArgumentOutOfRangeException(
                paramName: nameof(band),
                actualValue: band,
                message: $"The band index must be between zero and {Bands - 1}");
        }
    }
}
