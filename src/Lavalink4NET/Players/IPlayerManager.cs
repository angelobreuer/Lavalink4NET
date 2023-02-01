namespace Lavalink4NET.Players;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public interface IPlayerManager
{
    IEnumerable<ILavalinkPlayer> Players { get; }

    ValueTask<ILavalinkPlayer?> GetPlayerAsync(ulong guildId, CancellationToken cancellationToken = default);

    ValueTask<T?> GetPlayerAsync<T>(ulong guildId, CancellationToken cancellationToken = default) where T : class, ILavalinkPlayer;

    IEnumerable<T> GetPlayers<T>() where T : ILavalinkPlayer;

    bool HasPlayer(ulong guildId);

    ValueTask<T> JoinAsync<T>(ulong guildId, ulong voiceChannelId, PlayerFactory<T> playerFactory, PlayerJoinOptions options = default, CancellationToken cancellationToken = default) where T : ILavalinkPlayer;

    ValueTask<ILavalinkPlayer> JoinAsync(ulong guildId, ulong voiceChannelId, PlayerFactory<ILavalinkPlayer> playerFactory, PlayerJoinOptions options = default, CancellationToken cancellationToken = default);

    ValueTask<ILavalinkPlayer> JoinAsync(ulong guildId, ulong voiceChannelId, PlayerJoinOptions options = default, CancellationToken cancellationToken = default);
}