﻿namespace Lavalink4NET.Players;

using System.Threading;
using System.Threading.Tasks;

public interface ILavalinkSessionProvider
{
    ValueTask<LavalinkPlayerSession> GetSessionAsync(ulong guildId, CancellationToken cancellationToken = default);
}
