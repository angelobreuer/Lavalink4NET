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
using System.Buffers;
using System.Buffers.Binary;
using System.Buffers.Text;
using System.Diagnostics;
using System.IO;
using System.Text;
using Lavalink4NET.Player;

/// <summary>
///     An utility class for decoding lavalink tracks.
/// </summary>
public static class TrackDecoder
{
    public static LavalinkTrack DecodeTrack(string identifier)
    {
        var operationStatus = TryDecodeTrack(identifier, out var track);

        if (operationStatus is not OperationStatus.Done)
        {
            throw new InvalidDataException($"An error occurred while decoding the track: {operationStatus}");
        }

        return track!;
    }

    public static LavalinkTrackInfo Decode(string identifier)
    {
        var operationStatus = TryDecode(identifier, out var trackInfo);

        if (operationStatus is not OperationStatus.Done)
        {
            throw new InvalidDataException($"An error occurred while decoding the track: {operationStatus}");
        }

        return trackInfo!;
    }

    public static LavalinkTrackInfo Decode(ReadOnlySpan<byte> buffer)
    {
        var operationStatus = TryDecode(buffer, out var trackInfo);

        if (operationStatus is not OperationStatus.Done)
        {
            throw new InvalidDataException($"An error occurred while decoding the track: {operationStatus}");
        }

        return trackInfo!;
    }

    public static OperationStatus TryDecodeTrack(string identifier, out LavalinkTrack? track)
    {
        var operationStatus = TryDecode(identifier, out var trackInfo);

        if (operationStatus is not OperationStatus.Done)
        {
            track = default;
            return operationStatus;
        }

        track = new LavalinkTrack(identifier, trackInfo!);
        return OperationStatus.Done;
    }

    public static OperationStatus TryDecode(string identifier, out LavalinkTrackInfo? trackInfo)
    {
        var maxDecodedUtf8Length = Encoding.UTF8.GetMaxByteCount(identifier.Length);
        var pooledBuffer = ArrayPool<byte>.Shared.Rent(maxDecodedUtf8Length);

        try
        {
            var utf8BytesWritten = Encoding.UTF8.GetBytes(
                s: identifier,
                charIndex: 0,
                charCount: identifier.Length,
                bytes: pooledBuffer,
                byteIndex: 0);

            var operationStatus = Base64.DecodeFromUtf8InPlace(
                buffer: pooledBuffer.AsSpan(0, utf8BytesWritten),
                bytesWritten: out var bytesWritten);

            if (operationStatus is not OperationStatus.Done)
            {
                trackInfo = default;
                return operationStatus;
            }

            return TryDecode(pooledBuffer.AsSpan(0, bytesWritten), out trackInfo);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(pooledBuffer);
        }
    }

    public static OperationStatus TryDecode(ReadOnlySpan<byte> buffer, out LavalinkTrackInfo? trackInfo)
    {
        var operationStatus = TryReadHeader(ref buffer);
        trackInfo = default;

        if (operationStatus is not OperationStatus.Done)
        {
            return operationStatus;
        }

        operationStatus = TryReadString(ref buffer, out var title);

        if (operationStatus is not OperationStatus.Done)
        {
            return operationStatus;
        }

        operationStatus = TryReadString(ref buffer, out var author);

        if (operationStatus is not OperationStatus.Done)
        {
            return operationStatus;
        }

        operationStatus = TryReadInt64(ref buffer, out var length);

        if (operationStatus is not OperationStatus.Done)
        {
            return operationStatus;
        }

        operationStatus = TryReadString(ref buffer, out var identifier);

        if (operationStatus is not OperationStatus.Done)
        {
            return operationStatus;
        }

        operationStatus = TryReadBoolean(ref buffer, out var isStream);

        if (operationStatus is not OperationStatus.Done)
        {
            return operationStatus;
        }

        operationStatus = TryReadOptionalString(ref buffer, out var rawUri);

        if (operationStatus is not OperationStatus.Done)
        {
            return operationStatus;
        }

        var uri = default(Uri?);
        if (rawUri is not null && !Uri.TryCreate(rawUri, UriKind.Absolute, out uri))
        {
            return OperationStatus.InvalidData;
        }

        var sourceName = default(string?);
        if (!buffer.IsEmpty)
        {
            operationStatus = TryReadString(ref buffer, out sourceName);

            if (operationStatus is not OperationStatus.Done)
            {
                return operationStatus;
            }
        }

        var position = 0L;
        if (!buffer.IsEmpty)
        {
            operationStatus = TryReadInt64(ref buffer, out position);

            if (operationStatus is not OperationStatus.Done)
            {
                return operationStatus;
            }
        }

        // Ensure there is no rest data
        Debug.Assert(buffer.IsEmpty);

        trackInfo = new LavalinkTrackInfo
        {
            Title = title!,
            Author = author!,
            Duration = TimeSpan.FromMilliseconds(length),
            TrackIdentifier = identifier!,
            IsLiveStream = isStream,
            IsSeekable = !isStream,
            Uri = uri,
            Position = TimeSpan.FromMilliseconds(position),
            SourceName = sourceName,
        };

        return OperationStatus.Done;
    }

    private static OperationStatus TryReadBoolean(ref ReadOnlySpan<byte> buffer, out bool value)
    {
        if (buffer.IsEmpty)
        {
            value = default;
            return OperationStatus.NeedMoreData;
        }

        value = buffer[0] is not 0;
        buffer = buffer.Slice(1);
        return OperationStatus.Done;
    }

    private static OperationStatus TryReadInt64(ref ReadOnlySpan<byte> buffer, out long value)
    {
        if (buffer.Length < 8)
        {
            value = default;
            return OperationStatus.NeedMoreData;
        }

        value = BinaryPrimitives.ReadInt64BigEndian(buffer);
        buffer = buffer.Slice(8);
        return OperationStatus.Done;
    }

    private static OperationStatus TryReadString(ref ReadOnlySpan<byte> buffer, out string? value)
    {
        if (buffer.Length < 2)
        {
            value = default;
            return OperationStatus.NeedMoreData;
        }

        var length = BinaryPrimitives.ReadUInt16BigEndian(buffer);
        buffer = buffer.Slice(2);

        if (buffer.Length < length)
        {
            var bytesMissing = length - buffer.Length;
            value = default;
            return OperationStatus.NeedMoreData;
        }

        var stringBuffer = buffer.Slice(0, length);
        buffer = buffer.Slice(length);

#if NETSTANDARD2_1_OR_GREATER
        value = Encoding.UTF8.GetString(stringBuffer);
#else
        value = Encoding.UTF8.GetString(stringBuffer.ToArray());
#endif

        return OperationStatus.Done;
    }

    private static OperationStatus TryReadOptionalString(ref ReadOnlySpan<byte> buffer, out string? value)
    {
        var operationStatus = TryReadBoolean(ref buffer, out var isPresent);

        if (operationStatus is not OperationStatus.Done)
        {
            value = default;
            return operationStatus;
        }

        if (!isPresent)
        {
            value = null;
            return OperationStatus.Done;
        }

        return TryReadString(ref buffer, out value);
    }

    private static OperationStatus TryReadHeader(ref ReadOnlySpan<byte> buffer)
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
        if (size != buffer.Length)
        {
            return OperationStatus.InvalidData;
        }

        var version = 1;
        if (hasVersion)
        {
            if (buffer.IsEmpty)
            {
                return OperationStatus.NeedMoreData;
            }

            version = buffer[0];
            buffer = buffer.Slice(1);
        }

        // verify version
        if (version is not 2)
        {
            return OperationStatus.InvalidData;
        }

        return OperationStatus.Done;
    }
}
