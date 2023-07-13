namespace Lavalink4NET;

using System;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Events;
using Lavalink4NET.Events.Players;
using Lavalink4NET.Integrations;
using Lavalink4NET.Players;
using Lavalink4NET.Rest;
using Lavalink4NET.Tracks;

public abstract class AudioServiceBase : IAudioService, ILavalinkNodeListener
{
    protected AudioServiceBase(ILavalinkApiClientProvider apiClientProvider, IIntegrationManager integrations, IPlayerManager players, ITrackManager tracks)
    {
        ArgumentNullException.ThrowIfNull(apiClientProvider);
        ArgumentNullException.ThrowIfNull(integrations);
        ArgumentNullException.ThrowIfNull(players);
        ArgumentNullException.ThrowIfNull(tracks);

        ApiClientProvider = apiClientProvider;
        Integrations = integrations;
        Players = players;
        Tracks = tracks;
    }

    public ILavalinkApiClientProvider ApiClientProvider { get; }

    public IIntegrationManager Integrations { get; }

    public IPlayerManager Players { get; }

    public ITrackManager Tracks { get; }

    public abstract ValueTask StartAsync(CancellationToken cancellationToken = default);

    public abstract ValueTask StopAsync(CancellationToken cancellationToken = default);

    public abstract ValueTask WaitForReadyAsync(CancellationToken cancellationToken = default);

    public event AsyncEventHandler<TrackEndedEventArgs>? TrackEnded;

    public event AsyncEventHandler<TrackExceptionEventArgs>? TrackException;

    public event AsyncEventHandler<TrackStartedEventArgs>? TrackStarted;

    public event AsyncEventHandler<TrackStuckEventArgs>? TrackStuck;

    public event AsyncEventHandler<StatisticsUpdatedEventArgs>? StatisticsUpdated;

    protected virtual ValueTask OnTrackEndedAsync(TrackEndedEventArgs eventArgs, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(eventArgs);
        return TrackEnded.InvokeAsync(this, eventArgs);
    }

    protected virtual ValueTask OnTrackExceptionAsync(TrackExceptionEventArgs eventArgs, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(eventArgs);
        return TrackException.InvokeAsync(this, eventArgs);
    }

    protected virtual ValueTask OnTrackStartedAsync(TrackStartedEventArgs eventArgs, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(eventArgs);
        return TrackStarted.InvokeAsync(this, eventArgs);
    }

    protected virtual ValueTask OnTrackStuckAsync(TrackStuckEventArgs eventArgs, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(eventArgs);
        return TrackStuck.InvokeAsync(this, eventArgs);
    }

    protected virtual ValueTask OnStatisticsUpdatedAsync(StatisticsUpdatedEventArgs eventArgs, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(eventArgs);
        return StatisticsUpdated.InvokeAsync(this, eventArgs);
    }

    ValueTask ILavalinkNodeListener.OnTrackEndedAsync(TrackEndedEventArgs eventArgs, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return OnTrackEndedAsync(eventArgs, cancellationToken);
    }

    ValueTask ILavalinkNodeListener.OnTrackExceptionAsync(TrackExceptionEventArgs eventArgs, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return OnTrackExceptionAsync(eventArgs, cancellationToken);
    }

    ValueTask ILavalinkNodeListener.OnTrackStartedAsync(TrackStartedEventArgs eventArgs, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return OnTrackStartedAsync(eventArgs, cancellationToken);
    }

    ValueTask ILavalinkNodeListener.OnTrackStuckAsync(TrackStuckEventArgs eventArgs, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return OnTrackStuckAsync(eventArgs, cancellationToken);
    }

    ValueTask ILavalinkNodeListener.OnStatisticsUpdatedAsync(StatisticsUpdatedEventArgs eventArgs, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return OnStatisticsUpdatedAsync(eventArgs, cancellationToken);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);

        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    protected abstract void Dispose(bool disposing);

    protected abstract ValueTask DisposeAsyncCore();
}
