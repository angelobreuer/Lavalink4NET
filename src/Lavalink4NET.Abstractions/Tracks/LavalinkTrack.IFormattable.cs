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
        return ToString(format: null, formatProvider: null);
    }

    public string ToString(string? format, IFormatProvider? formatProvider = null)
    {
        Span<char> buffer = stackalloc char[256];

        int charsWritten;
        while (!TryFormat(buffer, out charsWritten, format ?? default, formatProvider))
        {
            buffer = GC.AllocateUninitializedArray<char>(buffer.Length * 2);
        }

        return new string(buffer[..charsWritten]);
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        var buffer = ArrayPool<byte>.Shared.Rent(destination.Length);

        try
        {
            var result = TryEncode(buffer, out var bytesWritten);

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

    private bool TryEncode(Span<byte> buffer, out int bytesWritten)
    {
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

        var span = buffer[..8];
        buffer = buffer[8..];
        bytesWritten += 8;

        BinaryPrimitives.WriteInt64BigEndian(
            destination: span,
            value: (long)Math.Round(Duration.TotalMilliseconds));

        // Write track identifier
        if (!TryEncodeString(ref buffer, Identifier, ref bytesWritten))
        {
            return false;
        }

        // Write stream and URI flags
        if (buffer.Length < 2)
        {
            return false;
        }

        span = buffer[..2];
        buffer = buffer[2..];
        bytesWritten += 2;

        span[0] = (byte)(IsLiveStream ? 1 : 0);

        // Write URI if possible
        if (Uri is not null)
        {
            span[1] = 1; // URI present

            if (!TryEncodeString(ref buffer, Uri.ToString(), ref bytesWritten))
            {
                return false;
            }
        }
        else
        {
            span[1] = 0; // URI not present
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

        span = buffer[..8];
        buffer = buffer[8..];
        bytesWritten += 8;

        BinaryPrimitives.WriteInt64BigEndian(
            destination: span,
            value: (long)Math.Round(StartPosition?.TotalMilliseconds ?? 0));

        var payloadLength = bytesWritten - 4;
        EncodeHeader(headerBuffer, payloadLength);

        return true;
    }

    private static void EncodeHeader(Span<byte> headerBuffer, int payloadLength)
    {
        // Set "has version" in header
        var header = 0b01000000000000000000000000000000 | payloadLength;
        BinaryPrimitives.WriteInt32BigEndian(headerBuffer, header);

        // version
        headerBuffer[4] = 2;
    }

    private static bool TryEncodeString(ref Span<byte> span, ReadOnlySpan<char> value, ref int bytesWritten)
    {
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

}
