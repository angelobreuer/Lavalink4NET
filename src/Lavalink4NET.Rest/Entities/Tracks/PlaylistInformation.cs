namespace Lavalink4NET.Rest.Entities.Tracks;

using System.Collections.Immutable;
using System.Text.Json.Nodes;
using Lavalink4NET.Tracks;

public readonly record struct PlaylistInformation(
    string Name,
    LavalinkTrack? SelectedTrack,
    IImmutableDictionary<string, JsonNode> AdditionalInformation);