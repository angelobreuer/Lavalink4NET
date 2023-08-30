# Custom trackers

Lavalink4NET allows you to write custom trackers to write fully customizable code to track your player's activity. In general, we can differentiate between two types of trackers: **Polling** and **Realtime**.

The revamped tracking system handles player activity updates in realtime with exact timings. If you write a polling tracker, your tracker will be called in regular intervals to update the inactivity tracking state for players. If you write a realtime tracker, you will need to manage the state yourself and update the state whenever the player's activity changes.

## How to implement

The rough overview of both tracker types is shown below. Normally, inactivity trackers are resolved using the dependency injection container. This means that you can inject any dependencies you need into your tracker. If you want to implement a custom tracker you have to implement the `IInactivityTracker` interface. The interface needs you to provide a `InactivityTrackerOptions` property and a `RunAsync` method. The `InactivityTrackerOptions` property is used to configure the tracker. The `RunAsync` method is called whenever the tracker should update the inactivity tracking state for players. The `RunAsync` method is called in regular intervals for polling trackers and and only once for realtime trackers.

```csharp
public sealed class MyCustomTracker : IInactivityTracker
{
    public InactivityTrackerOptions Options => [...];

    public async ValueTask RunAsync(IInactivityTrackerContext trackerContext, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(trackerContext);

        // ...
    }
}
```

## Reporting activity or inactivity

An inactivity tracker can report a player as active or inactive. To do so, you will need to create a commit scope for the report. You can update the activity state of multiple players in that scope. If you dispose the scope, the changes will be committed.

```csharp
using var scope = trackerContext.CreateScope();

// Mark a player as inactive
scope.MarkInactive(123UL);

// Mark a player as active
scope.MarkActive(123UL);
```

:::note
A tracker context scope (`InactivityTrackerScope`) should be disposed as soon as possible as they are short-lived objects. If you don't dispose the scope, the changes will not be committed. There can not be multiple active scopes at the same time.
:::

## Polling trackers

In order to implement a polling tracker, you will need to specify the type of the player in the `InactivityTrackerOptions` property:

```csharp
public InactivityTrackerOptions Options => InactivityTrackerOptions.Polling();
```

You can also specify the poll interval in the `InactivityTrackerOptions` property. If you don't specify the poll interval, the default value specified in the inactivity tracking options will be used. If you want to assign a label to the tracker to improve debugging, you can do so by specifying the label in the `InactivityTrackerOptions` property for that tracker.

Polling trackers will be called in regular intervals to update the inactivity tracking state for players. If you want to poll all players, you can resolve all players by injecting the `IPlayerManager` into your tracker.

### Example Polling Tracker

We will now demonstrate how to implement a polling tracker. In this example, we will implement a tracker that will mark a player as inactive if all users in the voice channel are deafened.

```csharp
public sealed class MyCustomTracker : IInactivityTracker
{
    private readonly DiscordSocketClient _discordClient;
    private readonly IPlayerManager _playerManager;

    public MyCustomTracker(DiscordSocketClient discordClient, IPlayerManager playerManager)
    {
        ArgumentNullException.ThrowIfNull(discordClient);
        ArgumentNullException.ThrowIfNull(playerManager);

        _discordClient = discordClient;
        _playerManager = playerManager;
    }

    public InactivityTrackerOptions Options => InactivityTrackerOptions.Polling();

    public ValueTask RunAsync(IInactivityTrackerContext trackerContext, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(trackerContext);

        static bool HasListener(IVoiceChannel voiceChannel) => ((SocketVoiceChannel)voiceChannel).ConnectedUsers
            .OfType<SocketGuildUser>()
            .Any(user => user is { IsBot: false, IsDeafened: false, IsSelfDeafened: false, });

        using var scope = trackerContext.CreateScope();

        foreach (var player in _playerManager.Players)
        {
            var channel = _discordClient.GetGuild(player.GuildId)?.GetVoiceChannel(player.VoiceChannelId);

            if (channel is not null && HasListener(channel))
            {
                scope.MarkActive(player.GuildId);
            }
            else
            {
                scope.MarkInactive(player.GuildId);
            }
        }

        return ValueTask.CompletedTask;
    }
}
```

