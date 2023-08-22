namespace Lavalink4NET.Integrations.Lavasearch;

using System.Collections.Immutable;
using System.Text.Json;

public readonly record struct TextResult(
	string Text,
	IImmutableDictionary<string, JsonElement> AdditionalInformation);