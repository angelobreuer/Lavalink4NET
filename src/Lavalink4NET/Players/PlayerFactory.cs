namespace Lavalink4NET.Players;

using System;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Players.Vote;

public static class PlayerFactory
{
    public static PlayerFactory<LavalinkPlayer, LavalinkPlayerOptions> Default { get; } = Create<LavalinkPlayer, LavalinkPlayerOptions>(static properties => new LavalinkPlayer(properties));

    public static PlayerFactory<QueuedLavalinkPlayer, QueuedLavalinkPlayerOptions> Queued { get; } = Create<QueuedLavalinkPlayer, QueuedLavalinkPlayerOptions>(static properties => new QueuedLavalinkPlayer(properties));

    public static PlayerFactory<VoteLavalinkPlayer, VoteLavalinkPlayerOptions> Vote { get; } = Create<VoteLavalinkPlayer, VoteLavalinkPlayerOptions>(static properties => new VoteLavalinkPlayer(properties));

    public static PlayerFactory<TPlayer, TOptions> Create<TPlayer, TOptions>(Func<IPlayerProperties<TPlayer, TOptions>, TPlayer> factory)
        where TPlayer : ILavalinkPlayer
        where TOptions : LavalinkPlayerOptions
    {
        ArgumentNullException.ThrowIfNull(factory);

        ValueTask<TPlayer> CreateAsync(IPlayerProperties<TPlayer, TOptions> properties, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.FromResult(factory(properties));
        }

        return CreateAsync;
    }
}

public delegate ValueTask<TPlayer> PlayerFactory<TPlayer, TOptions>(IPlayerProperties<TPlayer, TOptions> properties, CancellationToken cancellationToken = default)
    where TPlayer : ILavalinkPlayer
    where TOptions : LavalinkPlayerOptions;