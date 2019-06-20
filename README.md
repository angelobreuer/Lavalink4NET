<!-- Banner -->
<img src="https://i.imgur.com/e1jv23h.png"/>

<!-- Center badges -->
<p align="center">
	
<!-- CodeFactor.io Badge -->
<a href="https://www.codefactor.io/repository/github/angelobreuer/lavalink4net">
	<img alt="CodeFactor.io" src="https://www.codefactor.io/repository/github/angelobreuer/lavalink4net/badge?style=for-the-badge" />	
</a>

<!-- Travis CI Badge -->
<a href="https://travis-ci.org/angelobreuer/Lavalink4NET">
	<img alt="Travis CI" src="https://img.shields.io/travis/angelobreuer/Lavalink4NET.svg?style=for-the-badge" />	
</a>	

<!-- GitHub issues Badge -->
<a href="https://github.com/angelobreuer/Lavalink4NET/issues">
	<img alt="GitHub issues" src="https://img.shields.io/github/issues/angelobreuer/Lavalink4NET.svg?style=for-the-badge">	
</a>

</p>

<br />
<br />

[Lavalink4NET](https://github.com/angelobreuer/Lavalink4NET) is a [Lavalink](https://github.com/Frederikam/Lavalink) wrapper with node clustering, caching and custom players for .NET with support for [Discord.Net](https://github.com/RogueException/Discord.Net) and [DSharpPlus](https://github.com/DSharpPlus/DSharpPlus/).

### Features
- 🔌 Asynchronous Interface
- ⚖️ Node Clustering / Load Balancing
- ✳️ Extensible
- 🗳️ Queueing / Voting-System
- 🚰 Track Decoding
- 🔄 Auto-Reconnect and Resuming
- 📝 Optional Logging
- ⚡ Optional Request Caching
- ⏱️ Optional Inactivity Tracking
- ➕ Compatible with [DSharpPlus](https://github.com/DSharpPlus/DSharpPlus) and [Discord.Net](https://github.com/discord-net/Discord.Net).

### NuGet
- Download [Lavalink4NET Core ![NuGet - Lavalink4NET Core](https://img.shields.io/nuget/vpre/Lavalink4NET.svg?style=flat-square)](https://www.nuget.org/packages/Lavalink4NET/) 
- Download [Lavalink4NET for Discord.Net ![NuGet - Lavalink4NET Discord.Net](https://img.shields.io/nuget/vpre/Lavalink4NET.Discord.Net.svg?style=flat-square)](https://www.nuget.org/packages/Lavalink4NET.Discord.NET/) 
- Download [Lavalink4NET for DSharpPlus ![NuGet - Lavalink4NET DSharpPlus](https://img.shields.io/nuget/vpre/Lavalink4NET.DSharpPlus.svg?style=flat-square)](https://www.nuget.org/packages/Lavalink4NET.DSharpPlus/)
- Download [Lavalink4NET MemoryCache ![NuGet - Lavalink4NET MemoryCache](https://img.shields.io/nuget/vpre/Lavalink4NET.MemoryCache.svg?style=flat-square)](https://www.nuget.org/packages/Lavalink4NET.MemoryCache/)

### Prerequisites
- One or more Lavalink Nodes
- .NET Core >= 2.0

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

You can lookup these options in the `application.yml` of your Lavalink Node(s).

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

Now you can join a voice channel like the following:

```csharp
// get player
var player = _audioService.GetPlayer<LavalinkPlayer>(guildId) 
    ?? await _audioService.JoinAsync(guildId, voiceChannel);

// resolve a track from youtube
var myTrack = await player.GetTrackAsync("<search query>", SearchMode.YouTube);

// play track
await player.PlayAsync(myTrack);
```

For more documentation, see: [Lavalink4NET Wiki](https://github.com/angelobreuer/Lavalink4NET/wiki)

___

### Dependencies
- [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/) *(for Payload Serialization)*
