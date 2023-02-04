namespace Lavalink4NET.Players.Queued;

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Events.Players;
using Lavalink4NET.Protocol;
using Lavalink4NET.Rest;

/// <summary>
///     A lavalink player with a queuing system.
/// </summary>
public class QueuedLavalinkPlayer : LavalinkPlayer
{
    private readonly ILavalinkApiClient _apiClient;
    private readonly bool _disconnectOnStop;

    /// <summary>
    ///     Initializes a new instance of the <see cref="QueuedLavalinkPlayer"/> class.
    /// </summary>
    public QueuedLavalinkPlayer(PlayerProperties properties)
        : base(properties with { DisconnectOnStop = false, })
    {
        ArgumentNullException.ThrowIfNull(properties);

        _apiClient = properties.ApiClient;
        _disconnectOnStop = properties.DisconnectOnStop;

        Queue = new TrackQueue(); // TODO: setting to adjust capacity
    }

    /// <summary>
    ///     Gets the track queue.
    /// </summary>
    public TrackQueue Queue { get; }

    /// <summary>
    ///     Gets or sets the loop mode for this player.
    /// </summary>
    public TrackRepeatMode RepeatMode { get; set; }

    public bool Shuffle { get; set; }
    /// <summary>
    ///     Skips the current track asynchronously.
    /// </summary>
    /// <param name="count">the number of tracks to skip</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    /// <exception cref="InvalidOperationException">thrown if the player is destroyed</exception>
    public virtual ValueTask SkipAsync(int count = 1, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureNotDestroyed();

        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(count),
                count,
                "The count must not be negative.");
        }

        var track = GetNextTrack(count);

        if (!track.IsPresent)
        {
            // Do nothing, stop
            return StopAsync(_disconnectOnStop, cancellationToken);
        }

        return base.PlayAsync(track.Value.Track, properties: default, cancellationToken);
    }

    public override ValueTask StopAsync(bool disconnect = false, CancellationToken cancellationToken = default)
    {
        EnsureNotDestroyed();
        cancellationToken.ThrowIfCancellationRequested();

        Queue.Clear();

        return base.StopAsync(disconnect, cancellationToken);
    }

    /// <summary>
    ///     Asynchronously triggered when a track ends.
    /// </summary>
    /// <param name="eventArgs">the track event arguments</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    protected override ValueTask OnTrackEndAsync(TrackEndEventArgs eventArgs)
    {
        if (eventArgs.MayStartNext)
        {
            return SkipAsync();
        }

        return default;
    }
    private Optional<ITrackQueueItem> GetNextTrack(int count = 1)
    {
        var track = default(Optional<ITrackQueueItem>);

        if (RepeatMode is TrackRepeatMode.Track)
        {
            return CurrentTrack is null
                ? Optional<ITrackQueueItem>.Default
                : new Optional<ITrackQueueItem>(new TrackQueueItem(new TrackReference(CurrentTrack)));
        }

        while (count-- > 1)
        {
            if (!Queue.TryDequeue(out var peekedTrack))
            {
                break;
            }

            if (RepeatMode is TrackRepeatMode.Queue)
            {
                Queue.Enqueue(peekedTrack);
            }
        }

        if (count > 0)
        {
            Debug.Assert(count is 1);

            if (!Queue.TryDequeue(shuffle: Shuffle, out var peekedTrack))
            {
                return Optional<ITrackQueueItem>.Default; // do nothing
            }

            if (RepeatMode is TrackRepeatMode.Queue)
            {
                Queue.Enqueue(peekedTrack);
            }

            track = new Optional<ITrackQueueItem>(peekedTrack);
        }

        return track;
    }
}
