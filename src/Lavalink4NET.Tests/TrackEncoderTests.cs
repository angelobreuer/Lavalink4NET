namespace Lavalink4NET.Tests;

using System;
using Lavalink4NET.Decoding;
using Lavalink4NET.Player;
using Xunit;

public sealed class TrackEncoderTests
{
    [Theory]
    [InlineData("QAAAjAIAJFZhbmNlIEpveSAtICdSaXB0aWRlJyBPZmZpY2lhbCBWaWRlbwAObXVzaHJvb212aWRlb3MAAAAAAAMgyAALdUpfMUhNQUdiNGsAAQAraHR0cHM6Ly93d3cueW91dHViZS5jb20vd2F0Y2g/dj11Sl8xSE1BR2I0awAHeW91dHViZQAAAAAAAAAA")]
    [InlineData("QAAAjQIAK1JpdGEgT3JhIC0gWW91ciBTb25nIChPZmZpY2lhbCBMeXJpYyBWaWRlbykACFJpdGEgT3JhAAAAAAACwwgAC2k5NU5sYjdraVBvAAEAK2h0dHBzOi8vd3d3LnlvdXR1YmUuY29tL3dhdGNoP3Y9aTk1TmxiN2tpUG8AB3lvdXR1YmUAAAAAAAAAAA==")]
    [InlineData("QAAAmAIAMkx1a2FzIEdyYWhhbSAtIExvdmUgU29tZW9uZSBbT0ZGSUNJQUwgTVVTSUMgVklERU9dAAxMdWthcyBHcmFoYW0AAAAAAAOhsAALZE40NHhwSGpOeEUAAQAraHR0cHM6Ly93d3cueW91dHViZS5jb20vd2F0Y2g/dj1kTjQ0eHBIak54RQAHeW91dHViZQAAAAAAAAAA")]
    [InlineData("QAAAcQIADU1FVEFNT1JQSE9TSVMACklOVEVSV09STEQAAAAAAAIt7gAIOTU5OTA4ODcAAQA1aHR0cHM6Ly9tdXNpYy55YW5kZXgucnUvYWxidW0vMTk2MjI4MjYvdHJhY2svOTU5OTA4ODcAAAAAAAAAAAAA")]
    [InlineData("QAAAiwIAC0RlYXRoIG9mIE1lAApTQUlOVCBQSE5YAAAAAAAAdVgAKGh0dHBzOi8vd3d3Lm1ib3hkcml2ZS5jb20vc3RhcnRtdXNpYy5tcDMAAQAoaHR0cHM6Ly93d3cubWJveGRyaXZlLmNvbS9zdGFydG11c2ljLm1wMwAEaHR0cAADbXAzAAAAAAAAAAA=")]
    public void TestTrackDecodeEncodeRoundTrip(string trackIdentifier)
    {
        // Arrange
        var track = TrackDecoder.Decode(trackIdentifier);

        // Act
        var actualTrackIdentifier = TrackEncoder.Encode(track);

        // Assert
        Assert.Equal(trackIdentifier, actualTrackIdentifier);
    }

    [Fact]
    public void TestTrackDecodeEncodeRoundTripValidate()
    {
        var trackInfo = new LavalinkTrackInfo
        {
            Author = "Test Author",
            Duration = TimeSpan.FromSeconds(10),
            IsLiveStream = true,
            IsSeekable = false,
            Position = default,
            Uri = new Uri("https://example.com/"),
            SourceName = "mp3",
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
        Assert.Equal(trackInfo.SourceName, actualTrackInfo.SourceName);
        Assert.Equal(trackInfo.Title, actualTrackInfo.Title);
        Assert.Equal(trackInfo.TrackIdentifier, actualTrackInfo.TrackIdentifier);
    }
}
