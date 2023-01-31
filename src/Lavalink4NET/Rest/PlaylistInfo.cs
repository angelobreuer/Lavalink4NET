namespace Lavalink4NET.Rest;

using System.Text.Json.Serialization;

/// <summary>
///     The playlist info object for the track load response.
/// </summary>
public sealed class PlaylistInfo
{
    /// <summary>
    ///     Gets the name of the playlist.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    ///     Gets the index of the selected track.
    /// </summary>
    [JsonPropertyName("selectedTrack")]
    public int SelectedTrack { get; init; }
}
