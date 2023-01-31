namespace Lavalink4NET.Player;

/// <summary>
///     Represents different player states.
/// </summary>
public enum PlayerState
{
    /// <summary>
    ///     The player is playing a track.
    /// </summary>
    Playing,

    /// <summary>
    ///     The player is in idle state and is not playing any track.
    /// </summary>
    NotPlaying,

    /// <summary>
    ///     The connection to the voice server is closed and the player should not be used anymore.
    /// </summary>
    Destroyed,

    /// <summary>
    ///     The current track is paused and can be resumed.
    /// </summary>
    Paused,

    /// <summary>
    ///     The player is not connected to a voice channel.
    /// </summary>
    NotConnected
}
