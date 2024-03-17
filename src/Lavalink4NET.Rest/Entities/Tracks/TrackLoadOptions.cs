namespace Lavalink4NET.Rest.Entities.Tracks;

public readonly record struct TrackLoadOptions(
    TrackSearchMode SearchMode = default,
    StrictSearchBehavior SearchBehavior = StrictSearchBehavior.Throw,
    CacheMode CacheMode = CacheMode.Dynamic);
