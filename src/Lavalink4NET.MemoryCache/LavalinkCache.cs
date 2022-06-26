/*
 *  File:   LavalinkCache.cs
 *  Author: Angelo Breuer
 *
 *  The MIT License (MIT)
 *
 *  Copyright (c) Angelo Breuer 2022
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in
 *  all copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 *  THE SOFTWARE.
 */

namespace Lavalink4NET.MemoryCache;

using System;
using System.Collections.Specialized;
using System.Runtime.Caching;

/// <summary>
///     A wrapper for <see cref="ILavalinkCache"/> using a <see cref="MemoryCache"/>.
/// </summary>
public sealed class LavalinkCache : ILavalinkCache, IDisposable
{
    private readonly ObjectCache _cache;
    private readonly bool _dispose;
    private bool _disposed;

    /// <summary>
    ///     Initializes a new instance of the <see cref="LavalinkCache"/> class.
    /// </summary>
    /// <param name="cache">the cache</param>
    /// <param name="dispose">
    ///     a value indicating whether the cache should be disposed when the <see
    ///     cref="LavalinkCache"/> instance is about being disposed. Note this parameter only
    ///     has an effect if the <paramref name="cache"/> instance implements <see cref="IDisposable"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="cache"/> is <see langword="null"/>.
    /// </exception>
    public LavalinkCache(ObjectCache cache, bool dispose = true)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _dispose = dispose;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="LavalinkCache"/> class using the
    ///     default <see cref="MemoryCache"/> ( <see cref="MemoryCache.Default"/>).
    /// </summary>
    public LavalinkCache() : this(MemoryCache.Default, dispose: false)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="LavalinkCache"/> class using a new <see
    ///     cref="MemoryCache"/> with the specified <paramref name="name"/>.
    /// </summary>
    /// <param name="name">the name of the <see cref="MemoryCache"/></param>
    /// <param name="config">an optional configuration entry collection for the <see cref="MemoryCache"/></param>
    public LavalinkCache(string name, NameValueCollection? config = null)
        : this(new MemoryCache(name, config), dispose: true)
    {
    }

    /// <summary>
    ///     Gets the cache that caches the requests.
    /// </summary>
    public ObjectCache Cache
    {
        get
        {
            EnsureNotDisposed();
            return _cache;
        }
    }

    /// <summary>
    ///     Adds an item to the cache.
    /// </summary>
    /// <param name="key">the item cache scope / the location of the item</param>
    /// <param name="item">the item to cache</param>
    /// <param name="absoluteExpiration">
    ///     the Coordinated Universal Time (UTC) time point offset when the cache item will
    ///     expire and is marked to be removed from the cache.
    /// </param>
    public void AddItem(string key, object? item, DateTimeOffset absoluteExpiration)
    {
        EnsureNotDisposed();
        Cache.Add(key, item, absoluteExpiration);
    }

    /// <summary>
    ///     Disposes the underlying <see cref="Cache"/>.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (_dispose && Cache is IDisposable disposableCache)
        {
            disposableCache.Dispose();
        }
    }

    /// <summary>
    ///     Tries to retrieve an item from the cache.
    /// </summary>
    /// <typeparam name="T">the type of the cached object</typeparam>
    /// <param name="key">the item cache scope / the location of the item</param>
    /// <param name="item">
    ///     the item that was found in cache; or default if the item was not in cache
    /// </param>
    /// <returns>a value indicating whether the item was in cache</returns>
    public bool TryGetItem<T>(string key, out T item)
    {
        EnsureNotDisposed();

        var cacheItem = Cache.GetCacheItem(key);

        if (cacheItem != null
            && cacheItem.Value != default
            && cacheItem.Value is T tItem)
        {
            item = tItem;
            return true;
        }

        item = default!;
        return false;
    }

    /// <summary>
    ///     Ensures that the instance of the <see cref="LavalinkCache"/> instance.
    /// </summary>
    /// <exception cref="ObjectDisposedException">thrown if the instance is disposed</exception>
    private void EnsureNotDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(LavalinkCache));
        }
    }
}
