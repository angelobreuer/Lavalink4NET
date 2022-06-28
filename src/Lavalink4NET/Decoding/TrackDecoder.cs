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
using System.Buffers.Binary;
using System.IO;
using System.Text;
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
        => new(identifier, Decode(identifier, verify));

    /// <summary>
    ///     Decodes a lavalink track identifier.
    /// </summary>
    /// <param name="identifier">the track identifier (encoded in base64)</param>
    /// <param name="verify">a value indicating whether the track header should be verified</param>
    /// <returns>the decoded track info</returns>
    /// <exception cref="InvalidOperationException">thrown if the track header is invalid</exception>
    public static LavalinkTrackInfo Decode(string identifier, bool verify = true)
        => Decode(Convert.FromBase64String(identifier), verify);

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
    public static LavalinkTrackInfo Decode(ReadOnlySpan<byte> buffer, bool verify = true)
    {
        ReadHeader(ref buffer, verify);

        var title = ReadString(ref buffer);
        var author = ReadString(ref buffer);
        var length = ReadInt64(ref buffer);
        var identifier = ReadString(ref buffer);
        var isStream = ReadBoolean(ref buffer);
        var uri = ReadBoolean(ref buffer) ? ReadString(ref buffer) : null;

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

    private static bool ReadBoolean(ref ReadOnlySpan<byte> buffer)
    {
        if (buffer.IsEmpty)
        {
            throw new EndOfStreamException("Failed to read boolean, found EOF.");
        }

        var value = buffer[0] is not 0;
        buffer = buffer.Slice(1);
        return value;
    }

    private static long ReadInt64(ref ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < 8)
        {
            throw new EndOfStreamException("Failed to read UInt64, found EOF.");
        }

        var value = BinaryPrimitives.ReadInt64BigEndian(buffer);
        buffer = buffer.Slice(8);
        return value;
    }

    private static string ReadString(ref ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < 2)
        {
            throw new EndOfStreamException("Failed to read string length, found EOF.");
        }

        var length = BinaryPrimitives.ReadUInt16BigEndian(buffer);
        buffer = buffer.Slice(2);

        if (buffer.Length < length)
        {
            var bytesMissing = length - buffer.Length;
            throw new EndOfStreamException($"Failed to read string, found EOF, expected {bytesMissing} more byte(s).");
        }

        var stringBuffer = buffer.Slice(0, length);
        buffer = buffer.Slice(length);

#if NETSTANDARD2_1_OR_GREATER
        return Encoding.UTF8.GetString(stringBuffer);
#else
        return Encoding.UTF8.GetString(stringBuffer.ToArray());
#endif
    }

    private static void ReadHeader(ref ReadOnlySpan<byte> buffer, bool verify = true)
    {
        if (buffer.Length is < 4)
        {
            throw new InvalidDataException("Header is missing from track identifier.");
        }

        // the header is four bytes long, subtract
        var header = BinaryPrimitives.ReadUInt32BigEndian(buffer);
        buffer = buffer.Slice(4);

        var flags = (int)((header & 0xC0000000L) >> 30);
        var hasVersion = (flags & 1) is not 0;

        // verify size
        var size = header & 0x3FFFFFFF;
        if (verify && size != buffer.Length)
        {
            throw new InvalidOperationException($"Error while verifying track header: Track Identifier length was {buffer.Length}, but expected: {size}");
        }

        var version = 1;
        if (hasVersion)
        {
            if (buffer.IsEmpty)
            {
                throw new InvalidDataException("Content version is missing from track identifier.");
            }

            version = buffer[0];
            buffer = buffer.Slice(1);
        }


        // verify version
        if (verify && version is not 2)
        {
            throw new InvalidOperationException($"Error while verifying track header: Invalid track version: Was: {version}, expected: 2.");
        }
    }
}
