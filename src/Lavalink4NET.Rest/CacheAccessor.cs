namespace Lavalink4NET.Rest;

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using Lavalink4NET.Rest.Entities;
using Microsoft.Extensions.Caching.Memory;

internal readonly record struct CacheAccessor<T>(IMemoryCache MemoryCache, string Key, CacheMode Mode)
{
    public bool TryGet([MaybeNullWhen(false)] out T value)
    {
        var metricTag = KeyValuePair.Create<string, object?>("Key", Key);
        Diagnostics.CacheQueries.Add(1, metricTag);

        if (Mode is not CacheMode.Dynamic and not CacheMode.CacheOnly)
        {
            Diagnostics.CacheQueryInvalidations.Add(1, metricTag);

            value = default;
            return false;
        }

        var result = MemoryCache.TryGetValue(Key, out value);

        if (!result)
        {
            if (Mode is CacheMode.CacheOnly)
            {
                Diagnostics.CacheHardFaults.Add(1, metricTag);
                throw new InvalidOperationException("The item is not present in cache.");
            }

            Diagnostics.CacheSoftFaults.Add(1, metricTag);
        }
        else
        {
            Diagnostics.CacheHits.Add(1, metricTag);
        }

        return result;
    }

    public void Set(T value, TimeSpan relativeExpiration)
    {
        var metricTag = KeyValuePair.Create<string, object?>("Key", Key);

        if (Mode is CacheMode.Dynamic or CacheMode.Refresh)
        {
            Diagnostics.CacheUpdates.Add(1, metricTag);
            MemoryCache.Set(Key, value, relativeExpiration);
        }
        else
        {
            Diagnostics.CacheUpdateInvalidations.Add(1, metricTag);
        }
    }
}

file static class Diagnostics
{
    public static Counter<long> CacheHits { get; }

    public static Counter<long> CacheSoftFaults { get; }

    public static Counter<long> CacheHardFaults { get; }

    public static Counter<long> CacheQueries { get; }

    public static Counter<long> CacheUpdates { get; }

    public static Counter<long> CacheQueryInvalidations { get; }

    public static Counter<long> CacheUpdateInvalidations { get; }

    static Diagnostics()
    {
        var meter = new Meter("Lavalink4NET");

        CacheHits = meter.CreateCounter<long>(
            name: "cache-hits",
            unit: "Items",
            description: "The number of items that were retrieved from the cache.");

        CacheSoftFaults = meter.CreateCounter<long>(
            name: "cache-soft-faults",
            unit: "Items",
            description: "The number of queries that could not be retrieved from the cache.");

        CacheHardFaults = meter.CreateCounter<long>(
            name: "cache-hard-faults",
            unit: "Items",
            description: "The number of queries which are specified as required that could not be retrieved from the cache.");

        CacheQueries = meter.CreateCounter<long>(
            name: "cache-queries",
            unit: "Items",
            description: "The number of queries to the cache.");

        CacheUpdates = meter.CreateCounter<long>(
            name: "cache-updates",
            unit: "Items",
            description: "The number of items that were added to the cache.");

        CacheQueryInvalidations = meter.CreateCounter<long>(
            name: "cache-query-invalidations",
            unit: "Items",
            description: "The number of queries that specified to bypass the cache.");

        CacheUpdateInvalidations = meter.CreateCounter<long>(
            name: "cache-update-invalidations",
            unit: "Items",
            description: "The number of updates that specified to bypass the cache.");
    }
}