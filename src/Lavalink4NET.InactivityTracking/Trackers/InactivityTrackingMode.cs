namespace Lavalink4NET.InactivityTracking.Trackers;

public enum InactivityTrackingMode : byte
{
    /// <summary>
    ///     Denotes that the player is considered inactive if ANY of the trackers return <see langword="true"/>.
    /// </summary>
    Any,

    /// <summary>
    ///     Denotes that the player is considered inactive if ALL of the trackers return <see langword="true"/>.
    ///
    All,
}
