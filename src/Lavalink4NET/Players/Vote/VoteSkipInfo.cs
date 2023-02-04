namespace Lavalink4NET.Players.Vote;

using System.Collections.Immutable;

/// <summary>
///     Contains information about the current vote information of the player.
/// </summary>
/// <param name="votes">
///     a collection of the snowflake identifier values of the users that voted for skipping
///     the current track
/// </param>
/// <param name="totalUsers">
///     the total number of users in the voice channel (the bot is excluded)
/// </param>
public record class VoteSkipInfo(ImmutableArray<ulong> Votes, int TotalUsers)
{
    /// <summary>
    ///     Gets the vote percentage in range of 0 - 1f.
    /// </summary>
    public float Percentage => Votes.Length / (float)TotalUsers;
}
