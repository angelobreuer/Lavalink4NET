namespace Lavalink4NET.Protocol.Models;

using System.Text.Json.Serialization;
using Lavalink4NET.Protocol.Converters;

public sealed record class PlaylistInformationModel(
    [property: JsonRequired]
    [property: JsonPropertyName("name")]
    string Name,

    [property: JsonRequired]
    [property: JsonPropertyName("selectedTrack")]
    [property: JsonConverter(typeof(NullableInt32JsonConverter))]
    int? SelectedTrack);
