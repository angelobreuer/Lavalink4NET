using System.Text.Json.Serialization;

namespace Lavalink4NET.Integrations.LyricsJava.Models;

public sealed record class SearchResultModel
{
    [JsonPropertyName("videoId")]
    public string VideoId { get; set; } = null!;
    
    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;
}