/*
 *  File:   EndianConverterTests.cs
 *  Author: Angelo Breuer
 *
 *  The MIT License (MIT)
 *
 *  Copyright (c) Angelo Breuer 2022
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

namespace Lavalink4NET.Tests
{
    using Lavalink4NET.Decoding;
    using Xunit;

    /// <summary>
    ///     Contains tests for load balancing strategies ( <see cref="EndianConverter"/>).
    /// </summary>
    public sealed class EndianConverterTests
    {
        /// <summary>
        ///     Tests the endian converter endian conversion.
        /// </summary>
        /// <param name="source">the source data array</param>
        /// <param name="expect">the expected data array</param>
        /// <param name="sourceEndianness">
        ///     the source endianness of the specified <paramref name="source"/> data array
        /// </param>
        /// <param name="targetEndianness">
        ///     the target endianness of the specified <paramref name="expect"/> data array
        /// </param>
        [Theory]
        [InlineData(new byte[] { 1, 2 }, new byte[] { 1, 2 }, Endianness.BigEndian, Endianness.BigEndian)]
        [InlineData(new byte[] { 1, 2 }, new byte[] { 2, 1 }, Endianness.BigEndian, Endianness.LittleEndian)]
        [InlineData(new byte[] { 2, 1 }, new byte[] { 1, 2 }, Endianness.BigEndian, Endianness.LittleEndian)]
        [InlineData(new byte[] { 1, 2 }, new byte[] { 1, 2 }, Endianness.LittleEndian, Endianness.LittleEndian)]
        public void TestConvert(byte[] source, byte[] expect, Endianness sourceEndianness, Endianness targetEndianness)
        {
            // swap endian of the source
            EndianConverter.Convert(source, sourceEndianness, targetEndianness);

            // compare source -> swapped and expected array
            Assert.Equal(source, expect);
        }
    }
}
