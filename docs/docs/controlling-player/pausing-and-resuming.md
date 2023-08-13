---
sidebar_position: 5
---

# Pausing and resuming

## Pausing the player

To pause the player, we can use the `PauseAsync` method. This method will pause the player and keep the current position.

```csharp
[SlashCommand("pause", description: "Pauses the player.", runMode: RunMode.Async)]
public async Task PauseAsync()
{
    var player = await GetPlayerAsync(connectToVoiceChannel: false);

    if (player is null)
    {
        return;
    }

    if (player.State is  PlayerState.Paused)
    {
        await RespondAsync("Player is already paused.").ConfigureAwait(false);
        return;
    }

    await player.PauseAsync().ConfigureAwait(false);
    await RespondAsync("Paused.").ConfigureAwait(false);
}
```

![Pause discord message](../../static/images/introduction/pause.png)

---

## Resuming the player

To resume the player, we can use the `ResumeAsync` method. This method will resume the player.

```csharp
[SlashCommand("resume", description: "Resumes the player.", runMode: RunMode.Async)]
public async Task ResumeAsync()
{
    var player = await GetPlayerAsync(connectToVoiceChannel: false);

    if (player is null)
    {
        return;
    }

    if (player.State is not PlayerState.Paused)
    {
        await RespondAsync("Player is not paused.").ConfigureAwait(false);
        return;
    }

    await player.ResumeAsync().ConfigureAwait(false);
    await RespondAsync("Resumed.").ConfigureAwait(false);
}
```

![Resume discord message](../../static/images/introduction/resume.png)
