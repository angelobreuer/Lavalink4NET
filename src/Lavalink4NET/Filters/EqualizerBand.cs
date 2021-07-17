/*
 *  File:   EqualizerBand.cs
 *  Author: Angelo Breuer
 *
 *  The MIT License (MIT)
 *
 *  Copyright (c) Angelo Breuer 2020
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in
 *  all copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 *  THE SOFTWARE.
 */

namespace Lavalink4NET.Filters
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

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
            if (band < 0 || band > 14)
            {
                throw new ArgumentOutOfRangeException(nameof(band), band, "There are only 14 bands: 0-14.");
            }

            if (gain < -.25f || gain > 1f)
            {
                throw new ArgumentOutOfRangeException(nameof(gain), gain, "Gain out of range: -0.25f - 1f");
            }

            Band = band;
            Gain = gain;
        }

        /// <summary>
        ///     Gets the equalizer band number (0-14).
        /// </summary>
        [JsonRequired, JsonProperty("band")]
        public int Band { get; }

        /// <summary>
        ///     Gets the band gain (-0.25f - 1f).
        /// </summary>
        [JsonRequired, JsonProperty("gain")]
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
}
