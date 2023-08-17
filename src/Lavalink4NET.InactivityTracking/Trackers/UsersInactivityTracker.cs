namespace Lavalink4NET.InactivityTracking.Trackers;

using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Players;

public sealed class UsersInactivityTracker : IInactivityTracker
{
    private readonly UsersInactivityTrackerOptions _options;

    public UsersInactivityTracker(UsersInactivityTrackerOptions options)
    {
        _options = options;
    }

    public async ValueTask<PlayerActivityResult> CheckAsync(InactivityTrackingContext context, ILavalinkPlayer player, CancellationToken cancellationToken = default)
    {
        var includeBots = !_options.ExcludeBots.GetValueOrDefault(true);

        var usersInChannel = await context.Client
            .GetChannelUsersAsync(player.GuildId, player.VoiceChannelId, includeBots, cancellationToken)
            .ConfigureAwait(false);

        if (usersInChannel.Length >= _options.Threshold.GetValueOrDefault(1))
        {
            return PlayerActivityResult.Active;
        }

        return PlayerActivityResult.Inactive(_options.Timeout);
    }
}
