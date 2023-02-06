namespace Lavalink4NET.DiscordNet;

using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Lavalink4NET.Players;
using Microsoft.Extensions.Options;

/// <summary>
///     A set of different extension methods for the <see cref="IPlayerManager"/> class.
/// </summary>
public static class PlayerManagerExtensions
{
    public static ValueTask<T?> GetPlayerAsync<T>(this IPlayerManager playerManager, IGuild guild, CancellationToken cancellationToken = default) where T : class, ILavalinkPlayer
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(guild);

        return playerManager.GetPlayerAsync<T>(guild.Id, cancellationToken);
    }

    public static ValueTask<ILavalinkPlayer?> GetPlayerAsync(this IPlayerManager playerManager, IGuild guild, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(guild);

        return playerManager.GetPlayerAsync(guild.Id, cancellationToken);
    }

    public static bool HasPlayer(this IPlayerManager playerManager, IGuild guild)
    {
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(guild);

        return playerManager.HasPlayer(guild.Id);
    }

    public static ValueTask<TPlayer> JoinAsync<TPlayer, TOptions>(this IPlayerManager playerManager, IVoiceChannel voiceChannel, PlayerFactory<TPlayer, TOptions> playerFactory, IOptions<TOptions> options, CancellationToken cancellationToken = default)
        where TPlayer : ILavalinkPlayer
        where TOptions : LavalinkPlayerOptions
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);
        ArgumentNullException.ThrowIfNull(playerFactory);
        ArgumentNullException.ThrowIfNull(options);

        return playerManager.JoinAsync(voiceChannel.GuildId, voiceChannel.Id, playerFactory, options, cancellationToken);
    }


    public static ValueTask<TPlayer> JoinAsync<TPlayer, TOptions>(this IPlayerManager playerManager, IVoiceChannel voiceChannel, PlayerFactory<TPlayer, TOptions> playerFactory, Action<TOptions>? configure, CancellationToken cancellationToken = default)
        where TPlayer : ILavalinkPlayer
        where TOptions : LavalinkPlayerOptions, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);
        ArgumentNullException.ThrowIfNull(playerFactory);
        ArgumentNullException.ThrowIfNull(configure);

        return playerManager.JoinAsync(voiceChannel.GuildId, voiceChannel.Id, playerFactory, configure, cancellationToken);
    }

    public static ValueTask<TPlayer> JoinAsync<TPlayer, TOptions>(this IPlayerManager playerManager, IVoiceChannel voiceChannel, PlayerFactory<TPlayer, TOptions> playerFactory, TOptions options, CancellationToken cancellationToken = default)
        where TPlayer : ILavalinkPlayer
        where TOptions : LavalinkPlayerOptions
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);
        ArgumentNullException.ThrowIfNull(playerFactory);
        ArgumentNullException.ThrowIfNull(options);

        return playerManager.JoinAsync(voiceChannel.GuildId, voiceChannel.Id, playerFactory, options, cancellationToken);
    }

    public static ValueTask<TPlayer> JoinAsync<TPlayer, TOptions>(this IPlayerManager playerManager, IVoiceChannel voiceChannel, PlayerFactory<TPlayer, TOptions> playerFactory, CancellationToken cancellationToken = default)
        where TPlayer : ILavalinkPlayer
        where TOptions : LavalinkPlayerOptions, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);
        ArgumentNullException.ThrowIfNull(playerFactory);

        return playerManager.JoinAsync(voiceChannel.GuildId, voiceChannel.Id, playerFactory, cancellationToken);
    }

    public static ValueTask<ILavalinkPlayer> JoinAsync(this IPlayerManager playerManager, IVoiceChannel voiceChannel, PlayerFactory<ILavalinkPlayer, LavalinkPlayerOptions> playerFactory, IOptions<LavalinkPlayerOptions> options, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);
        ArgumentNullException.ThrowIfNull(playerFactory);
        ArgumentNullException.ThrowIfNull(options);

        return playerManager.JoinAsync(voiceChannel.GuildId, voiceChannel.Id, playerFactory, options, cancellationToken);
    }

    public static ValueTask<ILavalinkPlayer> JoinAsync(this IPlayerManager playerManager, IVoiceChannel voiceChannel, PlayerFactory<ILavalinkPlayer, LavalinkPlayerOptions> playerFactory, Action<LavalinkPlayerOptions>? configure, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);
        ArgumentNullException.ThrowIfNull(playerFactory);
        ArgumentNullException.ThrowIfNull(configure);

        return playerManager.JoinAsync(voiceChannel.GuildId, voiceChannel.Id, playerFactory, configure, cancellationToken);
    }

    public static ValueTask<ILavalinkPlayer> JoinAsync(this IPlayerManager playerManager, IVoiceChannel voiceChannel, PlayerFactory<ILavalinkPlayer, LavalinkPlayerOptions> playerFactory, LavalinkPlayerOptions options, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);
        ArgumentNullException.ThrowIfNull(playerFactory);
        ArgumentNullException.ThrowIfNull(options);

        return playerManager.JoinAsync(voiceChannel.GuildId, voiceChannel.Id, playerFactory, options, cancellationToken);
    }

    public static ValueTask<ILavalinkPlayer> JoinAsync(this IPlayerManager playerManager, IVoiceChannel voiceChannel, PlayerFactory<ILavalinkPlayer, LavalinkPlayerOptions> playerFactory, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);
        ArgumentNullException.ThrowIfNull(playerFactory);

        return playerManager.JoinAsync(voiceChannel.GuildId, voiceChannel.Id, playerFactory, cancellationToken);
    }

    public static ValueTask<LavalinkPlayer> JoinAsync(this IPlayerManager playerManager, IVoiceChannel voiceChannel, IOptions<LavalinkPlayerOptions> options, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);
        ArgumentNullException.ThrowIfNull(options);

        return playerManager.JoinAsync(voiceChannel.GuildId, voiceChannel.Id, options, cancellationToken);
    }

    public static ValueTask<LavalinkPlayer> JoinAsync(this IPlayerManager playerManager, IVoiceChannel voiceChannel, Action<LavalinkPlayerOptions>? configure, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);

        return playerManager.JoinAsync(voiceChannel.GuildId, voiceChannel.Id, configure, cancellationToken);
    }

    public static ValueTask<LavalinkPlayer> JoinAsync(this IPlayerManager playerManager, IVoiceChannel voiceChannel, LavalinkPlayerOptions options, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);
        ArgumentNullException.ThrowIfNull(options);

        return playerManager.JoinAsync(voiceChannel.GuildId, voiceChannel.Id, options, cancellationToken);
    }

    public static ValueTask<LavalinkPlayer> JoinAsync(this IPlayerManager playerManager, IVoiceChannel voiceChannel, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);

        return playerManager.JoinAsync(voiceChannel.GuildId, voiceChannel.Id, cancellationToken);
    }
}
