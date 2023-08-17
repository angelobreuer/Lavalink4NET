namespace Lavalink4NET.Tracks;

using System;
using System.Buffers;
using System.Buffers.Text;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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

    internal static bool TryParse(ReadOnlySpan<char> originalTrackData, ref LavalinkTrackDecoder trackDecoder, [MaybeNullWhen(false)] out LavalinkTrack result)
    {
        result = null;

        if (!trackDecoder.TryReadHeader(out var version) ||
            !trackDecoder.TryReadString(out var title) ||
            !trackDecoder.TryReadString(out var author) ||
            !trackDecoder.TryReadInt64(out var durationValue) ||
            !trackDecoder.TryReadString(out var identifier) ||
            !trackDecoder.TryReadBoolean(out var isStream) ||
            !trackDecoder.TryReadOptionalString(out var rawUri))
        {
            return false;
        }

        var rawArtworkUri = default(string?);
        var isrc = default(string?);

        if (version >= 3 && (!trackDecoder.TryReadOptionalString(out rawArtworkUri) || !trackDecoder.TryReadOptionalString(out isrc)))
        {
            return false;
        }

        var uri = default(Uri?);
        if (rawUri is not null && !Uri.TryCreate(rawUri, UriKind.Absolute, out uri))
        {
            return false;
        }

        var artworkUri = default(Uri?);
        if (rawArtworkUri is not null && !Uri.TryCreate(rawArtworkUri, UriKind.Absolute, out artworkUri))
        {
            return false;
        }

        if (!trackDecoder.TryReadString(out var sourceName))
        {
            return false;
        }

        var isProbingAudioTrack =
            sourceName.Equals("http", StringComparison.OrdinalIgnoreCase) ||
            sourceName.Equals("local", StringComparison.OrdinalIgnoreCase);

        var containerProbeInformation = default(string?);
        if (isProbingAudioTrack && !trackDecoder.TryReadString(out containerProbeInformation))
        {
            return false;
        }

        if (!trackDecoder.TryReadInt64(out var startPositionValue))
        {
            return false;
        }

        var startPosition = startPositionValue is 0
            ? default(TimeSpan?)
            : TimeSpan.FromMilliseconds(startPositionValue);

        var duration = durationValue == long.MaxValue
            ? TimeSpan.MaxValue
            : TimeSpan.FromMilliseconds(durationValue);

        result = new LavalinkTrack
        {
            Author = author,
            Identifier = identifier,
            Title = title,
            Duration = duration,
            IsLiveStream = isStream,
            IsSeekable = !isStream,
            ProbeInfo = containerProbeInformation,
            SourceName = sourceName,
            StartPosition = startPosition,
            Uri = uri,
            ArtworkUri = artworkUri,
            Isrc = isrc,
            TrackData = originalTrackData.ToString(),
        };

        return true;
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

            return TryParse(s, pool.AsSpan(0, decodedBytesWritten), out result);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(pool);
        }
    }

    internal static bool TryParse(ReadOnlySpan<char> originalTrackData, ReadOnlySpan<byte> buffer, [MaybeNullWhen(false)] out LavalinkTrack result)
    {
        var trackDecoder = new LavalinkTrackDecoder(buffer);
        return TryParse(originalTrackData, ref trackDecoder, out result);
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out LavalinkTrack result)
    {
        return TryParse(s is null ? default : s.AsSpan(), provider, out result);
    }
}
