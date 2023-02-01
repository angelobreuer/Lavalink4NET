namespace Lavalink4NET.DiscordNet;

using System;
using System.Threading;
using System.Threading.Tasks;
using global::Discord;
using Lavalink4NET.Players;

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

    public static ValueTask<T> JoinAsync<T>(this IPlayerManager playerManager, IVoiceChannel voiceChannel, PlayerFactory<T> playerFactory, PlayerJoinOptions options = default, CancellationToken cancellationToken = default) where T : ILavalinkPlayer
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);

        return playerManager.JoinAsync<T>(voiceChannel.GuildId, voiceChannel.Id, playerFactory, options, cancellationToken);
    }

    public static ValueTask<ILavalinkPlayer> JoinAsync(this IPlayerManager playerManager, IVoiceChannel voiceChannel, PlayerFactory<ILavalinkPlayer> playerFactory, PlayerJoinOptions options = default, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);

        return playerManager.JoinAsync(voiceChannel.GuildId, voiceChannel.Id, playerFactory, options, cancellationToken);
    }

    public static ValueTask<ILavalinkPlayer> JoinAsync(this IPlayerManager playerManager, IVoiceChannel voiceChannel, PlayerJoinOptions options = default, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(playerManager);
        ArgumentNullException.ThrowIfNull(voiceChannel);

        return playerManager.JoinAsync(voiceChannel.GuildId, voiceChannel.Id, options, cancellationToken);
    }
}
