namespace Lavalink4NET.ExampleCustomPlayer.Players;

using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
///     A custom player that saves the last used volume.
/// </summary>
public sealed class CustomPlayer : QueuedLavalinkPlayer
{
    private readonly IVolumeService _volumeService;

    public CustomPlayer(IPlayerProperties<CustomPlayer, CustomPlayerOptions> properties)
        : base(properties)
    {
        _volumeService = properties.ServiceProvider!.GetRequiredService<IVolumeService>();
    }

    public override async ValueTask SetVolumeAsync(float volume, CancellationToken cancellationToken = default)
    {
        EnsureNotDestroyed();

        // call the base method, without using it the volume would remain the same.
        await base
            .SetVolumeAsync(volume, cancellationToken)
            .ConfigureAwait(false);

        // store the volume of the player
        _volumeService.SetVolume(GuildId, volume);
    }
}
