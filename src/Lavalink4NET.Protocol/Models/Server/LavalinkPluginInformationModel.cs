namespace Lavalink4NET.Protocol.Tests.Models;

using System.Text.Json.Serialization;

public sealed record class LavalinkPluginInformationModel(
    [property: JsonRequired]
    [property: JsonPropertyName("name")]
    string Name,

    [property: JsonRequired]
    [property: JsonPropertyName("version")]
    string Version);
