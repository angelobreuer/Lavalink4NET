---
sidebar_position: 5
---

# Event handling

Lavalink4NET exposes various player interfaces which can be used to handle events inside of the player. One of such interfaces is the `IInactivityPlayerListener` interface.

```csharp
public sealed class CustomPlayer : QueuedLavalinkPlayer, IInactivityPlayerListener
{
    public CustomPlayer(IPlayerProperties<CustomPlayer, CustomPlayerOptions> properties)
        : base(properties)
    {
    }

    public ValueTask NotifyPlayerActiveAsync(CancellationToken cancellationToken = default)
    {
        // This method is called when the player was previously inactive and is now active again.
        // For example: All users in the voice channel left and now a user joined the voice channel again.
        cancellationToken.ThrowIfCancellationRequested();
        return default; // do nothing
    }

    public ValueTask NotifyPlayerInactiveAsync(CancellationToken cancellationToken = default)
    {
        // This method is called when the player reached the inactivity deadline.
        // For example: All users in the voice channel left and the player was inactive for longer than 30 seconds.
        cancellationToken.ThrowIfCancellationRequested();
        return default; // do nothing
    }

    public ValueTask NotifyPlayerTrackedAsync(CancellationToken cancellationToken = default)
    {
        // This method is called when the player was previously active and is now inactive.
        // For example: A user left the voice channel and now all users left the voice channel.
        cancellationToken.ThrowIfCancellationRequested();
        return default; // do nothing
    }
}
```

You can use the `IInactivityPlayerListener` interface to handle events when the player is active, inactive, or tracked. For example, some bots may want to send a message if the player is inactive for a certain amount of time and will be stopped soon.
