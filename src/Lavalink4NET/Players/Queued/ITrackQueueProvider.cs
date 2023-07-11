namespace Lavalink4NET.Players.Queued;

internal interface ITrackQueueProvider
{
    int Count { get; }

    void Add(ITrackQueueItem item);

    void Clear();


}
