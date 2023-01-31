namespace Lavalink4NET.Filters;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

/// <summary>
///     Represents a lavalink equalizer band.
/// </summary>
public sealed class EqualizerBand : IEquatable<EqualizerBand?>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="EqualizerBand"/> class.
    /// </summary>
    /// <param name="band">the equalizer band number (0-14)</param>
    /// <param name="gain">the band gain ( <c>-0.25f</c> - <c>1f</c>)</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     thrown if the specified <paramref name="band"/> is less than <c>1f</c> or greater
    ///     than <c>14</c>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     thrown if the specified <paramref name="gain"/> is less than <c>-0.25f</c> or
    ///     greater than <c>1f</c>.
    /// </exception>
    public EqualizerBand(int band, float gain)
    {
        if (band is < 0 or > 14)
        {
            throw new ArgumentOutOfRangeException(nameof(band), band, "There are only 14 bands: 0-14.");
        }

        if (gain is < (-.25f) or > 1f)
        {
            throw new ArgumentOutOfRangeException(nameof(gain), gain, "Gain out of range: -0.25f - 1f");
        }

        Band = band;
        Gain = gain;
    }

    /// <summary>
    ///     Gets the equalizer band number (0-14).
    /// </summary>
    [JsonPropertyName("band")]
    public int Band { get; }

    /// <summary>
    ///     Gets the band gain (-0.25f - 1f).
    /// </summary>
    [JsonPropertyName("gain")]
    public float Gain { get; }

    public static bool operator !=(EqualizerBand? left, EqualizerBand? right) => !(left == right);

    public static bool operator ==(EqualizerBand? left, EqualizerBand? right)
        => EqualityComparer<EqualizerBand?>.Default.Equals(left, right);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as EqualizerBand);

    /// <inheritdoc/>
    public bool Equals(EqualizerBand? other)
    {
        return other != null && Band == other.Band && Gain == other.Gain;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var hashCode = 1089811828;
        hashCode = (hashCode * -1521134295) + Band.GetHashCode();
        hashCode = (hashCode * -1521134295) + Gain.GetHashCode();
        return hashCode;
    }
}
