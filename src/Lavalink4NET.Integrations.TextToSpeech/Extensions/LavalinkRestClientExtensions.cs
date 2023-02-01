namespace Lavalink4NET.Rest;

using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Integrations.TextToSpeech;
using Lavalink4NET.Integrations.TextToSpeech.Synthesis;
using Lavalink4NET.Tracks;

public static class LavalinkRestClientExtensions
{
    public static ValueTask<LavalinkTrack?> GetTextToSpeechTrackAsync(
        this ILavalinkRestClient restClient,
        string textToSpeech,
        string? languageName = null,
        bool noCache = false,
        CancellationToken cancellationToken = default)
    {
        var encodedUri = TextToSpeechTrackEncoder.EncodeMessage(textToSpeech, languageName);
        return restClient.GetTrackAsync(encodedUri.ToString(), mode: SearchMode.None, noCache, cancellationToken);
    }

    public static ValueTask<LavalinkTrack?> GetTextToSpeechTrackAsync(
        this ILavalinkRestClient restClient,
        SynthesisInput input,
        VoiceSelectionParameters? voiceSelectionParameters = null,
        AudioConfiguration? audioConfiguration = null,
        bool noCache = false,
        CancellationToken cancellationToken = default)
    {
        var encodedUri = TextToSpeechTrackEncoder.EncodeMessage(input, voiceSelectionParameters, audioConfiguration);
        return restClient.GetTrackAsync(encodedUri.OriginalString, mode: SearchMode.None, noCache, cancellationToken);
    }
}
