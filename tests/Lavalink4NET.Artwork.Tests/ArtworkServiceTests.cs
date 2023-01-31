/*
 *  File:   ArtworkServiceTests.cs
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

namespace Lavalink4NET.Tests;

using System;
using System.Threading.Tasks;
using Lavalink4NET.Artwork;
using Lavalink4NET.Tracks;

public sealed class ArtworkServiceTests
{
    [Theory]
    [InlineData("https://soundcloud.com/luudeofficial/men-at-work-down-under-luude-remix-1", "https://i1.sndcdn.com/artworks-0WkSQsT8dR9bGMnj-aZU27w-t500x500.jpg")]
    public async Task TestResolveSoundCloudAsync(string uri, string thumbnail)
    {
        // Arrange
        using var artworkService = new ArtworkService(cache: null);

        var track = new LavalinkTrack
        {
            Identifier = string.Empty,
            Author = string.Empty,
            Duration = default,
            IsLiveStream = false,
            IsSeekable = false,
            Uri = new Uri(uri),
            SourceName = "soundcloud",
            StartPosition = TimeSpan.Zero,
            Title = string.Empty,
        };

        // Act
        var artwork = await artworkService.ResolveAsync(track);

        // Assert
        Assert.NotNull(artwork);
        Assert.Equal(thumbnail, artwork.OriginalString);
    }
}
