namespace Lavalink4NET.Players;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Clients;
using Lavalink4NET.Events;
using Lavalink4NET.Events.Players;
using Microsoft.Extensions.Options;

public interface IPlayerManager
{
    event AsyncEventHandler<PlayerCreatedEventArgs>? PlayerCreated;

    event AsyncEventHandler<PlayerDestroyedEventArgs>? PlayerDestroyed;

    event AsyncEventHandler<PlayerStateChangedEventArgs>? PlayerStateChanged;

    IDiscordClientWrapper DiscordClient { get; }

    IEnumerable<ILavalinkPlayer> Players { get; }

    ValueTask<ILavalinkPlayer?> GetPlayerAsync(ulong guildId, CancellationToken cancellationToken = default);

    ValueTask<T?> GetPlayerAsync<T>(ulong guildId, CancellationToken cancellationToken = default) where T : class, ILavalinkPlayer;

    bool TryGetPlayer(ulong guildId, [MaybeNullWhen(false)] out ILavalinkPlayer player);

    bool TryGetPlayer<T>(ulong guildId, [MaybeNullWhen(false)] out T? player) where T : class, ILavalinkPlayer;

    IEnumerable<T> GetPlayers<T>() where T : ILavalinkPlayer;

    bool HasPlayer(ulong guildId);

    ValueTask<TPlayer> JoinAsync<TPlayer, TOptions>(ulong guildId, ulong voiceChannelId, PlayerFactory<TPlayer, TOptions> playerFactory, IOptions<TOptions> options, CancellationToken cancellationToken = default)
        where TPlayer : ILavalinkPlayer
        where TOptions : LavalinkPlayerOptions;

    ValueTask<PlayerResult<TPlayer>> RetrieveAsync<TPlayer, TOptions>(
        ulong guildId,
        ulong? memberVoiceChannel,
        PlayerFactory<TPlayer, TOptions> playerFactory,
        IOptions<TOptions> options,
        PlayerRetrieveOptions retrieveOptions = default,
        CancellationToken cancellationToken = default)
        where TPlayer : class, ILavalinkPlayer
        where TOptions : LavalinkPlayerOptions;
}