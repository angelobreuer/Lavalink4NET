/*
 *  File:   TrackDecoder.cs
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

namespace Lavalink4NET.Decoding;

using System;
using System.IO;
using Lavalink4NET.Player;

/// <summary>
///     An utility class for decoding lavalink tracks.
/// </summary>
public static class TrackDecoder
{
    /// <summary>
    ///     Decodes a lavalink track identifier.
    /// </summary>
    /// <param name="identifier">the track identifier (encoded in base64)</param>
    /// <param name="verify">a value indicating whether the track header should be verified</param>
    /// <returns>the decoded track</returns>
    /// <exception cref="InvalidOperationException">thrown if the track header is invalid</exception>
    public static LavalinkTrack DecodeTrack(string identifier, bool verify = true)
        => new(identifier, DecodeTrackInfo(identifier, verify));

    /// <summary>
    ///     Decodes a lavalink track identifier.
    /// </summary>
    /// <param name="identifier">the track identifier (encoded in base64)</param>
    /// <param name="verify">a value indicating whether the track header should be verified</param>
    /// <returns>the decoded track info</returns>
    /// <exception cref="InvalidOperationException">thrown if the track header is invalid</exception>
    public static LavalinkTrackInfo DecodeTrackInfo(string identifier, bool verify = true)
        => DecodeTrackInfo(Convert.FromBase64String(identifier), verify);

    /// <summary>
    ///     Decodes a lavalink track identifier.
    /// </summary>
    /// <param name="buffer">the raw track identifier</param>
    /// <param name="verify">a value indicating whether the track header should be verified</param>
    /// <returns>the decoded track info</returns>
    /// <exception cref="InvalidOperationException">thrown if the track header is invalid</exception>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="buffer"/> is <see langword="null"/>.
    /// </exception>
    public static LavalinkTrackInfo DecodeTrackInfo(byte[] buffer, bool verify = true)
    {
        if (buffer is null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        using var memoryStream = new MemoryStream(buffer);
        using var reader = new DataInputReader(memoryStream);

        ReadHeader(reader, buffer.Length, verify);

        var title = reader.ReadString();
        var author = reader.ReadString();
        var length = reader.ReadInt64();
        var identifier = reader.ReadString();
        var isStream = reader.ReadBoolean();
        var uri = reader.ReadBoolean() ? reader.ReadString() : null;

        return new LavalinkTrackInfo
        {
            Title = title,
            Author = author,
            Duration = TimeSpan.FromMilliseconds(length),
            TrackIdentifier = identifier,
            IsLiveStream = isStream,
            IsSeekable = !isStream,
            Source = uri
        };
    }

    /// <summary>
    ///     Reads the track header.
    /// </summary>
    /// <param name="reader">the reader to read from</param>
    /// <param name="length">the length of raw binary data</param>
    /// <param name="verify">a value indicating whether the track header should be verified</param>
    /// <exception cref="InvalidOperationException">thrown if the track header is invalid</exception>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="reader"/> is <see langword="null"/>.
    /// </exception>
    private static void ReadHeader(DataInputReader reader, int length, bool verify = true)
    {
        if (reader is null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        // the header is four bytes long, subtract
        length -= 4;

        var header = reader.ReadInt32();
        var flags = (int)((header & 0xC0000000L) >> 30);
        var hasVersion = (flags & 1) != 0;
        var version = hasVersion ? reader.ReadSByte() : 1;
        var size = header & 0x3FFFFFFF;

        // verify size
        if (verify && size != length)
        {
            throw new InvalidOperationException($"Error while verifying track header: Track Identifier length was {length}, but expected: {size}");
        }

        // verify version
        if (verify && version != 2)
        {
            throw new InvalidOperationException($"Error while verifying track header: Invalid track version: Was: {version}, expected: 2.");
        }
    }
}
