namespace Lavalink4NET.InactivityTracking;

public enum PlayerInactivityBehavior : byte
{
    /// <summary>
    ///     Denotes that no action should be taken when a player is inactive.
    /// </summary>
    None,

    /// <summary>
    ///     Denotes that the player should temporarily paused when it is inactive.
    ///     The player will be resumed when it becomes active again (if it was active before).
    /// </summary>
    Pause,
}
