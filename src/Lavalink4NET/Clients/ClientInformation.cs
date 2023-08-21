namespace Lavalink4NET.Clients;

public readonly record struct ClientInformation(string Label, ulong CurrentUserId, int ShardCount);