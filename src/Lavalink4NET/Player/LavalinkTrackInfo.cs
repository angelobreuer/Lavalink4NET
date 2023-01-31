namespace Lavalink4NET.Player;

using System;
using System.Text.Json.Serialization;
using Lavalink4NET.Converters;

/// <summary>
///     The information store for a lavalink track.
/// </summary>
public sealed class LavalinkTrackInfo
{
    /// <summary>
    ///     Gets the name of the track author.
    /// </summary>
    [JsonPropertyName("author")]
    public string Author { get; init; }

    /// <summary>
    ///     Gets the duration of the track.
    /// </summary>
    [JsonPropertyName("length"), JsonConverter(typeof(TimeSpanJsonConverter))]
    public TimeSpan Duration { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the track is a live stream.
    /// </summary>
    [JsonPropertyName("isStream")]
    public bool IsLiveStream { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the track is seek-able.
    /// </summary>
    [JsonPropertyName("isSeekable")]
    public bool IsSeekable { get; init; }

    /// <summary>
    ///     Gets the start position of the track.
    /// </summary>
    [JsonPropertyName("position")]
    [JsonConverter(typeof(TimeSpanJsonConverter))]
    public TimeSpan Position { get; init; }

    /// <summary>
    ///     Gets the URI of the track.
    /// </summary>
    [JsonPropertyName("uri")]
    public Uri? Uri { get; init; }

    /// <summary>
    ///     Gets the title of the track.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; init; }

    /// <summary>
    ///     Gets the unique track identifier (Example: dQw4w9WgXcQ, YouTube Video ID).
    /// </summary>
    [JsonPropertyName("identifier")]
    public string TrackIdentifier { get; init; }

    /// <summary>
    ///     Gets the name of the source (e.g. youtube, mp3, ...).
    /// </summary>
    [JsonPropertyName("sourceName")]
    public string? SourceName { get; init; }

    /// <summary>
    ///     Gets or initializes the container probe information of the track
    ///     for HTTP and local audio source managers. Must be set manually if
    ///     the track was requested over the REST API of Lavalink.
    /// </summary>
    [JsonPropertyName("probeInfo")]
    public string? ProbeInfo { get; init; }
}
