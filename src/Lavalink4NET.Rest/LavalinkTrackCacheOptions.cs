namespace Lavalink4NET.Rest;

using System;

public readonly record struct LavalinkTrackCacheOptions(
    TimeSpan? SuccessCacheDuration = null, // default: 2 hours
    TimeSpan? FailureCacheDuration = null) // default: 10 minutes
{
    public static LavalinkTrackCacheOptions Default => default;
}