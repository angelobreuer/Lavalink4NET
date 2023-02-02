namespace Lavalink4NET.Rest;

using System.Diagnostics.CodeAnalysis;
using Lavalink4NET.Rest.Entities;
using Microsoft.Extensions.Caching.Memory;

internal readonly record struct CacheAccessor<T>(IMemoryCache MemoryCache, string Key, CacheMode Mode)
{
    public bool TryGet([MaybeNullWhen(false)] out T value)
    {
        if (Mode is not CacheMode.Dynamic and not CacheMode.CacheOnly)
        {
            value = default;
            return false;
        }

        var result = MemoryCache.TryGetValue(Key, out value);

        if (!result && Mode is CacheMode.CacheOnly)
        {
            throw new InvalidOperationException("The item is not present in cache.");
        }

        return result;
    }

    public void Set(T value, TimeSpan relativeExpiration)
    {
        if (Mode is CacheMode.Dynamic or CacheMode.Refresh)
        {
            MemoryCache.Set(Key, value, relativeExpiration);
        }
    }
}
