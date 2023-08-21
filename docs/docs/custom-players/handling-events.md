---
sidebar_position: 6
---

# Handling events

Most players allow you to override methods to handle events inside of the player. For example, the `QueuedLavalinkPlayer` class allows you to override the `OnTrackStartedAsync` method to handle the `TrackStarted` event.

```csharp
public sealed class CustomPlayer : QueuedLavalinkPlayer
{
    private readonly ITextChannel _textChannel;

    public CustomPlayer(IPlayerProperties<CustomPlayer, CustomPlayerOptions> properties)
        : base(properties)
    {
        _textChannel = properties.Options.Value.TextChannel;
    }

    protected override async ValueTask OnTrackStartedAsync(LavalinkTrack track, CancellationToken cancellationToken = default)
    {
        await base
            .OnTrackStartedAsync(track, cancellationToken)
            .ConfigureAwait(false);

        // send a message to the text channel
        await _textChannel
            .SendMessageAsync($"Now playing: {track.Title}")
            .ConfigureAwait(false);
    }
}
```

## Player listeners

Lavalink4NET also provides various interfaces you can implement to handle additional events in the player. For example, the `IInactivityPlayerListener` interface allows you to handle events when the player is active, inactive, or tracked. See the [inactivity tracking](/docs/inactivity-tracking/event-handling) guide for more information.
