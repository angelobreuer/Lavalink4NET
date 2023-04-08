namespace Lavalink4NET.Protocol.Models.Server;

using System.Text.Json.Serialization;

public sealed record class LavalinkPluginInformationModel(
    [property: JsonRequired]
    [property: JsonPropertyName("name")]
    string Name,

    [property: JsonRequired]
    [property: JsonPropertyName("version")]
    string Version);
