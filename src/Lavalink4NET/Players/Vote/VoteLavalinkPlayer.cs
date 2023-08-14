namespace Lavalink4NET.Players.Vote;

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lavalink4NET.Clients;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Protocol.Payloads.Events;
using Lavalink4NET.Tracks;
using Microsoft.Extensions.Internal;

/// <summary>
///     A lavalink player with a queuing and voting system.
/// </summary>
public class VoteLavalinkPlayer : QueuedLavalinkPlayer, IVoteLavalinkPlayer
{
    private readonly IDiscordClientWrapper _discordClient;
    private readonly ISystemClock _systemClock;
    private readonly double _skipThreshold;
    private readonly bool _clearVotesAfterTrack;
    private readonly bool _requireUserToBeInVoiceChannel;

    /// <summary>
    ///     Initializes a new instance of the <see cref="VoteLavalinkPlayer"/> class.
    /// </summary>
    public VoteLavalinkPlayer(IPlayerProperties<VoteLavalinkPlayer, VoteLavalinkPlayerOptions> properties)
        : base(properties)
    {
        ArgumentNullException.ThrowIfNull(properties);

        Votes = properties.Options.Value.Votes ?? new VoteCollection();

        _discordClient = properties.DiscordClient;
        _systemClock = properties.SystemClock;

        _skipThreshold = properties.Options.Value.SkipThreshold;
        _clearVotesAfterTrack = properties.Options.Value.ClearVotesAfterTrack;
        _requireUserToBeInVoiceChannel = properties.Options.Value.RequireUserToBeInVoiceChannel;
    }

    public IVoteCollection Votes { get; }

    public async ValueTask<VoteSkipInformation> GetVotesAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var channelUsers = await _discordClient
            .GetChannelUsersAsync(GuildId, VoiceChannelId, includeBots: false, cancellationToken)
            .ConfigureAwait(false);

        return await ComputeAsync(channelUsers, cancellationToken).ConfigureAwait(false);
    }

    protected virtual async ValueTask<VoteSkipInformation> ComputeAsync(ImmutableArray<ulong> channelUsers, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var votes = Votes.ToImmutableArray();

        // Remove votes from the users that are not in the voice channel anymore
        if (_requireUserToBeInVoiceChannel)
        {
            foreach (var userId in votes.Select(x => x.UserId).Except(channelUsers))
            {
                await Votes
                    .TryRemoveAsync(userId, cancellationToken)
                    .ConfigureAwait(false);
            }

            votes = Votes.ToImmutableArray();
        }

        var totalUserCount = channelUsers.Length;
        var skipVoteCount = votes.Sum(x => x.Factor);

        var percentage = Math.Clamp(
            value: skipVoteCount / totalUserCount,
            min: 0F,
            max: 1F);

        return new VoteSkipInformation(
            Votes: votes,
            TotalUsers: totalUserCount,
            Percentage: percentage);
    }

    public virtual async ValueTask<UserVoteResult> VoteAsync(ulong userId, UserVoteOptions options, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var channelUsers = await _discordClient
            .GetChannelUsersAsync(GuildId, VoiceChannelId, includeBots: false, cancellationToken)
            .ConfigureAwait(false);

        if (_requireUserToBeInVoiceChannel && !channelUsers.Contains(userId))
        {
            return UserVoteResult.UserNotInChannel;
        }

        var factor = options.Factor.GetValueOrDefault(1.0F);
        var userVote = new UserVote(userId, _systemClock.UtcNow, factor);

        if (!await Votes.TryAddAsync(userVote, cancellationToken).ConfigureAwait(false))
        {
            return UserVoteResult.AlreadySubmitted;
        }

        var skipInformation = await ComputeAsync(channelUsers, cancellationToken).ConfigureAwait(false);

        if (!skipInformation.ShouldSkip(_skipThreshold))
        {
            return UserVoteResult.Submitted;
        }

        await SkipAsync(count: 1, cancellationToken).ConfigureAwait(false);
        return UserVoteResult.Skipped;
    }

    protected override async ValueTask NotifyTrackEndedAsync(LavalinkTrack track, TrackEndReason endReason, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(track);

        if (_clearVotesAfterTrack)
        {
            await Votes
                .ClearAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        await base
            .NotifyTrackEndedAsync(track, endReason, cancellationToken)
            .ConfigureAwait(false);
    }
}
