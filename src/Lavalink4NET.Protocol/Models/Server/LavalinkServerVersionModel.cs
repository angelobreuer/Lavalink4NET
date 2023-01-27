namespace Lavalink4NET.Protocol.Tests.Models;

using System.Text.Json.Serialization;

public sealed record class LavalinkServerVersionModel(
    [property: JsonRequired]
    [property: JsonPropertyName("semver")] // TODO: typo in lavalink docs
    string SemVer,

    [property: JsonRequired]
    [property: JsonPropertyName("major")]
    int Major,

    [property: JsonRequired]
    [property: JsonPropertyName("minor")]
    int Minor,

    [property: JsonRequired]
    [property: JsonPropertyName("patch")]
    int Patch,

    [property: JsonRequired]
    [property: JsonPropertyName("preRelease")]
    string? PreRelease);