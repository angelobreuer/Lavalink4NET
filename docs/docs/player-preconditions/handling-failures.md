# Handling precondition failures

A great feature of player preconditions is that they provide a centralized way to ensure that the player is in a certain state without having to write the same checks over and over again. However, if a precondition fails, the player will not be retrieved and an error will be returned. This error needs to be handled, otherwise the user will not know why the command failed.

## Error handling

In the introduction of player preconditions, we used a central `GetPlayerAsync` method for the command module. We will now look at an example of how to handle precondition failures in this method which is done in the `CreateErrorEmbed` method, which we will have a look at.

```csharp
private static Embed CreateErrorEmbed(PlayerResult<QueuedLavalinkPlayer> result)
{
    var title = result.Status switch
    {
        PlayerRetrieveStatus.UserNotInVoiceChannel => "You must be in a voice channel.",
        PlayerRetrieveStatus.BotNotConnected => "The bot is not connected to any channel.",
        PlayerRetrieveStatus.VoiceChannelMismatch => "You must be in the same voice channel as the bot.",

        PlayerRetrieveStatus.PreconditionFailed when result.Precondition == PlayerPrecondition.Playing => "The player is currently now playing any track.",
        PlayerRetrieveStatus.PreconditionFailed when result.Precondition == PlayerPrecondition.NotPaused => "The player is already paused.",
        PlayerRetrieveStatus.PreconditionFailed when result.Precondition == PlayerPrecondition.Paused => "The player is not paused.",
        PlayerRetrieveStatus.PreconditionFailed when result.Precondition == PlayerPrecondition.QueueEmpty => "The queue is empty.",

        _ => "Unknown error.",
    };

    return new EmbedBuilder().WithTitle(title).Build();
}
```

You can see that we check the `Status` property of the `PlayerResult` to determine the error message. If the status is `PreconditionFailed`, we can check the `Precondition` property to determine which precondition failed. If the status is not `PreconditionFailed`, we can check the `Status` property to determine the error message.
