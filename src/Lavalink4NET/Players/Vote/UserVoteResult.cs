namespace Lavalink4NET.Players.Vote;

public enum UserVoteResult : byte
{
    Submitted,
    Skipped,
    AlreadySubmitted,
    UserNotInChannel,
}
