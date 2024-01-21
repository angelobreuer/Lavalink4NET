namespace Lavalink4NET.Players.Preconditions;

using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

internal sealed record class PlayerStatePrecondition(ImmutableArray<PlayerState> AllowedStates) : IPlayerPrecondition
{
    public ValueTask<bool> CheckAsync(ILavalinkPlayer player, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(player);

        return new ValueTask<bool>(AllowedStates.Contains(player.State));
    }
}