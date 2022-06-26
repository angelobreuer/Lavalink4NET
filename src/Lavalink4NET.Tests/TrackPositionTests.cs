/*
 *  File:   TrackPositionTests.cs
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
    using Lavalink4NET.Player;
    using System;
    using Xunit;

    public class TrackPositionTests
    {
        [Theory]
        [InlineData(10, 20, 10, 1F)]
        [InlineData(20, 20, 20, 1F)]
        [InlineData(20, 40, 20, 1F)]
        public void TestUnsyncedDuration(long syncedAtUnits, long relativePositionUnits, long unsyncedDurationUnits, float timeStretchFactor)
        {
            var position = new TrackPosition(
                syncedAt: DateTimeOffset.UtcNow - TimeSpan.FromSeconds(syncedAtUnits),
                unstretchedRelativePosition: TimeSpan.FromSeconds(relativePositionUnits),
                timeStretchFactor: timeStretchFactor);

            Assert.Equal(unsyncedDurationUnits, position.UnsyncedDuration.Seconds);
        }

        [Theory]
        [InlineData(10, 20, 20, 2F)]
        [InlineData(20, 20, 10, 0.5F)]
        [InlineData(20, 40, 5, 0.25F)]
        public void TestUnsyncedDurationScaled(long syncedAtUnits, long relativePositionUnits, long unsyncedDurationUnits, float timeStretchFactor)
        {
            var position = new TrackPosition(
                syncedAt: DateTimeOffset.UtcNow - TimeSpan.FromSeconds(syncedAtUnits),
                unstretchedRelativePosition: TimeSpan.FromSeconds(relativePositionUnits),
                timeStretchFactor: timeStretchFactor);

            Assert.Equal(unsyncedDurationUnits, position.UnsyncedDuration.Seconds);
        }
    }
}
