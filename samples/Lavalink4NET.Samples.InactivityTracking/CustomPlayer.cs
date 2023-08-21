namespace Lavalink4NET.Samples.InactivityTracking;

using System.Threading;
using System.Threading.Tasks;
using Discord;
using Lavalink4NET.InactivityTracking.Players;
using Lavalink4NET.InactivityTracking.Trackers;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;

public sealed class CustomPlayer : QueuedLavalinkPlayer, IInactivityPlayerListener
{
    private readonly ITextChannel? _textChannel;

    public CustomPlayer(IPlayerProperties<CustomPlayer, CustomPlayerOptions> properties)
        : base(properties)
    {
        _textChannel = properties.Options.Value.TextChannel;
    }

    public async ValueTask NotifyPlayerActiveAsync(PlayerTrackingState trackingState, CancellationToken cancellationToken = default)
    {
        if (_textChannel is not null)
        {
            await _textChannel
                .SendMessageAsync("Player is being tracked as active.")
                .ConfigureAwait(false);
        }
    }

    public async ValueTask NotifyPlayerInactiveAsync(PlayerTrackingState trackingState, CancellationToken cancellationToken = default)
    {
        if (_textChannel is not null)
        {
            await _textChannel
                .SendMessageAsync("Player exceeded inactive timeout.")
                .ConfigureAwait(false);
        }
    }

    public async ValueTask NotifyPlayerTrackedAsync(PlayerTrackingState trackingState, CancellationToken cancellationToken = default)
    {
        if (_textChannel is not null)
        {
            await _textChannel
                .SendMessageAsync("Player is being tracked as inactive.")
                .ConfigureAwait(false);
        }
    }
}
