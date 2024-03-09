namespace Lavalink4NET.Integrations.LyricsJava.Models;

using System.Text.Json.Serialization;
using Lavalink4NET.Integrations.LyricsJava.Converters;

public sealed record class AlbumArtModel
{
    [JsonPropertyName("url")]
    [JsonConverter(typeof(StringUriJsonConverter))]
    public Uri Uri { get; set; } = null!;

    [JsonPropertyName("height")]
    [JsonConverter(typeof(ImageDimensionJsonConverter))]
    public int? Height { get; set; }

    [JsonPropertyName("width")]
    [JsonConverter(typeof(ImageDimensionJsonConverter))]
    public int? Width { get; set; }
}