---
sidebar_position: 2
---

# Stopping the player

To stop the player, we can use the `StopAsync` method. This method will stop the player and clear the queue.

```csharp
[SlashCommand("stop", description: "Stops the current track", runMode: RunMode.Async)]
public async Task Stop()
{
    // Retrieve the player using the method we created earlier.
    // We do not allow to connect to the voice channel if the user is not connected.
    // It would not make sense to connect the player to the voice channel, only to stop it.
    var player = await GetPlayerAsync(connectToVoiceChannel: false);

    if (player is null)
    {
        // We already sent an error message to the user
        return;
    }

    // Check if the player is playing
    if (player.CurrentTrack is null)
    {
        // If the player is not playing, we send an error message to the user
        await RespondAsync("Nothing playing!").ConfigureAwait(false);
        return;
    }

    // Stop the player and send a message to the user
    await player.StopAsync().ConfigureAwait(false);
    await RespondAsync("Stopped playing.").ConfigureAwait(false);
}
```

![Stop player discord message](../../static/images/introduction/stop-player.png)

:::info
Later when you got to know Lavalink4NET better, you would use player preconditions to determine the player state. For now, we will use the `CurrentTrack` property to check if the player is playing.
:::
