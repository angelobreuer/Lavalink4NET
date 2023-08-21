---
sidebar_position: 1
---

# Using player preconditions

For example, if you write a `pause` command for your music bot. Most users want to send a message if the player is already paused. We can use a precondition to ensure that the player is in the correct state before retrieving it, here we can use the `NotPaused` precondition to ensure the player is not paused.

```csharp
[SlashCommand("pause", "...")]
public async Task PauseCommandAsync()
{
    var player = await TryGetPlayerAsync(
        allowConnect: false,
        // Here we pass the NotPaused precondition to ensure the player is not paused.
        preconditions: ImmutableArray.Create(PlayerPrecondition.NotPaused));

    if (player is null)
    {
        return;
    }

    await player
        .PauseAsync()
        .ConfigureAwait(false);

    await RespondAsync(embed: ...);
}
```

If you try to execute this command now, you will probably get an error message indicating that an unknown error occurred. This is because we need to configure error handling, which we will do in the next section.
