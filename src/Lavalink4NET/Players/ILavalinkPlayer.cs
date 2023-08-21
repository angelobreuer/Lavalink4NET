namespace Lavalink4NET.Players;

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Clients;
using Lavalink4NET.Rest;
using Lavalink4NET.Tracks;

public interface ILavalinkPlayer : IAsyncDisposable
{
    IPlayerFilters Filters { get; }

    string Label { get; }

    ILavalinkApiClient ApiClient { get; }

    IDiscordClientWrapper DiscordClient { get; }

    LavalinkTrack? CurrentTrack { get; }

    ulong GuildId { get; }

    TrackPosition? Position { get; }

    string SessionId { get; }

    PlayerState State { get; }

    ulong VoiceChannelId { get; }

    PlayerConnectionState ConnectionState { get; }

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

    ValueTask DisconnectAsync(CancellationToken cancellationToken = default);
}
