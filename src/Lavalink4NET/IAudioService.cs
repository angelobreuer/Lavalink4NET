namespace Lavalink4NET;

using System;
using Lavalink4NET.Events;
using Lavalink4NET.Events.Players;
using Lavalink4NET.Integrations;
using Lavalink4NET.Players;
using Lavalink4NET.Rest;
using Lavalink4NET.Tracks;

/// <summary>
///     The interface for a lavalink audio provider service.
/// </summary>
public interface IAudioService : IDisposable
{
    event AsyncEventHandler<TrackEndedEventArgs>? TrackEnded;

    event AsyncEventHandler<TrackExceptionEventArgs>? TrackException;

    event AsyncEventHandler<TrackStartedEventArgs>? TrackStarted;

    event AsyncEventHandler<TrackStuckEventArgs>? TrackStuck;

    ILavalinkApiClient ApiClient { get; }

    IIntegrationManager Integrations { get; }

    IPlayerManager Players { get; }

    string? SessionId { get; }

    ITrackManager Tracks { get; }
}
