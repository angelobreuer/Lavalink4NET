---
sidebar_position: 3
---

# Custom tracker

Of course, it is possible to create your own inactivity tracker. This is useful if you want to track the activity of a player based on your own criteria.

## Creating a custom tracker

In this example, we will create a custom tracker that will report the player as inactive if the player is paused.

```csharp title="PausedInactivityTracker.cs"
public sealed class PausedInactivityTracker : IInactivityTracker
{
    public ValueTask<PlayerActivityStatus> CheckAsync(InactivityTrackingContext context, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var status = context.Player.State is PlayerState.Paused
            ? PlayerActivityStatus.Inactive
            : PlayerActivityStatus.Active;

        return new ValueTask<PlayerActivityStatus>(status);
    }
}
```

## Registering the custom tracker

In order to use the custom tracker, you can configure the inactivity tracking service using the dependency injection container.

```csharp title="Program.cs"
builder.Services.ConfigureInactivityTracking(options =>
{
    options.Trackers = ImmutableArray.Create<IInactivityTracker>(
        new UsersInactivityTracker(
            new UsersInactivityTrackerOptions(Threshold: 2)));
});
```

:::info
If you need to access a service from the dependency injection container, you can use the `IServiceProvider` as the first parameter of the lambda, and the configuration options as the second parameter.
:::

## Premium guilds/feature

Some bots offer a premium feature that allows users to stay in the voice channel even if the player is inactive. This is useful if you want to keep the voice connection alive to avoid the delay when the player is active again.

For example, if we take the paused tracker from above, we can add a premium guild feature to the tracker.

```csharp title="PausedInactivityTracker.cs"
public sealed class PausedInactivityTracker : IInactivityTracker
{
    private readonly IPremiumGuildService _premiumGuildService;

    public PausedInactivityTracker(IPremiumGuildService premiumGuildService)
    {
        ArgumentNullException.ThrowIfNull(premiumGuildService);

        _premiumGuildService = premiumGuildService;
    }

    public async ValueTask<PlayerActivityStatus> CheckAsync(InactivityTrackingContext context, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var premiumGuild = await _premiumGuildService
            .IsPremiumAsync(context.Player.VoiceChannel.GuildId)
            .ConfigureAwait(false);

        if (premiumGuild)
        {
            // If the guild is a premium guild, we will always return active.
            return PlayerActivityStatus.Active;
        }

        return context.Player.State is PlayerState.Paused
            ? PlayerActivityStatus.Inactive
            : PlayerActivityStatus.Active;
    }
}
```

The custom tracker uses a `IPremiumGuildService` to check if the guild is a premium guild. If the guild is a premium guild, the tracker will always report the player as active.

To register the tracker, you can use the following code:

```csharp title="Program.cs"
builder.Services.ConfigureInactivityTracking((serviceProvider, options) =>
{
    var premiumGuildService = serviceProvider.GetRequiredService<IPremiumGuildService>();

    options.Trackers = ImmutableArray.Create<IInactivityTracker>(
        new PausedInactivityTracker(premiumGuildService));
});
```
