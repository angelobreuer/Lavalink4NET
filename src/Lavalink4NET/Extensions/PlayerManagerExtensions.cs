namespace Lavalink4NET.Players;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

public static class PlayerManagerExtensions
{
    public static ValueTask<TPlayer> JoinAsync<TPlayer, TOptions>(this IPlayerManager playerManager, ulong guildId, ulong voiceChannelId, PlayerFactory<TPlayer, TOptions> playerFactory, Action<TOptions>? configure, CancellationToken cancellationToken = default)
        where TPlayer : ILavalinkPlayer
        where TOptions : LavalinkPlayerOptions, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(playerFactory);
        ArgumentNullException.ThrowIfNull(configure);

        return playerManager.JoinAsync(
            guildId: guildId,
            voiceChannelId: voiceChannelId,
            playerFactory,
            options: CreateOptions(configure),
            cancellationToken: cancellationToken);
    }

    public static ValueTask<TPlayer> JoinAsync<TPlayer, TOptions>(this IPlayerManager playerManager, ulong guildId, ulong voiceChannelId, PlayerFactory<TPlayer, TOptions> playerFactory, TOptions options, CancellationToken cancellationToken = default)
        where TPlayer : ILavalinkPlayer
        where TOptions : LavalinkPlayerOptions
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(playerFactory);
        ArgumentNullException.ThrowIfNull(options);

        return playerManager.JoinAsync(
            guildId: guildId,
            voiceChannelId: voiceChannelId,
            playerFactory,
            options: Options.Create(options),
            cancellationToken: cancellationToken);
    }

    public static ValueTask<TPlayer> JoinAsync<TPlayer, TOptions>(this IPlayerManager playerManager, ulong guildId, ulong voiceChannelId, PlayerFactory<TPlayer, TOptions> playerFactory, CancellationToken cancellationToken = default)
        where TPlayer : ILavalinkPlayer
        where TOptions : LavalinkPlayerOptions, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(playerFactory);

        return playerManager.JoinAsync(
            guildId: guildId,
            voiceChannelId: voiceChannelId,
            playerFactory,
            options: Options.Create(new TOptions()),
            cancellationToken: cancellationToken);
    }

    public static ValueTask<ILavalinkPlayer> JoinAsync(this IPlayerManager playerManager, ulong guildId, ulong voiceChannelId, PlayerFactory<ILavalinkPlayer, LavalinkPlayerOptions> playerFactory, IOptions<LavalinkPlayerOptions> options, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(playerFactory);
        ArgumentNullException.ThrowIfNull(options);

        return playerManager.JoinAsync(
            guildId: guildId,
            voiceChannelId: voiceChannelId,
            playerFactory,
            options: options,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<ILavalinkPlayer> JoinAsync(this IPlayerManager playerManager, ulong guildId, ulong voiceChannelId, PlayerFactory<ILavalinkPlayer, LavalinkPlayerOptions> playerFactory, Action<LavalinkPlayerOptions>? configure, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(playerFactory);
        ArgumentNullException.ThrowIfNull(configure);

        return playerManager.JoinAsync(
            guildId: guildId,
            voiceChannelId: voiceChannelId,
            playerFactory,
            options: CreateOptions(configure),
            cancellationToken: cancellationToken);
    }

    public static ValueTask<ILavalinkPlayer> JoinAsync(this IPlayerManager playerManager, ulong guildId, ulong voiceChannelId, PlayerFactory<ILavalinkPlayer, LavalinkPlayerOptions> playerFactory, LavalinkPlayerOptions options, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(playerFactory);
        ArgumentNullException.ThrowIfNull(options);

        return playerManager.JoinAsync(
            guildId: guildId,
            voiceChannelId: voiceChannelId,
            playerFactory,
            options: Options.Create(options),
            cancellationToken: cancellationToken);
    }

    public static ValueTask<ILavalinkPlayer> JoinAsync(this IPlayerManager playerManager, ulong guildId, ulong voiceChannelId, PlayerFactory<ILavalinkPlayer, LavalinkPlayerOptions> playerFactory, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(playerFactory);

        return playerManager.JoinAsync(
            guildId: guildId,
            voiceChannelId: voiceChannelId,
            playerFactory,
            options: Options.Create(new LavalinkPlayerOptions()),
            cancellationToken: cancellationToken);
    }

    public static ValueTask<LavalinkPlayer> JoinAsync(this IPlayerManager playerManager, ulong guildId, ulong voiceChannelId, IOptions<LavalinkPlayerOptions> options, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(options);

        return playerManager.JoinAsync(
            guildId: guildId,
            voiceChannelId: voiceChannelId,
            PlayerFactory.Default,
            options: options,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<LavalinkPlayer> JoinAsync(this IPlayerManager playerManager, ulong guildId, ulong voiceChannelId, Action<LavalinkPlayerOptions>? configure, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);

        return playerManager.JoinAsync(
            guildId: guildId,
            voiceChannelId: voiceChannelId,
            PlayerFactory.Default,
            options: CreateOptions(configure),
            cancellationToken: cancellationToken);
    }

    public static ValueTask<LavalinkPlayer> JoinAsync(this IPlayerManager playerManager, ulong guildId, ulong voiceChannelId, LavalinkPlayerOptions options, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(options);

        return playerManager.JoinAsync(
            guildId: guildId,
            voiceChannelId: voiceChannelId,
            PlayerFactory.Default,
            options: Options.Create(options),
            cancellationToken: cancellationToken);
    }

    public static ValueTask<LavalinkPlayer> JoinAsync(this IPlayerManager playerManager, ulong guildId, ulong voiceChannelId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);

        return playerManager.JoinAsync(
            guildId: guildId,
            voiceChannelId: voiceChannelId,
            PlayerFactory.Default,
            options: Options.Create(new LavalinkPlayerOptions()),
            cancellationToken: cancellationToken);
    }

    private static IOptions<TOptions> CreateOptions<TOptions>(Action<TOptions>? configure)
        where TOptions : LavalinkPlayerOptions, new()
    {
        var options = new TOptions();
        configure?.Invoke(options);
        return Options.Create(options);
    }
}
