using System.Text.Json.Serialization;

namespace Lavalink4NET.Integrations.LyricsJava.Models;

public sealed record class TimedLyricsLineModel
{
    [JsonPropertyName("line")]
    public string Line { get; set; } = null!;
    
    [JsonPropertyName("range")]
    public TimeRangeModel Range { get; set; } = null!;
}