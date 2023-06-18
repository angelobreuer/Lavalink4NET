namespace Lavalink4NET.Players;

using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Clients;

internal interface ILavalinkPlayerHandle
{
    ILavalinkPlayer? Player { get; }

    ValueTask<ILavalinkPlayer> GetPlayerAsync(CancellationToken cancellationToken = default);

    ValueTask UpdateVoiceServerAsync(VoiceServer voiceServer, CancellationToken cancellationToken = default);

    ValueTask UpdateVoiceStateAsync(VoiceState voiceState, CancellationToken cancellationToken = default);
}