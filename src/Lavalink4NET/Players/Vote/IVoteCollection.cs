namespace Lavalink4NET.Players.Vote;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public interface IVoteCollection : IEnumerable<UserVote>
{
    int Count { get; }

    ValueTask<bool> TryAddAsync(UserVote vote, CancellationToken cancellationToken = default);

    ValueTask<bool> TryRemoveAsync(ulong userId, CancellationToken cancellationToken = default);

    ValueTask ClearAsync(CancellationToken cancellationToken = default);
}
