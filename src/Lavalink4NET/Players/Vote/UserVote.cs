namespace Lavalink4NET.Players.Vote;

using System;

public sealed record class UserVote(ulong UserId, DateTimeOffset Timestamp, float Factor);