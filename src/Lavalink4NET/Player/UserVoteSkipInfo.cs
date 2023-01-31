namespace Lavalink4NET.Player;

using System.Collections.Generic;

/// <summary>
///     Contains information about the current vote information and the submitted user vote.
/// </summary>
public sealed class UserVoteSkipInfo : VoteSkipInfo
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UserVoteSkipInfo"/> class.
    /// </summary>
    /// <param name="info">the <see cref="VoteSkipInfo"/> to clone</param>
    /// <param name="wasSkipped">
    ///     a value indicating whether the user vote submit caused that the current playing track
    ///     was skipped
    /// </param>
    /// <param name="wasAdded">
    ///     a value indicating whether the user vote submit was added to the vote list
    /// </param>
    public UserVoteSkipInfo(VoteSkipInfo info, bool wasSkipped, bool wasAdded)
        : this(info.Votes, info.TotalUsers, wasSkipped, wasAdded)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="UserVoteSkipInfo"/> class.
    /// </summary>
    /// <param name="votes">
    ///     a collection of the snowflake identifier values of the users that voted for skipping
    ///     the current track
    /// </param>
    /// <param name="totalUsers">
    ///     the total number of users in the voice channel (the bot is excluded)
    /// </param>
    /// <param name="wasSkipped">
    ///     a value indicating whether the user vote submit caused that the current playing track
    ///     was skipped
    /// </param>
    /// <param name="wasAdded">
    ///     a value indicating whether the user vote submit was added to the vote list
    /// </param>
    public UserVoteSkipInfo(IReadOnlyCollection<ulong> votes, int totalUsers, bool wasSkipped, bool wasAdded)
        : base(votes, totalUsers)
    {
        WasSkipped = wasSkipped;
        WasAdded = wasAdded;
    }

    /// <summary>
    ///     Gets a value indicating whether the user vote submit caused that the current playing
    ///     track was skipped.
    /// </summary>
    public bool WasSkipped { get; }

    /// <summary>
    ///     Gets a value indicating whether the user vote submit was added to the vote list.
    /// </summary>
    public bool WasAdded { get; }
}
