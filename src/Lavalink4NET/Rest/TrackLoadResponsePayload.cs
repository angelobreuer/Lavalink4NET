namespace Lavalink4NET.Rest;

using System.Text.Json.Serialization;
using Player;

/// <summary>
///     t The RESTful api HTTP response object for request to the "/tracks" endpoint.
/// </summary>
public sealed class TrackLoadResponsePayload
{
    /// <summary>
    ///     Gets an exception that indicates why the track load failed (see: <see cref="LoadType"/>).
    /// </summary>
    /// <remarks>
    ///     This property is only available if <see cref="TrackLoadType"/> is <see cref="TrackLoadType.LoadFailed"/>.
    /// </remarks>
    [JsonPropertyName("exception")]
    public TrackLoadException? Exception { get; init; }

    /// <summary>
    ///     Gets the type of what was loaded.
    /// </summary>
    [JsonPropertyName("loadType")]
    public TrackLoadType LoadType { get; init; }

    /// <summary>
    ///     Gets the information about the playlist.
    /// </summary>
    [JsonPropertyName("playlistInfo")]
    public PlaylistInfo? PlaylistInfo { get; init; }

    /// <summary>
    ///     Gets the loaded tracks.
    /// </summary>
    [JsonPropertyName("tracks")]
    public LavalinkTrack[]? Tracks { get; init; }
}
