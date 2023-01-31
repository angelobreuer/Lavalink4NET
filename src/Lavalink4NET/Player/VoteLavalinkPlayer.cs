namespace Lavalink4NET.Player;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lavalink4NET.Events;

/// <summary>
///     A lavalink player with a queuing and voting system.
/// </summary>
public class VoteLavalinkPlayer : QueuedLavalinkPlayer
{
    private readonly IList<ulong> _skipVotes;

    /// <summary>
    ///     Initializes a new instance of the <see cref="VoteLavalinkPlayer"/> class.
    /// </summary>
    public VoteLavalinkPlayer() => _skipVotes = new List<ulong>();

    /// <summary>
    ///     Clears all user votes.
    /// </summary>
    public virtual void ClearVotes() => _skipVotes.Clear();

    /// <summary>
    ///     Gets the player skip vote info.
    /// </summary>
    /// <returns>the vote info</returns>
    /// <exception cref="InvalidOperationException">thrown if the player is destroyed</exception>
    public async Task<VoteSkipInfo> GetVoteInfoAsync()
    {
        EnsureNotDestroyed();

        if (VoiceChannelId is null)
        {
            return new VoteSkipInfo(Array.Empty<ulong>(), 0);
        }

        // get users in channel without the bot
        var users = (await Client.GetChannelUsersAsync(GuildId, VoiceChannelId.Value))
            .Where(s => s != Client.CurrentUserId);

        var votes = _skipVotes.Intersect(users).ToArray();
        return new VoteSkipInfo(votes, users.Count());
    }

    /// <summary>
    ///     Asynchronously triggered when a track ends.
    /// </summary>
    /// <param name="eventArgs">the track event arguments</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    public override Task OnTrackEndAsync(TrackEndEventArgs eventArgs)
    {
        ClearVotes();
        return base.OnTrackEndAsync(eventArgs);
    }

    /// <summary>
    ///     Submits an user vote asynchronously.
    /// </summary>
    /// <param name="userId">the user snowflake identifier</param>
    /// <param name="percentage">the minimum voting percentage to skip the track</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    /// <exception cref="InvalidOperationException">thrown if the player is destroyed</exception>
    public virtual async Task<UserVoteSkipInfo> VoteAsync(ulong userId, float percentage = .5f)
    {
        EnsureNotDestroyed();

        var info = await GetVoteInfoAsync();

        if (info.Votes.Contains(userId))
        {
            return new UserVoteSkipInfo(info, false, false);
        }

        // add vote and re-get info, because the votes were changed.
        _skipVotes.Add(userId);
        info = await GetVoteInfoAsync();

        if (info.Percentage >= percentage)
        {
            ClearVotes();
            await SkipAsync();

            return new UserVoteSkipInfo(info, true, true);
        }

        return new UserVoteSkipInfo(info, false, true);
    }
}
