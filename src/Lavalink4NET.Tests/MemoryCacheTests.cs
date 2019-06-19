namespace Lavalink4NET.Tests
{
    using System;
    using Lavalink4NET.MemoryCache;
    using Xunit;

    public class MemoryCacheTests
    {
        [Fact]
        public void TestMemoryCache()
        {
            using (var cache = new LavalinkCache())
            {
                // no item in cache
                Assert.False(cache.TryGetItem<string>("key", out _));

                // add item to cache
                cache.AddItem("key", string.Empty, DateTimeOffset.UtcNow + TimeSpan.FromSeconds(10));

                // item in cache
                Assert.True(cache.TryGetItem<string>("key", out _));
            }
        }
    }
}