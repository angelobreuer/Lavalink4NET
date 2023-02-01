namespace Lavalink4NET.Tracks;

using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Buffers.Text;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Unicode;

public partial record class LavalinkTrack
#if NET7_0_OR_GREATER
    : ISpanParsable<LavalinkTrack>
#endif
{
    public static LavalinkTrack Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (!TryParse(s, provider, out var result))
        {
            throw new ArgumentException("Invalid track.", nameof(s));
        }

        return result;
    }

    public static LavalinkTrack Parse(string s, IFormatProvider? provider)
    {
        return Parse(s.AsSpan(), provider);
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out LavalinkTrack result)
    {
        var pool = ArrayPool<byte>.Shared.Rent(s.Length);

        try
        {
            var operationStatus = Utf8.FromUtf16(
                source: s,
                destination: pool,
                charsRead: out _,
                bytesWritten: out var utf8BytesWritten);

            if (operationStatus is not OperationStatus.Done)
            {
                Debug.Assert(operationStatus is not OperationStatus.DestinationTooSmall);

                result = null;
                return false;
            }

            operationStatus = Base64.DecodeFromUtf8InPlace(
                buffer: pool.AsSpan(0, utf8BytesWritten),
                bytesWritten: out var decodedBytesWritten);

            if (operationStatus is not OperationStatus.Done)
            {
                Debug.Assert(operationStatus is not OperationStatus.DestinationTooSmall);

                result = null;
                return false;
            }

            return TryParse(pool.AsSpan(0, decodedBytesWritten), out result);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(pool);
        }
    }

    private static bool TryParse(ReadOnlySpan<byte> buffer, [MaybeNullWhen(false)] out LavalinkTrack result)
    {
        result = null;

        if (!TryReadHeader(ref buffer) ||
            !TryReadString(ref buffer, out var title) ||
            !TryReadString(ref buffer, out var author) ||
            !TryReadInt64(ref buffer, out var duration) ||
            !TryReadString(ref buffer, out var identifier) ||
            !TryReadBoolean(ref buffer, out var isStream) ||
            !TryReadOptionalString(ref buffer, out var rawUri))
        {
            return false;
        }

        var uri = default(Uri?);
        if (rawUri is not null && !Uri.TryCreate(rawUri, UriKind.Absolute, out uri))
        {
            return false;
        }

        if (!TryReadString(ref buffer, out var sourceName))
        {
            return false;
        }

        var isProbingAudioTrack =
            sourceName.Equals("http", StringComparison.OrdinalIgnoreCase) ||
            sourceName.Equals("local", StringComparison.OrdinalIgnoreCase);

        var containerProbeInformation = default(string?);
        if (isProbingAudioTrack && !TryReadString(ref buffer, out containerProbeInformation))
        {
            return false;
        }

        if (!TryReadInt64(ref buffer, out var startPosition))
        {
            return false;
        }

        result = new LavalinkTrack
        {
            Author = author,
            Identifier = identifier,
            Title = title,
            Duration = TimeSpan.FromMilliseconds(duration),
            IsLiveStream = isStream,
            IsSeekable = !isStream,
            ProbeInfo = containerProbeInformation,
            SourceName = sourceName,
            StartPosition = startPosition is 0 ? null : TimeSpan.FromMilliseconds(startPosition),
            Uri = uri,
        };

        return true;
    }

    private static bool TryReadHeader(ref ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length is < 4)
        {
            return false;
        }

        // the header is four bytes long, subtract
        var header = BinaryPrimitives.ReadUInt32BigEndian(buffer);
        buffer = buffer[4..];

        var flags = (int)((header & 0xC0000000L) >> 30);
        var hasVersion = (flags & 1) is not 0;

        // verify size
        var size = header & 0x3FFFFFFF;

        if (size != buffer.Length)
        {
            // Invalid following payload length
            return false;
        }

        var version = 1;
        if (hasVersion)
        {
            if (buffer.IsEmpty)
            {
                // Missing version
                return false;
            }

            version = buffer[0];
            buffer = buffer[1..];
        }

        // verify version
        if (version is not 2)
        {
            return false;
        }

        return true;
    }

    private static bool TryReadBoolean(ref ReadOnlySpan<byte> buffer, out bool value)
    {
        if (buffer.IsEmpty)
        {
            value = default;
            return false;
        }

        value = buffer[0] is not 0;
        buffer = buffer[1..];
        return true;
    }

    private static bool TryReadInt64(ref ReadOnlySpan<byte> buffer, out long value)
    {
        if (buffer.Length < 8)
        {
            value = default;
            return false;
        }

        value = BinaryPrimitives.ReadInt64BigEndian(buffer);
        buffer = buffer[8..];
        return true;
    }

    private static bool TryReadOptionalString(ref ReadOnlySpan<byte> buffer, out string? value)
    {
        if (!TryReadBoolean(ref buffer, out var isPresent))
        {
            value = default;
            return false;
        }

        if (!isPresent)
        {
            value = null;
            return true;
        }

        return TryReadString(ref buffer, out value);
    }

    private static bool TryReadString(ref ReadOnlySpan<byte> buffer, [MaybeNullWhen(false)] out string value)
    {
        if (buffer.Length < 2)
        {
            value = default;
            return false;
        }

        var length = BinaryPrimitives.ReadUInt16BigEndian(buffer);
        buffer = buffer[2..];

        if (buffer.Length < length)
        {
            value = default;
            return false;
        }

        var stringBuffer = buffer[..length];
        buffer = buffer[length..];

        value = Encoding.UTF8.GetString(stringBuffer);
        return true;
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out LavalinkTrack result)
    {
        return TryParse(s is null ? default : s.AsSpan(), provider, out result);
    }
}
