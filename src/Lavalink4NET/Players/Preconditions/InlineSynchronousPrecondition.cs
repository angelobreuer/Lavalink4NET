namespace Lavalink4NET.Players.Preconditions;

using System;
using System.Threading;
using System.Threading.Tasks;

internal sealed record class InlineSynchronousPrecondition(Func<ILavalinkPlayer, bool> Precondition) : IPlayerPrecondition
{
    public ValueTask<bool> CheckAsync(ILavalinkPlayer player, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(player);

        return new ValueTask<bool>(Precondition(player));
    }
}

internal sealed record class InlineSynchronousPrecondition<TPlayer>(Func<TPlayer, bool> Precondition) : IPlayerPrecondition where TPlayer : ILavalinkPlayer
{
    public ValueTask<bool> CheckAsync(ILavalinkPlayer player, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(player);

        return new ValueTask<bool>(player is TPlayer specializedPlayer && Precondition(specializedPlayer));
    }
}
