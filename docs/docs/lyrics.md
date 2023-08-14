# Lyrics

:::danger
The lyrics.ovh API is currently down and needs to be self-hosted. Please refer to the [lyrics.ovh GitHub repository](https://github.com/NTag/lyrics.ovh) for a guide.
:::

Some bots provide a feature to show the lyrics of the currently playing song. This feature is provided by the `Lavalink4NET.Lyrics` package. This package provides a `ILyricsService` interface which can be used to retrieve the lyrics of a song.

## Package

:::note
The lyrics feature is only available in the `Lavalink4NET.Lyrics` package.
:::

## Usage

First, to use the lyrics feature, you need to register the `ILyricsService` service in your service collection:

```csharp
builder.Services.AddLyrics();
```

The `ILyricsService` interface provides a `GetLyricsAsync` method which can be used to retrieve the lyrics of a song. The method returns a `LyricsResult` object which contains the lyrics of the song.

We will now add a command that retrieves the lyrics of the currently playing song:

```csharp
[SlashCommand("lyrics", description: "Searches for lyrics", runMode: RunMode.Async)]
public async Task Lyrics()
{
    await DeferAsync().ConfigureAwait(false);

    var player = await GetPlayerAsync(connectToVoiceChannel: false).ConfigureAwait(false);

    if (player is null)
    {
        return;
    }

    var track = player.CurrentTrack;

    if (track is null)
    {
        await FollowupAsync("🤔 No track is currently playing.").ConfigureAwait(false);
        return;
    }

    var lyrics = await _lyricsService.GetLyricsAsync(track.Title, track.Author).ConfigureAwait(false);

    if (lyrics is null)
    {
        await FollowupAsync("😖 No lyrics found.").ConfigureAwait(false);
        return;
    }

    await FollowupAsync($"📃 Lyrics for {track.Title} by {track.Author}:\n{lyrics}").ConfigureAwait(false);
}
```

## Configuration

You can configure the lyrics service using the dependency injection configuration by using the `ConfigureLyrics` method.

### Base address

If you want to use another lyrics API, you can change the base address of the lyrics service, for example your self-hosted lyrics.ovh instance:

```csharp
builder.Services.ConfigureLyrics(
    options => options.BaseAddress = new Uri("https://some-lyrics-api.com/"));
```
