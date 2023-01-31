namespace Lavalink4NET.Cluster;

/// <summary>
///     A set of possible node request types.
/// </summary>
public enum NodeRequestType : byte
{
    /// <summary>
    ///     Denotes that the node purpose is unspecified.
    /// </summary>
    Unspecified,

    /// <summary>
    ///     Denotes that the node purpose is to load a track.
    /// </summary>
    LoadTrack,

    /// <summary>
    ///     Denotes that the node purpose is to play tracks.
    /// </summary>
    PlayTrack,

    /// <summary>
    ///     Denotes that the node purpose is to backup players from another node.
    /// </summary>
    Backup
}
