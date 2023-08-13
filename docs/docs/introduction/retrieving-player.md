---
sidebar_position: 5
---

# Retrieving the player

Let us take a look at this method to retrieve the player. We call this method from the commands module, whenever we need a player instance. The method will check if a player already exists for the guild and return it. If no player exists, it will create a new player and return it for us.

```csharp title="MyCommands.cs"
private async ValueTask<QueuedLavalinkPlayer?> GetPlayerAsync(bool connectToVoiceChannel = true)
{
    var channelBehavior = connectToVoiceChannel 
        ? PlayerChannelBehavior.Join
        : PlayerChannelBehavior.None;

    var retrieveOptions = new PlayerRetrieveOptions(ChannelBehavior: channelBehavior);

    var result = await _audioService.Players
        .RetrieveAsync(Context, playerFactory: PlayerFactory.Queued, retrieveOptions)
        .ConfigureAwait(false);

    if (!result.IsSuccess)
    {
        var errorMessage = result.Status switch
        {
            PlayerRetrieveStatus.UserNotInVoiceChannel => "You are not connected to a voice channel.",
            PlayerRetrieveStatus.BotNotConnected => "The bot is currently not connected.",
            _ => "Unknown error.",
        };

        await FollowupAsync(errorMessage).ConfigureAwait(false);
        return null;
    }

    return result.Player;
}
```

If you never used Lavalink4NET, this code may look a bit much. But don't worry, we will go through it step by step. In most cases, the code will be the same for your bot. You can copy and paste it into your bot and use it as is.

---

```csharp
var channelBehavior = connectToVoiceChannel 
    ? PlayerChannelBehavior.Join
    : PlayerChannelBehavior.None;

var retrieveOptions = new PlayerRetrieveOptions(ChannelBehavior: channelBehavior);
```

Here we can specify the behavior of the player when the user is not connected to a voice channel. If the `connectToVoiceChannel` is true, we allow the player to join the voice channel. If the `connectToVoiceChannel` is `false`, then the player will not join thevoice channel and the method will return `null` if the user is not connected to a voice channel.

---

```csharp
var result = await _audioService.Players
    .RetrieveAsync(Context, playerFactory: PlayerFactory.Queued, retrieveOptions)
    .ConfigureAwait(false);
```

We use an extension method to create a player. The method will check if a player already exists for the guild and return it. If no player exists, it will create a new player and return it. We specify the player factory to use. In this case, we want to use the `QueuedLavalinkPlayer`.  The `QueuedLavalinkPlayer` is a player that allows you to queue tracks  and play them one after another.

---

```csharp
if (!result.IsSuccess)
{
    var errorMessage = result.Status switch
    {
        // The user is not connected to a voice channel. We can't play music if the user is not
        PlayerRetrieveStatus.UserNotInVoiceChannel => "You are not connected to a voice channel.",

        // The bot is not connected to a voice channel. We can't play music if the bot is not
        PlayerRetrieveStatus.BotNotConnected => "The bot is currently not connected.",

        // Something else happened. Later there are some features which may return additional
        // retrieve states, you can add an error message for these states here.
        // All error messages are managed in this method, so you can easily change them 
        // later and do not have to type them over and over again
        _ => "Unknown error.",
    };

    // Send the error message to the user.
    await FollowupAsync(errorMessage).ConfigureAwait(false);

    // Return null to indicate that no player was retrieved.
    return null;
}
```

Sometimes, something bad happens. In this case, we want to inform the user about the error and what can be done to fix it. Lavalink4NET provides a `PlayerRetrieveResult` which contains a `PlayerRetrieveStatus`. The `PlayerRetrieveStatus` indicates what happened when the player was retrieved. In this case, we check if the player was retrieved successfully. If not, we check the `PlayerRetrieveStatus` and send an error message to the user. We also return `null` to indicate that no player was retrieved.

---

```csharp
return result.Player;
```

If everything went well, we can return the player. The player is now ready to play music.
