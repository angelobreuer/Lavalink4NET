namespace Lavalink4NET.Tracks;

using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Buffers.Text;
using System.Text.Unicode;

public partial record class LavalinkTrack : ISpanFormattable
{
    public override string ToString()
    {
        return ToString(version: null, format: null, formatProvider: null);
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return ToString(version: null, format: format, formatProvider: formatProvider);
    }

    public string ToString(int? version)
    {
        return ToString(version: version, format: null, formatProvider: null);
    }

    public string ToString(int? version, string? format, IFormatProvider? formatProvider)
    {
        // The ToString method is culture-neutral and format-neutral
        if (TrackData is not null && version is null)
        {
            return TrackData;
        }

        Span<char> buffer = stackalloc char[256];

        int charsWritten;
        while (!TryFormat(buffer, out charsWritten, version, format ?? default, formatProvider))
        {
            buffer = GC.AllocateUninitializedArray<char>(buffer.Length * 2);
        }

        return TrackData = new string(buffer[..charsWritten]);
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        return TryFormat(destination, out charsWritten, version: null, format, provider);
    }

#pragma warning disable IDE0060
    public bool TryFormat(Span<char> destination, out int charsWritten, int? version, ReadOnlySpan<char> format, IFormatProvider? provider)
#pragma warning restore IDE0060
    {
        var buffer = ArrayPool<byte>.Shared.Rent(destination.Length);

        try
        {
            var result = TryEncode(buffer, version, out var bytesWritten);

            if (!result)
            {
                charsWritten = default;
                return false;
            }

            var operationStatus = Base64.EncodeToUtf8InPlace(
                buffer: buffer,
                dataLength: bytesWritten,
                bytesWritten: out var base64BytesWritten);

            if (operationStatus is not OperationStatus.Done)
            {
                if (operationStatus is OperationStatus.DestinationTooSmall)
                {
                    charsWritten = default;
                    return false;
                }

                throw new InvalidOperationException("Error while encoding to Base64.");
            }

            operationStatus = Utf8.ToUtf16(
                source: buffer.AsSpan(0, base64BytesWritten),
                destination: destination,
                bytesRead: out _,
                charsWritten: out charsWritten);

            if (operationStatus is not OperationStatus.Done)
            {
                if (operationStatus is OperationStatus.DestinationTooSmall)
                {
                    charsWritten = default;
                    return false;
                }

                throw new InvalidOperationException("Error while encoding to UTF-8.");
            }

            return true;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    internal bool TryEncode(Span<byte> buffer, int? version, out int bytesWritten)
    {
        var versionValue = version ?? 3;

        if (versionValue is not 2 and not 3)
        {
            throw new ArgumentOutOfRangeException(nameof(version));
        }

        if (SourceName is null)
        {
            throw new InvalidOperationException("Unknown source.");
        }

        var isProbingAudioTrack =
            SourceName.Equals("http", StringComparison.OrdinalIgnoreCase) ||
            SourceName.Equals("local", StringComparison.OrdinalIgnoreCase);

        if (isProbingAudioTrack && ProbeInfo is null)
        {
            throw new InvalidOperationException("For the HTTP and local source audio manager, a probe info must be given.");
        }

        if (buffer.Length < 5)
        {
            bytesWritten = 0;
            return false;
        }

        // Reserve 5 bytes for the header
        var headerBuffer = buffer[..5];
        buffer = buffer[5..];
        bytesWritten = 5;

        // Write title and author
        if (!TryEncodeString(ref buffer, Title, ref bytesWritten) ||
            !TryEncodeString(ref buffer, Author, ref bytesWritten))
        {
            return false;
        }

        // Write track duration
        if (buffer.Length < 8)
        {
            return false;
        }

        var duration = Duration == TimeSpan.MaxValue
            ? long.MaxValue
            : (long)Math.Round(Duration.TotalMilliseconds);

        BinaryPrimitives.WriteInt64BigEndian(
            destination: buffer[..8],
            value: duration);

        buffer = buffer[8..];
        bytesWritten += 8;

        // Write track identifier
        if (!TryEncodeString(ref buffer, Identifier, ref bytesWritten))
        {
            return false;
        }

        // Write stream flag
        if (buffer.Length < 1)
        {
            return false;
        }

        buffer[0] = (byte)(IsLiveStream ? 1 : 0);

        bytesWritten++;
        buffer = buffer[1..];

        var rawUri = Uri is null ? string.Empty : Uri.ToString();

        if (!TryEncodeOptionalString(ref buffer, rawUri, ref bytesWritten))
        {
            return false;
        }

        if (versionValue >= 3)
        {
            var rawArtworkUri = ArtworkUri is null ? string.Empty : ArtworkUri.ToString();

            if (!TryEncodeOptionalString(ref buffer, rawArtworkUri, ref bytesWritten) ||
                !TryEncodeOptionalString(ref buffer, Isrc, ref bytesWritten))
            {
                return false;
            }
        }

        // Write source name
        if (!TryEncodeString(ref buffer, SourceName, ref bytesWritten))
        {
            return false;
        }

        // Write probe information
        if (isProbingAudioTrack && !TryEncodeString(ref buffer, ProbeInfo, ref bytesWritten))
        {
            return false;
        }

        // Write track start position
        if (buffer.Length < 8)
        {
            return false;
        }

        BinaryPrimitives.WriteInt64BigEndian(
            destination: buffer[..8],
            value: (long)Math.Round(StartPosition?.TotalMilliseconds ?? 0));

        // buffer = buffer[8..];
        bytesWritten += 8;

        var payloadLength = bytesWritten - 4;
        EncodeHeader(headerBuffer, payloadLength, (byte)versionValue);

        return true;
    }

    private static void EncodeHeader(Span<byte> headerBuffer, int payloadLength, byte version)
    {
        // Set "has version" in header
        var header = 0b01000000000000000000000000000000 | payloadLength;
        BinaryPrimitives.WriteInt32BigEndian(headerBuffer, header);

        // version
        headerBuffer[4] = version;
    }

    private static bool TryEncodeString(ref Span<byte> span, ReadOnlySpan<char> value, ref int bytesWritten)
    {
        if (span.Length < 2)
        {
            return false;
        }

        var lengthBuffer = span[..2];
        span = span[2..];

        var operationStatus = Utf8.FromUtf16(
            source: value,
            destination: span,
            charsRead: out _,
            bytesWritten: out var utf8BytesWritten);

        if (operationStatus is not OperationStatus.Done)
        {
            if (operationStatus is OperationStatus.DestinationTooSmall)
            {
                return false;
            }

            throw new InvalidOperationException("Invalid data for UTF-8.");
        }

        span = span[utf8BytesWritten..];
        bytesWritten += utf8BytesWritten + 2;

        BinaryPrimitives.WriteUInt16BigEndian(lengthBuffer, (ushort)utf8BytesWritten);
        return true;
    }

    private static bool TryEncodeOptionalString(ref Span<byte> span, ReadOnlySpan<char> value, ref int bytesWritten)
    {
        if (span.Length < 1)
        {
            return false;
        }

        var present = !value.IsWhiteSpace();

        span[0] = (byte)(present ? 1 : 0);
        span = span[1..];
        bytesWritten++;

        if (!present)
        {
            return true;
        }

        if (!TryEncodeString(ref span, value, ref bytesWritten))
        {
            return false;
        }

        return true;
    }
}
