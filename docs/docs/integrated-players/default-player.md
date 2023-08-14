---
sidebar_position: 2
---

# Default player

The default player is the `LavalinkPlayer` class which implements the `ILavalinkPlayer` interface. The player implements the most basic features of a player, such as playing tracks, pausing, resuming, stopping, and seeking.

However, the player does not implement more advanced features such as queueing tracks, repeating tracks, track voting, or shuffling tracks. These features are implemented in the `QueuedLavalinkPlayer` or `VoteLavalinkPlayer` class.

If you do not need any of these advanced features, you can use the default player. This can be useful for bots which do not need player management features, for example when playing a radio stream in a channel.

The player implements the following important members:

```csharp
float Volume { get; }

ValueTask PauseAsync(CancellationToken cancellationToken = default);

ValueTask PlayAsync(TrackReference trackReference, TrackPlayProperties properties = default, CancellationToken cancellationToken = default);

ValueTask PlayAsync(LavalinkTrack track, TrackPlayProperties properties = default, CancellationToken cancellationToken = default);

ValueTask PlayAsync(string identifier, TrackPlayProperties properties = default, CancellationToken cancellationToken = default);

ValueTask ResumeAsync(CancellationToken cancellationToken = default);

ValueTask SeekAsync(TimeSpan position, CancellationToken cancellationToken = default);

ValueTask SeekAsync(TimeSpan position, SeekOrigin seekOrigin, CancellationToken cancellationToken = default);

ValueTask StopAsync(bool disconnect = false, CancellationToken cancellationToken = default);
```
