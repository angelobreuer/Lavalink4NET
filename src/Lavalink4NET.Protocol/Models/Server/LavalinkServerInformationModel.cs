namespace Lavalink4NET.Protocol.Tests.Models;

using System.Collections.Immutable;
using System.Text.Json.Serialization;

public sealed record class LavalinkServerInformationModel(
    [property: JsonRequired]
    [property: JsonPropertyName("version")]
    LavalinkServerVersionModel Version,

    [property: JsonRequired]
    [property: JsonPropertyName("buildTime")]
    [property: JsonConverter(typeof(UnixTimestampJsonConverter))]
    DateTimeOffset BuiltAt,

    [property: JsonRequired]
    [property: JsonPropertyName("git")]
    LavalinkCommitInformationModel CommitInformation,

    [property: JsonRequired]
    [property: JsonPropertyName("jvm")]
    string JavaVersion,

    [property: JsonRequired]
    [property: JsonPropertyName("lavaplayer")]
    string LavaplayerVersion,

    [property: JsonRequired]
    [property: JsonPropertyName("sourceManagers")]
    ImmutableArray<string> AvailableSourceManagers,

    [property: JsonRequired]
    [property: JsonPropertyName("filters")]
    ImmutableArray<string> AvailableFilters,

    [property: JsonRequired]
    [property: JsonPropertyName("plugins")]
    ImmutableArray<LavalinkPluginInformationModel> Plugins);