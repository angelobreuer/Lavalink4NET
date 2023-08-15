namespace Lavalink4NET.Players.Preconditions;

using System;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Players.Queued;

internal sealed class HistoryNotEmptyPrecondition : IPlayerPrecondition
{
    public ValueTask<bool> CheckAsync(ILavalinkPlayer player, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(player);

        if (player is not IQueuedLavalinkPlayer queuedLavalinkPlayer)
        {
            throw new InvalidOperationException("The player must be a IQueuedLavalinkPlayer.");
        }

        if (!queuedLavalinkPlayer.Queue.HasHistory)
        {
            throw new InvalidOperationException("The player must have a history.");
        }

        return new ValueTask<bool>(!queuedLavalinkPlayer.Queue.History.IsEmpty);
    }
}
