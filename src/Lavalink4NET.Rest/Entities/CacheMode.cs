namespace Lavalink4NET.Rest.Entities;

public enum CacheMode : byte
{
    Dynamic, // Prefers cache if available, else fetch
    Refresh, // Fetches the item and refreshes the cache
    Bypass, // Bypasses the cache and does not refresh the cache
    CacheOnly, // Force to only retrieve from cache
}
