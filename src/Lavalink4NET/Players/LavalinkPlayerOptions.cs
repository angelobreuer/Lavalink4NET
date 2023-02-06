namespace Lavalink4NET.Players;

public record class LavalinkPlayerOptions
{
    public bool DisconnectOnStop { get; set; }

    public string? Label { get; set; }

    public TrackReference? InitialTrack { get; set; }

    public float? InitialVolume { get; set; }

    public bool SelfDeaf { get; set; }

    public bool SelfMute { get; set; }
}
