---
sidebar_position: 2
---

# Integrated trackers

Inactivity trackers are used to determine if a player is active or inactive. Lavalink4NET provides two inactivity trackers out-of-the-box:

## IdleInactivityTracker

The `IdleInactivityTracker` will report the player as inactive if the player is idle, e.g. if the player is paused or if the player is not playing any track.

### Options

The `IdleInactivityTracker` can be configured using dependency injection by using the `IdleInactivityTrackerOptions` class. The following example shows how to configure the `IdleInactivityTracker`.

```csharp
services.Configure<IdleInactivityTrackerOptions>(config =>
{
    config.Timeout = TimeSpan.FromSeconds(10);
});
```

#### `Label`

The `Label` property is used to identify the tracker. If no label is specified, the tracker will use the type name as label.

#### `Timeout`

The `Timeout` property is used to specify the timeout after which the player is reported as inactive. The default value is the value set in the inactivity tracker service options.

#### `IdleStates`

The `IdleStates` property is used to specify the states that are considered as idle. The default values are `PlayerState.Paused` and `PlayerState.NotPlaying`.

## UsersInactivityTracker

The `UsersInactivityTracker` will report the player as inactive if all users left the voice channel. You can specify a threshold indicating how many users must be in the voice channel to report the player as active.

### Options

The `UsersInactivityTracker` can be configured using dependency injection by using the `UsersInactivityTrackerOptions` class. The following example shows how to configure the `UsersInactivityTracker`.

```csharp
services.Configure<UsersInactivityTrackerOptions>(config =>
{
    config.Timeout = TimeSpan.FromSeconds(10);
});
```

#### `Label`

The `Label` property is used to identify the tracker. If no label is specified, the tracker will use the type name as label.

#### `Timeout`

The `Timeout` property is used to specify the timeout after which the player is reported as inactive. The default value is the value set in the inactivity tracker service options.

#### `Threshold`

The `Threshold` property is used to specify the threshold indicating how many users must be in the voice channel to report the player as active. The default value is `1`. Note that a value below `1` will be treat all players as active.

#### `ExcludeBots`

The `ExcludeBots` property is used to specify if bots should be excluded from the user count. The default value is `true`.