This tracker checks if there is at least one user in the voice channel that is not deafened. If there is at least one user that is not deafened, the player will be marked as active. If there is no user that is not deafened, the player will be marked as inactive.

## Realtime trackers

Realtime trackers are a bit more complex than polling trackers. In order to implement a realtime tracker, you will need to specify the type of the player in the `InactivityTrackerOptions` property:

```csharp
public InactivityTrackerOptions Options => InactivityTrackerOptions.Realtime();
```

If you want to assign a label to the tracker to improve debugging, you can do so by specifying the label in the `InactivityTrackerOptions` property for that tracker.

Realtime trackers will be called once to update the inactivity tracking state for players. You need to manually manage the state of the players and update the state whenever the player's activity changes.

### Example Realtime Tracker

We will now implement the above polling tracker as a realtime tracker.

```csharp
public sealed class MyCustomTracker : IInactivityTracker
{
    private readonly DiscordSocketClient _discordClient;
    private readonly IPlayerManager _playerManager;

    public MyCustomTracker(DiscordSocketClient discordClient, IPlayerManager playerManager)
    {
        ArgumentNullException.ThrowIfNull(discordClient);
        ArgumentNullException.ThrowIfNull(playerManager);

        _discordClient = discordClient;
        _playerManager = playerManager;
    }

    public InactivityTrackerOptions Options => InactivityTrackerOptions.Realtime();

    public async ValueTask RunAsync(IInactivityTrackerContext trackerContext, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(trackerContext);

        static bool HasListener(IVoiceChannel voiceChannel) => ((SocketVoiceChannel)voiceChannel).ConnectedUsers
            .OfType<SocketGuildUser>()
            .Any(user => user is { IsBot: false, IsDeafened: false, IsSelfDeafened: false, });

        Task OnUserVoiceStateUpdated(SocketUser user, SocketVoiceState before, SocketVoiceState after)
        {
            if (user is not SocketGuildUser guildUser)
            {
                // User is not in a guild
                return Task.CompletedTask;
            }

            if (!_playerManager.TryGetPlayer(guildUser.Guild.Id, out var player))
            {
                // The player is not connected to the guild
                return Task.CompletedTask;
            }

            if (before.VoiceChannel.Id != player.VoiceChannelId && after.VoiceChannel.Id != player.VoiceChannelId)
            {
                // User is not in the player's voice channel
                return Task.CompletedTask;
            }

            using var scope = trackerContext.CreateScope();
            var voiceChannel = guildUser.Guild.GetVoiceChannel(player.VoiceChannelId);

            if (HasListener(voiceChannel))
            {
                scope.MarkActive(player.GuildId);
            }
            else
            {
                scope.MarkInactive(player.GuildId);
            }

            return Task.CompletedTask;
        }

        _discordClient.UserVoiceStateUpdated += OnUserVoiceStateUpdated;

        try
        {
            var taskCompletionSource = new TaskCompletionSource<object?>(
                creationOptions: TaskCreationOptions.RunContinuationsAsynchronously);

            using var cancellationTokenRegistration = cancellationToken.Register(
                state: taskCompletionSource,
                callback: taskCompletionSource.SetResult);

            await taskCompletionSource.Task.ConfigureAwait(false);
        }
        finally
        {
            _discordClient.UserVoiceStateUpdated -= OnUserVoiceStateUpdated;
        }
    }
}
```

This tracker will subscribe to the `UserVoiceStateUpdated` event of the Discord client. Whenever the event is raised, the tracker will check if the user is in the player's voice channel. If the user is in the player's voice channel, the tracker will check if there is at least one user in the voice channel that is not deafened. If there is at least one user that is not deafened, the player will be marked as active. If there is no user that is not deafened, the player will be marked as inactive.

You see, realtime trackers are a bit more complex than polling trackers. However, they are also more powerful and allow you to implement more complex logic. Polling trackers are easier to implement but they also consume more resources as they are called in regular intervals.
