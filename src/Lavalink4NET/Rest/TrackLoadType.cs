namespace Lavalink4NET.Rest;

using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Lavalink4NET.Converters;

/// <summary>
///     The possible track load types.
/// </summary>
[JsonConverter(typeof(TrackLoadTypeJsonConverter))]
public enum TrackLoadType
{
    /// <summary>
    ///     A single track was loaded.
    /// </summary>
    [EnumMember(Value = "TRACK_LOADED")]
    TrackLoaded,

    /// <summary>
    ///     A playlist was loaded.
    /// </summary>
    [EnumMember(Value = "PLAYLIST_LOADED")]
    PlaylistLoaded,

    /// <summary>
    ///     A search result was made.
    /// </summary>
    [EnumMember(Value = "SEARCH_RESULT")]
    SearchResult,

    /// <summary>
    ///     No matches were found for the given identifier.
    /// </summary>
    [EnumMember(Value = "NO_MATCHES")]
    NoMatches,

    /// <summary>
    ///     Something happened while loading the track(s).
    /// </summary>
    [EnumMember(Value = "LOAD_FAILED")]
    LoadFailed,
}
