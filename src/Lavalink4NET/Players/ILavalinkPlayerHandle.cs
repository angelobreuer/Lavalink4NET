namespace Lavalink4NET.Players;

using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Clients;

internal interface ILavalinkPlayerHandle
{
    ILavalinkPlayer? Player { get; }

    ValueTask AssociateAsync(string sessionId, CancellationToken cancellationToken = default);

    ValueTask<ILavalinkPlayer> GetPlayerAsync(CancellationToken cancellationToken = default);

    ValueTask UpdateVoiceServerAsync(VoiceServer voiceServer, CancellationToken cancellationToken = default);

    ValueTask UpdateVoiceServerAsync(VoiceState voiceState, CancellationToken cancellationToken = default);
}