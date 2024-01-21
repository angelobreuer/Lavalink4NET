namespace Lavalink4NET.Players.Preconditions;

using System;
using System.Threading;
using System.Threading.Tasks;

internal sealed record class InlineAsynchronousPrecondition(Func<ILavalinkPlayer, CancellationToken, ValueTask<bool>> Precondition) : IPlayerPrecondition
{
    public ValueTask<bool> CheckAsync(ILavalinkPlayer player, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(player);

        return Precondition(player, cancellationToken);
    }
}

internal sealed record class InlineAsynchronousPrecondition<TPlayer>(Func<TPlayer, CancellationToken, ValueTask<bool>> Precondition) : IPlayerPrecondition where TPlayer : ILavalinkPlayer
{
    public async ValueTask<bool> CheckAsync(ILavalinkPlayer player, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(player);

        return player is TPlayer specializedPlayer && await Precondition(specializedPlayer, cancellationToken).ConfigureAwait(false);
    }
}
