# LavaSearch

The [LavaSearch](https://github.com/topi314/LavaSearch) lavalink plugin allows to search tracks more advanced than the default Lavalink search.

## Installation

For using the Lavasrc integration, you need to install the [`Lavalink4NET.Integrations.Lavasearch`](https://www.nuget.org/packages/Lavalink4NET.Integrations.Lavasearch) package.

:::caution
You need to have the [Lavasearch plugin](https://github.com/topi314/Lavasearch#lavalink-usage) installed on your Lavalink server.
:::

## Usage

In order to resolve tracks using the Lavasearch integration, you can use the `SearchAsync` method on the `ITrackManager` interface.

:::info
In the following example, the track is resolved using Spotify. Lavasearch works together with search plugins like Lavasrc which need to be installed on the Lavalink server.
:::

```cs
var searchResult = await audio.Tracks.SearchAsync(
    query: "[...]",
    loadOptions: new TrackLoadOptions(SearchMode: TrackSearchMode.Spotify),
    categories: ImmutableArray.Create(SearchCategory.Track));

var tracks = searchResult.Tracks;
var playlists = searchResult.Playlists;
var albums = searchResult.Albums;
var artists = searchResult.Artists;
var texts = searchResult.Texts;
```

You can specify the search mode using the `TrackLoadOptions` parameter. The categories Lavasearch should search for can be specified using the `categories` parameter.

Depending on what categories you specified, the `SearchResult` object will contain tracks, playlists, albums, artists and/or texts.
