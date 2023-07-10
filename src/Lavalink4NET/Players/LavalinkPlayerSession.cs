namespace Lavalink4NET.Players;

using Lavalink4NET.Rest;

public readonly record struct LavalinkPlayerSession(ILavalinkApiClient ApiClient, string SessionId);