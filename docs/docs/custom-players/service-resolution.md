---
sidebar_position: 5
---

# Service resolution

Lavalink4NET is tightly integrated with the provided dependency injection system. This means that you can use dependency injection to resolve services in your custom player implementation.

```csharp title="CustomPlayer.cs"
public sealed class CustomPlayer : QueuedLavalinkPlayer
{
    private readonly IVolumeService _volumeService;

    public CustomPlayer(IPlayerProperties<CustomPlayer, CustomPlayerOptions> properties)
        : base(properties)
    {
        _volumeService = properties.ServiceProvider!.GetRequiredService<IVolumeService>();
    }
}
```

:::note
If you need lifetime management, you need to use the `IServiceProvider` instance to create a new scope and resolve the service from the scope.
:::
