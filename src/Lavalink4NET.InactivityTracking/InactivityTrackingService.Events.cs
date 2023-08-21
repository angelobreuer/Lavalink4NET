namespace Lavalink4NET.InactivityTracking;

using Lavalink4NET.Events;
using Lavalink4NET.InactivityTracking.Events;

public partial class InactivityTrackingService
{
    public event AsyncEventHandler<PlayerInactiveEventArgs>? PlayerInactive;

    public event AsyncEventHandler<PlayerActiveEventArgs>? PlayerActive;

    public event AsyncEventHandler<PlayerTrackedEventArgs>? PlayerTracked;

    public event AsyncEventHandler<TrackerActiveEventArgs>? TrackerActive;

    public event AsyncEventHandler<TrackerInactiveEventArgs>? TrackerInactive;

    internal bool HasPlayerInactiveEventHandler => PlayerInactive is not null;

    internal bool HasPlayerActiveEventHandler => PlayerActive is not null;

    internal bool HasPlayerTrackedEventHandler => PlayerTracked is not null;

    internal bool HasTrackerActiveEventHandler => TrackerActive is not null;

    internal bool HasTrackerInactiveEventHandler => TrackerInactive is not null;

    internal ValueTask OnPlayerInactiveAsync(PlayerInactiveEventArgs eventArgs)
        => PlayerInactive.InvokeAsync(this, eventArgs);

    internal ValueTask OnPlayerActiveAsync(PlayerActiveEventArgs eventArgs)
        => PlayerActive.InvokeAsync(this, eventArgs);

    internal ValueTask OnPlayerTrackedAsync(PlayerTrackedEventArgs eventArgs)
        => PlayerTracked.InvokeAsync(this, eventArgs);

    internal ValueTask OnTrackerActiveAsync(TrackerActiveEventArgs eventArgs)
        => TrackerActive.InvokeAsync(this, eventArgs);

    internal ValueTask OnTrackerInactiveAsync(TrackerInactiveEventArgs eventArgs)
        => TrackerInactive.InvokeAsync(this, eventArgs);
}
