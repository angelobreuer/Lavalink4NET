namespace Lavalink4NET.Tracking;

using System.Threading.Tasks;
using Lavalink4NET.Players;

/// <summary>
///     A set of default out-of-box inactivity trackers.
/// </summary>
public static class DefaultInactivityTrackers
{
    /// <summary>
    ///     An inactivity tracker ( <see cref="InactivityTracker"/>) which marks a player as
    ///     "inactive" when the player is not playing a track.
    /// </summary>
    public static InactivityTracker ChannelInactivityTracker { get; } = static (player, _, cancellationToken) =>
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(player.State is PlayerState.NotPlaying);
    };

    /// <summary>
    ///     An inactivity tracker ( <see cref="InactivityTracker"/>) which marks a player as
    ///     "inactive" when there are no users in the channel except the bot itself.
    /// </summary>
    public static InactivityTracker UsersInactivityTracker { get; } = static async (player, client, cancellationToken) =>
    {
        cancellationToken.ThrowIfCancellationRequested();

        var users = await client
            .GetChannelUsersAsync(player.GuildId, player.VoiceChannelId)
            .ConfigureAwait(false);

        // check if there are no users in the channel (bots excluded)
        return users.Length is 0;
    };
}
