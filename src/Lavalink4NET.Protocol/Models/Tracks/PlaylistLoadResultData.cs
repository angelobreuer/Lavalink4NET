namespace Lavalink4NET.Protocol.Responses;

using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Models;

public sealed record class PlaylistLoadResultData(
    [property: JsonRequired]
    [property: JsonPropertyName("info")]
    PlaylistInformationModel PlaylistInformation,

    [property: JsonRequired]
    [property: JsonPropertyName("pluginInfo")]
    IImmutableDictionary<string, JsonElement> AdditionalInformation,

    [property: JsonRequired]
    [property: JsonPropertyName("tracks")]
    ImmutableArray<TrackModel> Tracks) : ILoadResultData;

