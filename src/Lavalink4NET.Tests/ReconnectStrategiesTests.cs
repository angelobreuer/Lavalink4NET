/*
 *  File:   ReconnectStrategiesTests.cs
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

using System;
using Xunit;

namespace Lavalink4NET.Tests
{
    /// <summary>
    ///     Contains test for the <see cref="ReconnectStrategies"/>.
    /// </summary>
    public sealed class ReconnectStrategiesTests
    {
        /// <summary>
        ///     Test the <see cref="ReconnectStrategies.DefaultStrategy"/> reconnect strategy logic.
        /// </summary>
        [Fact]
        public void DefaultReconnectStrategyTest()
        {
            var strategy = ReconnectStrategies.DefaultStrategy;

            Assert.Equal(TimeSpan.FromSeconds(5 * 3), strategy(DateTimeOffset.UtcNow, 1));
            Assert.Equal(TimeSpan.FromSeconds(5 * 3), strategy(DateTimeOffset.UtcNow, 2));
            Assert.Equal(TimeSpan.FromSeconds(5 * 3), strategy(DateTimeOffset.UtcNow, 3));
            Assert.Equal(TimeSpan.FromSeconds(5 * 4), strategy(DateTimeOffset.UtcNow, 4));
            Assert.Equal(TimeSpan.FromSeconds(5 * 50), strategy(DateTimeOffset.UtcNow, 50));
            Assert.Equal(TimeSpan.FromSeconds(5 * 50000), strategy(DateTimeOffset.UtcNow, 50000));
        }

        /// <summary>
        ///     Test the <see cref="ReconnectStrategies.None"/> reconnect strategy logic.
        /// </summary>
        [Fact]
        public void NoneReconnectStrategyTest()
        {
            var strategy = ReconnectStrategies.None;

            Assert.Null(strategy(DateTimeOffset.UtcNow, 0));
            Assert.Null(strategy(DateTimeOffset.UtcNow, 3));
            Assert.Null(strategy(DateTimeOffset.UtcNow, 10));
            Assert.Null(strategy(DateTimeOffset.UtcNow, 10000));
        }
    }
}
