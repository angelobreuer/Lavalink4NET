# Extra filters

The extra filters package provides additional filters for your bot. The plugin currently only adds the `Echo` filter.

## Installation

For using the extra filters, you need to install the [`Lavalink4NET.Integrations.ExtraFilters`](https://www.nuget.org/packages/Lavalink4NET.Integrations.ExtraFilters) package.

:::caution
You need to have the [extra filters plugin](https://github.com/rohank05/lavalink-filter-plugin) installed on your Lavalink server.
:::

## Usage

You can add the `Echo` filter to a player using the player filter map:

```csharp
// Create filter options
var options = new EchoFilterOptions
{
    Delay = 1.0F,
};

// Add echo filter
player.Filters.Echo(options);

// Apply filter
await player.Filters
    .CommitAsync()
    .ConfigureAwait(false);
```
