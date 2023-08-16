namespace Lavalink4NET.Integrations.Lavasrc.Tests;

using System.Threading.Tasks;
using Lavalink4NET.Integrations.Lavasrc.TextToSpeech;
using Lavalink4NET.Rest;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;
using Moq;

public sealed class TrackManagerExtensionsTests
{
    [Fact]
    public async Task TestBuildFloweryTtsTrackSingleWordWithoutOptionsAsync()
    {
        // Arrange
        var trackManager = new Mock<ITrackManager>();

        trackManager.Setup(x => x
            .LoadTrackAsync(It.IsAny<string>(), It.IsAny<TrackLoadOptions>(), It.IsAny<LavalinkApiResolutionScope>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string identifier, TrackLoadOptions loadOptions, LavalinkApiResolutionScope resolutionScope, CancellationToken cancellationToken) =>
            {
                Assert.Equal("ftts://abc", identifier);

                return new LavalinkTrack { Author = "Lavasrc", Title = "TextToSpeech", Identifier = "AAA", };
            });

        // Act
        await trackManager.Object
            .GetTextToSpeechTrackAsync("abc")
            .ConfigureAwait(false);

        // Assert
        trackManager.Verify();
    }

    [Fact]
    public async Task TestBuildFloweryTtsTrackMultipleWordsWithoutOptionsAsync()
    {
        // Arrange
        var trackManager = new Mock<ITrackManager>();

        trackManager.Setup(x => x
            .LoadTrackAsync(It.IsAny<string>(), It.IsAny<TrackLoadOptions>(), It.IsAny<LavalinkApiResolutionScope>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string identifier, TrackLoadOptions loadOptions, LavalinkApiResolutionScope resolutionScope, CancellationToken cancellationToken) =>
            {
                Assert.Equal("ftts://abc%20abc%20abc", identifier);

                return new LavalinkTrack { Author = "Lavasrc", Title = "TextToSpeech", Identifier = "AAA", };
            });

        // Act
        await trackManager.Object
            .GetTextToSpeechTrackAsync("abc abc abc")
            .ConfigureAwait(false);

        // Assert
        trackManager.Verify();
    }

    [Theory]
    [InlineData("test 123", null, null, null, null, null, "ftts://test%20123")]
    [InlineData("test 123", "test", null, null, null, null, "ftts://test%20123?voice=test")]
    [InlineData("test 123", "test", 1.0F, null, null, null, "ftts://test%20123?voice=test&speed=1")]
    [InlineData("test 123", "test", 1.2F, null, null, null, "ftts://test%20123?voice=test&speed=1.2")]
    [InlineData("test 123", "test", 1.2F, 12, null, null, "ftts://test%20123?voice=test&speed=1.2&silence=12")]
    [InlineData("test 123", "test", 1.2F, 12, TextToSpeechFormat.Mp3, null, "ftts://test%20123?voice=test&speed=1.2&silence=12&audio_format=mp3")]
    [InlineData("test 123", null, null, null, null, true, "ftts://test%20123?translate=True")]
    [InlineData("test 123", null, null, null, TextToSpeechFormat.Mp3, null, "ftts://test%20123?audio_format=mp3")]
    [InlineData("test 123", null, null, null, TextToSpeechFormat.OggOpus, null, "ftts://test%20123?audio_format=ogg_opus")]
    [InlineData("test 123", null, null, null, TextToSpeechFormat.OggVorbis, null, "ftts://test%20123?audio_format=ogg_vorbis")]
    [InlineData("test 123", null, null, null, TextToSpeechFormat.Aac, null, "ftts://test%20123?audio_format=aac")]
    [InlineData("test 123", null, null, null, TextToSpeechFormat.Wav, null, "ftts://test%20123?audio_format=wav")]
    [InlineData("test 123", null, null, null, TextToSpeechFormat.Flac, null, "ftts://test%20123?audio_format=flac")]
    public async Task TestBuildFloweryTtsTrackWithOptionsAsync(string text, string? voice, float? speed, int? silence, TextToSpeechFormat? format, bool? translate, string uri)
    {
        // Arrange
        var trackManager = new Mock<ITrackManager>();

        trackManager.Setup(x => x
            .LoadTrackAsync(It.IsAny<string>(), It.IsAny<TrackLoadOptions>(), It.IsAny<LavalinkApiResolutionScope>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string identifier, TrackLoadOptions loadOptions, LavalinkApiResolutionScope resolutionScope, CancellationToken cancellationToken) =>
            {
                Assert.Equal(uri, identifier);

                return new LavalinkTrack { Author = "Lavasrc", Title = "TextToSpeech", Identifier = "AAA", };
            });

        var options = new TextToSpeechOptions(
            Voice: voice,
            Speed: speed,
            Silence: silence.HasValue ? TimeSpan.FromMilliseconds(silence.Value) : null,
            Format: format,
            Translate: translate);

        // Act
        await trackManager.Object
            .GetTextToSpeechTrackAsync(text, options)
            .ConfigureAwait(false);

        // Assert
        trackManager.Verify();
    }
}
