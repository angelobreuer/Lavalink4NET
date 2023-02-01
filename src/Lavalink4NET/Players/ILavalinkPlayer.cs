namespace Lavalink4NET.Players;

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Tracks;

public interface ILavalinkPlayer
{
    LavalinkTrack? CurrentTrack { get; }

    ulong GuildId { get; }

    TimeSpan Position { get; }

    PlayerState State { get; }

    ulong VoiceChannelId { get; }

    float Volume { get; }

    ValueTask PauseAsync(CancellationToken cancellationToken = default);

    ValueTask PlayAsync(TrackReference trackReference, TrackPlayProperties properties = default, CancellationToken cancellationToken = default);

    ValueTask PlayAsync(LavalinkTrack track, TrackPlayProperties properties = default, CancellationToken cancellationToken = default);

    ValueTask PlayAsync(string identifier, TrackPlayProperties properties = default, CancellationToken cancellationToken = default);

    ValueTask RefreshAsync(CancellationToken cancellationToken = default);

    ValueTask ResumeAsync(CancellationToken cancellationToken = default);

    ValueTask SeekAsync(TimeSpan position, CancellationToken cancellationToken = default);

    ValueTask SeekAsync(TimeSpan position, SeekOrigin seekOrigin, CancellationToken cancellationToken = default);

    ValueTask StopAsync(CancellationToken cancellationToken = default);
}
