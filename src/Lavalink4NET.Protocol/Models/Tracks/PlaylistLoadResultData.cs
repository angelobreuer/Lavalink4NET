namespace Lavalink4NET.Protocol.Responses;

using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Models;

public sealed record class PlaylistLoadResultData(
    [property: JsonRequired]
    [property: JsonPropertyName("playlistInfo")]
    PlaylistInformationModel PlaylistInformation,

    [property: JsonPropertyName("pluginInfo")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    IImmutableDictionary<string, object?>? PluginInformation,

    [property: JsonRequired]
    [property: JsonPropertyName("tracks")]
    ImmutableArray<TrackModel> Tracks) : ILoadResultData;

