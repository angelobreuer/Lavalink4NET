namespace Lavalink4NET.Integrations.Lavasrc;

using System.Collections.Immutable;
using System.Text.Json;
using Lavalink4NET.Rest.Entities.Tracks;

public readonly record struct ExtendedPlaylistInformation(PlaylistInformation Playlist)
{
	public string Name => Playlist.Name;

	public ExtendedLavalinkTrack? SelectedTrack
	{
		get
		{
			return Playlist.SelectedTrack is null
				? null
				: new ExtendedLavalinkTrack(Playlist.SelectedTrack);
		}
	}

	public IImmutableDictionary<string, JsonElement> AdditionalInformation => Playlist.AdditionalInformation;

	public PlaylistType? Type => !TryGetElement("type", out var typeElement) ? null : typeElement.GetString()! switch
	{
		"album" => PlaylistType.Album,
		"playlist" => PlaylistType.Playlist,
		"artist" => PlaylistType.Artist,
		"recommendations" => PlaylistType.Recommendations,
		_ => null,
	};

	public Uri? Uri
	{
		get
		{
			if (!TryGetElement("url", out var uriElement))
			{
				return null;
			}

			return new Uri(uriElement.GetString()!);
		}
	}

	public Uri? ArtworkUri
	{
		get
		{
			if (!TryGetElement("artworkUrl", out var artworkUriElement))
			{
				return null;
			}

			return new Uri(artworkUriElement.GetString()!);
		}
	}

	public string? Author => TryGetElement("author", out var authorElement) ? authorElement.GetString() : null;

	public int? TotalTracks => TryGetElement("totalTracks", out var totalTracksElement) ? totalTracksElement.GetInt32() : null;

	private bool TryGetElement(string key, out JsonElement element)
	{
		return Playlist.AdditionalInformation.TryGetValue(key, out element) && element.ValueKind is not JsonValueKind.Null;
	}
}
