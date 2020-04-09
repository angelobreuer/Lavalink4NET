/*
 *  File:   EndianConverter.cs
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

namespace Lavalink4NET.Decoding
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    ///     A utility class for conversion around endianness.
    /// </summary>
    public static class EndianConverter
    {
        /// <summary>
        ///     Gets the system endianness.
        /// </summary>
        public static Endianness SystemEndianess
            => BitConverter.IsLittleEndian ? Endianness.LittleEndian : Endianness.BigEndian;

        /// <summary>
        ///     Converts the <paramref name="data"/> endianness.
        /// </summary>
        /// <param name="data">the data to convert</param>
        /// <param name="source">the source endianness</param>
        /// <param name="target">the target endianness</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="data"/> is <see langword="null"/>.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Convert(byte[] data, Endianness source, Endianness target)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            // no conversion needed
            if (source == target)
            {
                return;
            }

            // reverse endian
            Array.Reverse(data);
        }
    }
}