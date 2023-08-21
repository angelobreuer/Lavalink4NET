namespace Lavalink4NET.Rest.Entities.Tracks;

public readonly record struct TrackLoadOptions(
    TrackSearchMode SearchMode = default,
    bool? StrictSearch = null,
    CacheMode CacheMode = CacheMode.Dynamic);