namespace Lavalink4NET.Protocol.Responses;

using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Models;
using Lavalink4NET.Protocol.Payloads;

public sealed record class TrackLoadResponse(
    [property: JsonRequired]
    [property: JsonPropertyName("loadType")]
    LoadResultType LoadType,

    [property: JsonPropertyName("playlistInfo")]
    PlaylistInformationModel? PlaylistInformation,

    [property: JsonPropertyName("tracks")]
    ImmutableArray<TrackModel> Tracks,

    [property: JsonPropertyName("exception")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    TrackExceptionModel? Exception);