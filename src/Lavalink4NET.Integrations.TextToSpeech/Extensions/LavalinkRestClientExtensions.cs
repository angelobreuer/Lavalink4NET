namespace Lavalink4NET.Rest;

using System;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Integrations.TextToSpeech;
using Lavalink4NET.Integrations.TextToSpeech.Synthesis;
using Lavalink4NET.Rest.Entities;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;

public static class LavalinkRestClientExtensions
{
    public static ValueTask<LavalinkTrack?> GetTextToSpeechTrackAsync(
        this ILavalinkApiClient apiClient,
        string textToSpeech,
        string? languageName = null,
        CacheMode cacheMode = CacheMode.Dynamic,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(apiClient);
        ArgumentNullException.ThrowIfNull(textToSpeech);

        var encodedUri = TextToSpeechTrackEncoder.EncodeMessage(textToSpeech, languageName);

        var loadOptions = new TrackLoadOptions(
            SearchMode: TrackSearchMode.None,
            StrictSearch: false,
            CacheMode: cacheMode);

        return apiClient.LoadTrackAsync(encodedUri.ToString(), loadOptions, cancellationToken);
    }

    public static ValueTask<LavalinkTrack?> GetTextToSpeechTrackAsync(
        this ILavalinkApiClient apiClient,
        SynthesisInput input,
        VoiceSelectionParameters? voiceSelectionParameters = null,
        AudioConfiguration? audioConfiguration = null,
        CacheMode cacheMode = CacheMode.Dynamic,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(apiClient);
        ArgumentNullException.ThrowIfNull(input);

        var encodedUri = TextToSpeechTrackEncoder.EncodeMessage(input, voiceSelectionParameters, audioConfiguration);

        var loadOptions = new TrackLoadOptions(
            SearchMode: TrackSearchMode.None,
            StrictSearch: false,
            CacheMode: cacheMode);

        return apiClient.LoadTrackAsync(encodedUri.ToString(), loadOptions, cancellationToken);
    }
}
