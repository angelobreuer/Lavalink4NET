namespace Lavalink4NET.Tracks;

using System.Collections.Immutable;
using System.Text.Json;

public partial record class LavalinkTrack
{
    private StreamProvider? _cachedProvider;
    private bool _providerCached;
    private string? _cachedTrackData;
    private LavalinkTrack? _trackDataOwner;

#if NET7_0_OR_GREATER
    required
#endif
    public string Title
    { get; init; } = null!;

#if NET7_0_OR_GREATER
    required
#endif

    public string Identifier
    { get; init; } = null!;

#if NET7_0_OR_GREATER
    required
#endif

    public string Author
    { get; init; } = null!;

    public TimeSpan Duration { get; init; }

    public bool IsLiveStream { get; init; }

    public bool IsSeekable { get; init; }

    public Uri? Uri { get; init; }

    public Uri? ArtworkUri { get; init; }

    public string? Isrc { get; init; }

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

    public IImmutableDictionary<string, JsonElement> AdditionalInformation { get; init; } = ImmutableDictionary<string, JsonElement>.Empty;

    internal string? TrackData
    {
        get => ReferenceEquals(this, _trackDataOwner) ? _cachedTrackData : null;

        set
        {
            _cachedTrackData = value;
            _trackDataOwner = this;
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

    private static bool IsExtendedTrack(string sourceName)
    {
        return sourceName.Equals("spotify", StringComparison.OrdinalIgnoreCase)
            || sourceName.Equals("applemusic", StringComparison.OrdinalIgnoreCase)
            || sourceName.Equals("deezer", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsProbingTrack(string sourceName)
    {
        return sourceName.Equals("http", StringComparison.OrdinalIgnoreCase)
            || sourceName.Equals("local", StringComparison.OrdinalIgnoreCase);
    }
}