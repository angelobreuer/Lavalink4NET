namespace Lavalink4NET.Tracks;

using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Json;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public partial record class LavalinkTrack
{
    private StreamProvider? _cachedProvider;
    private bool _providerCached;
    private string? _cachedTrackData;
    private LavalinkTrack? _trackDataOwner;
    private TimeSpan _startPosition;

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

    public TimeSpan? StartPosition
    {
        get => _startPosition == default ? null : _startPosition;
        init => _startPosition = value ?? TimeSpan.Zero;
    }

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

    public virtual bool Equals(LavalinkTrack? other)
    {
        if (ReferenceEquals(this, other))
        {
            return true; // same instance
        }

        if (other is null)
        {
            return false; // other is null
        }

        static bool AdditionalInformationEquals(
            IImmutableDictionary<string, JsonElement>? a,
            IImmutableDictionary<string, JsonElement>? b)
        {
            if (a is null || b is null)
            {
                return a is null && b is null;
            }

            if (a.Count != b.Count)
            {
                return false;
            }

            foreach (var (key, value) in a)
            {
                if (!b.TryGetValue(key, out var otherValue) || !value.Equals(otherValue))
                {
                    return false;
                }
            }

            return true;
        }

        return EqualityComparer<string>.Default.Equals(Title, other.Title)
            && EqualityComparer<string>.Default.Equals(Identifier, other.Identifier)
            && EqualityComparer<string>.Default.Equals(Author, other.Author)
            && EqualityComparer<TimeSpan>.Default.Equals(Duration, other.Duration)
            && EqualityComparer<bool>.Default.Equals(IsLiveStream, other.IsLiveStream)
            && EqualityComparer<bool>.Default.Equals(IsSeekable, other.IsSeekable)
            && EqualityComparer<Uri>.Default.Equals(Uri, other.Uri)
            && EqualityComparer<Uri>.Default.Equals(ArtworkUri, other.ArtworkUri)
            && EqualityComparer<string>.Default.Equals(Isrc, other.Isrc)
            && EqualityComparer<string>.Default.Equals(SourceName, other.SourceName)
            && EqualityComparer<TimeSpan?>.Default.Equals(StartPosition, other.StartPosition)
            && EqualityComparer<string>.Default.Equals(ProbeInfo, other.ProbeInfo)
            && AdditionalInformationEquals(AdditionalInformation, other.AdditionalInformation);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(Title, EqualityComparer<string>.Default);
        hash.Add(Identifier, EqualityComparer<string>.Default);
        hash.Add(Author, EqualityComparer<string>.Default);
        hash.Add(Duration, EqualityComparer<TimeSpan>.Default);
        hash.Add(IsLiveStream, EqualityComparer<bool>.Default);
        hash.Add(IsSeekable, EqualityComparer<bool>.Default);
        hash.Add(Uri, EqualityComparer<Uri?>.Default);
        hash.Add(ArtworkUri, EqualityComparer<Uri?>.Default);
        hash.Add(Isrc, EqualityComparer<string?>.Default);
        hash.Add(SourceName, EqualityComparer<string?>.Default);
        hash.Add(StartPosition, EqualityComparer<TimeSpan?>.Default);
        hash.Add(ProbeInfo, EqualityComparer<string?>.Default);

        hash.Add(AdditionalInformation.Count);

        foreach (var (key, value) in AdditionalInformation)
        {
            hash.Add(key, EqualityComparer<string>.Default);
            hash.Add(value, EqualityComparer<JsonElement>.Default);
        }

        return hash.ToHashCode();
    }

    internal string GetDebuggerDisplay() => $"{Title} ({Author}), {ToString()}";
}