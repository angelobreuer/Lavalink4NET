---
sidebar_position: 4
---

# Showing player position

To show the current player position, we can use the `CurrentPosition` property. This property returns a `TimeSpan` value that represents the current position of the player.

```csharp
[SlashCommand("position", description: "Shows the track position", runMode: RunMode.Async)]
public async Task Position()
{
    var player = await GetPlayerAsync(connectToVoiceChannel: false).ConfigureAwait(false);

    if (player is null)
    {
        return;
    }

    if (player.CurrentTrack is null)
    {
        await RespondAsync("Nothing playing!").ConfigureAwait(false);
        return;
    }

    await RespondAsync($"Position: {player.Position?.Position} / {player.CurrentTrack.Duration}.").ConfigureAwait(false);
}
```

:::info
The `player.Position` property returns a `TrackPosition` structure which contains a lot of information about the current position. We use the `Position` property of the `TrackPosition` structure to get the current position as a `TimeSpan` value.
:::

![Position discord message](../../static/images/introduction/position.png)

:::note
The `CurrentPosition` property is not updated in real-time. It is updated every 5 seconds by the lavalink server and is interpolated between updates. This means that the position may be slightly off.
:::
