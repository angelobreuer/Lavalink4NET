namespace Lavalink4NET.Integrations.TextToSpeech.Synthesis;

using System.Text.Json.Serialization;

public sealed class VoiceSelectionParameters
{
    [JsonPropertyName("languageCode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? LanguageCode { get; set; }

    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Name { get; set; }

    [JsonPropertyName("ssmlGender")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public SsmlVoiceGender Gender { get; set; }
}
