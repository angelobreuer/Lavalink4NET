namespace Lavalink4NET.Integrations.TextToSpeech.Synthesis;

using System.Text.Json.Serialization;

public sealed class SynthesisInput
{
    /// <summary>
    ///     Gets or sets the text to synthesize.
    /// </summary>
    [JsonPropertyName("text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Text { get; set; }

    /// <summary>
    ///     Gets or sets the SSML markup to synthesize.
    /// </summary>    
    [JsonPropertyName("ssml")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Ssml { get; set; }
}
