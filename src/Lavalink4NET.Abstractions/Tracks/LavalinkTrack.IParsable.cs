namespace Lavalink4NET.Tracks;

using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;
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

        var containerProbeInformation = default(string?);

        if (IsProbingTrack(sourceName) && !trackDecoder.TryReadString(out containerProbeInformation))
        {
            return false;
        }

        var additionalInformationBuilder = ImmutableDictionary.CreateBuilder<string, JsonElement>();

        if (IsExtendedTrack(sourceName))
        {
            if (!trackDecoder.TryReadOptionalString(out var albumName) ||
                !trackDecoder.TryReadOptionalString(out var rawAlbumUri) ||
                !trackDecoder.TryReadOptionalString(out var rawArtistUri) ||
                !trackDecoder.TryReadOptionalString(out var rawArtistArtworkUri) ||
                !trackDecoder.TryReadOptionalString(out var rawPreviewUri) ||
                !trackDecoder.TryReadBoolean(out var isPreview))
            {
                return false;
            }

            var data = new JsonObject
            {
                {"albumName", albumName },
                {"albumUrl", rawAlbumUri },
                {"artistUrl", rawArtistUri },
                {"artistArtworkUrl", rawArtistArtworkUri },
                {"previewUrl", rawPreviewUri },
                {"isPreview", isPreview },
            };

            var bufferWriter = new ArrayBufferWriter<byte>();
            using var utf8JsonWriter = new Utf8JsonWriter(bufferWriter);
            data.WriteTo(utf8JsonWriter);
            utf8JsonWriter.Dispose();

            var utf8JsonReader = new Utf8JsonReader(bufferWriter.WrittenSpan);
            var jsonDocument = JsonElement.ParseValue(ref utf8JsonReader);

            additionalInformationBuilder.Add("albumName", jsonDocument.GetProperty("albumName"));
            additionalInformationBuilder.Add("albumUrl", jsonDocument.GetProperty("albumUrl"));
            additionalInformationBuilder.Add("artistUrl", jsonDocument.GetProperty("artistUrl"));
            additionalInformationBuilder.Add("artistArtworkUrl", jsonDocument.GetProperty("artistArtworkUrl"));
            additionalInformationBuilder.Add("previewUrl", jsonDocument.GetProperty("previewUrl"));
            additionalInformationBuilder.Add("isPreview", jsonDocument.GetProperty("isPreview"));
        }

        if (!trackDecoder.TryReadInt64(out var startPositionValue))
        {
            return false;
        }

        var startPosition = startPositionValue is 0
            ? default(TimeSpan?)
            : TimeSpan.FromMilliseconds(startPositionValue);

        var duration = durationValue >= TimeSpan.MaxValue.TotalMilliseconds
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
            AdditionalInformation = additionalInformationBuilder.ToImmutable(),
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
