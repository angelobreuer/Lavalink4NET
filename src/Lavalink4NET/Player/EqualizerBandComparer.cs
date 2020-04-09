/*
 *  File:   EqualizerBandComparer.cs
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

namespace Lavalink4NET.Player
{
    using System.Collections.Generic;

    internal sealed class EqualizerBandComparer : IEqualityComparer<EqualizerBand>
    {
        private static IEqualityComparer<EqualizerBand> _instance;

        /// <summary>
        ///     Gets the shared instance of the <see cref="EqualizerBandComparer"/> class.
        /// </summary>
        public static IEqualityComparer<EqualizerBand> Instance
            => _instance ?? (_instance = new EqualizerBandComparer());

        /// <summary>
        ///     Checks the equality of the first <paramref name="band1"/> and the second
        ///     <paramref name="band2"/> by comparing it's gain value.
        /// </summary>
        /// <param name="band1">the first equalizer band</param>
        /// <param name="band2">the second equalizer band</param>
        /// <returns>a value indicating whether the first band is equal to the second</returns>
        public bool Equals(EqualizerBand band1, EqualizerBand band2)
            => band1.Band.Equals(band2.Band);

        /// <summary>
        ///     Gets the hash-code of the specified <paramref name="band"/> without the gain included.
        /// </summary>
        /// <param name="band">the equalizer band</param>
        /// <returns>the hash code</returns>
        public int GetHashCode(EqualizerBand band)
            => band.Band.GetHashCode();
    }
}