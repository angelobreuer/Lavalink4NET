namespace Lavalink4NET.Tests;

using System;
using Lavalink4NET.Decoding;
using Lavalink4NET.Player;
using Xunit;

public sealed class TrackEncoderTests
{
    [Fact]
    public void TestTrackDecodeEncodeRoundTrip()
    {
        var trackInfo = new LavalinkTrackInfo
        {
            Author = "Test Author",
            Duration = TimeSpan.FromSeconds(10),
            IsLiveStream = true,
            IsSeekable = false,
            Position = default,
            Source = "https://example.com/",
            Title = "Test Title",
            TrackIdentifier = "abcd",
        };

        var track = TrackEncoder.Encode(trackInfo);

        // decode back
        var actualTrackInfo = TrackDecoder.Decode(track);

        Assert.Equal(trackInfo.Author, actualTrackInfo.Author);
        Assert.Equal(trackInfo.Duration, actualTrackInfo.Duration);
        Assert.Equal(trackInfo.IsLiveStream, actualTrackInfo.IsLiveStream);
        Assert.Equal(trackInfo.IsSeekable, actualTrackInfo.IsSeekable);
        Assert.Equal(trackInfo.Position, actualTrackInfo.Position);
        Assert.Equal(trackInfo.Source, actualTrackInfo.Source);
        Assert.Equal(trackInfo.Title, actualTrackInfo.Title);
        Assert.Equal(trackInfo.TrackIdentifier, actualTrackInfo.TrackIdentifier);
    }
}
