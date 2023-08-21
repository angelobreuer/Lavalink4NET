---
sidebar_position: 3
---

# Controlling volume

To control the volume of the player, we can use the `SetVolumeAsync` method. This method takes a volume value between 0 and 1000. The default volume is 100.

```csharp
[SlashCommand("volume", description: "Sets the player volume (0 - 1000%)", runMode: RunMode.Async)]
public async Task Volume(int volume = 100)
{
    if (volume is > 1000 or < 0)
    {
        await RespondAsync("Volume out of range: 0% - 1000%!").ConfigureAwait(false);
        return;
    }

    var player = await GetPlayerAsync(connectToVoiceChannel: false).ConfigureAwait(false);

    if (player is null)
    {
        return;
    }

    await player.SetVolumeAsync(volume / 100f).ConfigureAwait(false);
    await RespondAsync($"Volume updated: {volume}%").ConfigureAwait(false);
}
```

![Volume discord message](../../static/images/introduction/volume.png)

:::info
The `SetVolumeAsync` method takes a float value between 0 and 1. We divide the volume by 100 to convert it to a float value between 0 and 10. Most users prefer to use a volume value from 0 to 100, so we convert it to a percentage value.
:::

:::note
Note that values above 100% may cause distortion.
:::
