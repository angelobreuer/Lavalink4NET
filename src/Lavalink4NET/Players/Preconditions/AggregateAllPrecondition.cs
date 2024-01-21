namespace Lavalink4NET.Players.Preconditions;

using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

internal sealed class AggregateAllPrecondition(ImmutableArray<IPlayerPrecondition> Preconditions) : IPlayerPrecondition
{
    public async ValueTask<bool> CheckAsync(ILavalinkPlayer player, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(player);

        foreach (var precondition in Preconditions)
        {
            if (!await precondition.CheckAsync(player, cancellationToken).ConfigureAwait(false))
            {
                return false;
            }
        }

        return true;
    }
}
