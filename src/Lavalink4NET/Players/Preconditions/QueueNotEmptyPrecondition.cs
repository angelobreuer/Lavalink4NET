namespace Lavalink4NET.Players.Preconditions;

using System;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Players.Queued;

internal sealed record class QueueNotEmptyPrecondition : IPlayerPrecondition
{
    public ValueTask<bool> CheckAsync(ILavalinkPlayer player, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(player);

        if (player is not IQueuedLavalinkPlayer queuedLavalinkPlayer)
        {
            throw new InvalidOperationException("The player must be a IQueuedLavalinkPlayer.");
        }

        return new ValueTask<bool>(!queuedLavalinkPlayer.Queue.IsEmpty);
    }
}
