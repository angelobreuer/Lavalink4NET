---
sidebar_position: 3
---

# Track affinity

Track affinity is an optimization that is available when using clustering with Lavalink4NET. By default, if no information is provided, Lavalink4NET will use any available node in the cluster to resolve a track. You can configure Lavalink4NET to use a specific node to resolve a track which can improve performance in some cases.

## How to use track affinity

Lavalink4NET provides a `LavalinkApiResolutionScope` which is used to specify a node to use when resolving tracks. You can use the `LavalinkApiResolutionScope` by passing the `ApiClient` parameter to set the node to the node which also should play the track.

## Example

```csharp
 var player = await GetPlayerAsync(connectToVoiceChannel: true).ConfigureAwait(false);

if (player is null)
{
    return;
}

var resolutionScope = new LavalinkApiResolutionScope(player.ApiClient);

var track = await _audioService.Tracks
    .LoadTrackAsync(query, TrackSearchMode.YouTube, resolutionScope)
    .ConfigureAwait(false);

if (track is null)
{
    await FollowupAsync("😖 No results.").ConfigureAwait(false);
    return;
}

var position = await player.PlayAsync(track).ConfigureAwait(false);

if (position is 0)
{
    await FollowupAsync($"🔈 Playing: {track.Uri}").ConfigureAwait(false);
}
else
{
    await FollowupAsync($"🔈 Added to queue: {track.Uri}").ConfigureAwait(false);
}
```

You can see that we are passing the `ApiClient` to the `LoadTrackAsync` method. This will make sure that the track is resolved by the node which also should play the track.
