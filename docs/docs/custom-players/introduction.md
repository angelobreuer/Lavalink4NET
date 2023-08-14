---
sidebar_position: 1
---

# Introduction

Some bots may want to use a custom player implementation. Custom players can be used to handle specific needs of your bot.

A custom player generally inherits from one of the provided player implementations. For example, if you want to use a queue, you can inherit from the `QueuedLavalinkPlayer` class.

```csharp title="CustomPlayer.cs"
public sealed class CustomPlayer : QueuedLavalinkPlayer
{
    public CustomPlayer(IPlayerProperties<CustomPlayer, CustomPlayerOptions> properties)
        : base(properties)
    {
    }
}
```

```csharp title="CustomPlayerOptions.cs"
public sealed record class CustomPlayerOptions : QueuedLavalinkPlayerOptions
{
}
```

The above example shows a custom player which inherits from the `QueuedLavalinkPlayer` class. The `QueuedLavalinkPlayer` class is a player implementation which uses a queue to play tracks. You can also inherit from the `LavalinkPlayer` class if you don't want to use a queue.

The constructor of the custom player must call the base constructor with the provided `IPlayerProperties` instance. The `IPlayerProperties` instance contains all required properties to initialize the player.
