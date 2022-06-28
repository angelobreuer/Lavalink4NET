namespace Lavalink4NET.Integrations.TextToSpeech.Synthesis;

using System.Collections.Immutable;
using System.Text.Json.Serialization;

public sealed class AudioConfiguration
{
    [JsonPropertyName("audioEncoding")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AudioEncoding? Encoding { get; init; }

    [JsonPropertyName("speakingRate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public float? SpeakingRate { get; init; }

    [JsonPropertyName("pitch")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public float? Pitch { get; init; }

    [JsonPropertyName("volumeGainDb")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public float? VolumeGain { get; init; }

    [JsonPropertyName("sampleRateHertz")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public float? SampleRate { get; init; }

    [JsonPropertyName("effectsProfileId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ImmutableArray<string> EffectProfiles { get; init; }
}
