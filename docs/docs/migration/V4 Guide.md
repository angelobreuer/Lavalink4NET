# V4 Migration Guide

## Introduction

This guide is intended to help you migrate your existing codebase from V3 to V4. It is not intended to be a comprehensive guide to all the new features in V4, but rather a guide to help you get your existing codebase working with V4.

Lavalink V4 has changed a lot of internal infrastructure, and as such, some things have changed a bit. Lavalink4NET v4 has been designed to be as backwards compatible as possible, but there are some breaking changes that you will need to be aware of.

First, we will start discussing the changes to Lavalink itself, and then we will discuss the changes to Lavalink4NET.

## Lavalink Changes

Lavalink prior to V4 used a single WebSocket connection to communicate with your bot instance. This connection was used for all events, and all commands. In Lavalink v4, this has changed to use a RESTful API for commands, and a WebSocket for events. Lavalink v4 also features an entirely new plugin system which also changes how plugins are used. In v4 beta 1, lavalink switched to a new lavaplayer fork which is now maintained by the lavalink community.

## Lavalink4NET Changes

Since the internal structure of Lavalink has changed, Lavalink4NET has also changed. We tried to keep the changes as minimal as possible, but there are some breaking changes that you will need to be aware of. We will go over these changes in detail below.

### Framework support

Lavalink4NET v3 supported .NET Standard 2.0 and upwards. Lavalink4NET v4 increases the minimum supported version to .NET 6.0. If you have the choice, try to use .NET 7.0 or higher to take advantage of the latest features.

### Dependency injection

Lavalink4NET no longer supports usage as an instantiated class. Instead, it is now a set of services that can be registered with your dependency injection container. This means that you will need to change your code to use dependency injection. If you are not familiar with dependency injection, you can read more about it [here](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-6.0).

A lot boilerplate has been removed that was previously required in v3, and the library is now much easier to use.

### Introducing player properties

Player properties provide a new approach to managing players. Players handle the state of a session in a Discord guild, and properties serve as the initial information required to instantiate the player state. The new properties feature facilitates easier passing of player options for users and accessing dependencies, such as those from the service container, enabling better integration with dependency injection. Player properties also reduce the impact of breaking changes when adding new service dependencies.

### Lavalink track independence

The LavalinkTrack type has undergone a complete overhaul to become stateless and implement new methods. This redesign simplifies the management of Lavalink tracks. Additionally, it is now possible to play a track without resolving it, reducing the roundtrip required to connect a player and start playing. Furthermore, track parsing and encoding are now closely integrated with the actual track type.

### Cache overhaul

Previously, Lavalink4NET employed its own caching system with an abstraction layer. The new approach facilitates easier integration of caching by utilizing the Microsoft.Extensions caching interface.

### Lifetime and cycle management overhaul

In the past, many users encountered issues with the correct order of initialization, requiring them to call InitializeAsync after the Discord client connected. With the new changes, Lavalink4NET manages its own lifecycle and provides plug'n'play support for dependency injection, minimizing the need for additional configuration.

### Inactivity tracking overhaul

The inactivity tracking service has been completely overhauled and is now packaged separately. This update allows for more precise control of inactivity tracking. The logic for starting and stopping the inactivity tracking service has been improved to run as a hosted service, seamlessly working with dependency injection.

### Lyrics service overhaul

The lyrics service has undergone a complete overhaul and is now packaged separately. It can now be used with dependency injection.

### Route Planner API implementation

Lavalink4NET now implements the route planner API used for marking/unmarking IP addresses in the IP range.

### Queue overhaul

Lavalink4NET aims to make playback as seamless as easy. The new queue implementation provides an easier accessible interface with more modularity. For example, it is now possible, to use a remote server to store queued tracks persistently.

### Player preconditions

When retrieving a player, I often see users writing a lot of boilerplate to ensure the player is in the correct state to continue. For example, when running a pause command, users want to ensure, that the player is not paused already, or that the player is playing a track. In order to do that player preconditions help with checking for the player state.

### Remora.Discord

