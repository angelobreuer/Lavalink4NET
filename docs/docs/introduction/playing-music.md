---
sidebar_position: 5
---

# Playing music

It's now time to play our first notes using Lavalink4NET. In practice, for most users it is easier to manage connection to voice channels and player retrieval using a single method in their commands module.

## Retrieving the player

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

## Adding music command

Now, you managed to retrieve the player. It is time to play some music. We will create a command that allows the user to play music. The command will take a search query as an argument and search for tracks on YouTube. If tracks are found, the first track will be played.

```csharp title="MyCommands.cs"
[SlashCommand("play", description: "Plays music", runMode: RunMode.Async)]
public async Task Play(string query)
{
    // Defer the response to indicate that the bot is working on the command.
    // Resolving tracks from YouTube may take some time, so we want to let the user
    // know that the bot is working on the command.
    await DeferAsync().ConfigureAwait(false);

    // Retrieve the player using the method we created earlier.
    // We allow to connect to the voice channel if the user is not connected.
    var player = await GetPlayerAsync(connectToVoiceChannel: true).ConfigureAwait(false);

    // If the player is null, something failed. We already sent an error message to the user
    if (player is null)
    {
        return;
    }

    // Load the track from YouTube. This may take some time, so we await the result.
    var track = await _audioService.Tracks
        .LoadTrackAsync(query, TrackSearchMode.YouTube)
        .ConfigureAwait(false);

    // If no track was found, we send an error message to the user.
    if (track is null)
    {
        await FollowupAsync("😖 No results.").ConfigureAwait(false);
        return;
    }

    // Play the track and inform the user about the track that is being played.
    await player.PlayAsync(track).ConfigureAwait(false);
    await FollowupAsync($"🔈 Playing: {track.Uri}").ConfigureAwait(false);
}
```

:::caution
Whenever you use the `GetPlayerAsync` method to retrieve a player and you allow the player to connect to the voice channel, you should specify `runMode: RunMode.Async` on the command. This will allow Lavalink4NET to receive internal events from Discord and update the player accordingly. `RunMode.Async` executes the command asynchronously, so you avoid blocking the bot receiving thread.
:::

:::info
For best practice, you should also defer the response before retrieving the player. This will let the user know that the bot is working on the command. Resolving tracks from YouTube may take some time, so we want to let the user know that the bot is working on the command.
:::

---

Great, we are now able to play music in our bot. If you call the command, the bot will join the voice channel and play the track. If you call the command again, the track will be added to the queue and played after the current track finished.

![Play music](../../static/images/introduction/play-music.png)

---

**What's next?**

We now implemented the most basic feature of a music bot: playing music. But there is more to it. In the next section, we will implement a command that allows the user to skip the current track, pause the player, resume the player, and more.

---

:::tip
If you need an overview of the code used in this chapter, you can find the complete source code for the bot [here](https://github.com/angelobreuer/Lavalink4NET/tree/feature/angelobreuer/lavalink-v4/samples/Lavalink4NET.Discord_NET.ExampleBot).
:::
