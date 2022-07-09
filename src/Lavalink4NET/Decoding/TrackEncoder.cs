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
        if (trackInfo is null)
        {
            throw new ArgumentNullException(nameof(trackInfo));
        }

        if (trackInfo.SourceName is null)
        {
            throw new InvalidOperationException("When encoding a track a source name must be given.");
        }

        var isProbingAudioTrack =
            trackInfo.SourceName.Equals("http", StringComparison.OrdinalIgnoreCase) ||
            trackInfo.SourceName.Equals("local", StringComparison.OrdinalIgnoreCase);

        if (isProbingAudioTrack && trackInfo.ProbeInfo is null)
        {
            throw new InvalidOperationException("For the HTTP and local source audio manager, a probe info must be given.");
        }

        using var pooledBufferWriter = new PooledBufferWriter();

        // reserve 5 bytes for the header
        pooledBufferWriter.GetSpan(5);
        pooledBufferWriter.Advance(5);

        WriteString(pooledBufferWriter, trackInfo.Title);
        WriteString(pooledBufferWriter, trackInfo.Author);

        var lengthBuffer = pooledBufferWriter.GetSpan(8);
        BinaryPrimitives.WriteInt64BigEndian(lengthBuffer, (long)trackInfo.Duration.TotalMilliseconds);
        pooledBufferWriter.Advance(8);

        WriteString(pooledBufferWriter, trackInfo.TrackIdentifier);

        var streamAndUriPresentSpan = pooledBufferWriter.GetSpan(2);
        streamAndUriPresentSpan[0] = (byte)(trackInfo.IsLiveStream ? 1 : 0);
        streamAndUriPresentSpan[1] = (byte)(trackInfo.Uri is not null ? 1 : 0);
        pooledBufferWriter.Advance(2);

        if (trackInfo.Uri is not null)
        {
            WriteString(pooledBufferWriter, trackInfo.Uri.OriginalString);
        }

        WriteString(pooledBufferWriter, trackInfo.SourceName);

        if (isProbingAudioTrack)
        {
            WriteString(pooledBufferWriter, trackInfo.ProbeInfo!);
        }

        var positionSpan = pooledBufferWriter.GetSpan(8);
        BinaryPrimitives.WriteInt64BigEndian(positionSpan, (long)trackInfo.Position.TotalMilliseconds);
        pooledBufferWriter.Advance(8);

        var segment = pooledBufferWriter.WrittenSegment;

        // subtract 4 for the header (version does also count to length)
        EncodeHeader(segment.AsSpan(0, 5), pooledBufferWriter.WrittenCount - 4);

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

        BinaryPrimitives.WriteUInt16BigEndian(span, (ushort)bytesWritten);

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
        BinaryPrimitives.WriteInt32BigEndian(buffer, header);

        // version
        buffer[4] = 2;
    }
}
