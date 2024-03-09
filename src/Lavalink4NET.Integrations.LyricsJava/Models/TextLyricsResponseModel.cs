namespace Lavalink4NET.Integrations.LyricsJava.Models;

using System.Text.Json.Serialization;

public sealed record class TextLyricsResponseModel : LyricsResponseModel
{
    [JsonRequired]
    [JsonPropertyName("text")]
    public string LyricsText { get; set; } = null!;
}
