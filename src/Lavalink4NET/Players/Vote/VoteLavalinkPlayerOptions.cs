namespace Lavalink4NET.Players.Vote;

using Lavalink4NET.Players.Queued;

public record class VoteLavalinkPlayerOptions : QueuedLavalinkPlayerOptions
{
    public bool ClearVotesAfterTrack { get; init; } = true;

    public bool RequireUserToBeInVoiceChannel { get; init; } = true;

    public IVoteCollection? Votes { get; init; } = null;

    public double SkipThreshold { get; init; } = 0.5D;
}
