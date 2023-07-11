namespace Lavalink4NET.Players.Vote;

using System.Collections.Immutable;

public readonly record struct VoteSkipInformation(ImmutableArray<UserVote> Votes, int TotalUsers, float Percentage)
{
    public bool ShouldSkip(double threshold) => Percentage >= threshold;
}