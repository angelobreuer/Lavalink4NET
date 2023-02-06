namespace Lavalink4NET;

using System;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Events;
using Lavalink4NET.Events.Players;

public partial class AudioService
{
    public event AsyncEventHandler<TrackEndedEventArgs>? TrackEnded;

    public event AsyncEventHandler<TrackExceptionEventArgs>? TrackException;

    public event AsyncEventHandler<TrackStartedEventArgs>? TrackStarted;

    public event AsyncEventHandler<TrackStuckEventArgs>? TrackStuck;

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
}
