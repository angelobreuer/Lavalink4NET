namespace Lavalink4NET.Lyrics.Models;

using System.Text.Json.Serialization;

/// <summary>
///     The response payload returned by the lyrics API service.
/// </summary>
public sealed record class LyricsResponse
{
    /// <summary>
    ///     Gets an hopefully descriptive error message indicating what error occurred.
    /// </summary>
    [JsonPropertyName("error")]
    public string? ErrorMessage { get; set; }

    /// <summary>
    ///     Gets the lyrics of the requested song.
    /// </summary>
    [JsonPropertyName("lyrics")]
    public string? Lyrics { get; set; }
}
