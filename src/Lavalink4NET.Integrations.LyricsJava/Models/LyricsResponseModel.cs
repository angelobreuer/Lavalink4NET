namespace Lavalink4NET.Integrations.LyricsJava.Models;

using System.Text.Json.Serialization;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type", UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization)]
[JsonDerivedType(typeof(TextLyricsResponseModel), "text")]
[JsonDerivedType(typeof(TimedLyricsResponseModel), "timed")]
public abstract record class LyricsResponseModel
{
    [JsonRequired]
    [JsonPropertyName("track")]
    public LyricsResponseTrackModel Track { get; set; } = null!;

    [JsonRequired]
    [JsonPropertyName("source")]
    public string Source { get; set; } = null!;
}
