namespace Lavalink4NET.Tracks;

public partial record class LavalinkTrack
{
    private StreamProvider? _cachedProvider;
    private bool _providerCached;

    public required string Author { get; init; }

    public required string Title { get; init; }

    public required string Identifier { get; init; }

    public TimeSpan Duration { get; init; }

    public bool IsLiveStream { get; init; }

    public bool IsSeekable { get; init; }

    public Uri? Uri { get; init; }

    public string? SourceName { get; init; }

    public TimeSpan? StartPosition { get; init; }

    public string? ProbeInfo { get; init; }

    public StreamProvider? Provider
    {
        get
        {
            if (_providerCached)
            {
                return _cachedProvider;
            }

            _cachedProvider = StreamHeuristics.GetStreamProvider(SourceName);
            _providerCached = true;

            return _cachedProvider;
        }
    }

    /// <summary>
    ///     Allows you to override a track that will be sent to Lavalink for playback
    /// </summary>
    /// <returns>Track which will be sent to Lavalink node</returns>
    public virtual ValueTask<LavalinkTrack> GetPlayableTrackAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(this);
    }
}