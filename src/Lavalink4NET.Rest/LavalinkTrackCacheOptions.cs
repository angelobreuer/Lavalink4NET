namespace Lavalink4NET.Rest;

using System;

public readonly record struct LavalinkTrackCacheOptions(
    TimeSpan? SuccessCacheDuration = null, // default: 8 hours
    TimeSpan? FailureCacheDuration = null) // default: 30 minutes
{
    public static LavalinkTrackCacheOptions Default => default;
}