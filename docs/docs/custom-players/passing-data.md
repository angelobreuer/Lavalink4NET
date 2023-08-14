---
sidebar_position: 4
---

# Passing data

Lavalink4NET provides a way to pass data to the player. This can be useful if you want to store information about the player.

We saw the `IPlayerProperties` interface earlier in the introduction. This interface has a generic type parameter which you can use to specify an options type, like `CustomPlayerOptions`. The options type is used to pass data to the player.

```csharp title="CustomPlayerOptions.cs"
public sealed record class CustomPlayerOptions : QueuedLavalinkPlayerOptions
{
    public ITextChannel TextChannel { get; }
}
```

You can later access this information in the constructor of your custom player using the `IPlayerProperties` instance.

```csharp title="CustomPlayer.cs"
public sealed class CustomPlayer : QueuedLavalinkPlayer
{
    private readonly ITextChannel _textChannel;

    public CustomPlayer(IPlayerProperties<CustomPlayer, CustomPlayerOptions> properties)
        : base(properties)
    {
        _textChannel = properties.Options.Value.TextChannel;
    }
}
```

:::note
While it is also possible to pass data using the constructor in the player factory of the custom player, it is recommended to use the options type instead.
:::
