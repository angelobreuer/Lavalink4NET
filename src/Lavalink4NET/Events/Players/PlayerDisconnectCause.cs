namespace Lavalink4NET.Events.Players;

/// <summary>
///     A set of possible player disconnection causes.
/// </summary>
public enum PlayerDisconnectCause : byte
{
    /// <summary>
    ///     Denotes that the player was stopped and disconnected (needs
    ///     <see cref="LavalinkNodeOptions.DisconnectOnStop"/> enabled).
    /// </summary>
    Stop,

    /// <summary>
    ///     Denotes that the player was disconnected due disposal.
    /// </summary>
    Dispose,

    /// <summary>
    ///     Denotes that the player was disconnected normally.
    /// </summary>
    Disconnected,

    /// <summary>
    ///     Denotes that the player was disconnected because the lavalink socket was closed.
    /// </summary>
    WebSocketClosed
}
