namespace Lavalink4NET.Decoding;

using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Text;
using Lavalink4NET.Payloads;
using Lavalink4NET.Player;

public static class TrackEncoder
{
    public static string Encode(LavalinkTrackInfo trackInfo)
    {
        using var pooledBufferWriter = new PooledBufferWriter();

        // reserve 5 bytes for the header
        pooledBufferWriter.GetSpan(5);
        pooledBufferWriter.Advance(5);

        WriteString(pooledBufferWriter, trackInfo.Title);
        WriteString(pooledBufferWriter, trackInfo.Author);

        var lengthBuffer = pooledBufferWriter.GetSpan(8);
        BinaryPrimitives.WriteInt64LittleEndian(lengthBuffer, (long)trackInfo.Duration.TotalMilliseconds);
        pooledBufferWriter.Advance(8);

        WriteString(pooledBufferWriter, trackInfo.TrackIdentifier);

        var streamAndUriPresentSpan = pooledBufferWriter.GetSpan(2);
        streamAndUriPresentSpan[0] = (byte)(trackInfo.IsLiveStream ? 1 : 0);
        streamAndUriPresentSpan[1] = (byte)(trackInfo.Source is not null ? 1 : 0);
        pooledBufferWriter.Advance(2);

        if (trackInfo.Source is not null)
        {
            WriteString(pooledBufferWriter, trackInfo.Source);
        }

        var segment = pooledBufferWriter.WrittenSegment;

        // subtract 5 for the header
        EncodeHeader(segment.AsSpan(0, 5), pooledBufferWriter.WrittenCount - 5);

        return Convert.ToBase64String(segment.Array, segment.Offset, segment.Count);
    }

    private static void WriteString(IBufferWriter<byte> bufferWriter, string value)
    {
        var maximumEncodedStringLength = Encoding.UTF8.GetMaxByteCount(value.Length);
        var maximumEncodedLength = maximumEncodedStringLength + 2;

        var span = bufferWriter.GetSpan(maximumEncodedLength);
        var stringSpan = span.Slice(2);

#if NETSTANDARD2_0
        var buffer = Encoding.UTF8.GetBytes(value);
        buffer.AsSpan().CopyTo(stringSpan);
        var bytesWritten = buffer.Length;
#else
        var bytesWritten = Encoding.UTF8.GetBytes(value, stringSpan);
#endif

        BinaryPrimitives.WriteUInt16LittleEndian(span, (ushort)bytesWritten);

        bufferWriter.Advance(bytesWritten + 2);
    }

    private static void EncodeHeader(Span<byte> buffer, int length)
    {
        if (length > 0x3FFFFFFF)
        {
            throw new InvalidOperationException("The track identifier content is too long to serialize.");
        }

        // Set "has version" in header
        var header = 0b01000000000000000000000000000000 | length;
        BinaryPrimitives.WriteInt32LittleEndian(buffer, header);

        // version
        buffer[4] = 2;
    }
}
