/*
 *  File:   LavalinkTrack.cs
 *  Author: Angelo Breuer
 *
 *  The MIT License (MIT)
 *
 *  Copyright (c) Angelo Breuer 2022
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in
 *  all copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 *  THE SOFTWARE.
 */

namespace Lavalink4NET.Player;

using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Lavalink4NET.Payloads.Player;

/// <summary>
///     The information of a lavalink track.
/// </summary>
public class LavalinkTrack
{
    private StreamProvider? _streamProvider;
    private TimeSpan? _position;

    /// <summary>
    ///     Initializes a new instance of the <see cref="LavalinkTrack"/> class.
    /// </summary>
    /// <param name="identifier">the track identifier</param>
    /// <param name="trackInformation">the track info</param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="identifier"/> is blank.
    /// </exception>
    [JsonConstructor]
    public LavalinkTrack(string identifier, LavalinkTrackInfo trackInformation)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            throw new ArgumentException("The specified identifier can not be blank.", nameof(identifier));
        }

        Identifier = identifier;
        TrackInformation = trackInformation;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="LavalinkTrack"/> class.
    /// </summary>
    /// <param name="identifier">an unique track identifier</param>
    /// <param name="author">the name of the track author</param>
    /// <param name="duration">the duration of the track</param>
    /// <param name="isLiveStream">a value indicating whether the track is a live stream</param>
    /// <param name="isSeekable">a value indicating whether the track is seek-able</param>
    /// <param name="source">the track source</param>
    /// <param name="title">the title of the track</param>
    /// <param name="trackIdentifier">
    ///     the unique track identifier (Example: dQw4w9WgXcQ, YouTube Video ID)
    /// </param>
    /// <param name="streamProvider">the stream provider (e.g. YouTube)</param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="identifier"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="trackIdentifier"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="author"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="title"/> is <see langword="null"/>.
    /// </exception>
    public LavalinkTrack(
        string identifier,
        string author,
        TimeSpan duration,
        bool isLiveStream,
        bool isSeekable,
        Uri? uri,
        string? sourceName,
        TimeSpan position,
        string title,
        string trackIdentifier,
        object? context = null,
        StreamProvider? streamProvider = null)
    {
        Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
        Context = context;

        TrackInformation = new LavalinkTrackInfo
        {
            Author = author,
            Duration = duration,
            IsLiveStream = isLiveStream,
            IsSeekable = isSeekable,
            SourceName = sourceName,
            Uri = uri,
            Position = position,
            Title = title,
            TrackIdentifier = trackIdentifier,
        };

        _streamProvider = streamProvider;
    }

    /// <summary>
    ///     Gets the name of the track author.
    /// </summary>
    [JsonIgnore]
    public virtual string Author => TrackInformation.Author;

    /// <summary>
    ///     Gets or sets an arbitrary object that can be used by the user for associating data with the track.
    /// </summary>
    /// <value>an arbitrary object that can be used by the user for associating data with the track.</value>
    [JsonIgnore]
    public object? Context { get; set; }

    /// <summary>
    ///     Gets the duration of the track.
    /// </summary>
    [JsonIgnore]
    public virtual TimeSpan Duration => TrackInformation.Duration;

    /// <summary>
    ///     Gets an unique track identifier.
    /// </summary>
    [JsonPropertyName("track")]
    public virtual string Identifier { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the track is a live stream.
    /// </summary>
    [JsonIgnore]
    public virtual bool IsLiveStream => TrackInformation.IsLiveStream;

    /// <summary>
    ///     Gets a value indicating whether the track is seek-able.
    /// </summary>
    [JsonIgnore]
    public virtual bool IsSeekable => TrackInformation.IsSeekable;

    /// <summary>
    ///     Gets the start position of the track.
    /// </summary>
    [JsonIgnore]
    public virtual TimeSpan Position
    {
        get => _position ?? TrackInformation.Position;
        private set => _position = value;
    }

    /// <summary>
    ///     Gets the stream provider (e.g. YouTube).
    /// </summary>
    [JsonIgnore]
    public virtual StreamProvider Provider
    {
        get
        {
            if (_streamProvider is not null)
            {
                return _streamProvider.Value;
            }

            var streamProvider = StreamProvider.Unknown;
            if (SourceName is not null)
            {
                streamProvider = StreamProviderUtil.GetStreamProvider(SourceName);
            }

            if (Uri is not null && streamProvider is StreamProvider.Unknown)
            {
                streamProvider = StreamProviderUtil.GetStreamProvider(Uri);
            }

            _streamProvider  = streamProvider;
            return streamProvider;
        }

        internal set => _streamProvider = value;
    }

    /// <summary>
    ///     Gets the URI of the track.
    /// </summary>
    [JsonIgnore]
    public virtual Uri? Uri => TrackInformation.Uri;

    /// <summary>
    ///     Gets the track source.
    /// </summary>
    [Obsolete("Please use the 'Uri' property.")]
    public string? Source => Uri?.ToString();

    /// <summary>
    ///     Gets the name of the source (e.g. youtube, mp3, ...).
    /// </summary>
    [JsonIgnore]
    public virtual string? SourceName => TrackInformation.SourceName;

    /// <summary>
    ///     Gets the title of the track.
    /// </summary>
    [JsonIgnore]
    public virtual string Title => TrackInformation.Title;

    /// <summary>
    ///     Gets the unique track identifier (Example: dQw4w9WgXcQ, YouTube Video ID).
    /// </summary>
    [JsonIgnore]
    public virtual string TrackIdentifier => TrackInformation.TrackIdentifier;

    [JsonPropertyName("info")]
    public LavalinkTrackInfo TrackInformation { get; }

    /// <summary>
    ///     Clones the current track.
    /// </summary>
    /// <returns>the cloned <see cref="LavalinkTrack"/> instance</returns>
    public LavalinkTrack Clone() => new(
        identifier: Identifier,
        author: Author,
        duration: Duration,
        isLiveStream: IsLiveStream,
        isSeekable: IsSeekable,
        uri: Uri,
        sourceName: SourceName,
        position: Position,
        title: Title,
        trackIdentifier: TrackIdentifier,
        context: Context,
        streamProvider: Provider);

    /// <summary>
    ///     Clones the current track and sets the starting position to the specified <paramref name="position"/>.
    /// </summary>
    /// <param name="position">the starting position</param>
    /// <returns>the cloned <see cref="LavalinkTrack"/> instance</returns>
    public LavalinkTrack WithPosition(TimeSpan position)
    {
        var clone = Clone();
        clone.Position = position;
        return clone;
    }

    /// <summary>
    ///     Allows you to override a track that will be sent to Lavalink for playback
    /// </summary>
    /// <returns>Track which will be sent to Lavalink node</returns>
    public virtual ValueTask<LavalinkTrack> GetPlayableTrack()
    {
        return new ValueTask<LavalinkTrack>(this);
    }
}
