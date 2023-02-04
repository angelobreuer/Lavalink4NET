namespace Lavalink4NET.Rest.Entities.Tracks;

public readonly record struct TrackSearchMode(string? Prefix)
{
    public static readonly TrackSearchMode YouTube = new("ytsearch");
    public static readonly TrackSearchMode YouTubeMusic = new("ytmsearch");
    public static readonly TrackSearchMode SoundCloud = new("scsearch");
    public static readonly TrackSearchMode None = new(null);
}
