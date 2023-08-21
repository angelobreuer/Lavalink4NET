---
sidebar_position: 3
---

# Creating custom players

Lavalink4NET provides a way to create custom players using player factories. Similar to integrated players, we will create and use a player factory used to create your player.

```csharp
// Create a player factory
static ValueTask<CustomPlayer> CreatePlayerAsync(IPlayerProperties<CustomPlayer, CustomPlayerOptions> properties, CancellationToken cancellationToken = default)
{
    cancellationToken.ThrowIfCancellationRequested();
    ArgumentNullException.ThrowIfNull(properties);

    return ValueTask.FromResult(new CustomPlayer(properties));
}

// Create a player options instance
var options = new CustomPlayerOptions();

// Create the custom player
var result = await _audioService.Players
    .RetrieveAsync<CustomPlayer, CustomPlayerOptions>(Context, CreatePlayerAsync, options, retrieveOptions)
    .ConfigureAwait(false);
```
