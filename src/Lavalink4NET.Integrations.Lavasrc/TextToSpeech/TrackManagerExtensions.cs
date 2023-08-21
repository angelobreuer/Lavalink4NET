namespace Lavalink4NET.Integrations.Lavasrc.TextToSpeech;

using System.Globalization;
using System.Web;
using Lavalink4NET.Rest;
using Lavalink4NET.Rest.Entities;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;

public static class TrackManagerExtensions
{
    public static async ValueTask<LavalinkTrack> GetTextToSpeechTrackAsync(
        this ITrackManager trackManager,
        string text,
        TextToSpeechOptions? options = null,
        LavalinkApiResolutionScope resolutionScope = default,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(trackManager);
        cancellationToken.ThrowIfCancellationRequested();

        options ??= TextToSpeechOptions.Default;

        var textToSpeechUri = $"ftts://{Uri.EscapeDataString(text)}";

        // optimize for the most common case where the user wants to use the default options,
        // so we don't have to create a new dictionary and copy all the values
        if (!ReferenceEquals(TextToSpeechOptions.Default, options))
        {
            var queryParameters = HttpUtility.ParseQueryString(string.Empty);

            if (options.Voice is not null)
            {
                queryParameters["voice"] = options.Voice;
            }

            if (options.Speed is not null)
            {
                queryParameters["speed"] = options.Speed.Value.ToString(CultureInfo.InvariantCulture);
            }

            if (options.Silence is not null)
            {
                queryParameters["silence"] = ((int)options.Silence.Value.TotalMilliseconds).ToString(CultureInfo.InvariantCulture);
            }

            if (options.Format is not null)
            {
                queryParameters["audio_format"] = options.Format.Value switch
                {
                    TextToSpeechFormat.Mp3 => "mp3",
                    TextToSpeechFormat.OggOpus => "ogg_opus",
                    TextToSpeechFormat.OggVorbis => "ogg_vorbis",
                    TextToSpeechFormat.Aac => "aac",
                    TextToSpeechFormat.Wav => "wav",
                    TextToSpeechFormat.Flac => "flac",
                    _ => throw new NotSupportedException(),
                };
            }

            if (options.Translate is not null)
            {
                queryParameters["translate"] = options.Translate.Value.ToString(CultureInfo.InvariantCulture);
            }

            textToSpeechUri = $"{textToSpeechUri}?{queryParameters}";
        }

        var loadOptions = new TrackLoadOptions(
            SearchMode: TrackSearchMode.None,
            StrictSearch: false,
            CacheMode: options.CacheMode ?? CacheMode.Dynamic);

        var track = await trackManager
            .LoadTrackAsync(textToSpeechUri.ToString().TrimEnd('/', '?'), loadOptions, resolutionScope, cancellationToken)
            .ConfigureAwait(false);

        return track ?? throw new InvalidOperationException("The Flowery TTS track could not be loaded. Ensure Flowery TTS is enabled.");
    }
}
