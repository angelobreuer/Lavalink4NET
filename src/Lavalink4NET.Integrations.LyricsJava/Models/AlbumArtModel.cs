namespace Lavalink4NET.Integrations.LyricsJava.Models;

using System.Text.Json.Serialization;
using Lavalink4NET.Integrations.LyricsJava.Converters;

public sealed record class AlbumArtModel
{
    [JsonPropertyName("url")]
    [JsonConverter(typeof(StringUriJsonConverter))]
    public Uri Url { get; set; } = null!;
    
    [JsonPropertyName("height")]
    public int Height { get; set; }
    
    [JsonPropertyName("width")]
    public int Width { get; set; }
}