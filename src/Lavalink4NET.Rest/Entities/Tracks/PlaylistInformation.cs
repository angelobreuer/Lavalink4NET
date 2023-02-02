namespace Lavalink4NET.Rest.Entities.Tracks;

using Lavalink4NET.Tracks;

public readonly record struct PlaylistInformation(string Name, LavalinkTrack? SelectedTrack);