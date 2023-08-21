---
sidebar_position: 1
---

# Introduction

Player preconditions are used to check certain conditions before a player can be retrieved. This can be used to ensure that the user is connected to a voice channel before a player can be retrieved, or that the user is in the same voice channel as the bot, the player is in a certain state, a track is playing, or not, and much more.

## Modify the `GetPlayerAsync` method

The player preconditions can be passed when calling the `RetrieveAsync` extension methods. In the introduction of this documentation, we used a central `GetPlayerAsync` method for the command module. We will now look at an example of a more advanced `GetPlayerAsync` method that uses player preconditions.

```csharp
private async ValueTask<QueuedLavalinkPlayer?> TryGetPlayerAsync(
    bool allowConnect = false,
    bool requireChannel = true,
    ImmutableArray<IPlayerPrecondition> preconditions = default,
    bool isDeferred = false,
    CancellationToken cancellationToken = default)
{
    cancellationToken.ThrowIfCancellationRequested();

    var options = new PlayerRetrieveOptions(
        ChannelBehavior: allowConnect ? PlayerChannelBehavior.Join : PlayerChannelBehavior.None,
        VoiceStateBehavior: requireChannel ? MemberVoiceStateBehavior.RequireSame : MemberVoiceStateBehavior.Ignore,
        Preconditions: preconditions);

    var result = await _audioService.Players
        .RetrieveAsync(Context, playerFactory: PlayerFactory.Queued, options, cancellationToken: cancellationToken)
        .ConfigureAwait(false);

    if (result.IsSuccess)
    {
        return result.Player;
    }

    // See the error handling section for more information
    var errorMessage = CreateErrorEmbed(result);

    if (isDeferred)
    {
        await FollowupAsync(embed: errorMessage).ConfigureAwait(false);
    }
    else
    {
        await RespondAsync(embed: errorMessage).ConfigureAwait(false);
    }

    return null;
}
```

We now allow to pass a list of player preconditions to the method.
