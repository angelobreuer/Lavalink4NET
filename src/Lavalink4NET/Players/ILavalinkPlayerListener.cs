namespace Lavalink4NET.Players;

using System;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Protocol.Payloads.Events;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;

public interface ILavalinkPlayerListener
{
    void NotifyChannelUpdate(ulong voiceChannelId);

    ValueTask NotifyTrackEndedAsync(LavalinkTrack track, TrackEndReason endReason, CancellationToken cancellationToken = default);

    ValueTask NotifyTrackExceptionAsync(LavalinkTrack track, TrackException exception, CancellationToken cancellationToken = default);

    ValueTask NotifyTrackStartedAsync(LavalinkTrack track, CancellationToken cancellationToken = default);

    ValueTask NotifyTrackStuckAsync(LavalinkTrack track, TimeSpan threshold, CancellationToken cancellationToken = default);

    ValueTask NotifyPlayerUpdateAsync(DateTimeOffset timestamp, TimeSpan position, bool connected, TimeSpan? latency, CancellationToken cancellationToken = default);
}
