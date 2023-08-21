namespace Lavalink4NET.Integrations.Lavasrc.TextToSpeech;

using Lavalink4NET.Rest.Entities;

public sealed record class TextToSpeechOptions(
    string? Voice = null,
    float? Speed = null,
    bool? Translate = null,
    TimeSpan? Silence = null,
    TextToSpeechFormat? Format = null,
    CacheMode? CacheMode = null)
{
    public static TextToSpeechOptions Default { get; } = new();
}