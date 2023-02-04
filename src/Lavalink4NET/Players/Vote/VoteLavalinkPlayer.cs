namespace Lavalink4NET.Players.Vote;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Clients;
using Lavalink4NET.Events.Players;
using Lavalink4NET.Players.Queued;

/// <summary>
///     A lavalink player with a queuing and voting system.
/// </summary>
public class VoteLavalinkPlayer : QueuedLavalinkPlayer
{
    public static PlayerFactory<VoteLavalinkPlayer> Factory => static properties => new(properties);

    private readonly IDiscordClientWrapper _discordClient;
    private readonly IList<ulong> _skipVotes;

    /// <summary>
    ///     Initializes a new instance of the <see cref="VoteLavalinkPlayer"/> class.
    /// </summary>
    public VoteLavalinkPlayer(PlayerProperties properties) : base(properties)
    {
        ArgumentNullException.ThrowIfNull(properties);

        _discordClient = properties.Client;
        _skipVotes = new List<ulong>();
    }

    /// <summary>
    ///     Clears all user votes.
    /// </summary>
    public virtual void ClearVotes() => _skipVotes.Clear();

    /// <summary>
    ///     Gets the player skip vote info.
    /// </summary>
    /// <returns>the vote info</returns>
    /// <exception cref="InvalidOperationException">thrown if the player is destroyed</exception>
    public async ValueTask<VoteSkipInfo> GetVoteInfoAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var users = await _discordClient
            .GetChannelUsersAsync(GuildId, VoiceChannelId, cancellationToken)
            .ConfigureAwait(false);

        var votes = _skipVotes
            .Intersect(users)
            .ToImmutableArray();

        return new VoteSkipInfo(votes, users.Length);
    }

    /// <summary>
    ///     Submits an user vote asynchronously.
    /// </summary>
    /// <param name="userId">the user snowflake identifier</param>
    /// <param name="percentage">the minimum voting percentage to skip the track</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    /// <exception cref="InvalidOperationException">thrown if the player is destroyed</exception>
    public virtual async ValueTask<UserVoteSkipInfo> VoteAsync(ulong userId, float percentage = .5f)
    {
        var info = await GetVoteInfoAsync();

        if (info.Votes.Contains(userId))
        {
            return new UserVoteSkipInfo(info.Votes, info.TotalUsers, false, false);
        }

        // add vote and re-get info, because the votes were changed.
        _skipVotes.Add(userId);
        info = await GetVoteInfoAsync();

        if (info.Percentage >= percentage)
        {
            ClearVotes();
            await SkipAsync();

            return new UserVoteSkipInfo(info.Votes, info.TotalUsers, true, true);
        }

        return new UserVoteSkipInfo(info.Votes, info.TotalUsers, false, true);
    }

    /// <summary>
    ///     Asynchronously triggered when a track ends.
    /// </summary>
    /// <param name="eventArgs">the track event arguments</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    protected override ValueTask OnTrackEndAsync(TrackEndEventArgs eventArgs)
    {
        ClearVotes();
        return base.OnTrackEndAsync(eventArgs);
    }
}
