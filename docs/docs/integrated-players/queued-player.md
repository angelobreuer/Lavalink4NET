---
sidebar_position: 3
---

# Queued player

The queued player is a player that plays tracks in a queue. It is the most common player type and is used in nearly all music bots. The player implements the `IQueuedLavalinkPlayer` interface.

## Members

The player implements the following important members in addition to the members of the default player:

```csharp
ITrackQueue Queue { get; }

TrackRepeatMode RepeatMode { get; set; }

bool Shuffle { get; set; }

ValueTask<int> PlayAsync(ITrackQueueItem queueItem, bool enqueue = true, TrackPlayProperties properties = default, CancellationToken cancellationToken = default);

ValueTask<int> PlayAsync(LavalinkTrack track, bool enqueue = true, TrackPlayProperties properties = default, CancellationToken cancellationToken = default);

ValueTask<int> PlayAsync(string identifier, bool enqueue = true, TrackPlayProperties properties = default, CancellationToken cancellationToken = default);

ValueTask<int> PlayAsync(TrackReference trackReference, bool enqueue = true, TrackPlayProperties properties = default, CancellationToken cancellationToken = default);

ValueTask SkipAsync(int count = 1, CancellationToken cancellationToken = default);
```

### Queue

You will notice that the player has a `Queue` property. This property returns an `ITrackQueue` instance which represents the queue of the player. The queue is a collection of `ITrackQueueItem` instances. Each queue item represents a track that is queued in the player.

### RepeatMode

The player also has a `RepeatMode` property which represents the repeat mode of the player. The repeat mode can be set to one of the following values:

- `TrackRepeatMode.None`: The player will not repeat any tracks.
- `TrackRepeatMode.Track`: The player will repeat the current track.
- `TrackRepeatMode.Queue`: The player will repeat the entire queue.

### Shuffle

The player also has a `Shuffle` property which represents whether the queue is shuffled or not. If the queue is shuffled, the player will play tracks in a random order.

## Usage

Lavalink4NET provides a player factory for this player which can be used to create the queued player without additional configuration. You can pass the player factory to the `RetrieveAsync` method:

```csharp
var result = await _audioService.Players
    .RetrieveAsync(Context, playerFactory: PlayerFactory.Queued, retrieveOptions)
    .ConfigureAwait(false);
```
