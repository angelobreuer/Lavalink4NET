namespace Lavalink4NET.Players.Vote;

using System.Collections.Immutable;

/// <summary>
///     Contains information about the current vote information and the submitted user vote.
/// </summary>
public sealed record class UserVoteSkipInfo(ImmutableArray<ulong> Votes, int TotalUsers, bool WasSkipped, bool WasAdded) : VoteSkipInfo(Votes, TotalUsers);
