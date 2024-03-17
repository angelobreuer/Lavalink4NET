namespace Lavalink4NET.Rest;

using System;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Integrations.TextToSpeech;
using Lavalink4NET.Integrations.TextToSpeech.Synthesis;
using Lavalink4NET.Rest.Entities;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;

public static class TrackManagerExtensions
{
    public static ValueTask<LavalinkTrack?> GetTextToSpeechTrackAsync(
        this ITrackManager trackManager,
        string textToSpeech,
        string? languageName = null,
        CacheMode cacheMode = CacheMode.Dynamic,
        LavalinkApiResolutionScope resolutionScope = default,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(trackManager);
        ArgumentNullException.ThrowIfNull(textToSpeech);

        var encodedUri = TextToSpeechTrackEncoder.EncodeMessage(textToSpeech, languageName);

        var loadOptions = new TrackLoadOptions(
            SearchMode: TrackSearchMode.None,
            SearchBehavior: StrictSearchBehavior.Passthrough,
            CacheMode: cacheMode);

        return trackManager.LoadTrackAsync(encodedUri.ToString(), loadOptions, resolutionScope, cancellationToken);
    }

    public static ValueTask<LavalinkTrack?> GetTextToSpeechTrackAsync(
        this ITrackManager trackManager,
        SynthesisInput input,
        VoiceSelectionParameters? voiceSelectionParameters = null,
        AudioConfiguration? audioConfiguration = null,
        CacheMode cacheMode = CacheMode.Dynamic,
        LavalinkApiResolutionScope resolutionScope = default,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(trackManager);
        ArgumentNullException.ThrowIfNull(input);

        var encodedUri = TextToSpeechTrackEncoder.EncodeMessage(input, voiceSelectionParameters, audioConfiguration);

        var loadOptions = new TrackLoadOptions(
            SearchMode: TrackSearchMode.None,
            SearchBehavior: StrictSearchBehavior.Passthrough,
            CacheMode: cacheMode);

        return trackManager.LoadTrackAsync(encodedUri.ToString(), loadOptions, resolutionScope, cancellationToken);
    }
}
