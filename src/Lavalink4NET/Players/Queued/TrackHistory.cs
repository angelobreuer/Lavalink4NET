namespace Lavalink4NET.Players.Queued;

using System.Collections.Generic;
using System.Linq;

public sealed class TrackHistory : TrackCollection, ITrackHistory
{
    public TrackHistory(int? capacity)
    {
        Capacity = capacity;
    }

    public int? Capacity { get; }

    public override int Add(ITrackQueueItem item)
    {
        int index;
        lock (SyncRoot)
        {
            if (Capacity is not null)
            {
                var capacity = Capacity.Value;

                if (Items.Count >= capacity)
                {
                    Items = Items.RemoveAt(0);
                }
            }

            Items = Items.Add(item);
            index = Items.Count;
        }

        return index;
    }

    public override int AddRange(IReadOnlyList<ITrackQueueItem> items)
    {
        int index;
        lock (SyncRoot)
        {
            if (Capacity is not null)
            {
                var capacity = Capacity.Value;

                if (items.Count > capacity)
                {
                    items = items.Skip(items.Count - capacity).ToList();
                }

                if (Items.Count + items.Count >= capacity)
                {
                    var removeCount = Items.Count + items.Count - capacity;
                    Items = Items.RemoveRange(0, removeCount);
                }
            }

            Items = Items.AddRange(items);
            index = Items.Count;
        }

        return index;
    }
}
