/*
 *  File:   IArtworkService.cs
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

namespace Lavalink4NET.Artwork;

using System;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Tracks;

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
