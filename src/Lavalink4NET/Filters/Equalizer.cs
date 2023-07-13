namespace Lavalink4NET.Filters;

using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Size = EqualizerData.Size)]
public sealed record class Equalizer
{
    public const int Bands = 15; // 0-14

    private readonly EqualizerData _data;

    public Equalizer() : this(data: default)
    {
    }

    private Equalizer(EqualizerData data)
    {
        _data = data;
    }

    public static Equalizer Default { get; } = new();

    public float Band0
    {
        get => this[0];
        init => this[0] = value;
    }

    public float Band1
    {
        get => this[1];
        init => this[1] = value;
    }

    public float Band2
    {
        get => this[2];
        init => this[2] = value;
    }

    public float Band3
    {
        get => this[3];
        init => this[3] = value;
    }

    public float Band4
    {
        get => this[4];
        init => this[4] = value;
    }

    public float Band5
    {
        get => this[5];
        init => this[5] = value;
    }

    public float Band6
    {
        get => this[6];
        init => this[6] = value;
    }

    public float Band7
    {
        get => this[7];
        init => this[7] = value;
    }

    public float Band8
    {
        get => this[8];
        init => this[8] = value;
    }

    public float Band9
    {
        get => this[9];
        init => this[9] = value;
    }

    public float Band10
    {
        get => this[10];
        init => this[10] = value;
    }

    public float Band11
    {
        get => this[11];
        init => this[11] = value;
    }

    public float Band12
    {
        get => this[12];
        init => this[12] = value;
    }

    public float Band13
    {
        get => this[13];
        init => this[13] = value;
    }

    public float Band14
    {
        get => this[14];
        init => this[14] = value;
    }

    public float this[int band]
    {
        get => _data[band];
        init => _data[band] = value;
    }

    public static Builder CreateBuilder(Equalizer equalizer) => new(equalizer._data);

    public static Builder CreateBuilder() => default;

    public struct Builder
    {
        private EqualizerData _equalizer;

        internal Builder(EqualizerData data)
        {
            _equalizer = data;
        }

        public float this[int band]
        {
            get => _equalizer[band];
            set => _equalizer[band] = value;
        }

        public float Band0
        {
            get => this[0];
            set => this[0] = value;
        }

        public float Band1
        {
            get => this[1];
            set => this[1] = value;
        }

        public float Band2
        {
            get => this[2];
            set => this[2] = value;
        }

        public float Band3
        {
            get => this[3];
            set => this[3] = value;
        }

        public float Band4
        {
            get => this[4];
            set => this[4] = value;
        }

        public float Band5
        {
            get => this[5];
            set => this[5] = value;
        }

        public float Band6
        {
            get => this[6];
            set => this[6] = value;
        }

        public float Band7
        {
            get => this[7];
            set => this[7] = value;
        }

        public float Band8
        {
            get => this[8];
            set => this[8] = value;
        }

        public float Band9
        {
            get => this[9];
            set => this[9] = value;
        }

        public float Band10
        {
            get => this[10];
            set => this[10] = value;
        }

        public float Band11
        {
            get => this[11];
            set => this[11] = value;
        }

        public float Band12
        {
            get => this[12];
            set => this[12] = value;
        }

        public float Band13
        {
            get => this[13];
            set => this[13] = value;
        }

        public float Band14
        {
            get => this[14];
            set => this[14] = value;
        }

        public readonly Equalizer Build() => new(_equalizer);
    }
}

internal struct EqualizerData
{
    public const int Size = Equalizer.Bands * sizeof(float);

    public unsafe fixed float Data[Equalizer.Bands];

    private unsafe ref float GetBand(int band)
    {
        if (band is < 0 or >= Equalizer.Bands)
        {
            throw new ArgumentOutOfRangeException(
                paramName: nameof(band),
                actualValue: band,
                message: $"The band index must be between zero and {Equalizer.Bands - 1}");
        }

        return ref Data[band];
    }

    public float this[int band]
    {
        get => GetBand(band);
        set => GetBand(band) = Math.Clamp(value, -0.25F, 1.0F);
    }
}