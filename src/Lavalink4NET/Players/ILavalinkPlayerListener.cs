namespace Lavalink4NET.Players;

using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Clients;
using Lavalink4NET.Protocol.Payloads.Events;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;

public interface ILavalinkPlayerListener
{
    ValueTask NotifyVoiceStateUpdatedAsync(VoiceState voiceState, CancellationToken cancellationToken = default);

    ValueTask NotifyVoiceServerUpdatedAsync(VoiceServer voiceServer, CancellationToken cancellationToken = default);

    ValueTask NotifyChannelUpdateAsync(ulong? voiceChannelId, CancellationToken cancellationToken = default);

    ValueTask NotifyTrackEndedAsync(LavalinkTrack track, TrackEndReason endReason, CancellationToken cancellationToken = default);

    ValueTask NotifyTrackExceptionAsync(LavalinkTrack track, TrackException exception, CancellationToken cancellationToken = default);

    ValueTask NotifyTrackStartedAsync(LavalinkTrack track, CancellationToken cancellationToken = default);

    ValueTask NotifyTrackStuckAsync(LavalinkTrack track, TimeSpan threshold, CancellationToken cancellationToken = default);

    ValueTask NotifyPlayerUpdateAsync(DateTimeOffset timestamp, TimeSpan position, bool connected, TimeSpan? latency, CancellationToken cancellationToken = default);

    ValueTask NotifyWebSocketClosedAsync(WebSocketCloseStatus closeStatus, string reason, bool byRemote = false, CancellationToken cancellationToken = default);
}
