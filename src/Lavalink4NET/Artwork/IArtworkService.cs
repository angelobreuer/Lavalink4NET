namespace Lavalink4NET.Artwork;

using System;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Player;

/// <summary>
///     A service that can be used to resolve artworks for tracks.
/// </summary>
public interface IArtworkService
{
    /// <summary>
    ///     Resolves the artwork for the specified track asynchronously.
    /// </summary>
    /// <param name="lavalinkTrack">the track to resolve the artwork for.</param>
    /// <param name="cancellationToken">a cancellation token used to propagate notification that the operation should be canceled.</param>
    /// <returns>a task that represents the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">thrown if the operation was canceled.</exception>
    ValueTask<Uri?> ResolveAsync(LavalinkTrack lavalinkTrack, CancellationToken cancellationToken = default);
}
