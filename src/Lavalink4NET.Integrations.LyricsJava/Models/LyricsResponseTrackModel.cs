namespace Lavalink4NET.Integrations.LyricsJava.Models;

using System.Collections.Immutable;
using System.Text.Json.Serialization;

public sealed record class LyricsResponseTrackModel
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;
    
    [JsonPropertyName("author")]
    public string Author { get; set; } = null!;
    
    [JsonPropertyName("album")]
    public string Album { get; set; } = null!;
    
    [JsonPropertyName("albumArt")]
    public ImmutableArray<AlbumArtModel> AlbumArt { get; set; }
}