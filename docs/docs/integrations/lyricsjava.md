# Lyrics.Java

The Lyrics.Java plugin for Lavalink allows you to fetch lyrics from YouTube or genius. The plugin will automatically fetch lyrics for the current track.

Lavalink4NET provides an integration for the Lyrics.Java plugin with the [`Lavalink4NET.Integrations.LyricsJava`](https://www.nuget.org/packages/Lavalink4NET.Integrations.LyricsJava) package.

## Installation

For using Lyrics.Java, you need to install the [`Lavalink4NET.Integrations.LyricsJava`](https://www.nuget.org/packages/Lavalink4NET.Integrations.LyricsJava) package.

:::caution
You need to have the [LyricsJava](https://github.com/DuncteBot/java-timed-lyrics) plugin installed on your Lavalink server.
:::

## Usage

First, you need to integrate the LyricsJava plugin with Lavalink4NET. You can do this by calling `UseLyricsJava` on either the host or the audio service:

```csharp
var app = builder.Build();

app.UseLyricsJava();

app.Run();
```

That's it! The LyricsJava plugin is now integrated with Lavalink4NET.

### Getting lyrics for the current track

For getting the lyrics of the current track, you can use the `GetCurrentTrackLyricsAsync` method. This method will return the lyrics of the current track. The method requires the session id and the guild id both of which you can get from player properties.

```csharp
var apiClient = await AudioService.ApiClientProvider
    .GetClientAsync()
    .ConfigureAwait(false);

var player = await audioService.Players
    .GetPlayerAsync(guildId)
    .ConfigureAwait(false);

var lyrics = await apiClient
    .GetCurrentTrackLyricsAsync(player.SessionId, player.GuildId)
    .ConfigureAwait(false);
```

### Getting lyrics from youtube

For getting the lyrics of a youtube video, you can use the `GetYoutubeLyricsAsync` method. This method will return the lyrics of the youtube video. The method requires a youtube video id, which can be acquired by using the `SearchAsync` method if using a different provider (e.g. Spotify).

```csharp
var apiClient = await AudioService.ApiClientProvider
    .GetClientAsync()
    .ConfigureAwait(false);

var results = await apiClient
    .SearchAsync("Queen - Bohemian Rhapsody")
    .ConfigureAwait(false);

var videoId = results.First().VideoId;

var lyrics = await apiClient
    .GetYoutubeLyricsAsync(videoId) // Youtube Video Id (e.g. dQw4w9WgXcQ)
    .ConfigureAwait(false);
```

### Getting lyrics from genius

For getting the lyrics of a song from genius, you can use the `GetGeniusLyricsAsync` method. This method will return the lyrics of the song. The method requires the song name and the artist name.

```csharp
var apiClient = await AudioService.ApiClientProvider
    .GetClientAsync()
    .ConfigureAwait(false);

var lyrics = await apiClient
    .GetGeniusLyricsAsync("Queen - Bohemian Rhapsody")
    .ConfigureAwait(false);
```

## Player listener

Similar to the inactivity tracking service, the LyricsJava integration also implements a player listener for receiving event notifications. The player listener can be used to receive notifications for when the lyrics of the current track has been loaded.

```csharp
public interface ILavaLyricsPlayerListener : ILavalinkPlayerListener
{
    ValueTask NotifyLyricsLoadedAsync(Lyrics lyrics, CancellationToken cancellationToken = default);
}
```
