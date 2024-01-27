namespace Lavalink4NET.Rest.Entities.Tracks;

public readonly record struct TrackLoadOptions(
    TrackSearchMode SearchMode = default,
    StrictSearchBehavior SearchBehavior = StrictSearchBehavior.Throw,
    CacheMode CacheMode = CacheMode.Dynamic)
{
    // Constructor for compatibility
    public TrackLoadOptions(
        TrackSearchMode SearchMode = default,
        bool? StrictSearch = null,
        CacheMode CacheMode = CacheMode.Dynamic) : this(
            SearchMode: SearchMode,
            SearchBehavior: StrictSearch.GetValueOrDefault(true) ? StrictSearchBehavior.Throw : StrictSearchBehavior.Passthrough,
            CacheMode: CacheMode)
    {

    }
}