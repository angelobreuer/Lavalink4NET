namespace Lavalink4NET.Players;

using System;

public readonly record struct PlayerConnectionState(bool IsConnected, TimeSpan? Latency);