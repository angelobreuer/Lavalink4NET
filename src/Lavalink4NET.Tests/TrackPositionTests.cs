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
                relativePosition: TimeSpan.FromSeconds(relativePositionUnits),
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
                relativePosition: TimeSpan.FromSeconds(relativePositionUnits),
                timeStretchFactor: timeStretchFactor);

            Assert.Equal(unsyncedDurationUnits, position.UnsyncedDuration.Seconds);
        }

        [Theory]
        [InlineData(0, 0, 1F, 0, 1F, 0)] // Not stretched, track beginning
        [InlineData(20, 0, 1F, 0, 1F, 20)] // Synced before 20, with relative position 0, no stretch, resync now
        [InlineData(40, 0, 1F, 0, 1F, 40)] // Synced before 40, with relative position 0, no stretch, resync now
        [InlineData(40, 0, 2F, 0, 1F, 80)] // Synced before 40, with relative position 0, 2x stretch, resync now
        [InlineData(40, 0, 4F, 0, 1F, 160)] // Synced before 40, with relative position 0, 4x stretch, resync now
        [InlineData(40, 0, 0.5F, 0, 1F, 20)] // Synced before 40, with relative position 0, 0.5x stretch, resync now
        [InlineData(40, 0, 0.25F, 0, 1F, 10)] // Synced before 40, with relative position 0, 0.25x stretch, resync now
        [InlineData(40, 0, 2F, 20, 1F, 60)] // Synced before 40 with 2x stretch, with relative position 0, no stretch, resync before 20
        [InlineData(40, 0, 1F, 20, 1F, 40)] // Synced before 40, with relative position 0, no stretch, resync before 20
        [InlineData(40, 20, 1F, 20, 1F, 60)] // Synced before 40, with relative position 0, no stretch, resync before 20
        [InlineData(40, 20, 2F, 20, 1F, 80)] // Synced before 40 with 2x stretch, with relative position 0, no stretch, resync before 20
        public void FixAndStretchTests(
            long syncedAtUnits,
            long relativePositionUnits,
            float oldTimeStretchFactor,
            long resyncTimestampUnits,
            float resyncTimeStretchFactor,
            long expectedPositionUnits)
        {
            var utcNow = DateTimeOffset.UtcNow;

            var syncedAt = utcNow - TimeSpan.FromSeconds(syncedAtUnits);
            var relativePosition = TimeSpan.FromSeconds(relativePositionUnits);

            var resyncTimestamp = utcNow - TimeSpan.FromSeconds(resyncTimestampUnits);

            var trackPosition = new TrackPosition(syncedAt, relativePosition, oldTimeStretchFactor);
            var newTrackPosition = trackPosition.FixAndStretch(resyncTimestamp, resyncTimeStretchFactor);

            Assert.Equal(expectedPositionUnits, (long)newTrackPosition.Position.TotalSeconds);
        }
    }
}
