namespace Lavalink4NET.ExampleCustomPlayer;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Players;

/// <summary>
///     A custom player that saves the last used volume.
/// </summary>
public sealed class MyCustomPlayer : LavalinkPlayer
{
    /// <summary>
    ///     The dictionary holding the last used volumes.
    /// </summary>
    /// <remarks>
    ///     This can be replaced with a database or something else to keep the values persistent.
    /// </remarks>
    private readonly static Dictionary<ulong, float> _volumes = new();

    public MyCustomPlayer(PlayerProperties properties)
        : base(properties)
    {
        // TODO: restore volume
    }

    public override async ValueTask SetVolumeAsync(float volume = 1, CancellationToken cancellationToken = default)
    {
        EnsureNotDestroyed();

        // call the base method, without using it the volume would remain the same.
        await base
            .SetVolumeAsync(volume, cancellationToken)
            .ConfigureAwait(false);

        // store the volume of the player
        _volumes[GuildId] = volume;
    }
}
