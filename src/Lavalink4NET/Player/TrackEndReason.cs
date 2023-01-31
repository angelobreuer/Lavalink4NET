namespace Lavalink4NET.Player;

using System.Text.Json.Serialization;
using Lavalink4NET.Converters;

/// <summary>
///     The different reason for a track ending.
/// </summary>
[JsonConverter(typeof(TrackEndReasonJsonConverter))]
public enum TrackEndReason
{
    /// <summary>
    ///     The track finished.
    /// </summary>
    Finished,

    /// <summary>
    ///     The load of the track failed.
    /// </summary>
    LoadFailed,

    /// <summary>
    ///     The track was stopped.
    /// </summary>
    Stopped,

    /// <summary>
    ///     The track was replaced by another.
    /// </summary>
    Replaced,

    /// <summary>
    ///     The player was destroyed.
    /// </summary>
    CleanUp,
}
