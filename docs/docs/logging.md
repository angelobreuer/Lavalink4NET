# Logging

Lavalink4NET uses [Microsoft.Extensions.Logging](https://www.nuget.org/packages/Microsoft.Extensions.Logging) for logging. This means that you can use any logging provider you want. If you already use [Microsoft.Extensions.Logging](https://www.nuget.org/packages/Microsoft.Extensions.Logging) in your project, you don't need to add any additional packages or configuration.

## Usage

To enable logging, you just need to register the logging service in the dependency injection container. You can do this by calling the `AddLogging` extension method on the `IServiceCollection` instance.

```csharp
builder.Services.AddLogging();
```

:::note
You need to add the `Microsoft.Extensions.Logging.Console` package to your project to use the console logger.
:::

If you want to receive all log messages, you can set the log level to `Trace`.

```csharp
builder.Services.AddLogging(x => x.AddConsole().SetMinimumLevel(LogLevel.Trace));
```

## Using other logging providers

Most logging providers provide support for integrating with `Microsoft.Extensions.Logging`. Please refer to the documentation of the logging provider you want to use for more information.

## Example log output

```
dbug: Microsoft.Extensions.Hosting.Internal.Host[1]
      Hosting starting
warn: Lavalink4NET.Rest.LavalinkApiClient[0]
      The default Lavalink password is currently being used. It is highly recommended to change the password immediately to enhance the security of your system.
info: Lavalink4NET.LavalinkNode[7]
      [Lavalink-0HMSSIGJVLE6D] Starting audio service...
dbug: Lavalink4NET.LavalinkNode[9]
      [Lavalink-0HMSSIGJVLE6D] Waiting for client being ready...
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Production
dbug: Microsoft.Extensions.Hosting.Internal.Host[2]
      Hosting started
dbug: Lavalink4NET.LavalinkNode[11]
      [Lavalink-0HMSSIGJVLE6D] Discord client (discord.net) is ready.
info: Lavalink4NET.LavalinkNode[12]
      [Lavalink-0HMSSIGJVLE6D] Audio Service is ready (1501ms).
dbug: Lavalink4NET.Socket.LavalinkSocket[1]
      [Lavalink-0HMSSIGJVLE6E] Attempting to connect to Lavalink node...
```
