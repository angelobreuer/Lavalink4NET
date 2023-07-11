namespace Lavalink4NET.Players.Preconditions;

using System;
using System.Threading;
using System.Threading.Tasks;

internal sealed record class NotPausedPrecondition : IPlayerPrecondition
{
    public ValueTask<bool> CheckAsync(ILavalinkPlayer player, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(player);

        return new ValueTask<bool>(player.State is not PlayerState.Paused);
    }
}
