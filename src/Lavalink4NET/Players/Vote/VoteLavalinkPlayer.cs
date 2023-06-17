namespace Lavalink4NET.Players.Vote;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Clients;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Protocol.Payloads.Events;
using Lavalink4NET.Tracks;

/// <summary>
///     A lavalink player with a queuing and voting system.
/// </summary>
public class VoteLavalinkPlayer : QueuedLavalinkPlayer
{
    private readonly IDiscordClientWrapper _discordClient;
    private readonly IList<ulong> _skipVotes;

    /// <summary>
    ///     Initializes a new instance of the <see cref="VoteLavalinkPlayer"/> class.
    /// </summary>
    public VoteLavalinkPlayer(IPlayerProperties<VoteLavalinkPlayer, VoteLavalinkPlayerOptions> properties)
        : base(properties)
    {
        ArgumentNullException.ThrowIfNull(properties);

        _discordClient = properties.DiscordClient;
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
            .GetChannelUsersAsync(GuildId, VoiceChannelId, includeBots: false, cancellationToken)
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

    protected override ValueTask OnTrackEndedAsync(LavalinkTrack track, TrackEndReason endReason, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(track);

        ClearVotes();

        return base.OnTrackEndedAsync(track, endReason, cancellationToken);
    }
}
