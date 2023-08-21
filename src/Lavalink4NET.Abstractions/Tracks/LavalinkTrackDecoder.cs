namespace Lavalink4NET.Tracks;

using System;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Text;

internal ref struct LavalinkTrackDecoder
{
    public LavalinkTrackDecoder(ReadOnlySpan<byte> buffer)
    {
        Buffer = buffer;
    }

    public ReadOnlySpan<byte> Buffer { get; set; }

    public bool TryReadHeader(out int version)
    {
        version = 1;

        if (Buffer.Length is < 4)
        {
            return false;
        }

        // the header is four bytes long, subtract
        var header = BinaryPrimitives.ReadUInt32BigEndian(Buffer);
        Buffer = Buffer[4..];

        var flags = (int)((header & 0xC0000000L) >> 30);
        var hasVersion = (flags & 1) is not 0;

        // verify size
        var size = header & 0x3FFFFFFF;

        if (size != Buffer.Length)
        {
            // Invalid following payload length
            return false;
        }

        if (hasVersion)
        {
            if (Buffer.IsEmpty)
            {
                // Missing version
                return false;
            }

            version = Buffer[0];
            Buffer = Buffer[1..];
        }

        // verify version
        if (version is not 2 and not 3)
        {
            // unsupported version
            return false;
        }

        return true;
    }

    public bool TryReadBoolean(out bool value)
    {
        if (Buffer.IsEmpty)
        {
            value = default;
            return false;
        }

        value = Buffer[0] is not 0;
        Buffer = Buffer[1..];
        return true;
    }

    public bool TryReadInt64(out long value)
    {
        if (Buffer.Length < 8)
        {
            value = default;
            return false;
        }

        value = BinaryPrimitives.ReadInt64BigEndian(Buffer);
        Buffer = Buffer[8..];
        return true;
    }

    public bool TryReadOptionalString(out string? value)
    {
        if (!TryReadBoolean(out var isPresent))
        {
            value = default;
            return false;
        }

        if (!isPresent)
        {
            value = null;
            return true;
        }

        return TryReadString(out value);
    }

    public bool TryReadString([MaybeNullWhen(false)] out string value)
    {
        if (Buffer.Length < 2)
        {
            value = default;
            return false;
        }

        var length = BinaryPrimitives.ReadUInt16BigEndian(Buffer);
        Buffer = Buffer[2..];

        if (Buffer.Length < length)
        {
            value = default;
            return false;
        }

        var stringBuffer = Buffer[..length];
        Buffer = Buffer[length..];

        value = Encoding.UTF8.GetString(stringBuffer);
        return true;
    }
}
