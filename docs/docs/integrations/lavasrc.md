# Lavasrc

The [lavasrc](https://github.com/topi314/LavaSrc) lavalink plugin provides additional Lavaplayer and Lavasearch audio source managers.

## Installation

For using the Lavasrc integration, you need to install the [`Lavalink4NET.Integrations.Lavasrc`](https://www.nuget.org/packages/Lavalink4NET.Integrations.Lavasrc) package.

:::caution
You need to have the [Lavasrc plugin](https://github.com/topi314/LavaSrc#lavalink-usage) installed on your Lavalink server.
:::

## Usage

The Lavasrc integrations provides types to interpret the additional metadata provided by the Lavasrc plugin.

:::info
The following tracks are resolved using the Lavasearch integration which allows to resolve tracks more advanced.
:::

When you resolve a track, you can wrap the `LavalinkTrack` in an `ExtendedLavalinkTrack` to access the additional metadata.

```csharp
var searchResult = await audio.Tracks.SearchAsync(
    query: "[...]",
    loadOptions: new TrackLoadOptions(SearchMode: TrackSearchMode.Spotify),
    categories: ImmutableArray.Create(SearchCategory.Track));

var track = new ExtendedLavalinkTrack(searchResult.Tracks[0]);

var artist = track.Artist;
var album = track.Album;
var isPreview = track.IsPreview;
var previewUri = track.PreviewUri;
```

Similarly, you can wrap the `PlaylistInformation` in an `ExtendedPlaylistInformation` to access the additional metadata.

```csharp
var searchResult = await audio.Tracks.SearchAsync(
    query: "[...]",
    loadOptions: new TrackLoadOptions(SearchMode: TrackSearchMode.Spotify),
    categories: ImmutableArray.Create(SearchCategory.Playlist));

var playlist = new ExtendedPlaylistInformation(searchResult.Playlists[0]);

var author = playlist.Author;
var totalTracks = playlist.TotalTracks;
var artworkUri = playlist.ArtworkUri;
var uri = playlist.Uri;
var type = playlist.Type;
```
