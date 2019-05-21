## Lavalink4NET


Lavalink4NET is a wrapper for [Lavalink](https://github.com/Frederikam/Lavalink). 
With support for [DSharpPlus](https://github.com/DSharpPlus/DSharpPlus) and [Discord.Net](https://github.com/discord-net/Discord.Net).

![Lavalink4NET Icon](https://imgur.com/DbTYXxY.png)

### Features
- Asynchronous Interface
- Node Clustering / Load Balancing
- Extensible
- Queueing / Voting-System
- Track Decoding
- Optional Logging
- Compatible with [DSharpPlus](https://github.com/DSharpPlus/DSharpPlus) and [Discord.Net](https://github.com/discord-net/Discord.Net).

### Prerequisites
- One or more Lavalink Nodes
- .NET Core >= 2.2

### Getting Started

You can use Lavalink4NET in 3 different modes: Cluster, Node and Rest.
- **Cluster** is useful for combining a bunch of nodes to one to load balance.
- **Node** is useful when you have a small discord bot with one Lavalink Node.
- **Rest** is useful when you only want to resolve tracks.
___

Here is an example, how you can create the AudioService for a single node:
```csharp
var audioService = new LavalinkNode(new LavalinkNodeOptions
{
	RestUri = "http://localhost:8080/",
	WebSocketUri = "ws://localhost:8080/",
	Password = "youshallnotpass"
}, new DiscordClientWrapper(client));
```

These options can be looked up in the `application.yml` of your Lavalink Node.

You need to initialize the service before using it:
```csharp
client.Ready += () => audioService.InitializeAsync();
```
___
It is recommended to use Lavalink4NET in combination with DependencyInjection:
```csharp
services.AddSingleton<IAudioService, LavalinkNode>();
services.AddSingleton<IDiscordClientWrapper, DiscordClientWrapper>();
services.AddSingleton(new LavalinkNodeOptions {[...]});
```

### Dependencies
- [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/) *(for Payload Serialization)*
- [Microsoft.Extensions.Logging.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.Logging.Abstractions/) *(for Logging)*
