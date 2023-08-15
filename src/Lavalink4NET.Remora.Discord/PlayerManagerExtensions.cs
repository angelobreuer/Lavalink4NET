namespace Lavalink4NET.Remora.Discord;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using global::Remora.Discord.API.Abstractions.Objects;
using global::Remora.Discord.Commands.Contexts;
using global::Remora.Discord.Commands.Extensions;
using Lavalink4NET.Extensions;
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
        IOperationContext operationContext,
        PlayerFactory<TPlayer, TOptions> playerFactory,
        IOptions<TOptions> options,
        PlayerRetrieveOptions retrieveOptions = default,
        CancellationToken cancellationToken = default)
        where TPlayer : class, ILavalinkPlayer
        where TOptions : LavalinkPlayerOptions
    {
        ArgumentNullException.ThrowIfNull(playerManager);

        if (!operationContext.TryGetGuildID(out var guildId))
        {
            throw new InvalidOperationException("The operation context must be a guild operation context.");
        }

        var discordClient = (DiscordClientWrapper)playerManager.DiscordClient;

        var memberVoiceChannelId = operationContext.TryGetUserID(out var userId) &&
            discordClient.TryGetUserChannelId(guildId.Value, userId.Value, out var memberVoiceChannel)
            ? memberVoiceChannel : default(ulong?);

        return playerManager.RetrieveAsync(
            guildId: guildId.Value,
            memberVoiceChannel: memberVoiceChannelId,
            playerFactory: playerFactory,
            options: options,
            retrieveOptions: retrieveOptions,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<PlayerResult<TPlayer>> RetrieveAsync<TPlayer, TOptions>(
        this IPlayerManager playerManager,
        IOperationContext operationContext,
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
            operationContext: operationContext,
            playerFactory: playerFactory,
            options: CreateOptions(configure),
            retrieveOptions: retrieveOptions,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<PlayerResult<TPlayer>> RetrieveAsync<TPlayer, TOptions>(
        this IPlayerManager playerManager,
        IOperationContext operationContext,
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
            operationContext: operationContext,
            playerFactory: playerFactory,
            options: Options.Create(options),
            retrieveOptions: retrieveOptions,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<PlayerResult<TPlayer>> RetrieveAsync<TPlayer, TOptions>(
        this IPlayerManager playerManager,
        IOperationContext operationContext,
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
            operationContext: operationContext,
            playerFactory: playerFactory,
            options: Options.Create(new TOptions()),
            retrieveOptions: retrieveOptions,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<PlayerResult<ILavalinkPlayer>> RetrieveAsync(
        this IPlayerManager playerManager,
        IOperationContext operationContext,
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
            operationContext: operationContext,
            playerFactory,
            options: CreateOptions(configure),
            retrieveOptions: retrieveOptions,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<PlayerResult<ILavalinkPlayer>> RetrieveAsync(
        this IPlayerManager playerManager,
        IOperationContext operationContext,
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
            operationContext: operationContext,
            playerFactory: playerFactory,
            options: Options.Create(options),
            retrieveOptions: retrieveOptions,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<PlayerResult<ILavalinkPlayer>> RetrieveAsync(
        this IPlayerManager playerManager,
        IOperationContext operationContext,
        PlayerFactory<ILavalinkPlayer, LavalinkPlayerOptions> playerFactory,
        PlayerRetrieveOptions retrieveOptions = default,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(playerFactory);

        return playerManager.RetrieveAsync(
            operationContext: operationContext,
            playerFactory: playerFactory,
            options: Options.Create(new LavalinkPlayerOptions()),
            retrieveOptions: retrieveOptions,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<PlayerResult<LavalinkPlayer>> RetrieveAsync(
        this IPlayerManager playerManager,
        IOperationContext operationContext,
        IOptions<LavalinkPlayerOptions> options,
        PlayerRetrieveOptions retrieveOptions = default,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(options);

        return playerManager.RetrieveAsync(
            operationContext: operationContext,
            playerFactory: PlayerFactory.Default,
            options: options,
            retrieveOptions: retrieveOptions,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<PlayerResult<LavalinkPlayer>> RetrieveAsync(
        this IPlayerManager playerManager,
        IOperationContext operationContext,
        Action<LavalinkPlayerOptions>? configure,
        PlayerRetrieveOptions retrieveOptions = default,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);

        return playerManager.RetrieveAsync(
            operationContext: operationContext,
            playerFactory: PlayerFactory.Default,
            options: CreateOptions(configure),
            retrieveOptions: retrieveOptions,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<PlayerResult<LavalinkPlayer>> RetrieveAsync(
        this IPlayerManager playerManager,
        IOperationContext operationContext,
        LavalinkPlayerOptions options,
        PlayerRetrieveOptions retrieveOptions = default,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(options);

        return playerManager.RetrieveAsync(
            operationContext: operationContext,
            playerFactory: PlayerFactory.Default,
            options: Options.Create(options),
            retrieveOptions: retrieveOptions,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<PlayerResult<LavalinkPlayer>> RetrieveAsync(
        this IPlayerManager playerManager,
        IOperationContext operationContext,
        PlayerRetrieveOptions retrieveOptions = default,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);

        return playerManager.RetrieveAsync(
            operationContext: operationContext,
            playerFactory: PlayerFactory.Default,
            options: Options.Create(new LavalinkPlayerOptions()),
            retrieveOptions: retrieveOptions,
            cancellationToken: cancellationToken);
    }

    public static ValueTask<T?> GetPlayerAsync<T>(this IPlayerManager playerManager, IGuild guild, CancellationToken cancellationToken = default) where T : class, ILavalinkPlayer
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(guild);

        return playerManager.GetPlayerAsync<T>(guild.ID.Value, cancellationToken);
    }

    public static ValueTask<ILavalinkPlayer?> GetPlayerAsync(this IPlayerManager playerManager, IGuild guild, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(guild);

        return playerManager.GetPlayerAsync(guild.ID.Value, cancellationToken);
    }

    public static bool HasPlayer(this IPlayerManager playerManager, IGuild guild)
    {
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(guild);

        return playerManager.HasPlayer(guild.ID.Value);
    }

    public static ValueTask<TPlayer> JoinAsync<TPlayer, TOptions>(
        this IPlayerManager playerManager,
        IChannel voiceChannel,
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

        return playerManager.JoinAsync(voiceChannel.GuildID.Value.Value, voiceChannel.ID.Value, playerFactory, options, cancellationToken);
    }

    public static ValueTask<TPlayer> JoinAsync<TPlayer, TOptions>(this IPlayerManager playerManager, IChannel voiceChannel, PlayerFactory<TPlayer, TOptions> playerFactory, Action<TOptions>? configure, CancellationToken cancellationToken = default)
        where TPlayer : ILavalinkPlayer
        where TOptions : LavalinkPlayerOptions, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);
        ArgumentNullException.ThrowIfNull(playerFactory);
        ArgumentNullException.ThrowIfNull(configure);

        return playerManager.JoinAsync(voiceChannel.GuildID.Value.Value, voiceChannel.ID.Value, playerFactory, configure, cancellationToken);
    }

    public static ValueTask<TPlayer> JoinAsync<TPlayer, TOptions>(this IPlayerManager playerManager, IChannel voiceChannel, PlayerFactory<TPlayer, TOptions> playerFactory, TOptions options, CancellationToken cancellationToken = default)
        where TPlayer : ILavalinkPlayer
        where TOptions : LavalinkPlayerOptions
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);
        ArgumentNullException.ThrowIfNull(playerFactory);
        ArgumentNullException.ThrowIfNull(options);

        return playerManager.JoinAsync(voiceChannel.GuildID.Value.Value, voiceChannel.ID.Value, playerFactory, options, cancellationToken);
    }

    public static ValueTask<TPlayer> JoinAsync<TPlayer, TOptions>(this IPlayerManager playerManager, IChannel voiceChannel, PlayerFactory<TPlayer, TOptions> playerFactory, CancellationToken cancellationToken = default)
        where TPlayer : ILavalinkPlayer
        where TOptions : LavalinkPlayerOptions, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);
        ArgumentNullException.ThrowIfNull(playerFactory);

        return playerManager.JoinAsync(voiceChannel.GuildID.Value.Value, voiceChannel.ID.Value, playerFactory, cancellationToken);
    }

    public static ValueTask<ILavalinkPlayer> JoinAsync(this IPlayerManager playerManager, IChannel voiceChannel, PlayerFactory<ILavalinkPlayer, LavalinkPlayerOptions> playerFactory, IOptions<LavalinkPlayerOptions> options, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);
        ArgumentNullException.ThrowIfNull(playerFactory);
        ArgumentNullException.ThrowIfNull(options);

        return playerManager.JoinAsync(voiceChannel.GuildID.Value.Value, voiceChannel.ID.Value, playerFactory, options, cancellationToken);
    }

    public static ValueTask<ILavalinkPlayer> JoinAsync(this IPlayerManager playerManager, IChannel voiceChannel, PlayerFactory<ILavalinkPlayer, LavalinkPlayerOptions> playerFactory, Action<LavalinkPlayerOptions>? configure, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);
        ArgumentNullException.ThrowIfNull(playerFactory);
        ArgumentNullException.ThrowIfNull(configure);

        return playerManager.JoinAsync(voiceChannel.GuildID.Value.Value, voiceChannel.ID.Value, playerFactory, configure, cancellationToken);
    }

    public static ValueTask<ILavalinkPlayer> JoinAsync(this IPlayerManager playerManager, IChannel voiceChannel, PlayerFactory<ILavalinkPlayer, LavalinkPlayerOptions> playerFactory, LavalinkPlayerOptions options, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);
        ArgumentNullException.ThrowIfNull(playerFactory);
        ArgumentNullException.ThrowIfNull(options);

        return playerManager.JoinAsync(voiceChannel.GuildID.Value.Value, voiceChannel.ID.Value, playerFactory, options, cancellationToken);
    }

    public static ValueTask<ILavalinkPlayer> JoinAsync(this IPlayerManager playerManager, IChannel voiceChannel, PlayerFactory<ILavalinkPlayer, LavalinkPlayerOptions> playerFactory, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);
        ArgumentNullException.ThrowIfNull(playerFactory);

        return playerManager.JoinAsync(voiceChannel.GuildID.Value.Value, voiceChannel.ID.Value, playerFactory, cancellationToken);
    }

    public static ValueTask<LavalinkPlayer> JoinAsync(this IPlayerManager playerManager, IChannel voiceChannel, IOptions<LavalinkPlayerOptions> options, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);
        ArgumentNullException.ThrowIfNull(options);

        return playerManager.JoinAsync(voiceChannel.GuildID.Value.Value, voiceChannel.ID.Value, options, cancellationToken);
    }

    public static ValueTask<LavalinkPlayer> JoinAsync(this IPlayerManager playerManager, IChannel voiceChannel, Action<LavalinkPlayerOptions>? configure, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);

        return playerManager.JoinAsync(voiceChannel.GuildID.Value.Value, voiceChannel.ID.Value, configure, cancellationToken);
    }

    public static ValueTask<LavalinkPlayer> JoinAsync(this IPlayerManager playerManager, IChannel voiceChannel, LavalinkPlayerOptions options, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);
        ArgumentNullException.ThrowIfNull(options);

        return playerManager.JoinAsync(voiceChannel.GuildID.Value.Value, voiceChannel.ID.Value, options, cancellationToken);
    }

    public static ValueTask<LavalinkPlayer> JoinAsync(this IPlayerManager playerManager, IChannel voiceChannel, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);

        return playerManager.JoinAsync(voiceChannel.GuildID.Value.Value, voiceChannel.ID.Value, cancellationToken);
    }

    private static IOptions<TOptions> CreateOptions<TOptions>(Action<TOptions>? configure)
        where TOptions : LavalinkPlayerOptions, new()
    {
        var options = new TOptions();
        configure?.Invoke(options);
        return Options.Create(options);
    }
}
