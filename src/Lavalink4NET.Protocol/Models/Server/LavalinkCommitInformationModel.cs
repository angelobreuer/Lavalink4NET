namespace Lavalink4NET.Protocol.Models.Server;

using System.Text.Json.Serialization;

public sealed record class LavalinkCommitInformationModel(
    [property: JsonRequired]
    [property: JsonPropertyName("branch")]
    string BranchName,

    [property: JsonRequired]
    [property: JsonPropertyName("commit")]
    string CommitId,

    [property: JsonRequired]
    [property: JsonPropertyName("commitTime")]
    [property: JsonConverter(typeof(UnixTimestampJsonConverter))]
    DateTimeOffset CommittedAt);
