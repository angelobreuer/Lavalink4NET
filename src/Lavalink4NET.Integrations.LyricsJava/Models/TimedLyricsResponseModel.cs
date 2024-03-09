namespace Lavalink4NET.Integrations.LyricsJava.Models;

using System.Collections.Immutable;
using System.Text.Json.Serialization;

public sealed record class TimedLyricsResponseModel : LyricsResponseModel
{
    [JsonRequired]
    [JsonPropertyName("lines")]
    public ImmutableArray<TimedLyricsLineModel> TimedLines { get; set; }
}
