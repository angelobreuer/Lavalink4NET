---
sidebar_position: 4
---

# Vote player

The vote player implements a voting system for tracks known from other music bots. The player allows users to vote for skipping a track. If enough users voted for skipping the track, the track will be skipped.

## Members

The player implements the `IVoteLavalinkPlayer` interface. The player implements the following important members in addition to the members of the queued player:

```csharp
IVoteCollection Votes { get; }

ValueTask<VoteSkipInformation> GetVotesAsync(CancellationToken cancellationToken = default);

ValueTask<UserVoteResult> VoteAsync(ulong userId, UserVoteOptions options, CancellationToken cancellationToken = default);
```

### Votes

You will notice that the player has a `Votes` property. This property returns an `IVoteCollection` instance which represents the votes of the player. The vote collection is a collection of `IVote` instances. Each vote represents a vote for skipping a track.

### GetVotesAsync

The player also has a `GetVotesAsync` method which returns a `VoteSkipInformation` instance. This instance contains information about the votes of the player. The information includes the number of votes for skipping the track, the number of votes required for skipping the track, and the number of votes required for skipping the track in percent.

### VoteAsync

The player also has a `VoteAsync` method which allows users to vote for skipping the track. The method returns a `UserVoteResult` instance which contains information about the vote of the user. The information includes whether the user voted for skipping the track, whether the user's vote was successful, and whether the user's vote was the last vote required for skipping the track.

It is also possible to set a factor for the vote power of a user when submitting a vote. This can be useful for premium users or users with a certain role. The factor can be set in the `UserVoteOptions` instance passed to the `VoteAsync` method.

## Usage

Lavalink4NET provides a player factory for this player which can be used to create the vote player without additional configuration. You can pass the player factory to the `RetrieveAsync` method:

```csharp
var result = await _audioService.Players
    .RetrieveAsync(Context, playerFactory: PlayerFactory.Vote, retrieveOptions)
    .ConfigureAwait(false);
```
