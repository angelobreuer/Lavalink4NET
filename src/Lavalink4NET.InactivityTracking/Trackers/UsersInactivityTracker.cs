namespace Lavalink4NET.InactivityTracking.Trackers;

using System.Threading;
using System.Threading.Tasks;

public sealed class UsersInactivityTracker : IInactivityTracker
{
    private readonly UsersInactivityTrackerOptions _options;

    public UsersInactivityTracker(UsersInactivityTrackerOptions options)
    {
        _options = options;
    }

    public async ValueTask<PlayerActivityStatus> CheckAsync(InactivityTrackingContext context, CancellationToken cancellationToken = default)
    {
        var includeBots = !_options.ExcludeBots.GetValueOrDefault(true);

        var usersInChannel = await context.Client
            .GetChannelUsersAsync(context.Player.GuildId, context.Player.VoiceChannelId, includeBots, cancellationToken)
            .ConfigureAwait(false);

        return usersInChannel.Length >= _options.Threshold.GetValueOrDefault(1)
            ? PlayerActivityStatus.Active
            : PlayerActivityStatus.Inactive;
    }
}
