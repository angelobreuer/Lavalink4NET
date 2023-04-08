namespace Lavalink4NET.Players.Queued;

using System;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Extensions;
using Lavalink4NET.Protocol;
using Lavalink4NET.Protocol.Payloads.Events;
using Lavalink4NET.Tracks;

/// <summary>
///     A lavalink player with a queuing system.
/// </summary>
public class QueuedLavalinkPlayer : LavalinkPlayer
{
    private readonly bool _disconnectOnStop;

    /// <summary>
    ///     Initializes a new instance of the <see cref="QueuedLavalinkPlayer"/> class.
    /// </summary>
    public QueuedLavalinkPlayer(IPlayerProperties<QueuedLavalinkPlayer, QueuedLavalinkPlayerOptions> properties)
        : base(properties)
    {
        ArgumentNullException.ThrowIfNull(properties);

        var options = properties.Options.Value;

        _disconnectOnStop = options.DisconnectOnStop;

        Queue = new TrackQueue(
            initialCapacity: options.InitialCapacity,
            historyCapacity: options.HistoryCapacity);
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

    public async ValueTask<int> PlayAsync(ITrackQueueItem queueItem, bool enqueue = true, TrackPlayProperties properties = default, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(queueItem);
        EnsureNotDestroyed();

        // check if the track should be enqueued (if a track is already playing)
        if (enqueue && (Queue.Count > 0 || State == PlayerState.Playing || State == PlayerState.Paused))
        {
            // add the track to the queue
            Queue.Enqueue(queueItem);

            // return track queue position
            return Queue.Count;
        }

        // play the track immediately
        await base
            .PlayAsync(queueItem.Track, properties, cancellationToken)
            .ConfigureAwait(false);

        // 0 = now playing
        return 0;
    }

    public ValueTask<int> PlayAsync(LavalinkTrack track, bool enqueue = true, TrackPlayProperties properties = default, CancellationToken cancellationToken = default)
    {
        EnsureNotDestroyed();
        cancellationToken.ThrowIfCancellationRequested();

        return PlayAsync(new TrackReference(track), enqueue, properties, cancellationToken);
    }

    public ValueTask<int> PlayAsync(string identifier, bool enqueue = true, TrackPlayProperties properties = default, CancellationToken cancellationToken = default)
    {
        EnsureNotDestroyed();
        cancellationToken.ThrowIfCancellationRequested();

        return PlayAsync(new TrackReference(identifier), enqueue, properties, cancellationToken);
    }

    public ValueTask<int> PlayAsync(TrackReference trackReference, bool enqueue = true, TrackPlayProperties properties = default, CancellationToken cancellationToken = default)
    {
        EnsureNotDestroyed();
        cancellationToken.ThrowIfCancellationRequested();

        return PlayAsync(new TrackQueueItem(trackReference), enqueue, properties, cancellationToken);
    }

    public override async ValueTask PlayAsync(TrackReference trackReference, TrackPlayProperties properties = default, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await PlayAsync(trackReference, enqueue: true, properties, cancellationToken).ConfigureAwait(false);
    }

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

    protected override ValueTask OnTrackEndedAsync(LavalinkTrack track, TrackEndReason endReason, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(track);

        if (endReason.MayStartNext())
        {
            return SkipAsync(count: 1, cancellationToken);
        }

        return ValueTask.CompletedTask;
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

        if (count >= 0)
        {
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
