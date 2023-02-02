namespace Lavalink4NET.Protocol.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Models;

internal sealed class LoadResultTypeJsonConverter : JsonConverter<LoadResultType>
{
    public override LoadResultType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(typeToConvert);
        ArgumentNullException.ThrowIfNull(options);

        var value = reader.GetString()!;

        return value.ToUpperInvariant() switch
        {
            "TRACK_LOADED" => LoadResultType.TrackLoaded,
            "PLAYLIST_LOADED" => LoadResultType.PlaylistLoaded,
            "SEARCH_RESULT" => LoadResultType.SearchResult,
            "NO_MATCHES" => LoadResultType.NoMatches,
            "LOAD_FAILED" => LoadResultType.LoadFailed,
            _ => throw new JsonException("Invalid load result type."),
        };
    }

    public override void Write(Utf8JsonWriter writer, LoadResultType value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(options);

        var strValue = value switch
        {
            LoadResultType.TrackLoaded => "TRACK_LOADED",
            LoadResultType.PlaylistLoaded => "PLAYLIST_LOADED",
            LoadResultType.SearchResult => "SEARCH_RESULT",
            LoadResultType.NoMatches => "NO_MATCHES",
            LoadResultType.LoadFailed => "LOAD_FAILED",
            _ => throw new ArgumentException("Invalid load result type."),
        };

        writer.WriteStringValue(strValue);
    }
}
