namespace Lavalink4NET.Players.Vote;

using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Players.Queued;

public interface IVoteLavalinkPlayer : IQueuedLavalinkPlayer
{
    IVoteCollection Votes { get; }

    ValueTask<VoteSkipInformation> GetVotesAsync(CancellationToken cancellationToken = default);

    ValueTask<UserVoteResult> VoteAsync(ulong userId, UserVoteOptions options, CancellationToken cancellationToken = default);
}