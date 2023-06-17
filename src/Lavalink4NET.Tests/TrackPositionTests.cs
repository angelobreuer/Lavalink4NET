namespace Lavalink4NET.Tests;

using System;
using Lavalink4NET.Players;
using Microsoft.Extensions.Internal;
using Xunit;

public class TrackPositionTests
{
    [Theory]
    [InlineData(10, 20, 10, 1F)] // Non scaled
    [InlineData(20, 20, 20, 1F)]
    [InlineData(20, 40, 20, 1F)]

    [InlineData(10, 20, 20, 2F)] // Scaled
    [InlineData(20, 20, 10, 0.5F)]
    [InlineData(20, 40, 5, 0.25F)]
    public void TestUnsyncedDuration(long syncedAtUnits, long relativePositionUnits, long unsyncedDurationUnits, float timeStretchFactor)
    {
        var position = new TrackPosition(
            SystemClock: new SystemClock(),
            SyncedAt: DateTimeOffset.UtcNow - TimeSpan.FromSeconds(syncedAtUnits),
            UnstretchedRelativePosition: TimeSpan.FromSeconds(relativePositionUnits),
            TimeStretchFactor: timeStretchFactor);

        Assert.Equal(unsyncedDurationUnits, position.UnsyncedDuration.Seconds);
    }
}
