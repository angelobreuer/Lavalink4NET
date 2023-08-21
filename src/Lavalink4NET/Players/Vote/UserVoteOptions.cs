namespace Lavalink4NET.Players.Vote;

/// <summary>
///     Structure used to pass additional information to user votes.
/// </summary>
/// <param name="Factor">
///     The factor of the value of the vote. The default is 1.0F.
///     This option can be used to give certain users more voting power.
///     1.0F equals to a single user. For example, in the case you have
///     users with a super-vote feature, you can set this value to 2.0F
///     to give them twice the voting power.
/// </param>
public readonly record struct UserVoteOptions(float? Factor = null);