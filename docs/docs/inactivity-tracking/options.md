---
sidebar_position: 4
---

# Options

The inactivity tracking service can be configured during the registration of the service using the `ConfigureInactivityTracking` method. The `ConfigureInactivityTracking` method accepts an `Action<InactivityTrackingOptions>` delegate which allows you to configure the inactivity tracking service.

## `Disconnect Delay`

The `DisconnectDelay` property allows you to specify the delay before the player will be disconnected from the voice channel. The default value is 30 seconds. If the player is active again before the delay is reached, the player will not be disconnected from the voice channel.

```csharp title="Program.cs"
builder.Services.ConfigureInactivityTracking(options =>
{
    options.DisconnectDelay = TimeSpan.FromSeconds(30);
});
```

## `Poll Interval`

The `PollInterval` property allows you to specify the interval between each poll. The default value is 5 seconds. If you lower the interval, the inactivity tracking service will poll more often, but will also consume more resources. For huge bot instances, it is recommended to increase the interval.

```csharp title="Program.cs"
builder.Services.ConfigureInactivityTracking(options =>
{
    options.PollInterval = TimeSpan.FromSeconds(5);
});
```

## `Trackers`

The `Trackers` property allows you to specify the inactivity trackers that will be used to track the activity of a player. The default value are the two inactivity trackers that are provided by the library (see integrated trackers).

```csharp title="Program.cs"

```csharp title="Program.cs"
builder.Services.ConfigureInactivityTracking(options =>
{
    options.Trackers = ImmutableArray.Create<IInactivityTracker>(...);
});
```

## `Mode`

The `Mode` property allows you to specify the mode that will be used to determine if the player is active or inactive. The default value is `Any`.

```csharp title="Program.cs"
builder.Services.ConfigureInactivityTracking(options =>
{
    options.Mode = InactivityTrackingMode.Any;
});
```

If you set the mode to `Any`, the player will be considered inactive if ANY of the trackers return `true`. If you set the mode to `All`, the player will be considered inactive if ALL of the trackers return `true`.
