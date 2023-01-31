namespace Lavalink4NET.Player;

using System;
using System.Collections.Generic;

/// <summary>
///     Contains information about the current vote information of the player.
/// </summary>
public class VoteSkipInfo
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="VoteSkipInfo"/> struct.
    /// </summary>
    /// <param name="votes">
    ///     a collection of the snowflake identifier values of the users that voted for skipping
    ///     the current track
    /// </param>
    /// <param name="totalUsers">
    ///     the total number of users in the voice channel (the bot is excluded)
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     the specified <paramref name="votes"/> can not be <see langword="null"/>.
    /// </exception>
    internal VoteSkipInfo(IReadOnlyCollection<ulong> votes, int totalUsers)
    {
        Votes = votes ?? throw new ArgumentNullException(nameof(votes));
        TotalUsers = totalUsers;
    }

    /// <summary>
    ///     Gets the vote percentage in range of 0 - 1f.
    /// </summary>
    public float Percentage => Votes.Count / (float)TotalUsers;

    /// <summary>
    ///     Gets a collection of the snowflake identifier values of the users that voted for
    ///     skipping the current track.
    /// </summary>
    public IReadOnlyCollection<ulong> Votes { get; }

    /// <summary>
    ///     Gets the total number of users in the voice channel (the bot is excluded).
    /// </summary>
    public int TotalUsers { get; }
}
