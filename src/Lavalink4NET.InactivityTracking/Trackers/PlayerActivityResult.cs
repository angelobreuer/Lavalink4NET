namespace Lavalink4NET.InactivityTracking;

using Lavalink4NET.InactivityTracking.Trackers;

public readonly struct PlayerActivityResult
{
    private readonly bool _inactive;

    private PlayerActivityResult(bool inactive, TimeSpan? timeout)
    {
        _inactive = inactive;
        Timeout = timeout;
    }

    public PlayerActivityStatus Status => _inactive ? PlayerActivityStatus.Inactive : PlayerActivityStatus.Active;

    public TimeSpan? Timeout { get; }

    public static PlayerActivityResult Inactive(TimeSpan? timeout = null) => new(true, timeout);

    public static PlayerActivityResult Active => new(false, null);
}