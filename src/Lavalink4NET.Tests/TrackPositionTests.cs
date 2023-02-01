namespace Lavalink4NET.Tests;

using System;
using Lavalink4NET.Players;
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
