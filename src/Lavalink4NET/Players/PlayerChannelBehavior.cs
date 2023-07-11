namespace Lavalink4NET.Players;

public enum PlayerChannelBehavior : byte
{
    /// <summary>
    ///     The player will not automatically connect to a voice channel.
    /// </summary>
    None,

    /// <summary>
    ///     The player will automatically connect to the voice channel of the user if the player is not connected.
    /// </summary>
    Join,

    /// <summary>
    ///     The player will automatically connect to the voice channel of the user.
    /// </summary>
    Move,
}
