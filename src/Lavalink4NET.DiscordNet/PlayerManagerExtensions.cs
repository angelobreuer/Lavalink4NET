namespace Lavalink4NET.DiscordNet;

using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Lavalink4NET.Clients;
using Lavalink4NET.Extensions;
using Lavalink4NET.Players;
using Microsoft.Extensions.Options;

/// <summary>
///     A set of different extension methods for the <see cref="IPlayerManager"/> class.
/// </summary>
public static class PlayerManagerExtensions
{
    public static async ValueTask<PlayerJoinResult<TPlayer>> GetOrJoinAsync<TPlayer, TOptions>(
        this IPlayerManager playerManager,
        IInteractionContext interactionContext,
        PlayerFactory<TPlayer, TOptions> playerFactory,
        IOptions<TOptions> options,
        PlayerJoinOptions joinOptions = default,
        CancellationToken cancellationToken = default)
        where TPlayer : class, ILavalinkPlayer
        where TOptions : LavalinkPlayerOptions
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(interactionContext);
        ArgumentNullException.ThrowIfNull(playerFactory);
        ArgumentNullException.ThrowIfNull(options);

        var player = await playerManager
            .GetPlayerAsync<TPlayer>(interactionContext.Guild.Id, cancellationToken)
            .ConfigureAwait(false);

        if (player is not null)
        {
            return new PlayerJoinResult<TPlayer>(player);
        }

        var requestOptions = cancellationToken.CanBeCanceled
            ? new RequestOptions { CancelToken = cancellationToken, }
            : RequestOptions.Default;

        var user = await interactionContext.Guild
            .GetUserAsync(interactionContext.User.Id, options: requestOptions)
            .ConfigureAwait(false);

        if (user.VoiceChannel is null)
        {
            return new PlayerJoinResult<TPlayer>(PlayerJoinStatus.UserNotInVoiceChannel);
        }

        var connectToVoiceChannel = joinOptions.ConnectToVoiceChannel.GetValueOrDefault(true);

        if (!connectToVoiceChannel)
        {
            return new PlayerJoinResult<TPlayer>(PlayerJoinStatus.BotNotConnected);
        }

        player = await playerManager
            .JoinAsync(user.Guild.Id, user.VoiceChannel.Id, playerFactory, options, cancellationToken)
            .ConfigureAwait(false);

