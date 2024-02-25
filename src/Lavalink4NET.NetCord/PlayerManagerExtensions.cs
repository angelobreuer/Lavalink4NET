namespace Lavalink4NET.NetCord;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using global::NetCord;
using global::NetCord.Rest;
using global::NetCord.Services;
using Lavalink4NET.Extensions;
using Lavalink4NET.NetCord;
using Lavalink4NET.Players;
using Microsoft.Extensions.Options;

/// <summary>
///     A set of different extension methods for the <see cref="IPlayerManager"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public static class PlayerManagerExtensions
{
    public static ValueTask<PlayerResult<TPlayer>> RetrieveAsync<TPlayer, TOptions>(
        this IPlayerManager playerManager,
        IGuildContext context,
        PlayerFactory<TPlayer, TOptions> playerFactory,
        IOptions<TOptions> options,
        PlayerRetrieveOptions retrieveOptions = default,
        CancellationToken cancellationToken = default)
        where TPlayer : class, ILavalinkPlayer
        where TOptions : LavalinkPlayerOptions
    {
        ArgumentNullException.ThrowIfNull(playerManager);

        var memberVoiceChannelId = context is IUserContext userContext && context.Guild!.VoiceStates.TryGetValue(userContext.User.Id, out var voiceState)
            ? voiceState.ChannelId
            : default(ulong?);

        return playerManager.RetrieveAsync(
            guildId: context.Guild!.Id,
            memberVoiceChannel: memberVoiceChannelId,
            playerFactory: playerFactory,
            options: options,
            retrieveOptions: retrieveOptions,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<PlayerResult<TPlayer>> RetrieveAsync<TPlayer, TOptions>(
        this IPlayerManager playerManager,
        IGuildContext context,
        PlayerFactory<TPlayer, TOptions> playerFactory,
        Action<TOptions>? configure,
        PlayerRetrieveOptions retrieveOptions = default,
        CancellationToken cancellationToken = default)
        where TPlayer : class, ILavalinkPlayer
        where TOptions : LavalinkPlayerOptions, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(playerFactory);
        ArgumentNullException.ThrowIfNull(configure);

        return playerManager.RetrieveAsync(
            context: context,
            playerFactory: playerFactory,
            options: CreateOptions(configure),
            retrieveOptions: retrieveOptions,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<PlayerResult<TPlayer>> RetrieveAsync<TPlayer, TOptions>(
        this IPlayerManager playerManager,
        IGuildContext context,
        PlayerFactory<TPlayer, TOptions> playerFactory,
        TOptions options,
        PlayerRetrieveOptions retrieveOptions = default,
        CancellationToken cancellationToken = default)
        where TPlayer : class, ILavalinkPlayer
        where TOptions : LavalinkPlayerOptions
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(playerFactory);
        ArgumentNullException.ThrowIfNull(options);

        return playerManager.RetrieveAsync(
            context: context,
            playerFactory: playerFactory,
            options: Options.Create(options),
            retrieveOptions: retrieveOptions,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<PlayerResult<TPlayer>> RetrieveAsync<TPlayer, TOptions>(
        this IPlayerManager playerManager,
        IGuildContext context,
        PlayerFactory<TPlayer, TOptions> playerFactory,
        PlayerRetrieveOptions retrieveOptions = default,
        CancellationToken cancellationToken = default)
        where TPlayer : class, ILavalinkPlayer
        where TOptions : LavalinkPlayerOptions, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(playerFactory);

        return playerManager.RetrieveAsync(
            context: context,
            playerFactory: playerFactory,
            options: Options.Create(new TOptions()),
            retrieveOptions: retrieveOptions,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<PlayerResult<ILavalinkPlayer>> RetrieveAsync(
        this IPlayerManager playerManager,
        IGuildContext context,
        PlayerFactory<ILavalinkPlayer, LavalinkPlayerOptions> playerFactory,
        Action<LavalinkPlayerOptions>? configure,
        PlayerRetrieveOptions retrieveOptions = default,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(playerFactory);
        ArgumentNullException.ThrowIfNull(configure);

        return playerManager.RetrieveAsync(
            context: context,
            playerFactory,
            options: CreateOptions(configure),
            retrieveOptions: retrieveOptions,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<PlayerResult<ILavalinkPlayer>> RetrieveAsync(
        this IPlayerManager playerManager,
        IGuildContext context,
        PlayerFactory<ILavalinkPlayer, LavalinkPlayerOptions> playerFactory,
        LavalinkPlayerOptions options,
        PlayerRetrieveOptions retrieveOptions = default,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(playerFactory);
        ArgumentNullException.ThrowIfNull(options);

        return playerManager.RetrieveAsync(
            context: context,
            playerFactory: playerFactory,
            options: Options.Create(options),
            retrieveOptions: retrieveOptions,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<PlayerResult<ILavalinkPlayer>> RetrieveAsync(
        this IPlayerManager playerManager,
        IGuildContext context,
        PlayerFactory<ILavalinkPlayer, LavalinkPlayerOptions> playerFactory,
        PlayerRetrieveOptions retrieveOptions = default,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(playerFactory);

        return playerManager.RetrieveAsync(
            context: context,
            playerFactory: playerFactory,
            options: Options.Create(new LavalinkPlayerOptions()),
            retrieveOptions: retrieveOptions,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<PlayerResult<LavalinkPlayer>> RetrieveAsync(
        this IPlayerManager playerManager,
        IGuildContext context,
        IOptions<LavalinkPlayerOptions> options,
        PlayerRetrieveOptions retrieveOptions = default,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(options);

        return playerManager.RetrieveAsync(
            context: context,
            playerFactory: PlayerFactory.Default,
            options: options,
            retrieveOptions: retrieveOptions,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<PlayerResult<LavalinkPlayer>> RetrieveAsync(
        this IPlayerManager playerManager,
        IGuildContext context,
        Action<LavalinkPlayerOptions>? configure,
        PlayerRetrieveOptions retrieveOptions = default,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);

        return playerManager.RetrieveAsync(
            context: context,
            playerFactory: PlayerFactory.Default,
            options: CreateOptions(configure),
            retrieveOptions: retrieveOptions,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<PlayerResult<LavalinkPlayer>> RetrieveAsync(
        this IPlayerManager playerManager,
        IGuildContext context,
        LavalinkPlayerOptions options,
        PlayerRetrieveOptions retrieveOptions = default,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(options);

        return playerManager.RetrieveAsync(
            context: context,
            playerFactory: PlayerFactory.Default,
            options: Options.Create(options),
            retrieveOptions: retrieveOptions,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<PlayerResult<LavalinkPlayer>> RetrieveAsync(
        this IPlayerManager playerManager,
        IGuildContext context,
        PlayerRetrieveOptions retrieveOptions = default,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);

        return playerManager.RetrieveAsync(
            context: context,
            playerFactory: PlayerFactory.Default,
            options: Options.Create(new LavalinkPlayerOptions()),
            retrieveOptions: retrieveOptions,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<T?> GetPlayerAsync<T>(this IPlayerManager playerManager, RestGuild guild, CancellationToken cancellationToken = default) where T : class, ILavalinkPlayer
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(guild);

        return playerManager.GetPlayerAsync<T>(guild.Id, cancellationToken);
    }

    public static ValueTask<ILavalinkPlayer?> GetPlayerAsync(this IPlayerManager playerManager, RestGuild guild, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(guild);

        return playerManager.GetPlayerAsync(guild.Id, cancellationToken);
    }

    public static bool HasPlayer(this IPlayerManager playerManager, RestGuild guild)
    {
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(guild);

        return playerManager.HasPlayer(guild.Id);
    }

    public static ValueTask<TPlayer> JoinAsync<TPlayer, TOptions>(
        this IPlayerManager playerManager,
        IVoiceGuildChannel voiceChannel,
        PlayerFactory<TPlayer, TOptions> playerFactory,
        IOptions<TOptions> options,
        CancellationToken cancellationToken = default)
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

    public static ValueTask<TPlayer> JoinAsync<TPlayer, TOptions>(this IPlayerManager playerManager, IVoiceGuildChannel voiceChannel, PlayerFactory<TPlayer, TOptions> playerFactory, Action<TOptions>? configure, CancellationToken cancellationToken = default)
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

    public static ValueTask<TPlayer> JoinAsync<TPlayer, TOptions>(this IPlayerManager playerManager, IVoiceGuildChannel voiceChannel, PlayerFactory<TPlayer, TOptions> playerFactory, TOptions options, CancellationToken cancellationToken = default)
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

    public static ValueTask<TPlayer> JoinAsync<TPlayer, TOptions>(this IPlayerManager playerManager, IVoiceGuildChannel voiceChannel, PlayerFactory<TPlayer, TOptions> playerFactory, CancellationToken cancellationToken = default)
        where TPlayer : ILavalinkPlayer
        where TOptions : LavalinkPlayerOptions, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);
        ArgumentNullException.ThrowIfNull(playerFactory);

        return playerManager.JoinAsync(voiceChannel.GuildId, voiceChannel.Id, playerFactory, cancellationToken);
    }

    public static ValueTask<ILavalinkPlayer> JoinAsync(this IPlayerManager playerManager, IVoiceGuildChannel voiceChannel, PlayerFactory<ILavalinkPlayer, LavalinkPlayerOptions> playerFactory, IOptions<LavalinkPlayerOptions> options, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);
        ArgumentNullException.ThrowIfNull(playerFactory);
        ArgumentNullException.ThrowIfNull(options);

        return playerManager.JoinAsync(voiceChannel.GuildId, voiceChannel.Id, playerFactory, options, cancellationToken);
    }

    public static ValueTask<ILavalinkPlayer> JoinAsync(this IPlayerManager playerManager, IVoiceGuildChannel voiceChannel, PlayerFactory<ILavalinkPlayer, LavalinkPlayerOptions> playerFactory, Action<LavalinkPlayerOptions>? configure, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);
        ArgumentNullException.ThrowIfNull(playerFactory);
        ArgumentNullException.ThrowIfNull(configure);

        return playerManager.JoinAsync(voiceChannel.GuildId, voiceChannel.Id, playerFactory, configure, cancellationToken);
    }

    public static ValueTask<ILavalinkPlayer> JoinAsync(this IPlayerManager playerManager, IVoiceGuildChannel voiceChannel, PlayerFactory<ILavalinkPlayer, LavalinkPlayerOptions> playerFactory, LavalinkPlayerOptions options, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);
        ArgumentNullException.ThrowIfNull(playerFactory);
        ArgumentNullException.ThrowIfNull(options);

        return playerManager.JoinAsync(voiceChannel.GuildId, voiceChannel.Id, playerFactory, options, cancellationToken);
    }

    public static ValueTask<ILavalinkPlayer> JoinAsync(this IPlayerManager playerManager, IVoiceGuildChannel voiceChannel, PlayerFactory<ILavalinkPlayer, LavalinkPlayerOptions> playerFactory, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);
        ArgumentNullException.ThrowIfNull(playerFactory);

        return playerManager.JoinAsync(voiceChannel.GuildId, voiceChannel.Id, playerFactory, cancellationToken);
    }

    public static ValueTask<LavalinkPlayer> JoinAsync(this IPlayerManager playerManager, IVoiceGuildChannel voiceChannel, IOptions<LavalinkPlayerOptions> options, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);
        ArgumentNullException.ThrowIfNull(options);

        return playerManager.JoinAsync(voiceChannel.GuildId, voiceChannel.Id, options, cancellationToken);
    }

    public static ValueTask<LavalinkPlayer> JoinAsync(this IPlayerManager playerManager, IVoiceGuildChannel voiceChannel, Action<LavalinkPlayerOptions>? configure, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);

        return playerManager.JoinAsync(voiceChannel.GuildId, voiceChannel.Id, configure, cancellationToken);
    }

    public static ValueTask<LavalinkPlayer> JoinAsync(this IPlayerManager playerManager, IVoiceGuildChannel voiceChannel, LavalinkPlayerOptions options, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);
        ArgumentNullException.ThrowIfNull(options);

        return playerManager.JoinAsync(voiceChannel.GuildId, voiceChannel.Id, options, cancellationToken);
    }

    public static ValueTask<LavalinkPlayer> JoinAsync(this IPlayerManager playerManager, IVoiceGuildChannel voiceChannel, CancellationToken cancellationToken = default)
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
