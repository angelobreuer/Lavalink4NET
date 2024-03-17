namespace Lavalink4NET.Players;

using System;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Clients;

internal interface ILavalinkPlayerHandle : IAsyncDisposable
{
    ILavalinkPlayer? Player { get; }

    ValueTask<ILavalinkPlayer> GetPlayerAsync(CancellationToken cancellationToken = default);

    ValueTask UpdateVoiceServerAsync(VoiceServer voiceServer, CancellationToken cancellationToken = default);

    ValueTask UpdateVoiceStateAsync(VoiceState voiceState, CancellationToken cancellationToken = default);
}