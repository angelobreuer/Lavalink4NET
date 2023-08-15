namespace Lavalink4NET.Players;

using Lavalink4NET.Rest.Entities.Tracks;

public record class LavalinkPlayerOptions
{
    public bool DisconnectOnStop { get; set; }

    public bool DestroyPlayerOnDisconnect { get; set; } = true;

    public string? Label { get; set; }

    public TrackReference? InitialTrack { get; set; }

    public TrackLoadOptions InitialLoadOptions { get; set; }

    public float? InitialVolume { get; set; }

    public bool SelfDeaf { get; set; }

    public bool SelfMute { get; set; }
}
