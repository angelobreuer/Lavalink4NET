namespace Lavalink4NET.Players.Queued;

public interface ITrackHistory : ITrackCollection
{
    int? Capacity { get; }
}
