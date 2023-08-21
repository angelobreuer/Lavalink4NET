namespace Lavalink4NET.Players;

using System;

public readonly record struct TrackPlayProperties(
    TimeSpan? StartPosition = null,
    TimeSpan? EndTime = null,
    bool NoReplace = false);
