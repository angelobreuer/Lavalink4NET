namespace Lavalink4NET.Integrations.LyricsJava.Models;

using System.Collections.Immutable;
using System.Text.Json.Serialization;

public sealed record class LyricsResponseModel
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;
    
    [JsonPropertyName("track")]
    public LyricsResponseTrackModel? Track { get; set; }
    
    [JsonPropertyName("source")]
    public string Source { get; set; } = null!;
    
    [JsonPropertyName("text")]
    public string? LyricsText { get; set; }
    
    [JsonPropertyName("lines")]
    public ImmutableArray<TimedLyricsLineModel>? TimedLines { get; set; }
}