        return new PlayerJoinResult<TPlayer>(player);
    }

    public static ValueTask<PlayerJoinResult<TPlayer>> GetOrJoinAsync<TPlayer, TOptions>(
        this IPlayerManager playerManager,
        IInteractionContext interactionContext,
        PlayerFactory<TPlayer, TOptions> playerFactory,
        Action<TOptions>? configure,
        PlayerJoinOptions joinOptions = default,
        CancellationToken cancellationToken = default)
        where TPlayer : class, ILavalinkPlayer
        where TOptions : LavalinkPlayerOptions, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(playerFactory);
        ArgumentNullException.ThrowIfNull(configure);

        return playerManager.GetOrJoinAsync(
            interactionContext,
            playerFactory,
            options: CreateOptions(configure),
            joinOptions,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<PlayerJoinResult<TPlayer>> GetOrJoinAsync<TPlayer, TOptions>(
        this IPlayerManager playerManager,
        IInteractionContext interactionContext,
        PlayerFactory<TPlayer, TOptions> playerFactory,
        TOptions options,
        PlayerJoinOptions joinOptions = default,
        CancellationToken cancellationToken = default)
        where TPlayer : class, ILavalinkPlayer
        where TOptions : LavalinkPlayerOptions
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(playerFactory);
        ArgumentNullException.ThrowIfNull(options);

        return playerManager.GetOrJoinAsync(
            interactionContext: interactionContext,
            playerFactory: playerFactory,
            options: Options.Create(options),
            joinOptions: joinOptions,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<PlayerJoinResult<TPlayer>> GetOrJoinAsync<TPlayer, TOptions>(
        this IPlayerManager playerManager,
        IInteractionContext interactionContext,
        PlayerFactory<TPlayer, TOptions> playerFactory,
        PlayerJoinOptions joinOptions = default,
        CancellationToken cancellationToken = default)
        where TPlayer : class, ILavalinkPlayer
        where TOptions : LavalinkPlayerOptions, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(playerFactory);

        return playerManager.GetOrJoinAsync(
            interactionContext: interactionContext,
            playerFactory: playerFactory,
            options: Options.Create(new TOptions()),
            joinOptions: joinOptions,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<PlayerJoinResult<ILavalinkPlayer>> GetOrJoinAsync(
        this IPlayerManager playerManager,
        IInteractionContext interactionContext,
        PlayerFactory<ILavalinkPlayer, LavalinkPlayerOptions> playerFactory,
        IOptions<LavalinkPlayerOptions> options,
        PlayerJoinOptions joinOptions = default,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(playerFactory);
        ArgumentNullException.ThrowIfNull(options);

        return playerManager.GetOrJoinAsync(
            interactionContext: interactionContext,
            playerFactory: playerFactory,
            options: options,
            joinOptions: joinOptions,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<PlayerJoinResult<ILavalinkPlayer>> GetOrJoinAsync(
        this IPlayerManager playerManager,
        IInteractionContext interactionContext,
        PlayerFactory<ILavalinkPlayer, LavalinkPlayerOptions> playerFactory,
        Action<LavalinkPlayerOptions>? configure,
        PlayerJoinOptions joinOptions = default,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(playerFactory);
        ArgumentNullException.ThrowIfNull(configure);

        return playerManager.GetOrJoinAsync(
            interactionContext: interactionContext,
            playerFactory,
            options: CreateOptions(configure),
            joinOptions: joinOptions,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<PlayerJoinResult<ILavalinkPlayer>> GetOrJoinAsync(
        this IPlayerManager playerManager,
        IInteractionContext interactionContext,
        PlayerFactory<ILavalinkPlayer, LavalinkPlayerOptions> playerFactory,
        LavalinkPlayerOptions options,
        PlayerJoinOptions joinOptions = default,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(playerFactory);
        ArgumentNullException.ThrowIfNull(options);

        return playerManager.GetOrJoinAsync(
            interactionContext: interactionContext,
            playerFactory: playerFactory,
            options: Options.Create(options),
            joinOptions: joinOptions,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<PlayerJoinResult<ILavalinkPlayer>> GetOrJoinAsync(
        this IPlayerManager playerManager,
        IInteractionContext interactionContext,
        PlayerFactory<ILavalinkPlayer, LavalinkPlayerOptions> playerFactory,
        PlayerJoinOptions joinOptions = default,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(playerFactory);

        return playerManager.GetOrJoinAsync(
            interactionContext: interactionContext,
            playerFactory: playerFactory,
            options: Options.Create(new LavalinkPlayerOptions()),
            joinOptions: joinOptions,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<PlayerJoinResult<LavalinkPlayer>> GetOrJoinAsync(
        this IPlayerManager playerManager,
        IInteractionContext interactionContext,
        IOptions<LavalinkPlayerOptions> options,
        PlayerJoinOptions joinOptions = default,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(options);

        return playerManager.GetOrJoinAsync(
            interactionContext: interactionContext,
            playerFactory: PlayerFactory.Default,
            options: options,
            joinOptions: joinOptions,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<PlayerJoinResult<LavalinkPlayer>> GetOrJoinAsync(
        this IPlayerManager playerManager,
        IInteractionContext interactionContext,
        Action<LavalinkPlayerOptions>? configure,
        PlayerJoinOptions joinOptions = default,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);

        return playerManager.GetOrJoinAsync(
            interactionContext: interactionContext,
            playerFactory: PlayerFactory.Default,
            options: CreateOptions(configure),
            joinOptions: joinOptions,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<PlayerJoinResult<LavalinkPlayer>> GetOrJoinAsync(
        this IPlayerManager playerManager,
        IInteractionContext interactionContext,
        LavalinkPlayerOptions options,
        PlayerJoinOptions joinOptions = default,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(options);

        return playerManager.GetOrJoinAsync(
            interactionContext: interactionContext,
            playerFactory: PlayerFactory.Default,
            options: Options.Create(options),
            joinOptions: joinOptions,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<PlayerJoinResult<LavalinkPlayer>> GetOrJoinAsync(
        this IPlayerManager playerManager,
        IInteractionContext interactionContext,
        PlayerJoinOptions joinOptions = default,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);

        return playerManager.GetOrJoinAsync(
            interactionContext: interactionContext,
            playerFactory: PlayerFactory.Default,
            options: Options.Create(new LavalinkPlayerOptions()),
            joinOptions: joinOptions,
            cancellationToken: cancellationToken);
    }

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

    private static IOptions<TOptions> CreateOptions<TOptions>(Action<TOptions>? configure)
        where TOptions : LavalinkPlayerOptions, new()
    {
        var options = new TOptions();
        configure?.Invoke(options);
        return Options.Create(options);
    }
}
