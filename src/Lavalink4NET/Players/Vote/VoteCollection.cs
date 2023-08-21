namespace Lavalink4NET.Players.Vote;

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

internal sealed class VoteCollection : IVoteCollection
{
    private readonly ConcurrentDictionary<ulong, UserVote> _userIds;

    public VoteCollection()
    {
        _userIds = new ConcurrentDictionary<ulong, UserVote>();
    }

    public int Count => _userIds.Count;

    public bool TryAdd(UserVote vote)
    {
        return _userIds.TryAdd(vote.UserId, vote);
    }

    public bool TryRemove(ulong userId)
    {
        return _userIds.TryRemove(userId, out _);
    }

    public void Clear()
    {
        _userIds.Clear();
    }

    ValueTask<bool> IVoteCollection.TryAddAsync(UserVote vote, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return new ValueTask<bool>(TryAdd(vote));
    }

    ValueTask<bool> IVoteCollection.TryRemoveAsync(ulong userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return new ValueTask<bool>(TryRemove(userId));
    }

    ValueTask IVoteCollection.ClearAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Clear();
        return default;
    }

    public IEnumerator<UserVote> GetEnumerator() => _userIds.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}