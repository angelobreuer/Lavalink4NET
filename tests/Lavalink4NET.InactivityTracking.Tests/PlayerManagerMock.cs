namespace Lavalink4NET.InactivityTracking.Tests;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Clients;
using Lavalink4NET.Events;
using Lavalink4NET.Events.Players;
using Lavalink4NET.Players;
using Microsoft.Extensions.Options;

internal class PlayerManagerMock : IPlayerManager
{
    private readonly IReadOnlyDictionary<ulong, ILavalinkPlayer> _players;

    public event AsyncEventHandler<PlayerCreatedEventArgs>? PlayerCreated;
    public event AsyncEventHandler<PlayerDestroyedEventArgs>? PlayerDestroyed;
    public event AsyncEventHandler<PlayerStateChangedEventArgs>? PlayerStateChanged;

    public PlayerManagerMock(IDiscordClientWrapper discordClient, IEnumerable<ILavalinkPlayer> players)
    {
        ArgumentNullException.ThrowIfNull(discordClient);
        ArgumentNullException.ThrowIfNull(players);

        _players = players.ToDictionary(x => x.GuildId);
        DiscordClient = discordClient;
    }

    public IDiscordClientWrapper DiscordClient { get; }

    public IEnumerable<ILavalinkPlayer> Players => _players.Values;

    public ValueTask<ILavalinkPlayer?> GetPlayerAsync(ulong guildId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return new ValueTask<ILavalinkPlayer?>(_players.GetValueOrDefault(guildId));
    }

    public IEnumerable<T> GetPlayers<T>() where T : ILavalinkPlayer
    {
        return Players.OfType<T>();
    }

    public bool HasPlayer(ulong guildId)
    {
        return _players.ContainsKey(guildId);
    }

    public ValueTask<TPlayer> JoinAsync<TPlayer, TOptions>(ulong guildId, ulong voiceChannelId, PlayerFactory<TPlayer, TOptions> playerFactory, IOptions<TOptions> options, CancellationToken cancellationToken = default)
        where TPlayer : ILavalinkPlayer
        where TOptions : LavalinkPlayerOptions
    {
        throw new NotImplementedException();
    }

    public bool TryGetPlayer(ulong guildId, [MaybeNullWhen(false)] out ILavalinkPlayer player)
    {
        return _players.TryGetValue(guildId, out player);
    }

    public bool TryGetPlayer<T>(ulong guildId, [MaybeNullWhen(false)] out T? player) where T : class, ILavalinkPlayer
    {
        player = _players.GetValueOrDefault(guildId) as T;
        return player is not null;
    }

    ValueTask<T?> IPlayerManager.GetPlayerAsync<T>(ulong guildId, CancellationToken cancellationToken) where T : class
    {
        cancellationToken.ThrowIfCancellationRequested();
        return new ValueTask<T?>(_players.GetValueOrDefault(guildId) as T);
    }

    public ValueTask<PlayerResult<TPlayer>> RetrieveAsync<TPlayer, TOptions>(
        ulong guildId,
        ulong? memberVoiceChannel,
        PlayerFactory<TPlayer, TOptions> playerFactory,
        IOptions<TOptions> options,
        PlayerRetrieveOptions retrieveOptions = default,
        CancellationToken cancellationToken = default)
        where TPlayer : class, ILavalinkPlayer
        where TOptions : LavalinkPlayerOptions
    {
        throw new NotImplementedException();
    }
}
