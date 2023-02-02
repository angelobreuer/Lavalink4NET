namespace Lavalink4NET.Tracks;

public partial record class LavalinkTrack
{
    private string? _trackData;
    private StreamProvider? _cachedProvider;
    private bool _providerCached;

#if NET7_0_OR_GREATER
    required
#endif
    public string Title
    { get; init; } = null!;

#if NET7_0_OR_GREATER
    required
#endif

    public string Identifier
    { get; init; }

#if NET7_0_OR_GREATER
    required
#endif

    public string Author
    { get; init; }

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

    internal string? TrackData
    {
        get => _trackData;
        init => _trackData = value;
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