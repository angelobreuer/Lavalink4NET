namespace Lavalink4NET.Rest;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Player;

/// <summary>
///     An interface for a lavalink rest client, that can load tracks from a node.
/// </summary>
public interface ILavalinkRestClient : IDisposable
{
    /// <summary>
    ///     Gets the track for the specified <paramref name="query"/> asynchronously.
    /// </summary>
    /// <param name="query">the track search query</param>
    /// <param name="mode">the track search mode</param>
    /// <param name="noCache">
    ///     a value indicating whether the track should be returned from cache, if it is cached.
    ///     Note this parameter does only take any effect is a cache provider is specified in constructor.
    /// </param>
    /// <param name="cancellationToken">
    ///     a cancellation token that can be used by other objects or threads to receive notice
    ///     of cancellation.
    /// </param>
    /// <returns>
    ///     a task that represents the asynchronous operation. The task result is the track
    ///     found for the specified <paramref name="query"/>
    /// </returns>
    ValueTask<LavalinkTrack?> GetTrackAsync(
        string query,
        SearchMode mode = SearchMode.None,
        bool noCache = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets the tracks for the specified <paramref name="query"/> asynchronously.
    /// </summary>
    /// <param name="query">the track search query</param>
    /// <param name="mode">the track search mode</param>
    /// <param name="noCache">
    ///     a value indicating whether the track should be returned from cache, if it is cached.
    ///     Note this parameter does only take any effect is a cache provider is specified in constructor.
    /// </param>
    /// <param name="cancellationToken">
    ///     a cancellation token that can be used by other objects or threads to receive notice
    ///     of cancellation.
    /// </param>
    /// <returns>
    ///     a task that represents the asynchronous operation. The task result are the tracks
    ///     found for the specified <paramref name="query"/>
    /// </returns>
    ValueTask<IEnumerable<LavalinkTrack>> GetTracksAsync(
        string query,
        SearchMode mode = SearchMode.None,
        bool noCache = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Loads the tracks specified by the <paramref name="query"/> asynchronously.
    /// </summary>
    /// <param name="query">the search query</param>
    /// <param name="mode">the track search mode</param>
    /// <param name="noCache">
    ///     a value indicating whether the track should be returned from cache, if it is cached.
    ///     Note this parameter does only take any effect is a cache provider is specified in constructor.
    /// </param>
    /// <param name="cancellationToken">
    ///     a cancellation token that can be used by other objects or threads to receive notice
    ///     of cancellation.
    /// </param>
    /// <returns>
    ///     a task that represents the asynchronous operation. The task result is the request
    ///     response for the specified <paramref name="query"/>.
    /// </returns>
    ValueTask<TrackLoadResponsePayload> LoadTracksAsync(
        string query,
        SearchMode mode = SearchMode.None,
        bool noCache = false,
        CancellationToken cancellationToken = default);
}
