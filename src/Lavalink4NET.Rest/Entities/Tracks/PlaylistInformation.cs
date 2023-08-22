namespace Lavalink4NET.Rest.Entities.Tracks;

using System.Collections.Immutable;
using System.Text.Json;
using Lavalink4NET.Tracks;

public sealed record class PlaylistInformation(
	string Name,
	LavalinkTrack? SelectedTrack,
	IImmutableDictionary<string, JsonElement> AdditionalInformation);