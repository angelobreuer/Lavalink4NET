namespace Lavalink4NET.Protocol.Models;

using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;

public sealed record class TrackModel(
	[property: JsonRequired]
	[property: JsonPropertyName("encoded")]
	string Data,

	[property:JsonRequired]
	[property:JsonPropertyName("info")]
	TrackInformationModel Information,

	[property: JsonRequired]
	[property: JsonPropertyName("pluginInfo")]
	IImmutableDictionary<string, JsonElement> AdditionalInformation);