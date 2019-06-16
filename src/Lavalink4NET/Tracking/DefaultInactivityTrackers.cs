namespace Lavalink4NET.Tracking
{
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    ///     A set of default out-of-box inactivity trackers.
    /// </summary>
    public static class DefaultInactivityTrackers
    {
        /// <summary>
        ///     An inactivity tracker ( <see cref="InactivityTracker"/>) which marks a player as
        ///     "inactive" when there are no users in the channel except the bot itself.
        /// </summary>
        public static InactivityTracker UsersInactivityTracker { get; } = async (player, client) =>
        {
            // count the users in the player voice channel (bot excluded)
            var userCount = (await client.GetChannelUsersAsync(player.GuildId, player.VoiceChannelId.Value))
                .Where(s => s != client.CurrentUserId)
                .Count();

            // check if there are no users in the channel (bot excluded)
            return userCount == 0;
        };

        /// <summary>
        ///     An inactivity tracker ( <see cref="InactivityTracker"/>) which marks a player as
        ///     "inactive" when the player is not playing a track.
        /// </summary>
        public static InactivityTracker ChannelInactivityTracker { get; } = (player, _)
            => Task.FromResult(player.State != Player.PlayerState.NotPlaying);
    }
}