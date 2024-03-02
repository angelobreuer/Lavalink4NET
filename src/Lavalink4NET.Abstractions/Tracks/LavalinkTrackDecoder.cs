namespace Lavalink4NET.Tracks;

using System;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;

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

        value = ReadModifiedUtf8(stringBuffer);
        return true;
    }

    private static string ReadModifiedUtf8(ReadOnlySpan<byte> value)
    {
        // Ported from https://android.googlesource.com/platform/prebuilts/fullsdk/sources/android-29/+/refs/heads/androidx-wear-release/java/io/DataInputStream.java

        Span<char> buffer = value.Length < 256
            ? stackalloc char[256]
            : GC.AllocateUninitializedArray<char>(value.Length * 2);

        var length = value.Length;
        var count = 0;
        var charactersWritten = 0;

        // Fast-read all ASCII characters
        while (!value.IsEmpty)
        {
            var character = value[0];

            if (character > 127)
            {
                break;
            }

            count++;
            value = value[1..];
            buffer[charactersWritten++] = (char)character;
        }

        while (!value.IsEmpty)
        {
            var character = value[0];

            switch (character >> 4)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                    // 0xxxxxxx
                    count++;
                    buffer[charactersWritten++] = (char)character;
                    value = value[1..];
                    break;

                case 12:
                case 13:
                    // 110x xxxx   10xx xxxx
                    count += 2;

                    if (count > length)
                    {
                        throw new InvalidDataException("Found partial character at end.");
                    }

                    var additionalCharacter = value[1];

                    if ((additionalCharacter & 0xC0) != 0x80)
                    {
                        throw new InvalidDataException($"malformed input around byte {count}");
                    }

                    buffer[charactersWritten++] = (char)(((character & 0x1F) << 6) | (additionalCharacter & 0x3F));
                    value = value[2..];

                    break;

                case 14:
                    // 1110 xxxx  10xx xxxx  10xx xxxx
                    count += 3;

                    if (count > length)
                    {
                        throw new InvalidDataException("Found malformed input due to partial character at end");
                    }

                    var secondCharacter = (int)value[1];
                    var thirdCharacter = (int)value[2];

                    if (((secondCharacter & 0xC0) != 0x80) || ((thirdCharacter & 0xC0) != 0x80))
                    {
                        throw new InvalidDataException($"Found malformed input around byte {count - 1}");
                    }

                    buffer[charactersWritten++] = (char)(((character & 0x0F) << 12) | ((secondCharacter & 0x3F) << 6) | ((thirdCharacter & 0x3F) << 0));
                    value = value[3..];

                    break;

                default:
                    // 10xx xxxx,  1111 xxxx
                    throw new InvalidDataException($"Found malformed input around byte {count}");
            }
        }

        return buffer[..charactersWritten].ToString();
    }
}
