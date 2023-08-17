namespace Lavalink4NET.InactivityTracking.Trackers;

/// <param name="ExcludeBots">
///     A value indicating whether to respect bot users in the channel.
///     By default, this is <see langword="true"/>.
/// </param>
/// <param name="Threshold">
///     The number of users and bots (depending on <paramref name="ExcludeBots"/>) that must be in the channel
///     to mark the player as "active". By default, this is 1, so that at least one user must be in the channel.
/// </param>
public readonly record struct UsersInactivityTrackerOptions(bool? ExcludeBots = null, int? Threshold = null, TimeSpan? Timeout = null)
{
    public static UsersInactivityTrackerOptions Default => default;
}