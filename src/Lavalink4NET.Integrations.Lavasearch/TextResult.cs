namespace Lavalink4NET.Integrations.Lavasearch;

using System.Collections.Immutable;
using System.Text.Json.Nodes;

public readonly record struct TextResult(
    string Text,
    IImmutableDictionary<string, JsonNode> AdditionalInformation);