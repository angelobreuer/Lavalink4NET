using System.Text.Json.Serialization;
using Lavalink4NET.Integrations.LyricsJava.Converters;

namespace Lavalink4NET.Integrations.LyricsJava.Models;

public sealed record class TimeRangeModel
{
    [JsonPropertyName("start")]
    [JsonConverter(typeof(NumberTimeSpanJsonConverter))]
    public TimeSpan Start { get; set; }
    
    [JsonPropertyName("end")]
    [JsonConverter(typeof(NumberTimeSpanJsonConverter))]
    public TimeSpan End { get; set; }
}