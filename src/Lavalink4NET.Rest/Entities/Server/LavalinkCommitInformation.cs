namespace Lavalink4NET.Rest.Entities.Server;

using System;

public readonly record struct LavalinkCommitInformation(string BranchName, string CommitId, DateTimeOffset CommittedAt);
