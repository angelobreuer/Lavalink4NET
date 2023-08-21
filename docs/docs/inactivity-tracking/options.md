---
sidebar_position: 4
---

# Options

The inactivity tracking service can be configured during the registration of the service using the `ConfigureInactivityTracking` method. The `ConfigureInactivityTracking` method accepts an `Action<InactivityTrackingOptions>` delegate which allows you to configure the inactivity tracking service.

## `Default Timeout`

The `DefaultTimeout` property allows you to specify the default timeout before the player will be disconnected from the voice channel. The default value is 30 seconds. If the player is active again before the delay is reached, the player will not be disconnected from the voice channel.

```csharp title="Program.cs"
builder.Services.ConfigureInactivityTracking(options =>
{
    options.DefaultTimeout = TimeSpan.FromSeconds(30);
});
```

## `Default Poll Interval`

The `DefaultPollInterval` property allows you to specify the poll interval for polling inactivity trackers which do not specify the polling interval on their own. The default value is 5 seconds. If you lower the interval, the inactivity tracker will poll more often, but will also consume more resources. For huge bot instances, it is recommended to increase the interval. Note that the default trackers are realtime trackers and are not affected by this property.

```csharp title="Program.cs"
builder.Services.ConfigureInactivityTracking(options =>
{
    options.DefaultPollInterval = TimeSpan.FromSeconds(5);
});
```

## `Trackers`

The `Trackers` property allows you to specify the inactivity trackers that will be used to track the activity of a player. The default value are the two inactivity trackers that are provided by the library (see integrated trackers). If you set this property, any trackers that are added to the service provider are ignored.

```csharp title="Program.cs"

```csharp title="Program.cs"
builder.Services.ConfigureInactivityTracking(options =>
{
    options.Trackers = ImmutableArray.Create<IInactivityTracker>(...);
});
```

## `TrackingMode`

The `TrackingMode` property allows you to specify the mode that will be used to determine if the player is active or inactive. The default value is `Any`.

```csharp title="Program.cs"
builder.Services.ConfigureInactivityTracking(options =>
{
    options.TrackingMode = InactivityTrackingMode.Any;
});
```

If you set the mode to `Any`, the player will be considered inactive if ANY of the trackers return `true`. If you set the mode to `All`, the player will be considered inactive if ALL of the trackers return `true`.

## `UseDefaultTrackers`

The `UseDefaultTrackers` property controls whether the default trackers should be used if no trackers are specified. The default value is `true`.

```csharp title="Program.cs"
builder.Services.ConfigureInactivityTracking(options =>
{
    options.UseDefaultTrackers = true;
});
```

## `TimeoutBehavior`

The `TimeoutBehavior` property specifies the behavior to choose the timeout to use when the player is considered inactive. The default value is `Lowest`.

```csharp title="Program.cs"
builder.Services.ConfigureInactivityTracking(options =>
{
    options.TimeoutBehavior = InactivityTimeoutBehavior.Lowest;
});
```

If you set the behavior to `Lowest`, the lowest timeout of all trackers will be used. If you set the behavior to `Highest`, the highest timeout of all trackers will be used. If you set the behavior to `Average`, the average timeout of all trackers will be used.

## `InactivityBehavior`

The `InactivityBehavior` property allows you to specify a behavior for a player that is considered inactive but not timed out. The default value is `None`.

```csharp title="Program.cs"
builder.Services.ConfigureInactivityTracking(options =>
{
    options.InactivityBehavior = InactivityBehavior.None;
});
```

You can set the behavior to `None` to perform no action if the player is being considered inactive. If you set the behavior to `Pause` the player will be paused if the player is being considered inactive. If the player is active again, the player will be resumed.