Lavalink v4 adds support for the [Remora](https://github.com/Remora/Remora.Discord) discord client.

### Lavasrc Integration

Lavalink4NET adds support primitives for using the Lavasrc plugin using Lavalink4NET.

### Lavasearch Integration

Lavalin4NET adds support primitives for using the Lavasearch plugin using Lavalink4NET.

## How to migrate

### Installing Lavalink4NET v4

Lavalink4NET v4 is currently in beta. You can install the beta version by enabling pre-release packages in your package manager. You need to install the package made for your discord client. If you are using Discord.NET, you need to install the `Lavalink4NET.Discord.NET` package. If you are using DSharpPlus, you need to install the `Lavalink4NET.DSharpPlus` package. If you are using Remora, you need to install the `Lavalink4NET.Remora.Discord` package. The inactivity tracking service is now packaged separately. You can install it by installing the `Lavalink4NET.InactivityTracking` package. The lyrics service is now also packaged separately. You can install it by installing the `Lavalink4NET.Lyrics` package.

### Dependency injection

Lavalink4NET v4 is now used as a service which can be registered with your dependency injection container. This means that you will need to change your code to use dependency injection.

Previously, you would create the ServiceCollection and register the Lavalink4NET service like this:

```csharp
var serviceProvider = new ServiceCollection()
	.AddSingleton<IAudioService, LavalinkNode>()	
	.AddSingleton<IDiscordClientWrapper, DiscordClientWrapper>();
	.AddSingleton(new LavalinkNodeOptions {[...]})
	[...]
	.BuildServiceProvider();
	
var audioService = serviceProvider.GetRequiredService<IAudioService>();
```

Now, in v4, you only need to call `.AddLavalink()` on your service collection, and then you can retrieve the service from the container like this:

```csharp
var serviceProvider = new ServiceCollection()
    .AddLavalink()
    .BuildServiceProvider();
```

There is no need anymore to initialize the audio service, since it is now done automatically by the library if you use the host builder. If you are not using the host builder, you are required to call `StartAsync()` on the audio service. However, it is not needed to wait for the discord client to be ready anymore.

### Joining a voice channel

In v3, normally users used a GetPlayer/JoinAsync combination to join a voice channel. This is no longer needed in v4. Instead, you can now use the `RetrieveAsync` method to retrieve a player. This method will automatically join the voice channel if needed. If you want to join a voice channel, you can use the `JoinAsync` method.

```csharp
private async ValueTask<VoteLavalinkPlayer?> GetPlayerAsync(bool connectToVoiceChannel = true)
{
    var retrieveOptions = new PlayerRetrieveOptions(
        ChannelBehavior: connectToVoiceChannel ? PlayerChannelBehavior.Join : PlayerChannelBehavior.None);

    var result = await _audioService.Players
        .RetrieveAsync(Context, playerFactory: PlayerFactory.Vote, retrieveOptions)
        .ConfigureAwait(false);

    if (!result.IsSuccess)
    {
        var errorMessage = result.Status switch
        {
            PlayerRetrieveStatus.UserNotInVoiceChannel => "You are not connected to a voice channel.",
            PlayerRetrieveStatus.BotNotConnected => "The bot is currently not connected.",
            _ => "Unknown error.",
        };

        await FollowupAsync(errorMessage).ConfigureAwait(false);
        return null;
    }

    return result.Player;
}
```

This method will create a player if it does not exist. The `RetrieveAsync` method also has an overload that allows you to specify a factory method to create the player. This is useful if you want to use a custom player type. You can also pass in a `PlayerRetrieveOptions` object to specify how the player should be retrieved. For example, you can specify if the player should be created if it does not exist, or if the bot should join the voice channel if it is not already connected.

### Caching

Lavalink4NET v4 no longer uses its own caching system. Instead, it uses the Microsoft.Extensions.Caching library. This means that you will need to change your code to use the new caching system. The new caching system is much more flexible and allows you to use any caching provider that implements the `IMemoryCache` interface. You can read more about the new caching system [here](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-6.0).

In v3, caching was done like this:

```csharp
services.AddSingleton<ILavalinkCache, LavalinkCache>();
```

In v4, you can use the `AddMemoryCache` extension method provided by Microsoft.Extensions.Caching.Memory to add the caching service to your service collection. Lavalink4NET will automatically use the caching service if it is registered in the service collection.

### Logging

Lavalink4NET v4 no longer uses its own logging system. Instead, it uses the Microsoft.Extensions.Logging library. This means that you will need to change your code to use the new logging system. You can read more about the new logging system [here](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-6.0).

In v3, logging was done like this:

```csharp
services.AddMicrosoftExtensionsLavalinkLogging();
```

In v4, you can use the `AddLogging` extension method provided by Microsoft.Extensions.Logging to add the logging service to your service collection. Lavalink4NET will automatically use the logging service if it is registered in the service collection.

Please remove any legacy `Lavalink4NET.Logging.*` packages from your project, as they are no longer needed.

### Track loading

In v4 the audio service has been completely overhauled. The audio service now uses a new track loading system that is much more flexible and extensible. The new track loading system is based on the new `LavalinkTrack` type. This type is now used to represent a track in Lavalink. The `LavalinkTrack` type is now stateless and implements new methods. This redesign simplifies the management of Lavalink tracks. Additionally, it is now possible to play a track without resolving it, reducing the roundtrip required to connect a player and start playing. Furthermore, track parsing and encoding are now closely integrated with the actual track type.

Previously you could load a track like this in Lavalink4NET v3:

```csharp
var track = await _audioService.GetTrackAsync(query, SearchMode.YouTube);
```

This has been changed and now you can access the `ITrackManager` from the audio service to load a track. The `ITrackManager` provides a lot of methods to load tracks.

```csharp
var track = await _audioService.Tracks
    .LoadTrackAsync(query, TrackSearchMode.YouTube)
    .ConfigureAwait(false);
```

### Getting the position of a player

In v4 the position property of a player has been made nullable, since it is not always possible to get the position of a player. This means that you will need to change your code to use the new nullable position property.

```csharp
    [SlashCommand("position", description: "Shows the track position", runMode: RunMode.Async)]
public async Task GetPositionAsync()
{
    var player = await GetPlayerAsync();

    if (player == null)
    {
        return;
    }

    if (player.CurrentTrack == null)
    {
        await ReplyAsync("Nothing playing!");
        return;
    }

    await ReplyAsync($"Position: {player.Position.Value.Position} / {player.CurrentTrack.Duration}.");
}
```

### SponsorBlock

In v4 the SponsorBlock API has changed a bit due to changes in the SponorBlock plugin for Lavalink. Setting default skip categories is no longer possible. Instead, you can now set the skip categories for each player individually. This means that you will need to change your code to use the new SponsorBlock API.

## Need help?

This guide covers the most common changes that you will need to make to migrate your codebase from v3 to v4. If you need help with migrating your codebase, feel free to join our Discord server. We will be happy to help you with migrating your codebase.

[![Lavalink4NET Support Server Banner](https://discordapp.com/api/guilds/894533462428635146/embed.png?style=banner3)](https://discord.gg/cD4qTmnqRg)
