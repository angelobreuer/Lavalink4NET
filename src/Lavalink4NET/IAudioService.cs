namespace Lavalink4NET;

using System;
using Lavalink4NET.Events;
using Lavalink4NET.Events.Players;
using Lavalink4NET.Integrations;
using Lavalink4NET.Players;
using Lavalink4NET.Tracks;

/// <summary>
///     The interface for a lavalink audio provider service.
/// </summary>
public interface IAudioService : IDisposable
{
    event AsyncEventHandler<TrackStartedEventArgs>? TrackStarted;

    event AsyncEventHandler<TrackEndedEventArgs>? TrackEnded;

    event AsyncEventHandler<TrackStuckEventArgs>? TrackStuck;

    event AsyncEventHandler<TrackExceptionEventArgs>? TrackException;

    IIntegrationCollection Integrations { get; }

    IPlayerManager Players { get; }

    ITrackManager Tracks { get; }
}
