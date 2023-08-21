# Caching

Lavalink4NET supports caching of track queries, track searches and lyrics resolution. Caching improves performance and reduces the amount of requests to the Lavalink server/lyrics API.

Lavalink4NET utilizes the [Microsoft.Extensions.Caching](https://www.nuget.org/packages/Microsoft.Extensions.Caching.Abstractions/) package to provide a caching abstraction. This allows you to use any caching provider you want. You can use the default in-memory cache of `Microsoft.Extensions.Caching` or build your own caching provider.

## Usage

To enable caching, you just need to register the caching service in the dependency injection container. You can do this by calling the `AddMemoryCache` extension method on the `IServiceCollection` instance.

```csharp
builder.Services.AddMemoryCache();
```

:::note
You need to add the `Microsoft.Extensions.Caching.Memory` package to your project to use the in-memory cache.
:::
